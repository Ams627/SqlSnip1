using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SqlSnip1
{
    class Program
    {
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

        public static DataTable RunStoredProcedureGetDataTable(string server, string dbName, string procname, SqlParameter[] sqlParams)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = dbName,
                IntegratedSecurity = true
            };

            using (var connection = new SqlConnection(builder.ToString()))
            using (var command = new SqlCommand(procname, connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(sqlParams);
                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }


        public static List<List<object>> RunStoredProcedureGetList(string server, string dbName, string procname, SqlParameter[] sqlParams)
        {
            var list = new List<List<object>>();

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = dbName,
                IntegratedSecurity = true
            };

            using (var connection = new SqlConnection(builder.ToString()))
            using (var command = new SqlCommand(procname, connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(sqlParams);
                var adapter = new SqlDataAdapter(command);
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
                    return list;
                }
            }
        }
        public static object RunStoredProcedure(string server, string dbName, string procname, SqlParameter[] sqlParams)
        {
            var list = new List<List<object>>();

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = dbName,
                IntegratedSecurity = true
            };

            using (var connection = new SqlConnection(builder.ToString()))
            using (var command = new SqlCommand(procname, connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(sqlParams);
                return command.ExecuteScalar();
            }
        }

        public static void RunStoredProcedureGetScalar(string server, string dbName, string procname, SqlParameter[] sqlParams)
        {

        }


        // test function - remove it as you will not need it!
        private static void TestIt()
        {
            var nq = $@"CREATE OR ALTER PROCEDURE LOGINFO @Message NVARCHAR(100) AS
                        BEGIN
	                        SELECT CONCAT(N'INFO:', @Message)
                        END
                        ";
            var result = SqlNonQuery(@"(localdb)\db1", "db1a", nq, new SqlParameter[] { });

            var nq2 = $@"CREATE OR ALTER PROCEDURE STOREINFO @Message NVARCHAR(100) AS
                        BEGIN
	                        INSERT INTO LIST VALUES(@Message)
                        END
                        ";
            var result2 = SqlNonQuery(@"(localdb)\db1", "db1a", nq2, new SqlParameter[] { });


            var dataTable = RunStoredProcedureGetDataTable(@"(localdb)\db1", "db1a", "LOGINFO", new[] { new SqlParameter("Message", "Hello") });
            var list = RunStoredProcedureGetList(@"(localdb)\db1", "db1a", "LOGINFO", new[] { new SqlParameter("Message", "Hello") });
            RunStoredProcedure(@"(localdb)\db1", "db1a", "STOREINFO", new[] { new SqlParameter("Message", "Hello") });
        }

        private static void Main(string[] args)
        {
            try
            {

                TestIt();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex}");
            }
        }
    }
}
