using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts speech status enumerated value to a string value.
    /// </summary>
    [ValueConversion(typeof(Model.SpeechProgressState), typeof(string))]
    public class SpeechProgressToStringConverter : DependencyObject, IValueConverter
    {
        /// <summary>
        /// Converts a <seealso cref="Model.SpeechProgressState"/> value to a <seealso cref="string"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="Model.SpeechProgressState"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="Model.SpeechProgressState"/>value converted to a <seealso cref="string"/> value.</returns>
        public string Convert(Model.SpeechProgressState? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return "";
            
            switch (value.Value)
            {
                case Model.SpeechProgressState.NotStarted:
                    return "Not Started";
                case Model.SpeechProgressState.SpeakingNormal:
                    return "Speaking";
                case Model.SpeechProgressState.SpeakingWithFault:
                    return "Speaking: Unexpected error encountered";
                case Model.SpeechProgressState.PausedNormal:
                    return "Paused";
                case Model.SpeechProgressState.PausedWithFault:
                    return "Paused: Unexpected error encountered";
                case Model.SpeechProgressState.CompletedSuccess:
                    return "Completed";
                case Model.SpeechProgressState.CompletedWithFault:
                    return "Finished with unexpected error";
                default:
                    return value.Value.ToString("F");
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(string)))
                return Convert(value as Model.SpeechProgressState?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
