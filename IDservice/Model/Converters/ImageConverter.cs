using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace IDservice.Model.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var objImage = new BitmapImage();                
                objImage.BeginInit();
                objImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                objImage.UriSource = new Uri ((string)value, UriKind.RelativeOrAbsolute);
                objImage.CacheOption = BitmapCacheOption.OnLoad;
                objImage.EndInit();
                return objImage;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
