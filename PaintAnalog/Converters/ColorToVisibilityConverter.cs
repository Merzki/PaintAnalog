using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PaintAnalog.Converters
{
    public class ColorToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is SolidColorBrush selectedColor && values[1] is SolidColorBrush currentColor)
            {
                return selectedColor.Color == currentColor.Color ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 