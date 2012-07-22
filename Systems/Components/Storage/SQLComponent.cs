using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Components.Core;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components.Storage
{
    class SQLComponent : ISystemComponent
    {
        public string ConnectionString
        {
            get;
            private set;
        }

        public bool Started
        {
            get;
            set;
        }

        public void Run()
        {
            var Host = string.Empty;

            if (!Framework.Get<SettingsComponent>().TryPop<string>("Trebuchet.Database.Host", out Host))
            {
                Trebuchet.ThrowException("Failed to get the Database Host.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Database.Host', it must be a valid string.");
                return;
            }

            var Port = default(uint);

            if (!Framework.Get<SettingsComponent>().TryPop<uint>("Trebuchet.Database.Port", out Port))
            {
                Trebuchet.ThrowException("Failed to get the Database Port.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Database.Port', it must be a valid uinteger.");
                return;
            }

            var Username = string.Empty;

            if (!Framework.Get<SettingsComponent>().TryPop<string>("Trebuchet.Database.Username", out Username))
            {
                Trebuchet.ThrowException("Failed to get the Database Username.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Database.Username', it must be a valid string.");
                return;
            }

            var Database = string.Empty;

            if (!Framework.Get<SettingsComponent>().TryPop<string>("Trebuchet.Database.Database", out Database))
            {
                Trebuchet.ThrowException("Failed to get the Database Database.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Database.Database', it must be a valid string.");
                return;
            }

            var Password = string.Empty;

            if (!Framework.Get<SettingsComponent>().TryPop<string>("Trebuchet.Database.Password", out Password))
            {
                Trebuchet.ThrowException("Failed to get the Database Password.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Database.Password', it must be a valid string.");
                return;
            }

            var Pooling = default(bool);

            if (!Framework.Get<SettingsComponent>().TryPop<bool>("Trebuchet.Database.Pooling.Enabled", out Pooling))
            {
                Trebuchet.ThrowException("Failed to get the Database Pooling.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Database.Pooling.Enabled', it must be a valid bool.");
                return;
            }

            var MinimumPooling = default(uint);

            if (!Framework.Get<SettingsComponent>().TryPop<uint>("Trebuchet.Database.Pooling.Minimal", out MinimumPooling))
            {
                Trebuchet.ThrowException("Failed to get the Database Minimal Pooling.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Database.Pooling.Minimal', it must be a valid uinteger.");
                return;
            }

            var MaximalPooling = default(uint);

            if (!Framework.Get<SettingsComponent>().TryPop<uint>("Trebuchet.Database.Pooling.Maximal", out MaximalPooling))
            {
                Trebuchet.ThrowException("Failed to get the Database Maximal Pooling.");
                Trebuchet.ThrowException("Check Setting 'Trebuchet.Database.Pooling.Maximal', it must be a valid uinteger.");
                return;
            }

            var Builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            Builder.Server = Host;
            Builder.Port = Port;
            Builder.UserID = Username;
            Builder.Database = Database;
            Builder.Password = Password;
            Builder.Pooling = Pooling;
            Builder.MinimumPoolSize = MinimumPooling;
            Builder.MaximumPoolSize = MaximalPooling;
            this.ConnectionString = Builder.ConnectionString;
        }
    }
}
