using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Components;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems
{
    static class Framework
    {        
        public static ICollection<ISystemComponent> SystemComponents
        {
            get;
            set;
        }

        public static void Boot()
        {            
            Framework.SystemComponents = new List<ISystemComponent>();

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
            Get<LogComponent>().Started = true;
            Get<LogComponent>().WriteLine("Welcome {0}, started booting at: {1}", Environment.UserName, DateTime.Now.ToShortTimeString());

            var CursorTop = Console.CursorTop;

            Get<LogComponent>().WriteLine("Starting components ({0}%)", TotPercent);

            try
            {
                foreach (var Component in SystemComponents)
                {
                    if (!Component.Started)
                    {
                        Component.Run();
                        Component.Started = true;
                    }

                    TotPercent += Percent;
                    Console.SetCursorPosition(0, CursorTop);
                    Get<LogComponent>().WriteLine("Starting components ({0}%)", (int)TotPercent);
                }

                Get<LogComponent>().WriteLine();
                Get<LogComponent>().BlurLine("LOG", ConsoleColor.DarkCyan, "Successfully booted framework, logging starts after this line:", ConsoleColor.Green);
            }
            catch
            {
                Get<LogComponent>().WriteLine();
                Get<LogComponent>().BlurLine("LOG", ConsoleColor.DarkCyan, "Failed booted framework, logging ends towards this line:", ConsoleColor.Red);
            }
            
            Get<LogComponent>().PrintLine("_______________________________________________________________________________");
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