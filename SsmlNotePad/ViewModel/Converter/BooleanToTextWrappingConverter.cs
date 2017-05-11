using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="bool"/> values to  <seealso cref="TextWrapping"/> values.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(TextWrapping))]
    public class BooleanToTextWrappingConverter : ToValueConverterBase<bool, TextWrapping>
    {
        #region NullSource Property Members

        /// <summary>
        /// Defines the name for the <see cref="NullSource"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NullSource = "NullSource";

        /// <summary>
        /// Identifies the <see cref="NullSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullSourceProperty = DependencyProperty.Register(DependencyPropertyName_NullSource, typeof(TextWrapping?),
            typeof(BooleanToTextWrappingConverter), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Nullable{TTarget}"/> value to represent a null source value.
        /// </summary>
        public override TextWrapping? NullSource
        {
            get { return (TextWrapping?)(GetValue(NullSourceProperty)); }
            set { SetValue(NullSourceProperty, value); }
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
        public static readonly DependencyProperty TrueProperty = DependencyProperty.Register(DependencyPropertyName_True, typeof(TextWrapping?), typeof(BooleanToTextWrappingConverter),
                new PropertyMetadata(TextWrapping.Wrap));

        /// <summary>
        /// <seealso cref="Nullable{TextWrapping}"/> value to represent a <seealso cref="true"/> binding source value.
        /// </summary>
        public TextWrapping? True
        {
            get { return (TextWrapping?)(GetValue(TrueProperty)); }
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
        public static readonly DependencyProperty FalseProperty = DependencyProperty.Register(DependencyPropertyName_False, typeof(TextWrapping?), typeof(BooleanToTextWrappingConverter),
                new PropertyMetadata(TextWrapping.NoWrap));

        /// <summary>
        /// <seealso cref="Nullable{TextWrapping}"/> value to represent a <seealso cref="false"/> binding source value.
        /// </summary>
        public TextWrapping? False
        {
            get { return (TextWrapping?)(GetValue(FalseProperty)); }
            set { SetValue(FalseProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="bool"/> value to a <seealso cref="TextWrapping"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="bool"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="bool"/> value converted to a <seealso cref="TextWrapping"/> or null value.</returns>
        public override TextWrapping? Convert(bool value, object parameter, CultureInfo culture)
        {
            return (value) ? True : False;
        }
    }
}
