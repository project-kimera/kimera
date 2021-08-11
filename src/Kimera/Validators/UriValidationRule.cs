using System;
using System.Globalization;
using System.Windows.Controls;

namespace Kimera.Validators
{
    public class UriValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Uri outUri;

            if (Uri.TryCreate((string)value, UriKind.Absolute, out outUri) && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps || outUri.Scheme == Uri.UriSchemeFile))
            {
                return new ValidationResult(true, (string)App.Current.Resources["VALID_URI_VALID_MSG"]);
            }

            return new ValidationResult(false, (string)App.Current.Resources["VALID_URI_NOT_VALID_MSG"]);
        }
    }
}