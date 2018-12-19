using Autofac;
using System;
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
        private readonly IQueryBuilder _queryBuilder;

        public DbTable()
        {
            _dataSourceManager = DependencyResolver.Container.Resolve<IDataSourceManager>();
            _queryBuilder = DependencyResolver.Container.Resolve<IQueryBuilder>();
        }

        public List<T> Read()
        {
            return _dataSourceManager.Read<T>(PrepareQuery());
        }

        public DbTable<T> Filter(Func<T, bool> predicate)
        {
            // TODO: Apply filter logic here
            return null;
        }

        private string PrepareQuery()
        {
            return $"SELECT TOP 10 * FROM {typeof(T).Name}";
        }
    }
}