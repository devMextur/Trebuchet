using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Network.Messages.Factory.Composers;
using Trebuchet.Interfaces.Network;

namespace Trebuchet.Classes.Network.Messages.Factory.Events
{
    class InitCryptoMessageEvent: IMessageEvent
    {
        public void Invoke(Sockets.UserToken Token, MessageEvent Message)
        {
            Token.PostSendComposer(new SessionParamsMessageComposer());
        }
    }

    class SSOTicketMessageEvent : IMessageEvent
    {
        public void Invoke(Sockets.UserToken Token, MessageEvent Message)
        {
            Token.PostSendComposer(new AuthenticationOKMessageComposer());
        }
    }
}
