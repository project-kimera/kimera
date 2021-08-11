using System.Globalization;
using System.Windows.Controls;

namespace Kimera.Validators
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return new ValidationResult(false, (string)App.Current.Resources["VALID_NOTEMPTY_EMPTY_MSG"]);
            }

            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, (string)App.Current.Resources["VALID_NOTEMPTY_WHITESPACE_MSG"]);
            }

            return new ValidationResult(true, (string)App.Current.Resources["VALID_NOTEMPTY_OK_MSG"]);
        }
    }
}