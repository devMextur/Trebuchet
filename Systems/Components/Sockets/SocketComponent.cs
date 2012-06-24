using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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

        public void Run()
        {
            this.ConstructSocket();
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

            var EndPoint = new IPEndPoint(IPAddress, Port);

            this.Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.Bind(EndPoint);
        }
    }
}
