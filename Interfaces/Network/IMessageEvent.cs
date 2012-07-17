using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Network.Sockets;

namespace Trebuchet.Interfaces.Network
{
    interface IMessageEvent
    {
        void Invoke(UserToken Token);
    }
}
