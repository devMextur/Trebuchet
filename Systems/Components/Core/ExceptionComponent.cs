using System;
using System.Collections.Generic;
using System.IO;
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

        public StreamWriter Stream
        {
            get;
            set;
        }

        public void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += OnCatchException;

            if (File.Exists("Exceptions.txt"))
            {
                this.Stream = new StreamWriter("Exceptions.txt");
            }
            else
            {
                this.Stream = File.AppendText("Exceptions.txt");
            }
        }

        public void OnCatchException(object Location, UnhandledExceptionEventArgs Args)
        {
            var Exception = Args.ExceptionObject as Exception;

            Trebuchet.ThrowException(Exception.ToString());

            using (StreamWriter Stream = this.Stream)
            {
                Stream.WriteLine(Args.ExceptionObject.ToString());
                Stream.WriteLine(Environment.NewLine);
            }
        }
    }
}
