using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SachoWifiManager.Base
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueTreatment { get; set; }

        public Visibility FalseTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BooleanToVisibilityConverter()
        {
            TrueTreatment = Visibility.Visible;
            FalseTreatment = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value != null && value is bool)
            {
                flag = (bool)value;
            }

            return (flag ? TrueTreatment : FalseTreatment);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
