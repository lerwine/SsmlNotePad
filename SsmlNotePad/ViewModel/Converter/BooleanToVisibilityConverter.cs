using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="bool"/> values to  <seealso cref="Visibility"/> values.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : ToValueConverterBase<bool, Visibility>
    {
        #region True Property Members

        /// <summary>
        /// Defines the name for the <see cref="True"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_True = "True";

        /// <summary>
        /// Identifies the <see cref="True"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TrueProperty = DependencyProperty.Register(DependencyPropertyName_True, typeof(Visibility?), typeof(BooleanToVisibilityConverter),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Value when source is true.
        /// </summary>
        public Visibility? True
        {
            get { return GetValue(TrueProperty) as Visibility?; }
            set { SetValue(TrueProperty, value); }
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
        public static readonly DependencyProperty FalseProperty = DependencyProperty.Register(DependencyPropertyName_False, typeof(Visibility?), typeof(BooleanToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Value when source is false.
        /// </summary>
        public Visibility? False
        {
            get { return GetValue(FalseProperty) as Visibility?; }
            set { SetValue(FalseProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="bool"/> value to a <seealso cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="bool"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="bool"/> value converted to a <seealso cref="Visibility"/> or null value.</returns>
        public override Visibility? Convert(bool value, object parameter, CultureInfo culture)
        {
            return (value) ? True : False;
        }
    }
}
