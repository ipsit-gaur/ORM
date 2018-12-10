using System;
using System.Linq;
using Autofac;

namespace ORM
{
    public abstract class DataContext : IDataContext
    {
        #region Private Variables
        private readonly IDataSourceManager _dataSourceManager;

        private string _connectionString;
        #endregion

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
            var dependencyResolver = new DependencyResolver();
            dependencyResolver.Register();
            _dataSourceManager = DependencyResolver.Container.Resolve<IDataSourceManager>();
            InitializeDataSets();
        }

        private void InitializeDataSets()
        {
            foreach (var property in this.GetType().GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(ODataSet<>)))
            {
                var listType = typeof(ODataSet<>);
                var constructedListType = listType.MakeGenericType(property.PropertyType.GetGenericTypeDefinition());

                var instance = Activator.CreateInstance(property.PropertyType);
                property.SetValue(this, instance);
            }
        }
    }
}
