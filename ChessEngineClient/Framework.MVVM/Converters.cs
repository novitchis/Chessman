using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Framework.MVVM.Converters
{
    public enum ConversionType
    {
        Normal,
        Inverted,
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility result = (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
            ConversionType conversionType = parameter != null ? (ConversionType)parameter : ConversionType.Normal;

            if (conversionType == ConversionType.Inverted)
                result = result == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool result = value is Visibility && (Visibility)value == Visibility.Visible;
            ConversionType conversionType = parameter != null ? (ConversionType)parameter : ConversionType.Normal;

            if (conversionType == ConversionType.Inverted)
                result = !result;

            return result;
        }
    }

    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            object enumParameter = Enum.Parse(value.GetType(), (string)parameter);

            if (value.Equals(enumParameter))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public class TimeSpanToDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new Duration((TimeSpan)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public class DoubleNegatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            return (double)value * -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (double)value * -1;
        }
    }
 
}
