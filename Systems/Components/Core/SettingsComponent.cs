using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trebuchet.Systems.Interfaces;

namespace Trebuchet.Systems.Components.Core
{
    class SettingsComponent : ISystemComponent
    {
        public bool Started
        {
            get;
            set;
        }

        public IReadOnlyDictionary<string, object> Settings
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<Type, MethodInfo> Convertors
        {
            get;
            private set;
        }

        public string SETTINGS_PATH
        {
            get
            {
                return ".//Assets//Settings.xml";
            }
        }

        public void Run()
        {
            Dictionary<string, object> Items = new Dictionary<string, object>();

            if (!System.IO.Directory.Exists(".//Assets"))
            {
                System.IO.Directory.CreateDirectory(".//Assets");
                Trebuchet.ThrowWarning("Assets folder not found, Trebuchet created one.");
            }

            if (!System.IO.File.Exists(SETTINGS_PATH))
            {
                if (DownloadXMLSettings())
                {
                    Trebuchet.ThrowWarning("XML file not found, Trebuchet synchronized it.");
                }
                else
                {
                    Trebuchet.ThrowException("XML file not found / Trebuchet failed to synchronize.");
                    return;
                }
            }

            var Lines = System.IO.File.ReadAllLines(SETTINGS_PATH);

            if (Lines.Count() == 0)
            {
                Trebuchet.ThrowException("Invalid Settings file,");
                Trebuchet.ThrowException("Make sure the Settings file is not empty.");
                return;
            }

            if (Lines[0] != "<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
            {
                Trebuchet.ThrowException("Invalid Settings file,");
                Trebuchet.ThrowException("Make sure the Settings file is not broken.");
                return;
            }

            foreach (string Line in File.ReadAllLines(SETTINGS_PATH))
            {
                var Split = Line.Split('<', '>');

                if (!Line.StartsWith("<") && !Line.EndsWith(">") && Split.Count() != 5)
                {
                    continue;
                }

                Items.Add(Split[1], Split[2]);
            }

            Settings = new Dictionary<string, object>(Items);

            Dictionary<Type, MethodInfo> ConvertorItems = new Dictionary<Type, MethodInfo>();

            foreach (MethodInfo Method in typeof(Convertors).GetMethods())
            {
                if (Method.IsStatic)
                {
                    ConvertorItems.Add(Method.ReturnType, Method);
                }
            }

            Convertors = new Dictionary<Type, MethodInfo>(ConvertorItems);
        }

        public bool DownloadXMLSettings()
        {
            try
            {
                WebClient WebClient = new WebClient();
                WebClient.DownloadFile("https://raw.github.com/devMextur/Trebuchet/master/Assets/Settings.xml", SETTINGS_PATH);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryPop<T>(string Key, out T Output)
        {
            Output = default(T);

            try
            {
                object Value;

                Settings.TryGetValue(Key, out Value);

                if (Value != null)
                {
                    if (Convertors.ContainsKey(typeof(T)))
                    {
                        Output = (T)Convertors[typeof(T)].Invoke(null, new object[] { Value });
                    }
                }

                return !Output.Equals(default(T));
            }
            catch { return false; }
        }
    }

    static class Convertors
    {
        public static string ConvertString(object Input)
        {
            return Input.ToString();
        }

        public static int ConvertInteger(object Input)
        {
            int Output = default(int);
            int.TryParse(Input.ToString(), out Output);
            return Output;
        }

        public static uint ConvertUniversalInteger(object Input)
        {
            uint Output = default(uint);
            uint.TryParse(Input.ToString(), out Output);
            return Output;
        }

        public static bool ConvertBoolean(object Input)
        {
            bool Output = default(bool);
            bool.TryParse(Input.ToString(), out Output);
            return Output;
        }

        public static IPAddress ConvertIPAddress(object Input)
        {
            IPAddress Output = default(IPAddress);
            IPAddress.TryParse(Input.ToString(), out Output);
            return Output;
        }
    }
}
