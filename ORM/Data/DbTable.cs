using Autofac;
using ORM.SQL;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        private Expression<Func<T, bool>> _predicate;

        public DbTable()
        {
            _dataSourceManager = DependencyResolver.Container.Resolve<IDataSourceManager>();
            _queryBuilder = DependencyResolver.Container.Resolve<IQueryBuilder>();
        }

        public List<T> Read()
        {
            var query = _queryBuilder.GetQuery(_predicate);
            return _dataSourceManager.Read<T>(query);
        }

        public DbTable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            _predicate = predicate;
            return this;
        }
    }
}