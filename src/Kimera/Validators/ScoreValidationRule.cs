using System.Globalization;
using System.Windows.Controls;

namespace Kimera.Validators
{
    public class ScoreValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double score;

            if (double.TryParse((string)value, out score))
            {
                if (score >= 0.0 && score <= 5.0)
                {
                    return new ValidationResult(true, (string)App.Current.Resources["VALID_SCORE_VALID_MSG"]);
                }
                else
                {
                    return new ValidationResult(false, (string)App.Current.Resources["VALID_SCORE_RANGE_ERROR_MSG"]);
                }
            }
            else
            {
                return new ValidationResult(false, (string)App.Current.Resources["VALID_SCORE_NOT_VALID_MSG"]);
            }
        }
    }
}