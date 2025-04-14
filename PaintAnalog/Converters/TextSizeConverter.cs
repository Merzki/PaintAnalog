using System;
using System.Globalization;
using System.Windows.Data;

namespace PaintAnalog.Converters
{
    public class TextSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as string;
            if (string.IsNullOrWhiteSpace(input)) return 12.0;

            if (double.TryParse(input, out var result))
                return result;

            return 12.0;
        }
    }
}
