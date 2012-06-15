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

        public void Run()
        {
            if (Started)
            {
                return;
            }

            Console.Title = "Trebuchet Aplha 1.0.0";

            this.ConsoleOut = TextWriter.Synchronized(Console.Out);
            this.ConsoleOut.WriteLine();
            this.Started = true;
        }

        public void WriteLine()
        {
            ConsoleOut.WriteLine();
        }

        public void WriteLine(string Line, params object[] Parameters)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            (ConsoleOut != null ? ConsoleOut : Console.Out).Write(" [LOG] ");
            Console.ForegroundColor = ConsoleColor.White;

            (ConsoleOut != null ? ConsoleOut : Console.Out).WriteLine(Line, Parameters);
        }

        public void BlurLine(string Header, ConsoleColor HeaderColor, string Line, ConsoleColor LineColor, params object[] Parameters)
        {
            Console.ForegroundColor = HeaderColor;
            (ConsoleOut != null ? ConsoleOut : Console.Out).Write(" [{0}] ",Header);
            Console.ForegroundColor = LineColor;

            (ConsoleOut != null ? ConsoleOut : Console.Out).WriteLine(Line, Parameters);
        }

        public void Freeze()
        {
            Console.In.ReadToEnd();
        }
    }
}
