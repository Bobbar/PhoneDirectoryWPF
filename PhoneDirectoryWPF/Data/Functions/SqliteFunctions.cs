using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.Data;
using System.Data;
using System.Data.Common;

namespace PhoneDirectoryWPF.Data.Functions
{
    public static class SqliteFunctions
    {

        public static void AddTableToCacheDB(DataTable table, string primaryKeyColumn, DbTransaction trans = null)
        {
            // Get the create statement string.
            var createStatement = BuildCreateStatement(table, primaryKeyColumn);
            
            // Execute the statement to create the table.
            DBFactory.GetSqliteDatabase().ExecuteNonQuery(createStatement, trans);

            // Populate the table with the data.
            DBFactory.GetSqliteDatabase().UpdateTable("SELECT * FROM " + table.TableName, CopyTable(table), trans);
        }


        /// <summary>
        /// Returns a copy of the specified table with all rows states set as added.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable CopyTable(DataTable table)
        {
            var newTable = table.Clone();

            foreach (DataRow row in table.Rows)
            {
                newTable.Rows.Add(row.ItemArray);
            }

            return newTable;
        }

        private static string BuildCreateStatement(DataTable table, string primaryKeyColumn)
        {
            string statement = "CREATE TABLE ";

            // Add the table name from the results parameter.
            // **REMEMEBER TO ADD THE TABLE NAME TO THE RESULTS DATATABLE BEFORE CALLING THIS FUNCTION**
            statement += " `" + table.TableName + "` ( ";

            // Iterate through the table columns.
            foreach (DataColumn column in table.Columns)
            {
                // Add the field/column name and data type to the statement.
                statement += "`" + column.ColumnName.ToString() + "` ";

                var type = column.DataType;
                if (type == typeof(string))
                {
                    statement += "varchar(" + column.MaxLength + ")";
                }
                else if (type == typeof(int) || type == typeof(Int16))
                {
                    statement += "int(" + column.MaxLength + ")";
                }
                else if (type == typeof(DateTime))
                {
                    statement += "datetime";
                }
                else if (type == typeof(sbyte) || type == typeof(byte) || type == typeof(bool))
                {
                    statement += "tinyint(1)";
                }
                else if (type == typeof(Decimal))
                {
                    statement += "float";
                }
                else
                {
                    throw new Exception("Unexpected type.");
                }

                // Add a column delimiter if we are not on the last item.
                if (table.Columns.IndexOf(column) != (table.Columns.Count - 1)) statement += ", ";
            }

            // Add primary keys declaration.
            statement += ", PRIMARY KEY (" + primaryKeyColumn + ")";

            // End of statement close parentheses.
            statement += ");";

            // Console.WriteLine(statement);

            return statement;
        }
    }
}
