using ORM.Base.Exception;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ORM.SQLServer
{
    public class SQLHelper
    {
        private readonly string _connectionString;

        public SQLHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteReader(string query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(query, connection);

                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                SqlDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
                catch (SqlException ex)
                {
                    // TODO: Log here using logger
                    throw new DbException(ex.Message);
                }
                finally
                {
                    reader?.Close();
                }
            }
        }
    }
}
