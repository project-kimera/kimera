using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kimera.Validators
{
    public class FilePathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!File.Exists((string)value))
            {
                return new ValidationResult(false, "The file does not found.");
            }

            return new ValidationResult(true, "The text is valid file path.");
        }
    }
}
