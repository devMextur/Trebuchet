using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Components;

namespace Trebuchet.Classes.Network.Sockets
{
    class BufferPool
    {
        public const int BUF_SIZE = 512;

        public byte[] Buffer
        {
            get;
            private set;
        }

        public int OffsetPointer
        {
            get;
            private set;
        }

        public BufferPool(int SupportedAmount)
        {
            this.Buffer = new byte[BUF_SIZE * 2 * SupportedAmount];
            this.OffsetPointer = default(int);
        }

        public void PushAllReceivers(ICollection<SocketAsyncEventArgs> Receivers)
        {
            foreach (SocketAsyncEventArgs Args in Receivers)
            {
                Args.SetBuffer(Buffer, OffsetPointer, BUF_SIZE);
                Args.Completed += Framework.Get<SocketComponent>().FinializeTraffic;

                OffsetPointer += BUF_SIZE;
            }
        }

        public void PushAllSenders(ICollection<SocketAsyncEventArgs> Senders)
        {
            foreach (SocketAsyncEventArgs Args in Senders)
            {
                Args.SetBuffer(Buffer, OffsetPointer, BUF_SIZE);
                Args.Completed += Framework.Get<SocketComponent>().FinializeTraffic;

                OffsetPointer += BUF_SIZE;
            }
        }
    }
}
