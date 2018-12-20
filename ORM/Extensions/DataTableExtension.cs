using ORM.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ORM.Extensions
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
                    var value = dataRow[property.Name];
                    Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (value.GetType() == typeof(DBNull))
                        property.SetValue(obj, null);
                    else
                    {
                        object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                        property.SetValue(obj, safeValue, null);
                    }
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

                if ((property.PropertyType != matchingColumn.DataType) &&
                    !HasImplicitConversion(matchingColumn.DataType, property.PropertyType) &&
                    !IsNullableTypeofType(matchingColumn.DataType, property.PropertyType))
                    throw new InvalidOperationException($"Cannot cast {matchingColumn.DataType} to {property.PropertyType} of class {typeof(T).Name}");
            }
        }

        private static bool IsNullableTypeofType(Type source, Type destination)
        {
            if (destination.GetGenericArguments()[0] == source ||
                HasImplicitConversion(source, destination.GetGenericArguments()[0]))
                return true;
            return false;
        }

        private static bool HasImplicitConversion(Type source, Type destination)
        {
            var sourceCode = Type.GetTypeCode(source);
            var destinationCode = Type.GetTypeCode(destination);
            switch (sourceCode)
            {
                case TypeCode.SByte:
                    switch (destinationCode)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Byte:
                    switch (destinationCode)
                    {
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int16:
                    switch (destinationCode)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.UInt16:
                    switch (destinationCode)
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int32:
                    switch (destinationCode)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.UInt32:
                    switch (destinationCode)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    switch (destinationCode)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Char:
                    switch (destinationCode)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Single:
                    return (destinationCode == TypeCode.Double);
            }
            return false;
        }
    }
}
