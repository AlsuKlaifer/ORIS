using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORIS.week9
{
    public class MyORM
    {
        public IDbConnection connection = null;
        public IDbCommand command = null;

        public MyORM(string connectionString)
        {
            this.connection = new SqlConnection(connectionString);
            this.command = connection.CreateCommand();
        }

        public MyORM AddParameter<T>(string name, T value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.Value = value;
            command.Parameters.Add(param);
            return this;
        }

        public int ExecuteNonQuery(string query)
        {
            int noOfAffectedRows = 0;

            using(connection)
            {
                command.CommandText = query;
                connection.Open();
                noOfAffectedRows = command.ExecuteNonQuery();
            }

            return noOfAffectedRows;
        }
        public IEnumerable<T> ExecuteQuery<T>(string query)
        {
            IList<T> list = new List<T>();
            Type t = typeof(T);

            using(connection)
            {
                command.CommandText = query;

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    T obj = (T)Activator.CreateInstance(t);
                    t.GetProperties().ToList().ForEach(p =>
                    {
                        p.SetValue(obj, reader[p.Name]);
                    });

                    list.Add(obj);
                }
            }
            return list;
        }
        public T ExecuteScalar<T>(string query)
        {
            T result = default(T);
            using(connection)
            {
                command.CommandText = query;
                connection.Open();
                result = (T)command.ExecuteScalar();
            }

            return result;
        }
    }
}
