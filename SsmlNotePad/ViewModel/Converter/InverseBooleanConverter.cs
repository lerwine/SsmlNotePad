using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="bool"/> values to their inverted values.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : ToValueConverterBase<bool, bool>
    {
        #region NullSource Property Members

        /// <summary>
        /// Defines the name for the <see cref="NullSource"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NullSource = "NullSource";

        /// <summary>
        /// Identifies the <see cref="NullSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullSourceProperty = DependencyProperty.Register(DependencyPropertyName_NullSource, typeof(bool?),
            typeof(InverseBooleanConverter), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Nullable{TTarget}"/> value to represent a null source value.
        /// </summary>
        public override bool? NullSource
        {
            get { return (bool?)(GetValue(NullSourceProperty)); }
            set { SetValue(NullSourceProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="bool"/> value to its inverse value.
        /// </summary>
        /// <param name="value">The <seealso cref="bool"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="bool"/> value converted to its inverse value.</returns>
        public override bool? Convert(bool value, object parameter, CultureInfo culture) { return !value; }
    }
}
