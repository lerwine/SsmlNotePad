using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="bool"/> values to display text.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToStringConverter : DependencyObject, IValueConverter
    {
        #region NullSource Property Members

        /// <summary>
        /// Defines the name for the <see cref="NullSource"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NullSource = "NullSource";

        /// <summary>
        /// Identifies the <see cref="NullSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullSourceProperty = DependencyProperty.Register(DependencyPropertyName_NullSource, typeof(string), typeof(BooleanToStringConverter),
                new PropertyMetadata(""));

        /// <summary>
        /// String value when source is null.
        /// </summary>
        public string NullSource
        {
            get { return GetValue(FalseProperty) as string; }
            set { SetValue(FalseProperty, value); }
        }

        #endregion

        #region True Property Members

        /// <summary>
        /// Defines the name for the <see cref="True"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_True = "True";

        /// <summary>
        /// Identifies the <see cref="True"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TrueProperty = DependencyProperty.Register(DependencyPropertyName_True, typeof(string), typeof(BooleanToStringConverter),
                new PropertyMetadata("Yes"));

        /// <summary>
        /// String value when source is true.
        /// </summary>
        public string True
        {
            get { return GetValue(FalseProperty) as string; }
            set { SetValue(FalseProperty, value); }
        }

        #endregion

        #region False Property Members

        /// <summary>
        /// Defines the name for the <see cref="False"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_False = "False";

        /// <summary>
        /// Identifies the <see cref="False"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FalseProperty = DependencyProperty.Register(DependencyPropertyName_False, typeof(string), typeof(BooleanToStringConverter),
                new PropertyMetadata("No"));

        /// <summary>
        /// String value when source is false.
        /// </summary>
        public string False
        {
            get { return GetValue(FalseProperty) as string; }
            set { SetValue(FalseProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="bool"/> value to display text.
        /// </summary>
        /// <param name="value">The <seealso cref="bool"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="bool"/> value converted to display text.</returns>
        public string Convert(bool? value, object parameter, CultureInfo culture)
        {
            if (value.HasValue)
                return (value.Value) ? True : False;

            return NullSource;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value as bool?, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
