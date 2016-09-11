using System;
using System.Globalization;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(VoiceAge?), typeof(string))]
    public class VoiceAgeToStringConverter : DependencyObject, IValueConverter
    {
        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <seealso cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(string), typeof(VoiceAgeToStringConverter),
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

        public string Convert(VoiceAge? value, object parameter, CultureInfo culture)
        {
            if (value.HasValue)
                return (value.Value == VoiceAge.NotSet) ? "Not Set" : value.Value.ToString("F");

            return NullValue;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(VoiceAge)) || targetType.Equals(typeof(VoiceAge?)))
                return Convert(value as VoiceAge?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
