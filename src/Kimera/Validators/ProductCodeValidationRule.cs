using Kimera.Network;
using Kimera.Services;
using Kimera.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
