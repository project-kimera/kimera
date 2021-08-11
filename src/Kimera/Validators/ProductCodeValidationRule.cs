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
                return new ValidationResult(true, (string)App.Current.Resources["VALID_PRODUCTCODE_VALID_MSG"]);
            }

            return new ValidationResult(false, (string)App.Current.Resources["VALID_PRODUCTCODE_NOTVALID_MSG"]);
        }
    }
}