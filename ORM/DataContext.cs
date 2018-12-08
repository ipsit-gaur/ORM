namespace ORM
{
    public abstract class DataContext : IDataContext
    {
        private string _connectionStringName;
        private IConnection _connection;

        public DataContext(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }

        private void ConnectToDB()
        {
            // Connect to DB here
        }
    }
}
