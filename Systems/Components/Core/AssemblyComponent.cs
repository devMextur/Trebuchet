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

            Client.Dispose();
        }

        public bool CheckForUpdates()
        {
            if (CheckVersion())
            {
                if (!System.IO.Directory.Exists(".//Releases"))
                {
                    System.IO.Directory.CreateDirectory(".//Releases");
                }

                if (!System.IO.File.Exists(".//Releases//Trebuchet-" + DevelopersVersion + "-" + DevelopersState + ".exe"))
                {
                    WebClient Client = new WebClient();
                    Client.DownloadFile("https://github.com/devMextur/Trebuchet/raw/master/bin/Debug/Trebuchet.exe", ".//Releases//Trebuchet-" + DevelopersVersion + "-" + DevelopersState + ".exe");
                    Client.Dispose();
                }

                Trebuchet.ThrowWarning("New version available: Trebuchet-" + DevelopersVersion + "-" + DevelopersState);
                Trebuchet.ThrowWarning("Look into your current folder for: 'Releases' to open the new Trebuchet.");
            }

            return CheckVersion();
        }

        public bool CheckVersion()
        {
            var ExecutingSplit = ExecutingVersion.Replace(".", string.Empty);
            var DeveloperSplit = DevelopersVersion.Replace(".", string.Empty);

            return int.Parse(DeveloperSplit) > int.Parse(ExecutingSplit);
        }
    }
}
