using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(bool?), typeof(TextWrapping?))]
    public class BooleanToTextWrappingConverter : DependencyObject, IValueConverter
    {
        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <see cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(TextWrapping?), typeof(BooleanToTextWrappingConverter),
                new PropertyMetadata(TextWrapping.NoWrap));

        /// <summary>
        /// TextWrapping value when source is null.
        /// </summary>
        public TextWrapping? NullValue
        {
            get
            {
                if (CheckAccess())
                    return (TextWrapping?)(GetValue(NullValueProperty));
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

        #region TrueValue Property Members

        public const string DependencyPropertyName_TrueValue = "TrueValue";

        /// <summary>
        /// Identifies the <see cref="TrueValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(DependencyPropertyName_TrueValue, typeof(TextWrapping?), typeof(BooleanToTextWrappingConverter),
                new PropertyMetadata(TextWrapping.Wrap));

        /// <summary>
        /// TextWrapping value when source is true.
        /// </summary>
        public TextWrapping? TrueValue
        {
            get
            {
                if (CheckAccess())
                    return (TextWrapping?)(GetValue(TrueValueProperty));
                return Dispatcher.Invoke(() => TrueValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(TrueValueProperty, value);
                else
                    Dispatcher.Invoke(() => TrueValue = value);
            }
        }

        #endregion

        #region FalseValue Property Members

        public const string DependencyPropertyName_FalseValue = "FalseValue";

        /// <summary>
        /// Identifies the <see cref="FalseValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(DependencyPropertyName_FalseValue, typeof(TextWrapping), typeof(BooleanToTextWrappingConverter),
                new PropertyMetadata(TextWrapping.NoWrap));

        /// <summary>
        /// TextWrapping value when source is false.
        /// </summary>
        public TextWrapping FalseValue
        {
            get
            {
                if (CheckAccess())
                    return (TextWrapping)(GetValue(FalseValueProperty));
                return Dispatcher.Invoke(() => FalseValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(FalseValueProperty, value);
                else
                    Dispatcher.Invoke(() => FalseValue = value);
            }
        }

        #endregion

        public TextWrapping? Convert(bool? value, object parameter, CultureInfo culture)
        {
            if (value.HasValue)
                return (value.Value) ? TrueValue : FalseValue;

            return NullValue;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(TextWrapping)) || targetType.Equals(typeof(TextWrapping?)))
                return Convert(value as bool?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
