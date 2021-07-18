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
                return new ValidationResult(false, "The text cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, "The text cannot be empty or consist only of spaces.");
            }

            return new ValidationResult(true, "The text is valid.");
        }
    }
}
