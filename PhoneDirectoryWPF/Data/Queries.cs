using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.Data.Classes;

namespace PhoneDirectoryWPF.Data
{
    public static class Queries
    {
        public static string SelectExtensionById(string id)
        {
            return "SELECT * FROM " + Tables.Extensions.TableName + " WHERE " + Tables.Extensions.ID + " = '" + id + "'";
        }

        public static string SelectMapObject(DataMapObject mapObject)
        {
            // return "SELECT * FROM " + mapObject.TableName + " WHERE " + mapObject.GetAttribute(nameof(mapObject.Guid)) + " = '" + mapObject.Guid + "'";

            return string.Format("SELECT * FROM {0} WHERE {1} ='{2}'", mapObject.TableName, mapObject.GetAttribute(nameof(mapObject.Guid)).ColumnName, mapObject.Guid);
        }
    }
}
