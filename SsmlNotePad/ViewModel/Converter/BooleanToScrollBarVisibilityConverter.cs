using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(bool?), typeof(ScrollBarVisibility?))]
    public class BooleanToScrollBarVisibilityConverter : DependencyObject, IValueConverter
    {
        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <seealso cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(ScrollBarVisibility?), typeof(BooleanToScrollBarVisibilityConverter),
                new PropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary>
        /// ScrollBarVisibility value when source is null
        /// </summary>
        public ScrollBarVisibility? NullValue
        {
            get
            {
                if (CheckAccess())
                    return (ScrollBarVisibility?)(GetValue(NullValueProperty));
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
        /// Identifies the <seealso cref="TrueValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(DependencyPropertyName_TrueValue, typeof(ScrollBarVisibility?), typeof(BooleanToScrollBarVisibilityConverter),
                new PropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Value when source is true.
        /// </summary>
        public ScrollBarVisibility? TrueValue
        {
            get
            {
                if (CheckAccess())
                    return (ScrollBarVisibility?)(GetValue(TrueValueProperty));
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
        /// Identifies the <seealso cref="FalseValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(DependencyPropertyName_FalseValue, typeof(ScrollBarVisibility?), typeof(BooleanToScrollBarVisibilityConverter),
                new PropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary>
        /// Value when source is false.
        /// </summary>
        public ScrollBarVisibility? FalseValue
        {
            get
            {
                if (CheckAccess())
                    return (ScrollBarVisibility?)(GetValue(FalseValueProperty));
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

        public ScrollBarVisibility? Convert(bool? value, object parameter, CultureInfo culture)
        {
            if (value.HasValue)
                return (value.Value) ? TrueValue : FalseValue;

            return NullValue;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(ScrollBarVisibility)) || targetType.Equals(typeof(ScrollBarVisibility?)))
                return Convert(value as bool?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
