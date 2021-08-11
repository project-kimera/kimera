using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace Kimera.Validators
{
    public class FilePathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!File.Exists((string)value))
            {
                return new ValidationResult(false, (string)App.Current.Resources["VALID_FILEPATH_NOT_FOUND_MSG"]);
            }

            return new ValidationResult(true, (string)App.Current.Resources["VALID_FILEPATH_OK_MSG"]);
        }
    }
}