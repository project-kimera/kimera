using Kimera.Network.Utilities;
using Serilog;
using System;
using System.Globalization;
using System.Net.Cache;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Kimera.Converters
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string url = value.ToString();

                if (!string.IsNullOrEmpty(url) && WebHelper.IsImageUrlAsync(url).GetAwaiter().GetResult())
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                    image.CacheOption = BitmapCacheOption.OnDemand;
                    image.CreateOptions = BitmapCreateOptions.DelayCreation;
                    image.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                    image.EndInit();

                    return image;
                }
                else
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri("..\\Resources\\Images\\placeholder.png", UriKind.Relative);
                    image.EndInit();

                    return image;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while processing a image.");

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri("..\\Resources\\Images\\placeholder.png", UriKind.Relative);
                image.EndInit();

                return image;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
