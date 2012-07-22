using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Habbo.Characters;
using Trebuchet.Classes.Storage;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components.Habbo
{
    class CharacterComponent : ISystemComponent
    {
        public bool Started
        {
            get;
            set;
        }

        public void Run() { }

        public bool GetCharacter(string Ticket, out Character Character)
        {
            Character = null;

            using (var Stream = new QueryStream("SELECT * FROM characters WHERE auth_ticket = @ticket LIMIT 1"))
            {
                Stream.AddParameter("ticket", Ticket);

                var DataRow = Stream.Excecute<DataRow>();

                if (DataRow == default(DataRow))
                {
                    return false;
                }

                Character = new Character(DataRow);
            }

            return true;
        }
    }
}
