using Autofac;
using ORM.Logger;
using System;
using System.Linq;

namespace ORM.Data
{
    /// <summary>
    /// DataContext class holding all Datatable sets
    /// </summary>
    public abstract class DataContext : IDataContext
    {
        #region Private Variables
        private string _connectionString;

        private readonly IDataSourceManager _dataSourceManager;
        private readonly LogManager _logManager;
        #endregion

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
            new DependencyResolver().Register();

            _dataSourceManager = DependencyResolver.Container.Resolve<IDataSourceManager>();
            _logManager = DependencyResolver.Container.Resolve<LogManager>();
            InitializeDataSets();
        }

        /// <summary>
        /// Initializes all the Datasets to a new instance
        /// </summary>
        private void InitializeDataSets()
        {
            _logManager.Info("Datasets initialized");
            foreach (var property in this.GetType().GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbTable<>)))
            {
                var listType = typeof(DbTable<>);
                var constructedListType = listType.MakeGenericType(listType.GetGenericArguments().FirstOrDefault());

                var instance = Activator.CreateInstance(property.PropertyType);
                property.SetValue(this, instance);
            }
        }
    }
}
