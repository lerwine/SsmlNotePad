using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="object"/> values to  <seealso cref="Visibility"/> values.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ObjectToVisibilityConverter : DependencyObject, IValueConverter
    {
        #region Null Property Members

        /// <summary>
        /// Defines the name for the <see cref="Null"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Null = "Null";

        /// <summary>
        /// Identifies the <see cref="Null"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullProperty = DependencyProperty.Register(DependencyPropertyName_Null, typeof(Visibility?), typeof(ObjectToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Nullable{Visibility}"/> value to represent a null source value.
        /// </summary>
        public Visibility? Null
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(NullProperty));
                return Dispatcher.Invoke(() => Null);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NullProperty, value);
                else
                    Dispatcher.Invoke(() => Null = value);
            }
        }

        #endregion

        #region NonNull Property Members

        /// <summary>
        /// Defines the name for the <see cref="NonNull"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NonNull = "NonNull";

        /// <summary>
        /// Identifies the <see cref="NonNull"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NonNullProperty = DependencyProperty.Register(DependencyPropertyName_NonNull, typeof(Visibility?), typeof(ObjectToVisibilityConverter),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// <seealso cref="Nullable{Visibility}"/> value to represent a non-null source value.
        /// </summary>
        public Visibility? NonNull
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(NonNullProperty));
                return Dispatcher.Invoke(() => NonNull);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NonNullProperty, value);
                else
                    Dispatcher.Invoke(() => NonNull = value);
            }
        }

        #endregion

        /// <summary>
        /// Converts an <seealso cref="object"/> value to a <seealso cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="object"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="object"/> value converted to a <seealso cref="Visibility"/> or null value.</returns>
        public virtual Visibility? Convert(object value, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Null;

            return NonNull;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
