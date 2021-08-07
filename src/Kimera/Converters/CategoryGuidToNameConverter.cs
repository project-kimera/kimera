using Kimera.Data.Entities;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Kimera.Converters
{
    [ValueConversion(typeof(Guid), typeof(string))]
    public class CategoryGuidToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid categoryGuid = (Guid)value;

            Category category = App.DatabaseContext.Categories.Where(c => c.SystemId == categoryGuid).FirstOrDefault();

            if (category != null)
            {
                return category.Name;
            }
            else
            {
                return "NULL";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
