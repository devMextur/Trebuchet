using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Network.Messages.Factory.Composers
{
    class HabboActivityPointNotificationMessageComposer : MessageComposer
    {
        public HabboActivityPointNotificationMessageComposer(int ActivityPoints)
        {
            New(Headers.MessageComposerIds.HabboActivityPointNotificationMessageComposer);
            Append(ActivityPoints);
        }
    }

    class CreditBalanceComposer : MessageComposer
    {
        public CreditBalanceComposer(int Credits)
        {
            New(Headers.MessageComposerIds.CreditBalanceComposer);
            Append(Credits + string.Empty);
        }
    }

    class UserRightsMessageComposer : MessageComposer
    {
        public UserRightsMessageComposer(int RightIndex)
        {
            New(Headers.MessageComposerIds.UserRightsMessageComposer);
            Append(RightIndex);
        }
    }
}
