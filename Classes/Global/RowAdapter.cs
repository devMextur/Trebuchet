using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Global
{
    class RowAdapter : IDisposable
    {
        public DataRow Row
        {
            get;
            set;
        }

        public RowAdapter(DataRow Row)
        {
            this.Row = Row;
        }

        public T Get<T>(string Key)
        {
            try
            {
                return (T)Row[Key];
            }
            catch { }

            return default(T);
        }

        public void Dispose() { }
    }
}
