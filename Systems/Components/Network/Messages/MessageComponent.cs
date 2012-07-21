using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Network.Messages;
using Trebuchet.Classes.Network.Messages.Encoding;
using Trebuchet.Classes.Network.Sockets;
using Trebuchet.Interfaces.Network;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components.Network
{
    class MessageComponent : ISystemComponent
    {
        public bool Started
        {
            get;
            set;
        }

        public Dictionary<short, IMessageEvent> MessageEvents
        {
            get;
            private set;
        }

        public Dictionary<short, string> UnknownMessageEvents
        {
            get;
            private set;
        }

        public byte[] PolicyResponse
        {
            get;
            private set;
        }

        public void Run()
        {
            MessageEvents = new Dictionary<short, IMessageEvent>();
            UnknownMessageEvents = new Dictionary<short,string>();

            foreach (FieldInfo MessageEvent in typeof(Headers.MessageEventIds).GetFields())
            {
                var MessageEventId = (short)MessageEvent.GetValue(0);
                var MessageEventName = MessageEvent.Name;

                foreach (Type Type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (Type.Name == MessageEventName)
                    {
                        var ContentMessageEvent = (Type.GetConstructor(new Type[] { }).Invoke(new object[] { }) as IMessageEvent);

                        MessageEvents.Add(MessageEventId, ContentMessageEvent);
                    }
                }

                if (!MessageEvents.ContainsKey(MessageEventId))
                {
                    UnknownMessageEvents.Add(MessageEventId, MessageEventName);
                }
            }

            PolicyResponse = Encoding.Default.GetBytes("<?xml version=\"1.0\"?>\r\n" +
                       "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                       "<cross-domain-policy>\r\n" +
                       "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                       "</cross-domain-policy>\x0");
        }

        public void ProcessBytes(UserToken Token, ref byte[] Bytes)
        {
            if (Token == null)
            {
                return;
            }

            var ProcessType = ChooseProcessType(ref Bytes);

            switch (ProcessType)
            {
                case ByteProcessType.Unknown:
                    return;
                case ByteProcessType.Policy:
                    HandlePolicy(Token);
                    return;
                case ByteProcessType.Single:
                    HandleSingle(Token,true, ref Bytes);
                    return;
                case ByteProcessType.Multi:
                    HandleMulti(Token, ref Bytes);
                    return;
            }
        }

        private ByteProcessType ChooseProcessType(ref byte[] Bytes)
        {
            if (Bytes[0] == 60)
            {
                return ByteProcessType.Policy;
            }
            else if (Bytes[0] == 64)
            {
                var Index = Base64Encoding.DecodeInt32(new byte[] { Bytes[0], Bytes[1], Bytes[2] });

                if (Index > (Bytes.Length - 3))
                {
                    return ByteProcessType.Multi;
                }
                else return ByteProcessType.Single;
            }
            else return ByteProcessType.Unknown;
        }

        private void HandlePolicy(UserToken Token)
        {
            Token.PostSendBytes(PolicyResponse);
            Token.PushSendBytes();
        }

        private void HandleSingle(UserToken Token, bool Scratch, ref byte[] Bytes)
        {
            using (var Reader = new BinaryReader(new MemoryStream(Bytes, false)))
            {
                var Index = Bytes.Length;

                if (Scratch)
                {
                    Index = Base64Encoding.DecodeInt32(Reader.ReadBytes(3));
                }

                var Header = (short)Base64Encoding.DecodeInt32(Reader.ReadBytes(2));
                var Content = Reader.ReadBytes(Index - 2);

                if (MessageEvents.ContainsKey(Header))
                {
                    MessageEvents[Header].Invoke(Token, new MessageEvent(Header, Content));
                    Token.PushSendBytes();
                    Framework.Get<LogComponent>().BlurLine("MSG", ConsoleColor.Green, "[{0}] {1}", ConsoleColor.White, Header, MessageEvents[Header].ToString().Split('.').Last());
                }
                else if (UnknownMessageEvents.ContainsKey(Header))
                {
                    Framework.Get<LogComponent>().BlurLine("MSG", ConsoleColor.Gray, "[{0}] {1}", ConsoleColor.White, Header, UnknownMessageEvents[Header]);
                }
                else
                {
                    Framework.Get<LogComponent>().BlurLine("MSG", ConsoleColor.Red, "[{0}] Unknown", ConsoleColor.White, Header);
                }
            }
        }

        private void HandleMulti(UserToken Token, ref byte[] Bytes)
        {
            using (var Reader = new BinaryReader(new MemoryStream(Bytes,false)))
            {
                for (int i = 0; i < Bytes.Length; )
                {
                    var Index = Base64Encoding.DecodeInt32(Reader.ReadBytes(3)); i += 3;
                    var SingleBytes = Reader.ReadBytes(Index); i += Index;

                    HandleSingle(Token, false, ref SingleBytes);
                }              
            }
        }
    }

    enum ByteProcessType
    {
       Unknown, Policy, Single, Multi
    }
}
