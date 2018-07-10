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

    public static class DataColumnNameAttributeExtensions
    {
        public static DataColumnNameAttribute GetAttribute(this DataMapObject source, string properyName)
        {
            var prop = source.GetType().GetProperty(properyName);

            if (prop == null)
                return null;

            var attr = prop.GetCustomAttribute<DataColumnNameAttribute>(true);
            return attr;
        }
    }
}