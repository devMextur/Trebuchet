using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Network.Messages;
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

        public Queue<byte[]> QueueByteSend
        {
            get;
            private set;
        }

        public byte[] LongBytesBuffer
        {
            get;
            private set;
        }

        public UserToken(SocketAsyncEventArgs AcceptArgs, SocketAsyncEventArgs ReceiveArgs, SocketAsyncEventArgs SendArgs) // if its null return;
        {
            this.Socket = AcceptArgs.AcceptSocket;
            this.ReceiveArgs = ReceiveArgs;
            this.SendArgs = SendArgs;
            this.QueueByteSend = new Queue<byte[]>();
        }

        public void FinializeSending()
        {
            this.QueueByteSend = new Queue<byte[]>();
            this.LongBytesBuffer = null;
        }

        public void PostSendComposer(MessageComposer Composer)
        {
            this.PostSendBytes(Composer.GetBytes());
        }

        public void PostSendBytes(byte[] Bytes)
        {
            this.QueueByteSend.Enqueue(Bytes);
        }

        public void PushSendBytes()
        {
            if (QueueByteSend.Count == 0)
            {
                return;
            }

            var Output = new List<byte>();

            for (int i = 0; i < QueueByteSend.Count; i++)
            {
                Output.AddRange(QueueByteSend.Dequeue());
            }

            this.LongBytesBuffer = Output.ToArray();

            Framework.Get<SocketComponent>().QueueSend(SendArgs);
        }

        public void OnConnectionClose()
        {
            Framework.Get<SocketComponent>().ReceiveArgsPool.Push(ReceiveArgs);
            Framework.Get<SocketComponent>().SendArgsPool.Push(SendArgs);
        }
    }
}
