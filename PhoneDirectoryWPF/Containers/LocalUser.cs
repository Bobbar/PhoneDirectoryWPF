using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Containers
{
    public sealed class LocalUser
    {
        public string UserName { get; }
        public string Fullname { get; }
        public int AccessLevel { get; }
        public string Guid { get; }

        public LocalUser() : this(string.Empty, string.Empty, 0, string.Empty)
        {
        }

        public LocalUser(string userName, string fullName, int accessLevel, string guid)
        {
            UserName = userName;
            Fullname = fullName;
            AccessLevel = accessLevel;
            Guid = guid;
        }
    }
}
