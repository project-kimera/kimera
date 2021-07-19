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
            if (DLSiteService.IsValidProductCode((string)value))
            {
                return new ValidationResult(true, "The text is valid product code.");
            }

            return new ValidationResult(false, "The text is not valid product code.");
        }
    }
}
