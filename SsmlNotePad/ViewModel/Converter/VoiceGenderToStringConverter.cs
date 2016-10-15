using System;
using System.Globalization;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(VoiceGender?), typeof(string))]
    public class VoiceGenderToStringConverter : DependencyObject, IValueConverter
    {
        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <see cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(string), typeof(VoiceGenderToStringConverter),
                new PropertyMetadata(""));

        /// <summary>
        /// 
        /// </summary>
        public string NullValue
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(NullValueProperty));
                return Dispatcher.Invoke(() => NullValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NullValueProperty, value);
                else
                    Dispatcher.Invoke(() => NullValue = value);
            }
        }
        
        #endregion

        public string Convert(VoiceGender? value, object parameter, CultureInfo culture)
        {
            if (value.HasValue)
                return (value.Value == VoiceGender.NotSet) ? "Not Set" : value.Value.ToString("F");

            return NullValue;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(VoiceGender)) || targetType.Equals(typeof(VoiceGender?)))
                return Convert(value as VoiceGender?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
