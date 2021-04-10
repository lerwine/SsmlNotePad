using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="string"/> values to  <seealso cref="Visibility"/> values.
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : ToValueConverterBase<string, Visibility>
    {
        #region NullSource Property Members

        /// <summary>
        /// Defines the name for the <see cref="NullSource"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NullSource = "NullSource";

        /// <summary>
        /// Identifies the <see cref="NullSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullSourceProperty = DependencyProperty.Register(DependencyPropertyName_NullSource, typeof(Visibility?),
            typeof(StringToVisibilityConverter), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Nullable{TTarget}"/> value to represent a null source value.
        /// </summary>
        public override Visibility? NullSource
        {
            get { return (Visibility?)(GetValue(NullSourceProperty)); }
            set { SetValue(NullSourceProperty, value); }
        }

        #endregion

        #region Null Property Members

        /// <summary>
        /// Defines the name for the <see cref="Null"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Null = "Null";

        /// <summary>
        /// Identifies the <see cref="Null"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullProperty = DependencyProperty.Register(DependencyPropertyName_Null, typeof(Visibility?), typeof(StringToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        // TODO: Replace obsolete member
        /// <summary>
        /// <seealso cref="Nullable{Visibility}"/> value to represent a null source value.
        /// </summary>
        [Obsolete("Use NullSource, instead")]
        public Visibility? Null
        {
            get { return GetValue(NullProperty) as Visibility?; }
            set { SetValue(NullProperty, value); }
        }

        #endregion

        #region Empty Property Members

        /// <summary>
        /// Defines the name for the <see cref="Empty"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Empty = "Empty";

        /// <summary>
        /// Identifies the <see cref="Empty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyProperty = DependencyProperty.Register(DependencyPropertyName_Empty, typeof(Visibility?), typeof(StringToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Nullable{Visibility}"/> value to represent a empty string source value.
        /// </summary>
        public Visibility? Empty
        {
            get { return GetValue(EmptyProperty) as Visibility?; }
            set { SetValue(EmptyProperty, value); }
        }

        #endregion

        #region Whitespace Property Members

        /// <summary>
        /// Defines the name for the <see cref="Whitespace"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Whitespace = "Whitespace";

        /// <summary>
        /// Identifies the <see cref="Whitespace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WhitespaceProperty = DependencyProperty.Register(DependencyPropertyName_Whitespace, typeof(Visibility?), typeof(StringToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Nullable{Visibility}"/> value to represent a non-empty source value which only contains whitespace.
        /// </summary>
        public Visibility? Whitespace
        {
            get { return GetValue(WhitespaceProperty) as Visibility?; }
            set { SetValue(WhitespaceProperty, value); }
        }

        #endregion

        #region NonWhitespace Property Members

        /// <summary>
        /// Defines the name for the <see cref="NonWhitespace"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NonWhitespace = "NonWhitespace";

        /// <summary>
        /// Identifies the <see cref="NonWhitespace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NonWhitespaceProperty = DependencyProperty.Register(DependencyPropertyName_NonWhitespace, typeof(Visibility?), typeof(StringToVisibilityConverter),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// <seealso cref="Nullable{Visibility}"/> value to represent a source value which contains at least one non-whitespace character.
        /// </summary>
        public Visibility? NonWhitespace
        {
            get { return GetValue(NonWhitespaceProperty) as Visibility?; }
            set { SetValue(NonWhitespaceProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="string"/> value to a <seealso cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="string"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="string"/> value converted to a <seealso cref="Visibility"/> or null value.</returns>
        public override Visibility? Convert(string value, object parameter, CultureInfo culture)
        {
            if (value.Length == 0)
                return Empty;

            if (value.Any(c => !Char.IsWhiteSpace(c)))
                return NonWhitespace;

            return Whitespace;
        }
    }
}
