using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    public abstract class ToClassConverterBase<TSource, TTarget> : DependencyObject, IValueConverter
        where TTarget : class
    {
        #region NullSource Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="NullSource"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NullSource = "NullSource";

        /// <summary>
        /// Identifies the <see cref="NullSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullSourceProperty = DependencyProperty.Register(DependencyPropertyName_NullSource, typeof(TTarget), 
            typeof(ToClassConverterBase<TSource, TTarget>), new PropertyMetadata(null));

        /// <summary>
        /// <typeparamref name="TTarget"/> to return when the binding source produces a null value.
        /// </summary>
        public TTarget NullSource
        {
            get { return (TTarget)(GetValue(NullSourceProperty)); }
            set { SetValue(NullSourceProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <typeparamref name="TSource"/> value to a <typeparamref name="TTarget"/> value.
        /// </summary>
        /// <param name="value">The <typeparamref name="TSource"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><typeparamref name="TSource"/> value converted to a <typeparamref name="TTarget"/> value.</returns>
        public abstract TTarget Convert(TSource value, object parameter, CultureInfo culture);

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return NullSource;

            if (!(value is TSource))
                return value;

            return Convert((TSource)value, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
