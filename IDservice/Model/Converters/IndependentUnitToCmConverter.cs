using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IDservice.Model.Converters
{
    public class IndependentUnitToCmConverter : IValueConverter
    {
        private const double factor = 37.795275590551181102362204724409;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;
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
