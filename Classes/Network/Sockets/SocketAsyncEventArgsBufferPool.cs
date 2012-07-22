using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Network.Sockets
{
    class SocketAsyncEventArgsBufferPool
    {        
        private int TotalBytesOfBuffer
        {
            get;
            set;
        }

        private byte[] Buffer
        {
            get;
            set;
        }

        private Stack<int> PoolOfFreeIndex
        {
            get;
            set;
        }

        private int IndexOfCurrentOffset
        {
            get;
            set;
        }

        public const int BufferSizeOfSAEA = 512;

        public SocketAsyncEventArgsBufferPool(int MaxConnections)
        {
            this.TotalBytesOfBuffer = MaxConnections * 2 * BufferSizeOfSAEA;
            this.Buffer = new byte[TotalBytesOfBuffer];
            this.PoolOfFreeIndex = new Stack<int>();
            this.IndexOfCurrentOffset = new int();
        }

        public bool SetBuffer(SocketAsyncEventArgs Args)
        {
            if (this.PoolOfFreeIndex.Count > 0)
            {
                Args.SetBuffer(this.Buffer, this.PoolOfFreeIndex.Pop(), BufferSizeOfSAEA);
            }
            else
            {
                if ((TotalBytesOfBuffer - BufferSizeOfSAEA) < this.IndexOfCurrentOffset)
                {
                    return false;
                }

                Args.SetBuffer(this.Buffer, this.IndexOfCurrentOffset, BufferSizeOfSAEA);
                this.IndexOfCurrentOffset += BufferSizeOfSAEA;
            }

            return true;
        }
    }
}
