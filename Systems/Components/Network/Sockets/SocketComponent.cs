using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trebuchet.Classes.Network.Sockets;
using Trebuchet.Systems.Components.Core;
using Trebuchet.Systems.Components.Network.Messages;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components.Network.Sockets
{
    class SocketComponent : ISystemComponent
    {
        #region SocketAsyncEventArgsPools
        private SocketAsyncEventArgsPool PoolOfAcceptSAEA
        {
            get;
            set;
        }

        private SocketAsyncEventArgsPool PoolOfReceiveSAEA
        {
            get;
            set;
        }

        private SocketAsyncEventArgsPool PoolOfSendSAEA
        {
            get;
            set;
        }
        #endregion

        #region Semaphores
        private SemaphoreSlim SemaphoreOfAcceptSAEA
        {
            get;
            set;
        }

        private SemaphoreSlim SemaphoreOfSendSAEA
        {
            get;
            set;
        }

        private SemaphoreSlim SemaphoreOfSocketSAEA
        {
            get;
            set;
        }
        #endregion

        private SocketAsyncEventArgsBufferPool BufferPoolOfSAEA
        {
            get;
            set;
        }

        public Socket AcceptSocket
        {
            get;
            private set;
        }

        public bool Started
        {
            get;
            set;
        }

        public void Run()
        {
            this.ConstructSocket();
            this.ConstructFramework();
        }

        private void ConstructSocket()
        {
            var IPAddress = default(IPAddress);

            if (!Framework.Get<SettingsComponent>().TryPop<IPAddress>("Trebuchet.Network.IP", out IPAddress))
            {
                Trebuchet.ThrowException("Failed to get the Network IP.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Network.IP', it must be a valid IP.");
                return;
            }

            var Port = default(int);

            if (!Framework.Get<SettingsComponent>().TryPop<int>("Trebuchet.Network.Port", out Port))
            {
                Trebuchet.ThrowException("Failed to get the Network Port.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Network.Port', it must be a valid integer.");
                return;
            }

            var Backlog = default(int);

            if (!Framework.Get<SettingsComponent>().TryPop<int>("Trebuchet.Network.Backlog", out Backlog))
            {
                Trebuchet.ThrowException("Failed to get the Network Backlog.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Network.Backlog', it must be a valid integer.");
                return;
            }

            var EndPoint = new IPEndPoint(IPAddress, Port);

            this.AcceptSocket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                this.AcceptSocket.Bind(EndPoint);
            }
            catch (SocketException)
            {
                Trebuchet.ThrowWarning(IPAddress + " not bindable, automaticly binded to local ip: 127.0.0.1.");
                EndPoint = new IPEndPoint(IPAddress.Any, Port);
                this.AcceptSocket.Bind(EndPoint);
            }

            this.AcceptSocket.Blocking = false;
            this.AcceptSocket.Listen(Backlog);
        }

        private void ConstructFramework()
        {
            var MaxConnections = default(int);

            if (!Framework.Get<SettingsComponent>().TryPop<int>("Trebuchet.Network.Max.Connections", out MaxConnections))
            {
                Trebuchet.ThrowException("Failed to get the Network Max connections.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Network.Max.Connections', it must be a valid integer.");
                return;
            }

            this.PoolOfAcceptSAEA = new SocketAsyncEventArgsPool();
            this.PoolOfReceiveSAEA = new SocketAsyncEventArgsPool();
            this.PoolOfSendSAEA = new SocketAsyncEventArgsPool();

            this.BufferPoolOfSAEA = new SocketAsyncEventArgsBufferPool(MaxConnections);

            this.SemaphoreOfAcceptSAEA = new SemaphoreSlim(MaxConnections, MaxConnections);
            this.SemaphoreOfSocketSAEA = new SemaphoreSlim(MaxConnections, MaxConnections);
            this.SemaphoreOfSendSAEA = new SemaphoreSlim(MaxConnections, MaxConnections);

            SerializeFramework(MaxConnections);
        }

        private void SerializeFramework(int MaxConnections)
        {
            for (int i = 0; i < MaxConnections; i++)
            {
                var AcceptArgs = new SocketAsyncEventArgs();
                AcceptArgs.Completed += AcceptArgs_Completed;
                PoolOfAcceptSAEA.Push(AcceptArgs);

                var ReceiveArgs = new SocketAsyncEventArgs();
                BufferPoolOfSAEA.SetBuffer(ReceiveArgs);
                ReceiveArgs.UserToken = new Session(i, ReceiveArgs);
                ReceiveArgs.Completed += ReceiveArgs_Completed;
                PoolOfReceiveSAEA.Push(ReceiveArgs);

                var SendArgs = new SocketAsyncEventArgs();
                BufferPoolOfSAEA.SetBuffer(SendArgs);
                SendArgs.UserToken = new SendToken();
                SendArgs.Completed += SendArgs_Completed;
                PoolOfSendSAEA.Push(SendArgs);

            }

            BeginAccept();
        }

        public bool OptainSendObject(Session Session, out SocketAsyncEventArgs SendArgs)
        {
            this.SemaphoreOfSendSAEA.Wait();

            if (this.PoolOfSendSAEA.TryPop(out SendArgs))
            {
                SendArgs.AcceptSocket = Session.Socket;
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Starters
        private void BeginAccept()
        {
            SocketAsyncEventArgs AcceptArgs;

            this.SemaphoreOfAcceptSAEA.Wait();

            if (this.PoolOfAcceptSAEA.TryPop(out AcceptArgs))
            {
                this.SemaphoreOfSocketSAEA.Wait();

                var Response = this.AcceptSocket.AcceptAsync(AcceptArgs);

                if (!Response)
                {
                    HandleAccept(AcceptArgs);
                }
            }
        }

        private void BeginReceive(SocketAsyncEventArgs ReceiveArgs)
        {
            var Session = (ReceiveArgs.UserToken as Session);

            if (Session.Socket == null)
            {
                Trebuchet.ThrowException(string.Format("[{0}] Someone is screwed: his socket died and he/she is still alive.", Session.Id));
                return;
            }

            var Response = Session.Socket.ReceiveAsync(ReceiveArgs);

            if (!Response)
            {
                HandleReceive(ReceiveArgs);
            }
        }

        public void BeginSend(SocketAsyncEventArgs SendArgs)
        {
            SendToken Token = (SendArgs.UserToken as SendToken);

            var BytesOfSend = new byte[0];

            if (Token.QueueOfPackets.TryDequeue(out BytesOfSend))
            {
                if (BytesOfSend.Length <= SocketAsyncEventArgsBufferPool.BufferSizeOfSAEA)
                {
                    SendArgs.SetBuffer(SendArgs.Offset, BytesOfSend.Length);
                    Buffer.BlockCopy(BytesOfSend, 0, SendArgs.Buffer, SendArgs.Offset, BytesOfSend.Length);
                }
                else
                {
                    SendArgs.SetBuffer(SendArgs.Offset, SocketAsyncEventArgsBufferPool.BufferSizeOfSAEA);
                    Buffer.BlockCopy(BytesOfSend, 0, SendArgs.Buffer, SendArgs.Offset, SocketAsyncEventArgsBufferPool.BufferSizeOfSAEA);
                }
            }
            
            var Response = SendArgs.AcceptSocket.SendAsync(SendArgs);

            if (!Response)
            {
                HandleSend(SendArgs);
            }
        }
        #endregion

        #region Handlers
        private void HandleAccept(SocketAsyncEventArgs AcceptArgs)
        {
            BeginAccept();

            if (AcceptArgs.SocketError != SocketError.Success)
            {
                HandleBadAccept(AcceptArgs);
                return;
            }

            SocketAsyncEventArgs ReceiveArgs;

            if (this.PoolOfReceiveSAEA.TryPop(out ReceiveArgs))
            {
                var Session = (ReceiveArgs.UserToken as Session);
                Session.SetSocket(AcceptArgs.AcceptSocket);

                AcceptArgs.AcceptSocket = null;
                this.PushReceiveSAEA(AcceptArgs);

                this.BeginReceive(ReceiveArgs);
            }
            else
            {
                HandleBadAccept(AcceptArgs);
                Trebuchet.ThrowException("You reached the max connections yout have set.");
            }
        }

        private void HandleReceive(SocketAsyncEventArgs ReceiveArgs)
        {
            var Session = (ReceiveArgs.UserToken as Session);

            if (ReceiveArgs.BytesTransferred > 0 && ReceiveArgs.SocketError == SocketError.Success)
            {
                var ReceivedBytes = new byte[ReceiveArgs.BytesTransferred];
                Buffer.BlockCopy(ReceiveArgs.Buffer, ReceiveArgs.Offset, ReceivedBytes, 0, ReceiveArgs.BytesTransferred);

                Framework.Get<MessageComponent>().ProcessBytes(Session, ref ReceivedBytes);
                BeginReceive(ReceiveArgs);
            }
            else
            {
                HandleCloseReceiveSocket(ReceiveArgs);
                PushReceiveSAEA(ReceiveArgs);
            }
        }

        private void HandleSend(SocketAsyncEventArgs SendArgs)
        {
            SendToken Token = (SendArgs.UserToken as SendToken);

            if (SendArgs.SocketError == SocketError.Success)
            {
                if (Token.QueueOfPackets.Count == 0)
                {
                    Token.OnConnectionClose();
                    PushSendSAEA(SendArgs);
                }
                else
                {
                    BeginSend(SendArgs);
                }
            }
            else
            {
                Token.OnConnectionClose();
                PushSendSAEA(SendArgs);
            }
        }
        #endregion

        #region Events
        private void AcceptArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            HandleAccept(e);
        }

        private void ReceiveArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            HandleReceive(e);
        }

        private void SendArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            HandleSend(e);
        }
        #endregion

        #region Finializers
        private void PushAcceptSAEA(SocketAsyncEventArgs Args)
        {
            this.PoolOfAcceptSAEA.Push(Args);
            this.SemaphoreOfAcceptSAEA.Release();
        }

        public void PushReceiveSAEA(SocketAsyncEventArgs Args)
        {
            this.PoolOfReceiveSAEA.Push(Args);
        }

        private void PushSendSAEA(SocketAsyncEventArgs Args)
        {
            this.PoolOfSendSAEA.Push(Args);
            this.SemaphoreOfSendSAEA.Release();
        }

        private void HandleBadAccept(SocketAsyncEventArgs AcceptArgs)
        {
            AcceptArgs.AcceptSocket.Shutdown(SocketShutdown.Both);
            AcceptArgs.AcceptSocket.Close();
            this.PoolOfAcceptSAEA.Push(AcceptArgs);
            this.SemaphoreOfAcceptSAEA.Release();
        }

        public void HandleCloseReceiveSocket(SocketAsyncEventArgs Args)
        {
            var Session = (Args.UserToken as Session);

            SemaphoreOfSocketSAEA.Release();

            try
            {
                Session.Socket.Shutdown(SocketShutdown.Both);
                Session.Socket.Close();
            }
            catch { }

            Session.OnConnectionClose();
        }
        #endregion
    }
}
