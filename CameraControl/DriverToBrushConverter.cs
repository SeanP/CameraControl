using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CameraControl
{
    class DriverToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is Driver))
            {
                System.Environment.Exit(-1);
            }

            string cc = (value as Driver).CarClass;
            if (cc.Contains("P2"))
            {
                return new SolidColorBrush(Color.FromRgb(200, 220, 255));
            }
            else if (cc.Contains("GT1"))
            {
                return new SolidColorBrush(Color.FromRgb(200, 255, 230));
            }
            else if (cc.Contains("GT2"))
            {
                return new SolidColorBrush(Color.FromRgb(230, 230, 200));
            }

            System.Environment.Exit(-1);
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
