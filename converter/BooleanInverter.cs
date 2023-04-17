﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace TextCrypt.converter
{
    internal class BooleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool boolean) && !boolean;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool boolean) && !boolean;
        }
    }
}
