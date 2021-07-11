using System.Globalization;
using System.Windows.Controls;

namespace Kimera.Validators
{
    public class NullStringValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return new ValidationResult(false, "The name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, "The name cannot be empty or consist only of spaces.");
            }

            // If it valid, return true.
            return new ValidationResult(true, null);
        }
    }
}
