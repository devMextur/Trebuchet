using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Systems.Interfaces
{
    interface ISystemComponent
    {
        bool Started { get; set; }
        void Run();
    }
}
