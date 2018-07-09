using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Database.Data;

namespace PhoneDirectoryWPF.Data
{
    public static class DBFactory
    {
        private static string serverIp = "10.10.0.89";

        public static IDatabase GetMySQLDatabase()
        {
            return new MySqlDatabase(serverIp, "phone_dir_user", "Ph0n3D1rU53rP455", "phone_directory");
        }

    }
}
