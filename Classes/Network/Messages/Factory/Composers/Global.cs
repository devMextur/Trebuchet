using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Network.Messages.Factory.Composers
{
    class MOTDNotificationComposer : MessageComposer
    {
        public MOTDNotificationComposer(string Message)
        {
            New(Headers.MessageComposerIds.MOTDNotificationComposer);
            Append(true);
            Append(Message);
        }
    }

    class HabboBroadcastMessageComposer : MessageComposer
    {
        public HabboBroadcastMessageComposer(string Message)
        {
            New(Headers.MessageComposerIds.HabboBroadcastMessageComposer);
            Append(Message);
        }
    }
}
