﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Components;
using Trebuchet.Systems.Components.Core;
using Trebuchet.Systems.Components.Storage;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet
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

            Get<AssemblyComponent>().Run();
            Get<AssemblyComponent>().Started = true;

            Get<LogComponent>().Run();
            Get<LogComponent>().Started = true;

            if (Get<AssemblyComponent>().CheckForUpdates() == true)
            {
                return;
            }

            Get<AssemblyComponent>().CheckReferences();

            Get<LogComponent>().WriteLine("Welcome {0}, started booting at: {1}", Environment.UserName, DateTime.Now.ToShortTimeString());

            var CursorTop = Console.CursorTop;

            Get<LogComponent>().WriteLine("Starting components ({0}%)", TotPercent);

            try
            {
                var OldCursorTop = Console.CursorTop;

                foreach (var Component in SystemComponents)
                {
                    if (!Component.Started)
                    {
                        Component.Run();
                        Component.Started = true;
                    }

                    TotPercent += Percent;
                    OldCursorTop = Console.CursorTop;
                    Console.SetCursorPosition(0, CursorTop);
                    Get<LogComponent>().WriteLine("Starting components ({0}%)", (int)TotPercent);
                }

                if (OldCursorTop > 0)
                {
                    Console.SetCursorPosition(0, OldCursorTop);
                }

                Get<LogComponent>().WriteLine();
                Get<LogComponent>().BlurLine("LOG", ConsoleColor.DarkCyan, "Successfully booted framework, logging starts after this line:", ConsoleColor.Green);

                Console.Beep();
            }
            catch (Exception ex)
            {
                Trebuchet.ThrowException(ex.Message);

                Get<LogComponent>().WriteLine();
                Get<LogComponent>().BlurLine("LOG", ConsoleColor.DarkCyan, "Failed booted framework, logging ends towards this line:", ConsoleColor.Red);
            }
            
            Get<LogComponent>().PrintLine("_______________________________________________________________________________");
            Get<LogComponent>().WriteLine();
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