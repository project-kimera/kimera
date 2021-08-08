using Kimera.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kimera.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValid(this DependencyObject instance)
        {
            // Validate recursivly
            return !Validation.GetHasError(instance) && LogicalTreeHelper.GetChildren(instance).OfType<DependencyObject>().All(child => child.IsValid());
        }

        public static bool IsValidGameMetadata(this GameMetadata metadata)
        {
            if (string.IsNullOrEmpty(metadata.Name))
            {
                return false;
            }

            /*if (!new ScoreValidationRule().Validate(metadata.Score.ToString(), CultureInfo.CurrentCulture).IsValid)
            {
                return false;
            }

            if (!new UriValidationRule().Validate(metadata.IconUri, CultureInfo.CurrentCulture).IsValid)
            {
                return false;
            }

            if (!new UriValidationRule().Validate(metadata.ThumbnailUri, CultureInfo.CurrentCulture).IsValid)
            {
                return false;
            }*/

            return true;
        }
    }
}
