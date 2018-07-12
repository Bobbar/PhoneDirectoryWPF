using PhoneDirectoryWPF.Data.Classes;
using PhoneDirectoryWPF.Data.Functions;

namespace PhoneDirectoryWPF.Data
{
    public static class Queries
    {
        public static string SelectExtensionById(string id)
        {
            return string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", Tables.Extensions.TableName, Tables.Extensions.ID, id);
        }

        public static string SelectExtensionByNumber(string number)
        {
            return string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", Tables.Extensions.TableName, Tables.Extensions.Number, number);
        }

        public static string SelectMapObject(DataMapObject mapObject)
        {
            return string.Format("SELECT * FROM {0} WHERE {1} ='{2}'", mapObject.TableName, mapObject.GetAttribute(nameof(mapObject.Guid)).ColumnName, mapObject.Guid);
        }

        /// <summary>
        /// SELECT * FROM <see cref="UsersCols.TableName"/> WHERE <see cref="UsersCols.UserName"/> = <paramref name="userName"/>
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string SelectUserByName(string userName)
        {
            return string.Format("SELECT * FROM {0} WHERE {1} = '{2}' LIMIT 1", Tables.Users.TableName, Tables.Users.UserName, userName);
        }

        /// <summary>
        /// SELECT * FROM <see cref="SecurityCols.TableName"/> ORDER BY <see cref="SecurityCols.AccessLevel"/>
        /// </summary>
        public static string SelectSecurityTable { get; } = string.Format("SELECT * FROM {0} ORDER BY {1}", Tables.Security.TableName, Tables.Security.AccessLevel);

    }
}