using System;
using System.Globalization;
using System.Windows.Data;

namespace IDservice.Model.Converters
{
    public class IndependentUnitToCmConverter : IValueConverter
    {
        private const double factor = 37.795275590551181102362204724409;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;
            if (parameter != null)
                return Math.Round(val/factor, 2);
            return val/factor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (value != null && double.TryParse(value.ToString(), out val))
                return val*factor;
            return 0;
        }
    }
}
