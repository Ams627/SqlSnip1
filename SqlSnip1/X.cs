using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace SqlSnip1
{
    class X
    {
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

        void TestGetResults()
        {
            var results = GetResults(@"(localdb)\db1", "db1a", "select * from t1 where Name=@Name", new[] { new SqlParameter("Name", "One") });
            foreach (var i in results)
            {
                var line = string.Join(" ", i);
                Console.WriteLine($"{line}");
            }
        }

        public static DataTable GetTable(string server, string dbName, string sql, SqlParameter[] sqlParams)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = dbName,
                IntegratedSecurity = true
            };

            using (var connection = new SqlConnection(builder.ToString()))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddRange(sqlParams);
                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        private static void TestGetTable()
        {
            var table = GetTable(@"(localdb)\db1", "db1a", "select * from t1 where Name=@Name", new[] { new SqlParameter("Name", "One") });
            foreach (DataRow row in table.Rows)
            {
                foreach (var col in row.ItemArray)
                {
                    Console.WriteLine($"{col}");
                }
            }
        }
        public static int SqlNonQuery(string server, string dbName, string sql, SqlParameter[] sqlParams)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = dbName,
                IntegratedSecurity = true
            };

            using (var connection = new SqlConnection(builder.ToString()))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddRange(sqlParams);
                return command.ExecuteNonQuery();
            }
        }

        // test function - remove it as you will not need it!
        private static void TestNonQuery()
        {
            var nq = $@"DECLARE @sql nvarchar(500)
                        SET @sql = Convert(nvarchar(500), CONCAT('CREATE TABLE ', @tablename, '(id int)'))
                        exec sp_executesql  @sql";

            var result = SqlNonQuery(@"(localdb)\db1", "db1a", nq, new[] { new SqlParameter("tablename", "Mytable001e") });
        }
    }
}
