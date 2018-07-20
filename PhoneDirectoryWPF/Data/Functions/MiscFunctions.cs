using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Data.Functions
{
    public static class MiscFunctions
    {
        public static async Task<List<object>> DepartmentList()
        {
            var depList = new List<object>();

            using (var results = await DBFactory.GetDatabase().DataTableFromQueryStringAsync(Queries.SelectDepartments))
            {
                foreach (DataRow row in results.Rows)
                {
                    if (!string.IsNullOrEmpty(row[Tables.Extensions.Department].ToString()))
                        depList.Add(row[Tables.Extensions.Department].ToString());
                }
            }

            return depList;
        }
    }
}