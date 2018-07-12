using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.Data.Classes;
using PhoneDirectoryWPF.Data;
using System.Data;

namespace PhoneDirectoryWPF.Containers
{
    public sealed class AccessGroup : DataMapObject
    {
        #region Constructors

        public AccessGroup()
        {
        }

        public AccessGroup(DataRow data) : base(data)
        {
        }

        #endregion Constructors

        #region Properties

        [DataColumnName(Tables.Security.SecModule)]
        public string Name { get; set; }

        [DataColumnName(Tables.Security.Description)]
        public string Description { get; set; }

        public override string Guid { get; set; }

        [DataColumnName(Tables.Security.AccessLevel)]
        public int Level { get; set; }

        public override string TableName { get; set; } = Tables.Security.TableName;

        #endregion Properties
    }
}
