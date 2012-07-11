using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Sockets
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

        public UserToken(SocketAsyncEventArgs AcceptArgs, SocketAsyncEventArgs ReceiveArgs) // if its null return;
        {
            this.Socket = AcceptArgs.AcceptSocket;
            this.ReceiveArgs = ReceiveArgs;
        }

        public void OnConnectionClose()
        {
        }
    }
}
