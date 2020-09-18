using System;
using System.Data;
using System.Data.SqlClient;
namespace SqlSnip1
{
    class X
    {
        public static DataTable GetTable(string server, string dbName, string sql, SqlParameter[] sqlParams)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = dbName,
                IntegratedSecurity = true
            };

            using (var connection = new SqlConnection(builder.ToString()))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddRange(sqlParams);
                    var adapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
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
    }
}
