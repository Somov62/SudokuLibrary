using System;
using System.Globalization;
using System.Windows.Data;

namespace SudokuWPF
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fontSize = System.Convert.ToDouble(value);
            if (fontSize == 0) return 1;
            return fontSize - 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
