using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(bool?), typeof(bool?))]
    public class InverseBooleanConverter : DependencyObject, IValueConverter
    {
        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <seealso cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(bool?), typeof(InverseBooleanConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Value to return when source is null.
        /// </summary>
        public bool? NullValue
        {
            get
            {
                if (CheckAccess())
                    return (bool?)(GetValue(NullValueProperty));
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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != null && !targetType.Equals(typeof(bool)) && !targetType.Equals(typeof(bool?)))
                return System.Convert.ChangeType(value, targetType);

            bool? source = value as bool?;
            return (source.HasValue) ? !source.Value : NullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
