using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="bool"/> values to  <seealso cref="ScrollBarVisibility"/> values.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(ScrollBarVisibility))]
    public class BooleanToScrollBarVisibilityConverter : ToValueConverterBase<bool, ScrollBarVisibility>
    {
        #region True Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="True"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_True = "True";

        /// <summary>
        /// Identifies the <see cref="True"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TrueProperty = DependencyProperty.Register(DependencyPropertyName_True, typeof(ScrollBarVisibility?), typeof(BooleanToScrollBarVisibilityConverter),
                new PropertyMetadata(ScrollBarVisibility.Visible));

        /// <summary>
        /// <seealso cref="Nullable{ScrollBarVisibility}"/> value to represent a <seealso cref="true"/> binding source value.
        /// </summary>
        public ScrollBarVisibility? True
        {
            get { return (ScrollBarVisibility?)(GetValue(TrueProperty)); }
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
        public static readonly DependencyProperty FalseProperty = DependencyProperty.Register(DependencyPropertyName_False, typeof(ScrollBarVisibility?), typeof(BooleanToScrollBarVisibilityConverter),
                new PropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary>
        /// <seealso cref="Nullable{ScrollBarVisibility}"/> value to represent a <seealso cref="false"/> binding source value.
        /// </summary>
        public ScrollBarVisibility? False
        {
            get { return (ScrollBarVisibility?)(GetValue(FalseProperty)); }
            set { SetValue(FalseProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="bool"/> value to a <seealso cref="ScrollBarVisibility"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="bool"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="bool"/> value converted to a <seealso cref="ScrollBarVisibility"/> or null value.</returns>
        public override ScrollBarVisibility? Convert(bool value, object parameter, CultureInfo culture)
        {
            return (value) ? True : False;
        }
    }
}
