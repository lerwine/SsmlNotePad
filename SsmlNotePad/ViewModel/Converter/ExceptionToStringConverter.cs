using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(Exception), typeof(string))]
    public class ExceptionToStringConverter : DependencyObject, IValueConverter
    {
        public virtual string Convert(Exception exception, object parameter, CultureInfo culture)
        {
            if (exception == null)
                return "";

            if (String.IsNullOrWhiteSpace(exception.Message))
                return exception.GetType().FullName;

            return exception.Message;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(string)))
                return Convert(value as Exception, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
