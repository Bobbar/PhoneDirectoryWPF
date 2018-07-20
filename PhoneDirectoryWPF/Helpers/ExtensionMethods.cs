using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Helpers
{
    public static class ExtensionMethods
    {
        // Copied from WPFToolkit System.Windows.Controls.Extensions internal class.
        public static bool Contains(this string s, string value, StringComparison comparision)
        {
            return s.IndexOf(value, comparision) >= 0;
        }
    }
}
