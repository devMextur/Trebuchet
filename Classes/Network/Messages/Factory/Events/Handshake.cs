using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Network.Messages.Factory.Composers;
using Trebuchet.Interfaces.Network;
using Trebuchet.Systems.Components;
using Trebuchet.Systems.Components.Core;
using Trebuchet.Systems.Components.Network.Sockets;

namespace Trebuchet.Classes.Network.Messages.Factory.Events
{
    class InitCryptoMessageEvent: IMessageEvent
    {
        public void Invoke(Sockets.Session Session, MessageEvent Message)
        {
            Session.Write(new SessionParamsMessageComposer());
        }
    }

    class SSOTicketMessageEvent : IMessageEvent
    {
        public void Invoke(Sockets.Session Session, MessageEvent Message)
        {
            var Ticket = Message.Get<string>();

            if (Session.SetTicket(Ticket))
            {
                Session.Write(new AuthenticationOKMessageComposer());
                Session.Write(new HabboBroadcastMessageComposer("Trebuchet "
                    + Framework.Get<AssemblyComponent>().ExecutingState + " "
                    + Framework.Get<AssemblyComponent>().ExecutingVersion + Environment.NewLine + "Welcome, " + Session.Character.Username));
            }
            else
            {
                Session.Disconnect();
            }
        }
    }
}
