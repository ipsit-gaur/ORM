using Autofac;
using System.Collections.Generic;

namespace ORM.Data
{
    /// <summary>
    /// Represents a Db Set of ORM Model
    /// </summary>
    /// <typeparam name="T">Type of the entity</typeparam>
    public sealed class ODataSet<T> where T : class
    {
        private List<T> _data;
        private readonly IDataSourceManager _dataSourceManager;

        public ODataSet()
        {
            _dataSourceManager = DependencyResolver.Container.Resolve<IDataSourceManager>();
        }

        public List<T> ToList()
        {
            return _dataSourceManager.Read<T>("SELECT TOP 10 * FROM CATEGORIES");
        }
    }
}