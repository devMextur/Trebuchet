using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Classes.Global;

namespace Trebuchet.Classes.Habbo.Characters
{
    class Character
    {
        public int Id
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public Character(DataRow Row)
        {
            using (var Adapter = new RowAdapter(Row))
            {
                this.Id = Adapter.Get<int>("id");
                this.Username = Adapter.Get<string>("username");
            }
        }
    }
}
