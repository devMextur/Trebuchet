using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Network.Sockets
{
    class SendToken
    {
        public ConcurrentQueue<byte[]> QueueOfPackets
        {
            get;
            private set;
        }

        public SendToken()
        {
            this.QueueOfPackets = new ConcurrentQueue<byte[]>();
        }

        public void OnConnectionClose()
        {
            this.QueueOfPackets = new ConcurrentQueue<byte[]>();
        }
    }
}
