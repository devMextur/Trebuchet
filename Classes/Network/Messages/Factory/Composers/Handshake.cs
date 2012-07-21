using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Network.Messages.Factory.Composers
{
    class SessionParamsMessageComposer : MessageComposer
    {
        public SessionParamsMessageComposer()
        {
            New(Headers.MessageComposerIds.SessionParamsMessageComposer);
            Append(default(int));
        }
    }

    class AuthenticationOKMessageComposer : MessageComposer
    {
        public AuthenticationOKMessageComposer()
        {
            New(Headers.MessageComposerIds.AuthenticationOKMessageComposer);
        }
    }
}
