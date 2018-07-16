using Database.Data;
using System.IO;
using PhoneDirectoryWPF.Security;
using PhoneDirectoryWPF.Data.Functions;
using PhoneDirectoryWPF.Helpers;


namespace PhoneDirectoryWPF.Data
{
    public static class DBFactory
    {
        public static bool CacheMode = false;

        private static string serverIp = "10.10.0.89";
        private static string sqlitePath = Directory.GetCurrentDirectory() + @"\cache\";

        public static IDatabase GetMySqlDatabase()
        {
            return new MySqlDatabase(serverIp, "phone_dir_user", "Ph0n3D1rU53rP455", "phone_test_db");
        }

        public static IDatabase GetSqliteDatabase()
        {
            return new SqliteDatabase(GetSqlPath(), "phonedirCacheP455");
        }

        public static string GetSqlPath()
        {
            if (!Directory.Exists(sqlitePath))
            {
                Directory.CreateDirectory(sqlitePath);
            }

            return sqlitePath + "cache.db";
        }

        public static bool CanReachServer()
        {
            try
            {
                var serverTime = GetMySqlDatabase().ExecuteScalarFromQueryString("SELECT NOW()").ToString();

                if (!string.IsNullOrEmpty(serverTime))
                {
                    CacheMode = false;
                    return true;

                }
            }
            catch
            {
                CacheMode = true;
                return false;
            }

            return false;
        }

       
    }
}