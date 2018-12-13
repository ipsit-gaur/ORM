using ORM.Data;
using ORM.SQL;
using System.Collections.Generic;
using System.Configuration;

namespace ORM
{
    internal class SQLManager : IDataSourceManager
    {
        private readonly SQLHelper _sqlHelper;

        public SQLManager(string configName)
        {
            _sqlHelper = new SQLHelper(ConfigurationManager.AppSettings[configName]);
        }

        List<T> IDataSourceManager.Read<T>(string query)
        {
            var data = _sqlHelper.ExecuteReader(query);
            return data.Map<T>();
        }
    }
}
