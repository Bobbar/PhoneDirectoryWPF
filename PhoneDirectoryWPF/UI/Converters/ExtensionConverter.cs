using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using PhoneDirectoryWPF.Helpers;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Formats for extensions and phone numbers.
    /// </summary>
    public sealed class ExtensionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var numString = value.ToString().DigitsOnly();

            // If the length is 10, it is a phone number, format accordingly.
            if (numString.Length == 10)
            {
                var formatInteger = System.Convert.ToInt64(numString);
                return string.Format("{0:(###) ###-####}", formatInteger);
            }

            // Otherwise it must be an extension, return the unformatted value.
            return numString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return value.ToString().DigitsOnly();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}