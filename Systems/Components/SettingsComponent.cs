using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components
{
    class SettingsComponent : ISystemComponent
    {
        public bool Started
        {
            get;
            set;
        }

        public Dictionary<string, string> Information
        {
            get;
            set;
        }

        public string SETTINGS_PATH
        {
            get
            {
                return ".//Assets//Settings.xml";
            }
        }

        public void Run()
        {
            this.ReadSettings();
        }

        public void ReadSettings()
        {
            Information = new Dictionary<string, string>();

            if (!System.IO.File.Exists(SETTINGS_PATH))
            {
                Trebuchet.ThrowException("Could not find Settings file,");
                Trebuchet.ThrowException("make sure the Assets folder exists, and the settings file is there.");
                return;
            }

            var Lines = System.IO.File.ReadAllLines(SETTINGS_PATH);

            if (Lines.Count() == 0)
            {
                Trebuchet.ThrowException("Invalid Settings file,");
                Trebuchet.ThrowException("Make sure the Settings file is not empty.");
                return;
            }

            if (Lines[0] != "<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
            {
                Trebuchet.ThrowException("Invalid Settings file,");
                Trebuchet.ThrowException("Make sure the Settings file is not broken.");
                return;
            }

            foreach (string Line in Lines)
            {
                var Components = Line.Split('<', '>');

                if (Components.Count() == 5)
                {
                    var Key = Components[1];
                    var Value = Components[2];

                    //TODO: Finish this...
                }
            }
        }
    }
}
