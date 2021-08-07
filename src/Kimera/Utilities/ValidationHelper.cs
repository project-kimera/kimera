﻿using Kimera.Data.Entities;
using Kimera.Validators;
using System.Globalization;
using System.Linq;
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

            if (!new ScoreValidationRule().Validate(metadata.Score.ToString(), CultureInfo.CurrentCulture).IsValid)
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
            }

            return true;
        }
    }
}
