using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Components;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet
{
    static class Trebuchet
    {
        public static ICollection<ISystemComponent> SystemComponents
        {
            get;
            set;
        }

        [MTAThread]
        public static void Main()
        {
            #region Boot Components
            Trebuchet.SystemComponents = new List<ISystemComponent>();

            foreach (Type Type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (Type.GetInterfaces().Contains(typeof(ISystemComponent)))
                {
                    ISystemComponent Component = Type.GetConstructor(new Type[] {}).Invoke(new object[] {}) as ISystemComponent;

                    SystemComponents.Add(Component);
                }
            }

            double Percent = (double)(100 / (double)SystemComponents.Count);
            double TotPercent = 0;

            Get<LogComponent>().Run();
            Get<LogComponent>().WriteLine("Welcome {0}", Environment.UserName);

            var CursorTop = Console.CursorTop;

            Get<LogComponent>().WriteLine("Starting components ({0}%)", TotPercent);

            foreach (var Component in SystemComponents)
            {
                Component.Run();
                TotPercent += Percent;
                Console.SetCursorPosition(0, CursorTop);
                Get<LogComponent>().WriteLine("Starting components ({0}%)", (int)TotPercent);
            }

            #endregion

            Get<LogComponent>().Freeze();
        }

        public static void ThrowException(string Message)
        {
            Get<LogComponent>().BlurLine("ERR", ConsoleColor.DarkRed, Message, ConsoleColor.Red);
        }

        public static T Get<T>()
        {
            foreach (ISystemComponent Component in SystemComponents)
            {
                if (Component.GetType() == typeof(T))
                {
                    return (T)Component;
                }
            }

            return default(T);
        }
    }
}
