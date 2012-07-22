using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Network.Messages.Factory.Composers
{
    class NavigatorSettingsComposer : MessageComposer
    {
        public NavigatorSettingsComposer(int StartRoomId)
        {
            New(Headers.MessageComposerIds.NavigatorSettingsComposer);
            Append(StartRoomId);
        }
    }
}
