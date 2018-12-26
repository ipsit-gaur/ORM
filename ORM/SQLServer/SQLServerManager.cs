﻿using ORM.Data;
using ORM.Extensions;
using System.Collections.Generic;
using System.Configuration;

namespace ORM.SQLServer
{
    // TODO: Should be in ORM.SQLServer Assembly
    internal class SQLServerManager : IDataSourceManager
    {
        private readonly SQLHelper _sqlHelper;

        public SQLServerManager(string configName)
        {
            // TODO: Resolve using DI
            _sqlHelper = new SQLHelper(ConfigurationManager.AppSettings[configName]);
        }

        List<T> IDataSourceManager.Read<T>(string query)
        {
            var data = _sqlHelper.ExecuteReader(query);
            return data.Map<T>();
        }
    }
}