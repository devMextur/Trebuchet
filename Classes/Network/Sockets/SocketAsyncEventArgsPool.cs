using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace Trebuchet.Classes.Network.Sockets
{
    class SocketAsyncEventArgsPool
    {
        private ConcurrentStack<SocketAsyncEventArgs> InnerStack
        {
            get;
            set;
        }

        public SocketAsyncEventArgsPool()
        {
            this.InnerStack = new ConcurrentStack<SocketAsyncEventArgs>();
        }

        public bool TryPop(out SocketAsyncEventArgs Args)
        {
            return this.InnerStack.TryPop(out Args);
        }

        public void Push(SocketAsyncEventArgs Args)
        {
            if (Args == null)
            {
                return;
            }

            this.InnerStack.Push(Args);
        }

        public ConcurrentStack<SocketAsyncEventArgs> GetInnerStack()
        {
            return InnerStack;
        }
    }
}
