using PhoneDirectoryWPF.Data.Classes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using System.Reflection;

namespace PhoneDirectoryWPF.Data.Functions
{
    public static class MapObjectFunctions
    {
        public static int Insert(this DataMapObject mapObject)
        {
            var selectQuery = "SELECT * FROM " + mapObject.TableName + " LIMIT 0";

            using (var results = DBFactory.GetMySqlDatabase().DataTableFromQueryString(selectQuery))
            {
                results.Rows.Add();
                PopulateRowFromObject(results.Rows[0], mapObject);

                return DBFactory.GetMySqlDatabase().UpdateTable(selectQuery, results);
            }
        }

        public static int Update(this DataMapObject mapObject)
        {
            var selectQuery = "SELECT * FROM " + mapObject.TableName + " WHERE " + mapObject.GetAttribute(nameof(mapObject.Guid)).ColumnName + " = '" + mapObject.Guid + "'";

            using (var results = DBFactory.GetMySqlDatabase().DataTableFromQueryString(selectQuery))
            {
                PopulateRowFromObject(results.Rows[0], mapObject);
                return DBFactory.GetMySqlDatabase().UpdateTable(selectQuery, results);
            }
        }

        public static DataColumnNameAttribute GetAttribute(this DataMapObject source, string properyName)
        {
            var prop = source.GetType().GetProperty(properyName);

            if (prop == null)
                return null;

            var attr = prop.GetCustomAttribute<DataColumnNameAttribute>(true);
            return attr;
        }

        private static void PopulateRowFromObject(DataRow row, object obj)
        {
            // Collect list of all properties in the object class.
            List<System.Reflection.PropertyInfo> props = obj.GetType().GetProperties().ToList();

            // Iterate through the properties.
            foreach (var prop in props)
            {
                // Check if the property contains a target attribute.
                if (prop.GetCustomAttributes(typeof(DataColumnNameAttribute), true).Length > 0)
                {
                    // Get the column name attached to the property.
                    var columnName = ((DataColumnNameAttribute)prop.GetCustomAttributes(false)[0]).ColumnName;

                    // Make sure the DataTable contains a matching column name.
                    if (row.Table.Columns.Contains(columnName))
                    {
                        // Set row column to property value.
                        var propValue = prop.GetValue(obj);

                        if (propValue != null)
                        {
                            row[columnName] = propValue;
                        }
                    }
                }
                else
                {
                    // If the property does not contain a target attribute, check to see if it is a nested class inheriting the DataMapping class.
                    if (typeof(DataMapObject).IsAssignableFrom(prop.PropertyType))
                    {
                        // Recurse with nested DataMapping properties.
                        var nestObject = prop.GetValue(obj, null);
                        PopulateRowFromObject(row, nestObject);
                    }
                }
            }
        }
    }
}