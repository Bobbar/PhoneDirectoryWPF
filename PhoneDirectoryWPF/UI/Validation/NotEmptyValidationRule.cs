using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PhoneDirectoryWPF.UI
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value as string))
            {
                return new ValidationResult(false, "Field is required.");
            }
            else
            {
                return ValidationResult.ValidResult;
            }

            //return string.IsNullOrWhiteSpace((value ?? "").ToString())
            //    ? new ValidationResult(false, "Field is required.")
            //    : ValidationResult.ValidResult;
        }
    }
}
