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
        public bool Started
        {
            get;
            set;
        }

        public void Run()
        {
            Assembly AssemblyCur = Assembly.GetExecutingAssembly();
            FileVersionInfo FileVersionCur = FileVersionInfo.GetVersionInfo(AssemblyCur.Location);

            var CurrentVersion = FileVersionCur.FileVersion;

            WebClient Client = new WebClient();
            var WebsiteSource = Client.DownloadString("https://raw.github.com/devMextur/Trebuchet/master/Properties/AssemblyInfo.cs");
            var Version = WebsiteSource.Split('\"')[WebsiteSource.Split('\"').Count() - 2];

            Console.WriteLine(Version);
        }
    }
}
