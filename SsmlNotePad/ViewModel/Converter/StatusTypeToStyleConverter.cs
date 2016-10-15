using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(XmlValidationStatus?), typeof(Style))]
    public class StatusTypeToStyleConverter : DependencyObject, IValueConverter
    {
        #region NullStyle Property Members

        public const string DependencyPropertyName_NullStyle = "NullStyle";

        /// <summary>
        /// Identifies the <see cref="NullStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullStyleProperty = DependencyProperty.Register(DependencyPropertyName_NullStyle, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is null.
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

        #region NoneStyle Property Members

        public const string DependencyPropertyName_NoneStyle = "NoneStyle";

        /// <summary>
        /// Identifies the <see cref="NoneStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NoneStyleProperty = DependencyProperty.Register(DependencyPropertyName_NoneStyle, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.None"/>.
        /// </summary>
        public Style NoneStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(NoneStyleProperty));
                return Dispatcher.Invoke(() => NoneStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NoneStyleProperty, value);
                else
                    Dispatcher.Invoke(() => NoneStyle = value);
            }
        }

        #endregion

        #region InformationStyle Property Members

        public const string DependencyPropertyName_InformationStyle = "InformationStyle";

        /// <summary>
        /// Identifies the <see cref="InformationStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InformationStyleProperty = DependencyProperty.Register(DependencyPropertyName_InformationStyle, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.Information"/>.
        /// </summary>
        public Style InformationStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(InformationStyleProperty));
                return Dispatcher.Invoke(() => InformationStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(InformationStyleProperty, value);
                else
                    Dispatcher.Invoke(() => InformationStyle = value);
            }
        }

        #endregion

        #region WarningStyle Property Members

        public const string DependencyPropertyName_WarningStyle = "WarningStyle";

        /// <summary>
        /// Identifies the <see cref="WarningStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WarningStyleProperty = DependencyProperty.Register(DependencyPropertyName_WarningStyle, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.Warning"/>.
        /// </summary>
        public Style WarningStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(WarningStyleProperty));
                return Dispatcher.Invoke(() => WarningStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(WarningStyleProperty, value);
                else
                    Dispatcher.Invoke(() => WarningStyle = value);
            }
        }

        #endregion

        #region ErrorStyle Property Members

        public const string DependencyPropertyName_ErrorStyle = "ErrorStyle";

        /// <summary>
        /// Identifies the <see cref="ErrorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorStyleProperty = DependencyProperty.Register(DependencyPropertyName_ErrorStyle, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.Error"/>.
        /// </summary>
        public Style ErrorStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(ErrorStyleProperty));
                return Dispatcher.Invoke(() => ErrorStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ErrorStyleProperty, value);
                else
                    Dispatcher.Invoke(() => ErrorStyle = value);
            }
        }

        #endregion

        #region CriticalStyle Property Members

        public const string DependencyPropertyName_CriticalStyle = "CriticalStyle";

        /// <summary>
        /// Identifies the <see cref="CriticalStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CriticalStyleProperty = DependencyProperty.Register(DependencyPropertyName_CriticalStyle, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.Critical"/>.
        /// </summary>
        public Style CriticalStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(CriticalStyleProperty));
                return Dispatcher.Invoke(() => CriticalStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CriticalStyleProperty, value);
                else
                    Dispatcher.Invoke(() => CriticalStyle = value);
            }
        }

        #endregion

        public Style Convert(XmlValidationStatus? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return NullStyle;

            switch (value.Value)
            {
                case XmlValidationStatus.Critical:
                    return CriticalStyle;
                case XmlValidationStatus.Error:
                    return ErrorStyle;
                case XmlValidationStatus.Warning:
                    return WarningStyle;
                case XmlValidationStatus.Information:
                    return InformationStyle;
            }

            return NoneStyle;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(Style)))
                return Convert(value as XmlValidationStatus?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
