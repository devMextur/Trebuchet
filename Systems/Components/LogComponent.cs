using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components
{
    class LogComponent : ISystemComponent
    {
        public void Run() { }

        public void Freeze()
        {
            Console.In.ReadToEnd();
        }
    }
}
