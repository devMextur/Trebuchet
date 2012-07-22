using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Trebuchet.Systems.Components.Storage;

namespace Trebuchet.Classes.Storage
{
    class QueryStream : IDisposable
    {
        public string Query
        {
            get;
            private set;
        }

        public List<MySqlParameter> Parameters
        {
            get;
            private set;
        }

        public QueryStream(string Query)
        {
            this.Query = Query;
            this.Parameters = new List<MySqlParameter>();
        }

        public void AddParameter(string Name, object Obj)
        {
            this.Parameters.Add(new MySqlParameter(Name, Obj));
        }

        public T Excecute<T>()
        {
            var ConnectionString = Framework.Get<SQLComponent>().ConnectionString;

            try
            {
                if (typeof(T) == typeof(DataTable))
                {
                    return (T)(object)MySqlHelper.ExecuteDataset(ConnectionString, Query, Parameters.ToArray()).Tables[0];
                }
                else if (typeof(T) == typeof(DataRow))
                {
                    return (T)(object)MySqlHelper.ExecuteDataRow(ConnectionString, Query, Parameters.ToArray());
                }
                else if (typeof(T) == typeof(Action))
                {
                    MySqlHelper.ExecuteNonQuery(ConnectionString, Query, Parameters.ToArray());
                }
                else return (T)(object)MySqlHelper.ExecuteScalar(ConnectionString, Query, Parameters.ToArray());
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

            return default(T);
        }

        public void Dispose()
        {
            Query = null;
            Parameters = null;
            GC.SuppressFinalize(this);
        }
    }
}
