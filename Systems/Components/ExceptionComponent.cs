using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components
{
    class ExceptionComponent : ISystemComponent
    {
        public bool Started
        {
            get;
            set;
        }

        public void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += OnCatchException;
        }

        public void OnCatchException(object Location, UnhandledExceptionEventArgs Args)
        {

        }
    }
}
