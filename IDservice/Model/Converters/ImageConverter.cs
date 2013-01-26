using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace IDservice.Model.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            try
            {
                if (!File.Exists((string) value)) return null;
                var objImage = new BitmapImage();
                objImage.BeginInit();
                objImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                objImage.UriSource = new Uri((string)value, UriKind.RelativeOrAbsolute);
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
