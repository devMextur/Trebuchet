using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Components;

namespace Trebuchet.Classes.Network.Sockets
{
    class UserToken
    {
        public Socket Socket
        {
            get;
            private set;
        }

        public SocketAsyncEventArgs ReceiveArgs
        {
            get;
            private set;
        }

        public SocketAsyncEventArgs SendArgs
        {
            get;
            private set;
        }

        public byte[] QueueBytesToSend
        {
            get;
            private set;
        }

        public UserToken(SocketAsyncEventArgs AcceptArgs, SocketAsyncEventArgs ReceiveArgs, SocketAsyncEventArgs SendArgs) // if its null return;
        {
            this.Socket = AcceptArgs.AcceptSocket;
            this.ReceiveArgs = ReceiveArgs;
            this.SendArgs = SendArgs;
        }

        public void FinializeSending()
        {
            this.QueueBytesToSend = null;
        }

        public void SendBytes(byte[] Bytes)
        {
            this.QueueBytesToSend = Bytes;
            Framework.Get<SocketComponent>().QueueSend(SendArgs);
        }

        public void OnConnectionClose()
        {
            Framework.Get<SocketComponent>().ReceiveArgsPool.Push(ReceiveArgs);
            Framework.Get<SocketComponent>().SendArgsPool.Push(SendArgs);
        }
    }
}
