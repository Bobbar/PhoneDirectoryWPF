using System;
using System.Reflection;

namespace PhoneDirectoryWPF.Data.Classes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DataColumnNameAttribute : Attribute
    {
        private string _columnName;

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        public DataColumnNameAttribute()
        {
            _columnName = string.Empty;
        }

        public DataColumnNameAttribute(string columnName)
        {
            _columnName = columnName;
        }
    }
}