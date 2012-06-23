using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components
{
    class MemoryComponent : ISystemComponent
    {
        public bool Started
        {
            get;
            set;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

        public Timer GarbageTimer
        {
            get;
            set;
        }

        public void Run()
        {
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);

            this.GarbageTimer = new Timer(HandleGarbage, GarbageTimer, 0, 60000);
        }

        private void HandleGarbage(object Obj)
        {
            GC.Collect();
        }
    }
}
