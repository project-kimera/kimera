using Kimera.Network;
using System.Globalization;
using System.Windows.Controls;

namespace Kimera.Validators
{
    public class ProductCodeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string typeName;

            if (MetadataServiceProvider.TryValidProductCode((string)value, out typeName))
            {
                return new ValidationResult(true, "The text is valid product code.");
            }

            return new ValidationResult(false, "The text is not valid product code.");
        }
    }
}
