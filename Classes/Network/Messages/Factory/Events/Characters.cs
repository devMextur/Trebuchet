using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trebuchet.Classes.Network.Messages.Factory.Composers;
using Trebuchet.Interfaces.Network;

namespace Trebuchet.Classes.Network.Messages.Factory.Events
{
    class InfoRetrieveMessageEvent : IMessageEvent
    {
        public void Invoke(Sockets.Session Session, MessageEvent Message)
        {
            Session.Write(new HabboActivityPointNotificationMessageComposer(Session.Character.ActivityPoints));
            Session.Write(new UserRightsMessageComposer(2)); // MEMBERSHIP > RIGHTS
            Session.Write(new NavigatorSettingsComposer(0)); // NAVIGATOR > STARTROOM
        }
    }

    class GetCreditsInfoEvent : IMessageEvent
    {
        public void Invoke(Sockets.Session Session, MessageEvent Message)
        {
            Session.Write(new CreditBalanceComposer(Session.Character.Credits));
        }
    }

}
