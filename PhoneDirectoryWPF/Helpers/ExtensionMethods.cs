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

        /// <summary>
        /// Removes all characters except digits and returns the concatenated result.
        /// </summary>
        public static string DigitsOnly(this string s)
        {
            // Get char array from value
            var charArray = s.ToCharArray();
            string digitString = string.Empty;

            // Iterate the array and concatenat only digit chars to a string.
            foreach (char val in charArray)
            {
                if (char.IsDigit(val))
                    digitString += val.ToString();
            }

            return digitString;
        }
    }
}
