using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPicker.Converters
{
    public class DisplayMemberConverter : IValueConverter
    {
        public string DisplayMemberPath { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(DisplayMemberPath))
                return value?.ToString() ?? string.Empty;

            var prop = value.GetType().GetProperty(DisplayMemberPath);
            return prop?.GetValue(value)?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
