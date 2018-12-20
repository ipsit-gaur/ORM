using Autofac;
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
        #region Private variables
        private readonly IDataSourceManager _dataSourceManager;
        private readonly IQueryBuilder _queryBuilder;

        private Expression<Func<T, bool>> _predicate;
        #endregion

        public DbTable()
        {
            _dataSourceManager = DependencyResolver.Container.Resolve<IDataSourceManager>();
            _queryBuilder = DependencyResolver.Container.Resolve<IQueryBuilder>();
        }

        /// <summary>
        /// Returns the data of the table from the DB
        /// </summary>
        /// <returns>List of Entities</returns>
        public List<T> Read()
        {
            var query = _queryBuilder.GetQuery(_predicate);
            return _dataSourceManager.Read<T>(query);
        }

        /// <summary>
        /// Filters the DbTable through a predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>DbTable applied with the filter</returns>
        public DbTable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            _predicate = predicate;
            return this;
        }
    }
}