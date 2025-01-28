using Avalonia.Data.Converters;
using System;
using System.Collections;
using System.Globalization;

namespace SimpleHardwareMonitor.Converters
{
    internal class SensorStatusConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isEmpty = value == null || (value is IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext());
            return isEmpty ? "Sensors not found" : string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
