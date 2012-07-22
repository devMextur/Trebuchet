using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Network.Messages.Encoding;

namespace Trebuchet.Classes.Network.Messages
{
    class MessageEvent
    {
        public short Id
        {
            get;
            private set;
        }

        public byte[] Content
        {
            get;
            private set;
        }

        public int Pointer
        {
            get;
            private set;
        }

        public int RemainingLength
        {
            get
            {
                return (Content.Length - Pointer);
            }
        }

        public MessageEvent(short Id, byte[] Content)
        {
            this.Id = Id;
            this.Content = Content;
            this.Pointer = 0;
        }

        public T Get<T>()
        {
            if (typeof(T) == typeof(int))
            {
                return (T)PopInt32();
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)PopString();
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)PopBoolean();
            }
            else if (typeof(T) == typeof(ICollection<int>))
            {
                return (T)PopCollection();
            }
            else return (T)PopString();
        }

        private object PopInt32()
        {
            try
            {
                using (var Stream = new BinaryReader(new MemoryStream(Content)))
                {
                    Stream.ReadBytes(Pointer);

                    byte[] WorkBytes = Stream.ReadBytes(RemainingLength < Wire64Encoding.MAX_INTEGER_BYTE_AMOUNT ? RemainingLength : Wire64Encoding.MAX_INTEGER_BYTE_AMOUNT);

                    int Length;
                    int Result = Wire64Encoding.DecodeInt32(WorkBytes, out Length);

                    Pointer += Length;

                    return Result;
                }
            }
            catch
            {
                return -1;
            }
        }

        private object PopString()
        {
            try
            {
                using (var Stream = new BinaryReader(new MemoryStream(Content)))
                {
                    Stream.ReadBytes(Pointer);

                    int Length = Base64Encoding.DecodeInt32(Stream.ReadBytes(2));
                    Pointer += 2;
                    Pointer += Length;

                    string Output = System.Text.Encoding.ASCII.GetString(Stream.ReadBytes(Length));

                    Output = Output.Replace(Convert.ToChar(1), ' ');
                    Output = Output.Replace(Convert.ToChar(2), ' ');
                    Output = Output.Replace(Convert.ToChar(3), ' ');
                    Output = Output.Replace(Convert.ToChar(9), ' ');

                    return Output;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private object PopBoolean()
        {
            return (int)PopInt32() > 0;
        }

        private object PopCollection()
        {
            ICollection<int> Output = new List<int>();

            int Length = (int)PopInt32();

            for (int i = 0; i < Length; i++)
            {
                int Obj = (int)PopInt32();

                if (!Output.Contains(Obj))
                {
                    Output.Add(Obj);
                }
            }

            return Output;
        }
    }
}
