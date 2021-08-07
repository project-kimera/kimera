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
                return new ValidationResult(true, "The text is valid URI.");
            }

            return new ValidationResult(false, "The text is not valid URI.");
        }
    }
}
