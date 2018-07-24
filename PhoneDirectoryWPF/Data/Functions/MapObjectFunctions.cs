using PhoneDirectoryWPF.Data.Classes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Data.Functions
{
    public static class MapObjectFunctions
    {
        public static async Task<int> InsertAsync(this DataMapObject mapObject)
        {
            var selectQuery = "SELECT * FROM " + mapObject.TableName + " LIMIT 0";

            using (var results = await DBFactory.GetMySqlDatabase().DataTableFromQueryStringAsync(selectQuery))
            {
                var newRow = results.Rows.Add();
                newRow[Tables.Extensions.ModifyUser] = Security.SecurityFunctions.LocalUser.UserName;
                PopulateRowFromObject(newRow, mapObject);

                return await DBFactory.GetMySqlDatabase().UpdateTableAsync(selectQuery, results);
            }
        }

        public static async Task<int> UpdateAsync(this DataMapObject mapObject)
        {
            var selectQuery = "SELECT * FROM " + mapObject.TableName + " WHERE " + mapObject.GetAttribute(nameof(mapObject.Guid)).ColumnName + " = '" + mapObject.Guid + "'";

            using (var results = await DBFactory.GetMySqlDatabase().DataTableFromQueryStringAsync(selectQuery))
            {
                results.Rows[0][Tables.Extensions.ModifyUser] = Security.SecurityFunctions.LocalUser.UserName;

                PopulateRowFromObject(results.Rows[0], mapObject);
                return await DBFactory.GetMySqlDatabase().UpdateTableAsync(selectQuery, results);
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

                        if (propValue == null)
                        {
                            row[columnName] = DBNull.Value;
                        }
                        else
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