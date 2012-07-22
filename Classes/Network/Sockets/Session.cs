using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Habbo.Characters;
using Trebuchet.Classes.Network.Messages;
using Trebuchet.Systems.Components.Habbo;
using Trebuchet.Systems.Components.Network.Sockets;

namespace Trebuchet.Classes.Network.Sockets
{
    class Session
    {
        public int Id
        {
            get;
            private set;
        }

        public Socket Socket
        {
            get;
            private set;
        }

        public SocketAsyncEventArgs CurrentSAEAOfReceiving
        {
            get;
            private set;
        }

        public SocketAsyncEventArgs CurrentSAEAOfSending
        {
            get;
            private set;
        }

        public Character Character
        {
            get;
            private set;
        }

        public Session(int Id, SocketAsyncEventArgs CurrentSAEAOfReceivingd)
        {
            this.Id = Id;
            this.CurrentSAEAOfReceiving = CurrentSAEAOfReceiving;
        }

        public void SetSocket(Socket AcceptSocket)
        {
            this.Socket = AcceptSocket;
        }

        public bool SetTicket(string Ticket)
        {
            Character Character = null;

            if (Framework.Get<CharacterComponent>().GetCharacter(Ticket, out Character))
            {
                this.Character = Character;
                return true;
            }

            return false;
        }

        #region Asynchronized sending
        public void Write(byte[] Bytes)
        {
            if (CurrentSAEAOfSending == null)
            {
                SocketAsyncEventArgs Args;

                if (Framework.Get<SocketComponent>().OptainSendObject(this, out Args))
                {
                    this.CurrentSAEAOfSending = Args;
                }
            }

            var Token = (CurrentSAEAOfSending.UserToken as SendToken);

            Token.QueueOfPackets.Enqueue(Bytes);
        }

        public void Write(MessageComposer Composer)
        {
            this.Write(Composer.GetBytes());
        }

        public void Push()
        {
            if (CurrentSAEAOfSending != null)
            {
                Framework.Get<SocketComponent>().BeginSend(CurrentSAEAOfSending);
                this.CurrentSAEAOfSending = null;
            }
        }
        #endregion

        public void Disconnect()
        {
            Framework.Get<SocketComponent>().HandleCloseReceiveSocket(CurrentSAEAOfReceiving);
            Framework.Get<SocketComponent>().PushReceiveSAEA(CurrentSAEAOfReceiving);
        }

        public void OnConnectionClose()
        {
            this.Socket = null;
            this.Character = null;
            this.CurrentSAEAOfSending = null;
        }
    }
}
