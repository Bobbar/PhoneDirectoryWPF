using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Helpers;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace PhoneDirectoryWPF.UI
{
    public class ValidExtensionValidationRule : ValidationRule
    {
        private const int extensionLength = 4;
        private const int phoneNumLength = 10;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var number = ((Extension)((BindingExpression)value).DataItem).Number;
            var length = number.DigitsOnly().Length;

            if (length == extensionLength || length == phoneNumLength)
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Invalid extension. (4-digit extension or 10-digit phone number)");
        }
    }
}