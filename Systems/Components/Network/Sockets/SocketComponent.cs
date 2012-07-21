using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trebuchet.Classes.Global;
using Trebuchet.Classes.Network.Sockets;
using Trebuchet.Systems.Components.Network;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components
{
    class SocketComponent : ISystemComponent
    {
        public bool Started
        {
            get;
            set;
        }

        public Socket Socket
        {
            get;
            set;
        }

        #region Pooling
        public TPool<SocketAsyncEventArgs> AcceptArgsPool
        {
            get;
            set;
        }

        public TPool<SocketAsyncEventArgs> ReceiveArgsPool
        {
            get;
            set;
        }

        public TPool<SocketAsyncEventArgs> SendArgsPool
        {
            get;
            set;
        }
        #endregion

        public SemaphoreSlim Semaphore
        {
            get;
            set;
        }

        public BufferPool Buffer
        {
            get;
            set;
        }

        public void Run()
        {
            this.ConstructSocket();
            this.ConstructFramework();
            this.QueueAccept();
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

            this.Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                this.Socket.Bind(EndPoint);
            }
            catch (SocketException)
            {
                Trebuchet.ThrowWarning(IPAddress + " not bindable, automaticly binded to local ip: 127.0.0.1.");
                EndPoint = new IPEndPoint(IPAddress.Any, Port);
                this.Socket.Bind(EndPoint);
            }

            this.Socket.Blocking = false;
            this.Socket.Listen(Backlog);
        }

        private void ConstructFramework()
        {
            var SupportedAmount = default(int);

            if (!Framework.Get<SettingsComponent>().TryPop<int>("Trebuchet.Network.SupportedAmount", out SupportedAmount))
            {
                Trebuchet.ThrowException("Failed to get the Network SupportedAmount.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Network.SupportedAmount', it must be a valid integer.");
                return;
            }

            this.AcceptArgsPool = new TPool<SocketAsyncEventArgs>(SupportedAmount);
            this.ReceiveArgsPool = new TPool<SocketAsyncEventArgs>(SupportedAmount);
            this.SendArgsPool = new TPool<SocketAsyncEventArgs>(SupportedAmount);
            this.Semaphore = new SemaphoreSlim(SupportedAmount, SupportedAmount);
            this.Buffer = new BufferPool(SupportedAmount);
            this.Buffer.PushAllReceivers(this.ReceiveArgsPool.PushAndHandleAll());
            this.Buffer.PushAllSenders(this.SendArgsPool.PushAndHandleAll());

            foreach (SocketAsyncEventArgs Args in this.AcceptArgsPool.PushAndHandleAll())
            {
                Args.Completed += AcceptCompleted;
            }
        }

        #region Accepting

        public void QueueAccept()
        {
            SocketAsyncEventArgs AcceptArgs;

            if (AcceptArgsPool.TryPop(out AcceptArgs))
            {
                Semaphore.Wait();

                bool Result = Socket.AcceptAsync(AcceptArgs);

                if (!Result)
                {
                    ProccesAccept(AcceptArgs);
                }
            }
        }

        public void ProccesAccept(SocketAsyncEventArgs Args)
        {
            if (Args.SocketError != SocketError.Success)
            {
                FinializeAccept(true, Args);
                return;
            }

            QueueAccept();

            SocketAsyncEventArgs ReceiveArgs;
            SocketAsyncEventArgs SendArgs;

            if (this.ReceiveArgsPool.TryPop(out ReceiveArgs) && this.SendArgsPool.TryPop(out SendArgs))
            {
                ReceiveArgs.UserToken = new UserToken(Args, ReceiveArgs,SendArgs);
                SendArgs.UserToken = ReceiveArgs.UserToken;

                FinializeAccept(false, Args);

                QueueReceive(ReceiveArgs);

                Framework.Get<LogComponent>().WriteLine("Accepted: {0}", (ReceiveArgs.UserToken as UserToken).Socket.RemoteEndPoint);
            }
            else FinializeAccept(true, Args);
        }

        private void AcceptCompleted(object Pointer, SocketAsyncEventArgs Args)
        {
            this.ProccesAccept(Args);
        }

        private void FinializeAccept(bool Close, SocketAsyncEventArgs Args)
        {
            if (Close)
            {
                Args.AcceptSocket.Close();
            }

            Args.AcceptSocket = null;
            this.AcceptArgsPool.Push(Args);
        }

        #endregion

        #region Receiving
        public void QueueReceive(SocketAsyncEventArgs Args)
        {
            bool Result = (Args.UserToken as UserToken).Socket.ReceiveAsync(Args);

            if (!Result)
            {
                ReceiveCompleted(Args);
            }
        }

        public void ReceiveCompleted(SocketAsyncEventArgs Args)
        {
            var Token = (Args.UserToken as UserToken);

            if (Args.BytesTransferred > 0 && Args.SocketError == SocketError.Success)
            {
                byte[] Bytes = new byte[Args.BytesTransferred];

                Array.Copy(Buffer.Buffer, Args.Offset, Bytes, 0, Args.BytesTransferred);

                Framework.Get<MessageComponent>().ProcessBytes(Token, ref Bytes);

                QueueReceive(Args);
            }
            else
            {
                // TODO: write error
                CloseClientSocket(Args);
            } 
        }
        #endregion

        #region Sending
        public void QueueSend(SocketAsyncEventArgs Args)
        {
            var Token = (Args.UserToken as UserToken);

            if (Token.LongBytesBuffer != null)
            {
                Args.SetBuffer(Args.Offset, Token.LongBytesBuffer.Length);
                System.Buffer.BlockCopy(Token.LongBytesBuffer, 0, Args.Buffer, Args.Offset, Token.LongBytesBuffer.Length);

                Token.FinializeSending();
            }

            bool Result = (Args.UserToken as UserToken).Socket.SendAsync(Args);

            if (!Result)
            {
                SendCompleted(Args);
            }
        }

        public void SendCompleted(SocketAsyncEventArgs Args)
        {
            var Token = (Args.UserToken as UserToken);

            if (Args.SocketError != SocketError.Success)
            {
                // TODO: write error
                CloseClientSocket(Args);
            }
        }
        #endregion

        #region Finializing
        public void FinializeTraffic(object Pointer, SocketAsyncEventArgs Args)
        {
            switch (Args.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ReceiveCompleted(Args);
                    break;
                case SocketAsyncOperation.Send:
                    SendCompleted(Args);
                    break;
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs Args)
        {
            UserToken Token = (Args.UserToken as UserToken);

            this.Semaphore.Release();

            Framework.Get<LogComponent>().WriteLine("Closed: {0}", Token.Socket.RemoteEndPoint);

            try
            {
                Token.Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }

            Token.Socket.Close();
            Token.OnConnectionClose();
        }
        #endregion
    }
}
