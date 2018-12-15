using Autofac;
using ORM.Common;
using ORM.SQL;
using System.Collections.Generic;
using System;

namespace ORM.Data
{
    /// <summary>
    /// Represents a Db Set of ORM Model
    /// </summary>
    /// <typeparam name="T">Type of the entity</typeparam>
    public sealed class DbTable<T> : Query<T> where T : DbEntity
    {
        private readonly IDataSourceManager _dataSourceManager;

        // TODO: Remove hardcoded dependency
        public DbTable() : base(new SQLQueryProvider())
        {
            _dataSourceManager = DependencyResolver.Container.Resolve<IDataSourceManager>();
        }

        public List<T> Read()
        {
            return _dataSourceManager.Read<T>(PrepareQuery());
        }

        private string PrepareQuery()
        {
            return $"SELECT TOP 10 * FROM {this.GetType().Name}";
        }
    }
}