using ORM.Data;
using ORM.Extensions;
using System.Collections.Generic;
using System.Configuration;
using System;
using ORM.Configuration;

namespace ORM.SQLServer
{
    // TODO: Should be in ORM.SQLServer Assembly
    internal class SQLServerManager : IDataSourceManager
    {
        private readonly SQLHelper _sqlHelper;

        public SQLServerManager(ORMConfiguration config)
        {
            // TODO: Resolve using DI
            _sqlHelper = new SQLHelper(config.ConnectionString);
        }

        public int Execute(string query)
        {
            return _sqlHelper.ExecuteNonQuery(query);
        }

        List<T> IDataSourceManager.Read<T>(string query)
        {
            var data = _sqlHelper.ExecuteReader(query);
            return data.Map<T>();
        }
    }
}
