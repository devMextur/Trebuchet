using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trebuchet.Systems.Components.Core;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components
{
    class LogComponent : ISystemComponent
    {
        public bool Started
        {
            get;
            set;
        }

        public TextWriter ConsoleOutput
        {
            get;
            set;
        }

        public TextReader ConsoleInput
        {
            get;
            set;
        }

        public void Run()
        {
            Console.Title = "Trebuchet " + Framework.Get<AssemblyComponent>().ExecutingState + " " + Framework.Get<AssemblyComponent>().ExecutingVersion;

            this.ConsoleOutput = TextWriter.Synchronized(Console.Out);
            this.ConsoleInput = TextReader.Synchronized(Console.In);
            this.WriteCredits();
        }

        private void WriteCredits()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            this.PrintLine("                                                                               ");
            this.PrintLine("        ___  __   ___  __        __        ___ ___                             ");
            this.PrintLine("         |  |__) |__  |__) |  | /  ` |__| |__   |                              ");
            this.PrintLine("         |  |  \\ |___ |__) \\__/ \\__, |  | |___  |     {1} {0}{2}",
                Framework.Get<AssemblyComponent>().ExecutingVersion,
                Framework.Get<AssemblyComponent>().ExecutingState, 
                GetNulledEnd(Framework.Get<AssemblyComponent>().ExecutingVersion,
                Framework.Get<AssemblyComponent>().ExecutingState));
            this.PrintLine("                                                                               ");
            this.PrintLine("                        developer by Mextur - Superior C# Frameworks.          ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            this.PrintLine("_______________________________________________________________________________");
            Console.BackgroundColor = ConsoleColor.Black;
            this.WriteLine();
        }

        private string GetNulledEnd(string Version, string State)
        {
            var Output = string.Empty;
            var Length = Version.Length + State.Length + 1;
            var NulledLength = 25 - Length;

            for (int i = 0; i < NulledLength; i++)
            {
                Output += ' ';
            }

            return Output;
        }

        public void OutLineAsync(string Line = "", params object[] Parameters)
        {
            this.ConsoleOutput.WriteLineAsync(string.Format(Line, Parameters).ToArray(), 0, string.Format(Line, Parameters).Length);
        }

        public void OutAsync(string Line = "", params object[] Parameters)
        {
            this.ConsoleOutput.WriteAsync(string.Format(Line, Parameters).ToArray(), 0, string.Format(Line, Parameters).Length);
        }

        public void WriteLine(string Line = "", params object[] Parameters)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            if (Line.Length > 0)
            {
                this.OutAsync(" [LOG] ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            this.OutLineAsync(Line, Parameters);
        }

        public void PrintLine(string Line = "", params object[] Parameters)
        {
            this.OutLineAsync(Line, Parameters);
        }

        public void BlurLine(string Header = "LOG", 
            ConsoleColor HeaderColor = ConsoleColor.DarkCyan, 
            string Line = "", 
            ConsoleColor LineColor = ConsoleColor.White, 
            params object[] Parameters)
        {
            Console.ForegroundColor = HeaderColor;

            if (Line.Length > 0)
            {
                this.OutAsync(" [{0}] ", Header);
            }

            Console.ForegroundColor = LineColor;
            this.OutLineAsync(Line, Parameters);
        }

        public void Freeze()
        {
            ConsoleInput.ReadToEndAsync();
        }
    }
}
