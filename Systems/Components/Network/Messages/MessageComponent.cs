using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Trebuchet.Classes.Network.Messages;
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

        public void Run()
        {
            MessageEvents = new Dictionary<short, IMessageEvent>();

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
            }
        }

        public void ProcessBytes(UserToken Token, ref byte[] Bytes)
        {
            if (Token == null)
            {
                return;
            }

            Framework.Get<LogComponent>().WriteLine("Packet: {0}", Encoding.Default.GetString(Bytes));
        }
    }
}
