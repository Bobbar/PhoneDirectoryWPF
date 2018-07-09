using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhoneDirectoryWPF.Data.Classes;
using System.Data;

namespace PhoneDirectoryWPF.Containers
{
    public sealed class Extension : Data.Classes.MappableObject
    {
        public string ID { get; private set; }

        [DataColumnName("id")]
        public override string Guid { get { return ID; } set { ID = value; } }

        public override string TableName { get; set; } = "extensions";

        [DataColumnName("extension")]
        public string Number { get; private set; }

        [DataColumnName("user")]
        public string User { get; private set; }

        [DataColumnName("department")]
        public string Department { get; private set; }

        [DataColumnName("firstname")]
        public string FirstName { get; private set; }

        [DataColumnName("lastname")]
        public string LastName { get; private set; }

        public Extension(string id, string number, string firstName, string lastName)
        {
            this.ID = id;
            this.Number = number;
            this.FirstName = firstName;
            this.LastName = lastName;

            this.User = string.Format("{0}, {1}", this.LastName, this.FirstName);
        }

        public Extension(DataTable data) : base(data) { }

        public Extension(DataRow data) : base(data) { }
    }
}
