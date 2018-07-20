using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Data
{
    public static class Tables
    {

        public static class Extensions
        {
            public static string TableName = "extensions";

            public static string ID = "id";
            public static string Number = "extension";
            public static string User = "user";
            public static string Department = "department";
            public static string Firstname = "firstname";
            public static string Lastname = "lastname";
        }

        public static class Security
        {
            public const string TableName = "security";
            public const string SecModule = "sec_module";
            public const string AccessLevel = "sec_access_level";
            public const string Description = "sec_desc";
        }

        public static class Users
        {
            public const string TableName = "users";
            public const string UserName = "usr_username";
            public const string FullName = "usr_fullname";
            public const string AccessLevel = "usr_access_level";
            public const string Guid = "usr_UID";
        }

    }
}
