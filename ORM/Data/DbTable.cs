using Autofac;
using System.Collections.Generic;

namespace ORM.Data
{
    /// <summary>
    /// Represents a Db Set of ORM Model
    /// </summary>
    /// <typeparam name="T">Type of the entity</typeparam>
    public sealed class DbTable<T> where T : DbEntity
    {
        private readonly IDataSourceManager _dataSourceManager;

        public DbTable()
        {
            _dataSourceManager = DependencyResolver.Container.Resolve<IDataSourceManager>();
        }

        public List<T> ToList()
        {
            return _dataSourceManager.Read<T>("SELECT TOP 10 * FROM CATEGORIES");
        }
    }
}