using System;
using System.Globalization;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="VoiceGender"/> values to display text.
    /// </summary>
    [ValueConversion(typeof(VoiceGender), typeof(string))]
    public class VoiceGenderToStringConverter : DependencyObject, IValueConverter
    {
        #region NullSource Property Members

        /// <summary>
        /// Defines the name for the <see cref="NullSource"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NullSource = "NullSource";

        /// <summary>
        /// Identifies the <see cref="NullSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullSourceProperty = DependencyProperty.Register(DependencyPropertyName_NullSource, typeof(string), typeof(VoiceGenderToStringConverter),
                new PropertyMetadata(""));

        /// <summary>
        /// <seealso cref="string"/> value to represent a null source value.
        /// </summary>
        public string NullSource
        {
            get { return GetValue(NullSourceProperty) as string; }
            set { SetValue(NullSourceProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="VoiceGender"/> value to a <seealso cref="string"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="VoiceGender"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="VoiceGender"/> value converted to a <seealso cref="string"/> or null value.</returns>
        public string Convert(VoiceGender? value, object parameter, CultureInfo culture)
        {
            if (value.HasValue)
                return (value.Value == VoiceGender.NotSet) ? "Not Set" : value.Value.ToString("F");

            return NullSource;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value as VoiceGender?, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
