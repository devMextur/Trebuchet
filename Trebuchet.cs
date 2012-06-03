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

        public static void Main()
        {
            #region Boot Components
            Trebuchet.SystemComponents = new List<ISystemComponent>();

            foreach (Type Type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (Type.GetInterfaces().Contains(typeof(ISystemComponent)))
                {
                    ISystemComponent Component = Type.GetConstructor(new Type[] {}).Invoke(new object[] {}) as ISystemComponent;

                    Component.Run();

                    SystemComponents.Add(Component);
                }
            }
            #endregion

            Get<LogComponent>().Freeze();
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
