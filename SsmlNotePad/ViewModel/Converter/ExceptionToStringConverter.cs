using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="Exception"/> values to display text.
    /// </summary>
    [ValueConversion(typeof(Exception), typeof(string))]
    public class ExceptionToStringConverter : DependencyObject, IValueConverter
    {
        /// <summary>
        /// Converts a <seealso cref="FileSaveStatus"/> value to display text.
        /// </summary>
        /// <param name="value">The <seealso cref="Exception"/> object produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="Exception"/> value converted to display text.</returns>
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
            return Convert(value as Exception, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
