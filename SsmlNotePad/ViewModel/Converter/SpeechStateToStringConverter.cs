using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(SpeechState?), typeof(string))]
    public class SpeechStateToStringConverter : DependencyObject, IValueConverter
    {
        public string Convert(SpeechState? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return "";
            
            switch (value.Value)
            {
                case SpeechState.NotStarted:
                    return "Not Started";
                case SpeechState.Faulted:
                    return "Unexpected error";
                default:
                    return value.Value.ToString("F");
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(string)))
                return Convert(value as SpeechState?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
