using Kimera.Data.Entities;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Kimera.Converters
{
    [ValueConversion(typeof(Guid), typeof(string))]
    public class CategoryGuidToDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid categoryGuid = (Guid)value;

            Category category = App.DatabaseContext.Categories.Where(c => c.SystemId == categoryGuid).FirstOrDefault();

            if (category != null)
            {
                string details = $"{(string)App.Current.Resources["CONV_CGTODETAILS_PREFIX"]} : {category.Name}";
                return details;
            }
            else
            {
                string details = $"{(string)App.Current.Resources["CONV_CGTODETAILS_PREFIX"]}";
                return details;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}