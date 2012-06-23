using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public TextWriter ConsoleOut
        {
            get;
            set;
        }

        public TextReader ConsoleIn
        {
            get;
            set;
        }

        public void Run()
        {
            Console.Title = "Trebuchet Aplha 1.0.0";

            this.ConsoleOut = TextWriter.Synchronized(Console.Out);
            this.ConsoleIn = TextReader.Synchronized(Console.In);
            this.WriteCredits();
        }

        private void WriteCredits()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            this.PrintLine("                                                                               ");
            this.PrintLine("        ___  __   ___  __        __        ___ ___                             ");
            this.PrintLine("         |  |__) |__  |__) |  | /  ` |__| |__   |                              ");
            this.PrintLine("         |  |  \\ |___ |__) \\__/ \\__, |  | |___  |    REV 1.0.0.0               ");
            this.PrintLine("                                                                               ");
            this.PrintLine("                        developer by Mextur - Superior C# Frameworks.          ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            this.PrintLine("_______________________________________________________________________________");
            this.WriteLine();
        }

        public void WriteLine()
        {
            ConsoleOut.WriteLine();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public void WriteLine(string Line, params object[] Parameters)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            (ConsoleOut != null ? ConsoleOut : Console.Out).Write(" [LOG] ");
            Console.ForegroundColor = ConsoleColor.White;

            (ConsoleOut != null ? ConsoleOut : Console.Out).WriteLine(Line, Parameters);
        }

        public void PrintLine(string Line, params object[] Parameters)
        {
            (ConsoleOut != null ? ConsoleOut : Console.Out).WriteLine(Line, Parameters);
        }

        public void BlurLine(string Header, ConsoleColor HeaderColor, string Line, ConsoleColor LineColor, params object[] Parameters)
        {
            Console.ForegroundColor = HeaderColor;
            (ConsoleOut != null ? ConsoleOut : Console.Out).Write(" [{0}] ", Header);
            Console.ForegroundColor = LineColor;

            (ConsoleOut != null ? ConsoleOut : Console.Out).WriteLine(Line, Parameters);
        }

        public void Freeze()
        {
            Console.In.ReadToEnd();
        }
    }
}
