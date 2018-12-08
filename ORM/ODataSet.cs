using System.Collections.Generic;

namespace ORM
{
    /// <summary>
    /// Represents a Db Set of ORM Model
    /// </summary>
    /// <typeparam name="T">Type of the entity</typeparam>
    public sealed class ODataSet<T> where T : class
    {
        private List<T> _data;
    }

    public static class DataSetExtensions
    {
        public static List<T> ToList<T>(this ODataSet<T> data) where T : class
        {
            // Call DataProcessor here and map the Response to Entity class
            return new List<T>();
        }
    }
}


// DataSet will have a DataSourceProcessor acc to connection which will generate Query for DS and sending to DB