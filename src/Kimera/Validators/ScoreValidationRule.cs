using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    return new ValidationResult(true, "The score is valid.");
                }
                else
                {
                    return new ValidationResult(false, "The score must be from 0 to 5 or less.");
                }
            }
            else
            {
                return new ValidationResult(false, "The text is not valid score.");
            }
        }
    }
}
