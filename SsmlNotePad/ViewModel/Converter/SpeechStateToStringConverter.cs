using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
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

            if (value.Value == SpeechState.NotStarted)
                return "Not Started";

            if (value.Value.HasFlag(SpeechState.Canceled))
            {
                if (value.Value.HasFlag(SpeechState.HasFault))
                    return "Cancelled (had errors)";

                return "Cancelled";
            }

            if (value.Value.HasFlag(SpeechState.Completed))
            {
                if (value.Value.HasFlag(SpeechState.HasFault))
                    return "Completed with errors";

                return "Completed";
            }

            if (value.Value.HasFlag(SpeechState.Speaking))
            {
                if (value.Value.HasFlag(SpeechState.HasFault))
                    return "Speaking (has errors)";

                return "Speaking";
            }

            return "Paused";
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
