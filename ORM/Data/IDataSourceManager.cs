using System.Collections.Generic;

namespace ORM.Data
{
    internal interface IDataSourceManager
    {
        List<T> Read<T>(string query) where T : class;
    }
}
