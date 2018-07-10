using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Helpers
{
    public sealed class DBControlAttribute
    {
        public string ColumnName { get; private set; }

        public ControlSearchType SearchType { get; private set; }

        public DBControlAttribute() { }

        public DBControlAttribute(string columnName)
        {
            this.ColumnName = columnName;
            this.SearchType = ControlSearchType.EntireValue;
        }

        public DBControlAttribute(string columnName, ControlSearchType searchType)
        {
            this.ColumnName = columnName;
            this.SearchType = searchType;
        }
    }

    public enum ControlSearchType
    {
        EntireValue,
        StartsWith,
        EndsWith
    }
}
