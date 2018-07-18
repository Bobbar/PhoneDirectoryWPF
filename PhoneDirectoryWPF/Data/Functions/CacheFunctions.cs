using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Data.Functions
{
    public static class CacheFunctions
    {
        private static bool refreshRunning = false;

        public static async void RefreshCache()
        {
            await Task.Run(() =>
            {
                if (refreshRunning)
                    return;

                refreshRunning = true;

                using (var trans = DBFactory.GetSqliteDatabase().StartTransaction())
                using (trans.Connection)
                {
                    try
                    {
                        DropTables(trans);

                        foreach (var table in TablesToCache())
                        {
                            CacheDBTable(table.TableName, table.PrimaryKey, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        refreshRunning = false;
                    }
                }
            });
        }

        public static bool VerifyCache()
        {
            bool allPresent = true;

            try
            {
                using (var tables = GetSqliteTables())
                {
                    if (tables.Rows.Count < 1) return false;

                    var expectedTables = TablesToCache();

                    foreach (DataRow row in tables.Rows)
                    {
                        if (!expectedTables.Exists(t => t.TableName == row["name"].ToString()))
                        {
                            allPresent = false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return allPresent;
        }

        private static void CacheDBTable(string tableName, string primaryKey, DbTransaction trans = null)
        {
            using (var results = DBFactory.GetMySqlDatabase().DataTableFromQueryString("SELECT * FROM " + tableName))
            {
                results.TableName = tableName;
                SqliteFunctions.AddTableToCacheDB(results, primaryKey, trans);
            }
        }

        private static void DropTables(DbTransaction trans)
        {
            using (var results = GetSqliteTables())
            {
                foreach (DataRow row in results.Rows)
                {
                    string dropQuery = "DROP TABLE " + row["name"];
                    DBFactory.GetSqliteDatabase().ExecuteNonQuery(dropQuery, trans);
                }
            }
        }

        private static DataTable GetSqliteTables()
        {
            string query = "SELECT * FROM sqlite_master WHERE type='table'";

            using (var results = DBFactory.GetSqliteDatabase().DataTableFromQueryString(query))
            {
                return results;
            }
        }

        private static List<CacheTable> TablesToCache()
        {
            var tables = new List<CacheTable>();
            tables.Add(new CacheTable(Tables.Extensions.TableName, Tables.Extensions.Number));
            tables.Add(new CacheTable(Tables.Security.TableName, Tables.Security.SecModule));
            tables.Add(new CacheTable(Tables.Users.TableName, Tables.Users.Guid));
            return tables;
        }

        private class CacheTable
        {
            public string TableName { get; private set; }
            public string PrimaryKey { get; private set; }

            public CacheTable(string tableName, string primaryKey)
            {
                this.TableName = tableName;
                this.PrimaryKey = primaryKey;
            }
        }
    }
}