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
        [MTAThread]
        public static void Main()
        {
            Framework.Boot();
            Framework.Get<LogComponent>().Freeze();
        }

        public static void ThrowException(string Message)
        {
            Framework.Get<LogComponent>().BlurLine("ERR", ConsoleColor.DarkRed, Message, ConsoleColor.Red);
        }
    }
}
