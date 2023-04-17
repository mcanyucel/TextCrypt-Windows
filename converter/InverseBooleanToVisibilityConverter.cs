using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TextCrypt.converter
{
    internal class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null and bool)
            {
                if ((bool)value)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else
            {
                throw new ArgumentException("Value is not of bool type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("BooleanToVisibility conversion is one-way");
        }
    }
}
