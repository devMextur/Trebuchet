using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components.Core
{
    class AssemblyComponent : ISystemComponent
    {
        public string ExecutingState
        {
            get;
            set;
        }

        public string ExecutingVersion
        {
            get;
            set;
        }

        public string DevelopersState
        {
            get;
            set;
        }

        public string DevelopersVersion
        {
            get;
            set;
        }

        public bool Started
        {
            get;
            set;
        }

        public void Run()
        {
            this.ExecutingState = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).Comments;
            this.ExecutingVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            WebClient Client = new WebClient();
            var WebsiteSource = Client.DownloadString("https://raw.github.com/devMextur/Trebuchet/master/Properties/AssemblyInfo.cs");

            this.DevelopersState = WebsiteSource.Split('\"')[3];
            this.DevelopersVersion = WebsiteSource.Split('\"')[WebsiteSource.Split('\"').Count() - 2];

            if (ExecutingVersion != DevelopersVersion)
            {
                if (!System.IO.Directory.Exists(".//Releases"))
                {
                    System.IO.Directory.CreateDirectory(".//Releases");
                }

                Client.DownloadFile("https://github.com/devMextur/Trebuchet/raw/master/bin/Debug/Trebuchet.exe", ".//Releases//Trebuchet-" + DevelopersVersion + "-" + DevelopersState + ".exe");
                Trebuchet.ThrowWarning("New version available: Trebuchet-" + DevelopersVersion + "-" + DevelopersState);
            }
        }
    }
}
