using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(SpeechState), typeof(Style))]
    public class SpeechStateToStyleConverter : DependencyObject, IValueConverter
    {
        #region NullStyle Property Members

        public const string DependencyPropertyName_NullStyle = "NullStyle";

        /// <summary>
        /// Identifies the <seealso cref="NullStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullStyleProperty = DependencyProperty.Register(DependencyPropertyName_NullStyle, typeof(Style), typeof(SpeechStateToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Style NullStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(NullStyleProperty));
                return Dispatcher.Invoke(() => NullStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NullStyleProperty, value);
                else
                    Dispatcher.Invoke(() => NullStyle = value);
            }
        }

        #endregion

        #region NormalStyle Property Members

        public const string DependencyPropertyName_NormalStyle = "NormalStyle";

        /// <summary>
        /// Identifies the <seealso cref="NormalStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalStyleProperty = DependencyProperty.Register(DependencyPropertyName_NormalStyle, typeof(Style), typeof(SpeechStateToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Style NormalStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(NormalStyleProperty));
                return Dispatcher.Invoke(() => NormalStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NormalStyleProperty, value);
                else
                    Dispatcher.Invoke(() => NormalStyle = value);
            }
        }

        #endregion

        #region FaultedStyle Property Members

        public const string DependencyPropertyName_FaultedStyle = "FaultedStyle";

        /// <summary>
        /// Identifies the <seealso cref="FaultedStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FaultedStyleProperty = DependencyProperty.Register(DependencyPropertyName_FaultedStyle, typeof(Style), typeof(SpeechStateToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Style FaultedStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(FaultedStyleProperty));
                return Dispatcher.Invoke(() => FaultedStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(FaultedStyleProperty, value);
                else
                    Dispatcher.Invoke(() => FaultedStyle = value);
            }
        }

        #endregion

        #region CanceledStyle Property Members

        public const string DependencyPropertyName_CanceledStyle = "CanceledStyle";

        /// <summary>
        /// Identifies the <seealso cref="CanceledStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanceledStyleProperty = DependencyProperty.Register(DependencyPropertyName_CanceledStyle, typeof(Style), typeof(SpeechStateToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Style CanceledStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(CanceledStyleProperty));
                return Dispatcher.Invoke(() => CanceledStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CanceledStyleProperty, value);
                else
                    Dispatcher.Invoke(() => CanceledStyle = value);
            }
        }

        #endregion

        #region PausedStyle Property Members

        public const string DependencyPropertyName_PausedStyle = "PausedStyle";

        /// <summary>
        /// Identifies the <seealso cref="PausedStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PausedStyleProperty = DependencyProperty.Register(DependencyPropertyName_PausedStyle, typeof(Style), typeof(SpeechStateToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Style PausedStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(PausedStyleProperty));
                return Dispatcher.Invoke(() => PausedStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PausedStyleProperty, value);
                else
                    Dispatcher.Invoke(() => PausedStyle = value);
            }
        }

        #endregion

        public Style Convert(SpeechState? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return NullStyle;

            switch (value.Value)
            {
                case SpeechState.Faulted:
                    return FaultedStyle;
                case SpeechState.Canceled:
                    return CanceledStyle;
                case SpeechState.Paused:
                    return PausedStyle;
            }

            return NormalStyle;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(Style)))
                return Convert(value as SpeechState?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
