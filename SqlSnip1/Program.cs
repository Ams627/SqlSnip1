using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SqlSnip1
{
    class Program
    {
        /// <summary>
        /// Return a result set as a List of Lists
        /// </summary>
        /// <param name="server">database server</param>
        /// <param name="dbName">database name</param>
        /// <param name="sql">sql to run</param>
        /// <param name="sqlParams">parameters for the query</param>
        /// <returns>A list of lists</returns>
        public static List<List<object>> GetResults(string server, string dbName, string sql, SqlParameter[] sqlParams)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = dbName,
                IntegratedSecurity = true
            };

            var list = new List<List<object>>();
            using (var connection = new SqlConnection(builder.ToString()))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddRange(sqlParams);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var innerList = new List<object>();
                        list.Add(innerList);

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            innerList.Add(reader[i]);
                        }
                    }
                }
            }
            return list;
        }

        private static void Main(string[] args)
        {
            try
            {
                var results = GetResults(@"(localdb)\db1", "db1a", "select * from t1 where Name=@Name", new[] { new SqlParameter("Name", "One") });
                foreach (var i in results)
                {
                    var line = string.Join(" ", i);
                    Console.WriteLine($"{line}");
                }
                Console.WriteLine("EXIT!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex}");
            }
        }
    }
}
