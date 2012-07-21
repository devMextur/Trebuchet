using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Network.Messages.Encoding;

namespace Trebuchet.Classes.Network.Messages
{
    class MessageComposer
    {
        private List<byte> Buffer
        {
            get;
            set;
        }

        public void New(short Header)
        {
            this.Buffer = new List<byte>();
            this.Buffer.AddRange(Base64Encoding.EncodeInt32((int)Header, 2));
        }

        public void Append(object e)
        {
            if (e == null)
            {
                return;
            }

            if (e is int)
            {
                Buffer.AddRange(Wire64Encoding.EncodeInt32((int)e));
            }
            else if (e is short)
            {
                Buffer.AddRange(Wire64Encoding.EncodeInt32((short)e));
            }
            else if (e is bool)
            {
                Buffer.AddRange(Wire64Encoding.EncodeInt32((bool)e ? 1 : 0));
            }
            else if (e is MessageComposer)
            {
                Buffer.AddRange((e as MessageComposer).GetBytes());
            }
            else
            {
                Buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(e.ToString()));
                Buffer.Add(2);
            }
        }

        public byte[] GetBytes()
        {
            if (Buffer.Last() != 1)
            {
                Buffer.Add(1);
            }

            return Buffer.ToArray();
        }
    }
}
