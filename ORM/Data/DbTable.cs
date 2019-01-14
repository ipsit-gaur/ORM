using Autofac;
using ORM.SQLServer;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (obj == null)
                throw new ArgumentNullException("Null object cannot be added");

            _data = _data ?? new List<T>();
            obj.State = DbEntityState.Added;
            _data.Add(obj);
        }

        public void Add(List<T> data)
        {
            if (data == null || data.Any(x => x == null))
                throw new ArgumentNullException("Null objects cannot be added");

            _data = _data ?? new List<T>();
            data.ForEach(x => { x.State = DbEntityState.Added; });
            _data.AddRange(data);
        }

        public void Update(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Null object cannot be updated");

            obj.State = DbEntityState.Modified;
        }

        public void Update(List<T> data)
        {
            if (data == null || data.Any(x => x == null))
                throw new ArgumentNullException("Null objects cannot be updated");

            data.ForEach(x => { x.State = DbEntityState.Modified; });
        }

        public void Save()
        {
            UpdateRecords();
            AddNewRecords();
        }

        private void AddNewRecords()
        {
            if (_data == null || _data.Count == 0 || !_data.Any(x => x.State == DbEntityState.Added))
                return;

            var query = _queryBuilder.PrepareQueryForInsert(_data.Where(x => x.State == DbEntityState.Added));
            _dataSourceManager.Execute(query);
        }

        private void UpdateRecords()
        {
            if (_data == null || _data.Count == 0 || !_data.Any(x => x.State == DbEntityState.Modified))
                return;

            var query = _queryBuilder.PrepareQueryForUpdate(_data.Where(x => x.State == DbEntityState.Modified));
            _dataSourceManager.Execute(query);
        }

        public int Maximum(Expression<Func<T, int>> predicate)
        {
            var query = _queryBuilder.PrepareQuery(_binaryPredicates, predicate, SQLServerKeywords.MAX);
            return 0;
        }
    }
}