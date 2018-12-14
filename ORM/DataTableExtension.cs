using ORM.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ORM
{
    internal static class DataTableExtension
    {
        internal static List<T> Map<T>(this DataTable dataTable) where T : DbEntity
        {
            if (dataTable == null)
                throw new ArgumentNullException("DataTable");

            ValidateSchema<T>(dataTable);

            return MapAndPrepareData<T>(dataTable);
        }

        private static List<T> MapAndPrepareData<T>(DataTable dataTable) where T : DbEntity
        {
            var result = new List<T>();
            var columns = dataTable.Columns;

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var obj = MapAndPrepareObject<T>(dataRow, columns);
                result.Add(obj);
            }

            return result;
        }

        private static T MapAndPrepareObject<T>(DataRow dataRow, DataColumnCollection columns) where T : DbEntity
        {
            var obj = Activator.CreateInstance<T>();

            foreach (var property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (columns[property.Name] != null)
                {
                    property.SetValue(obj, dataRow[property.Name]);
                }
            }
            return obj;
        }

        private static void ValidateSchema<T>(DataTable dataTable) where T : DbEntity
        {
            var tableColumns = dataTable.Columns;
            var type = typeof(T);
            foreach (var property in
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var matchingColumn = tableColumns[property.Name];

                // Assuming no Column is present in the entity so skipping
                // TODO: Make this configurable
                if (matchingColumn == null)
                    continue;

                if (property.PropertyType != matchingColumn.DataType)
                    throw new InvalidOperationException($"Cannot cast {matchingColumn.DataType} to {property.DeclaringType} of class {dataTable.TableName}");
            }
        }
    }
}
