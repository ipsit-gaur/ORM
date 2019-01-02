using Autofac;
using ORM.SQLServer;
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

        private List<T> _data;
        private List<T> _dataToUpdate;
        private List<T> _dataToAdd;

        private List<Expression<Func<T, bool>>> _binaryPredicates;
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
            var query = _queryBuilder.PrepareQuery(_binaryPredicates);
            _data = _dataSourceManager.Read<T>(query);
            return _data;
        }

        /// <summary>
        /// Filters the DbTable through a predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>DbTable applied with the filter</returns>
        public DbTable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            _binaryPredicates = _binaryPredicates ?? new List<Expression<Func<T, bool>>>();
            _binaryPredicates.Add(predicate);
            return this;
        }

        public void Add(T obj)
        {
            _dataToAdd = _dataToAdd ?? new List<T>();
            _dataToAdd.Add(obj);
        }

        public void Add(List<T> data)
        {
            _dataToAdd = _dataToAdd ?? new List<T>();
            _dataToAdd.AddRange(data);
        }

        public void Save()
        {
            UpdateRecords();
            AddNewRecords();
        }

        private void AddNewRecords()
        {
            if (_dataToAdd == null || _dataToAdd.Count == 0)
                return;

            var query = _queryBuilder.PrepareQueryForInsert(_dataToAdd);
            _dataSourceManager.Execute(query);
        }

        private void UpdateRecords()
        {

        }

        public int Maximum(Expression<Func<T, int>> predicate)
        {
            var query = _queryBuilder.PrepareQuery(_binaryPredicates, predicate, SQLServerKeywords.MAX);
            return 0;
        }
    }
}