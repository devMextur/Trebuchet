using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems;
using Trebuchet.Systems.Components;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet
{
    static class Trebuchet
    {
        public static bool Authenticated
        {
            get;
            set;
        }

        public static Queue<string> WarningQueue
        {
            get;
            set;
        }

        [MTAThread]
        public static void Main()
        {
            Authenticated = false;
            WarningQueue = new Queue<string>();

            Framework.Boot();
            
            Authenticated = true;

            for (int i = 0; i < WarningQueue.Count; i++)
            {
                ThrowWarning(WarningQueue.Dequeue());
            }

            Framework.Get<LogComponent>().Freeze();
        }

        public static void ThrowException(string Message)
        {
            Framework.Get<LogComponent>().BlurLine("ERR", ConsoleColor.DarkRed, Message, ConsoleColor.Red);
        }

        public static void ThrowWarning(string Message)
        {
            if (Authenticated)
            {
                Framework.Get<LogComponent>().BlurLine("WAR", ConsoleColor.Yellow, Message, ConsoleColor.Yellow);
            }
            else WarningQueue.Enqueue(Message);
        }
    }
}
