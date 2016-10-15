using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(string), typeof(Visibility?))]
    public class StringToVisibilityConverter : DependencyObject, IValueConverter
    {
        #region NullVisibility Property Members

        public const string DependencyPropertyName_NullVisibility = "NullVisibility";

        /// <summary>
        /// Identifies the <see cref="NullVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullVisibilityProperty = DependencyProperty.Register(DependencyPropertyName_NullVisibility, typeof(Visibility?), typeof(StringToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? NullVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(NullVisibilityProperty));
                return Dispatcher.Invoke(() => NullVisibility);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NullVisibilityProperty, value);
                else
                    Dispatcher.Invoke(() => NullVisibility = value);
            }
        }

        #endregion

        #region EmptyVisibility Property Members

        public const string DependencyPropertyName_EmptyVisibility = "EmptyVisibility";

        /// <summary>
        /// Identifies the <see cref="EmptyVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyVisibilityProperty = DependencyProperty.Register(DependencyPropertyName_EmptyVisibility, typeof(Visibility?), typeof(StringToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? EmptyVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(EmptyVisibilityProperty));
                return Dispatcher.Invoke(() => EmptyVisibility);
            }
            set
            {
                if (CheckAccess())
                    SetValue(EmptyVisibilityProperty, value);
                else
                    Dispatcher.Invoke(() => EmptyVisibility = value);
            }
        }

        #endregion

        #region WhitespaceVisibility Property Members

        public const string DependencyPropertyName_WhitespaceVisibility = "WhitespaceVisibility";

        /// <summary>
        /// Identifies the <see cref="WhitespaceVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WhitespaceVisibilityProperty = DependencyProperty.Register(DependencyPropertyName_WhitespaceVisibility, typeof(Visibility?), typeof(StringToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? WhitespaceVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(WhitespaceVisibilityProperty));
                return Dispatcher.Invoke(() => WhitespaceVisibility);
            }
            set
            {
                if (CheckAccess())
                    SetValue(WhitespaceVisibilityProperty, value);
                else
                    Dispatcher.Invoke(() => WhitespaceVisibility = value);
            }
        }

        #endregion

        #region NonWhitespaceVisibility Property Members

        public const string DependencyPropertyName_NonWhitespaceVisibility = "NonWhitespaceVisibility";

        /// <summary>
        /// Identifies the <see cref="NonWhitespaceVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NonWhitespaceVisibilityProperty = DependencyProperty.Register(DependencyPropertyName_NonWhitespaceVisibility, typeof(Visibility?), typeof(StringToVisibilityConverter),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? NonWhitespaceVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(NonWhitespaceVisibilityProperty));
                return Dispatcher.Invoke(() => NonWhitespaceVisibility);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NonWhitespaceVisibilityProperty, value);
                else
                    Dispatcher.Invoke(() => NonWhitespaceVisibility = value);
            }
        }

        #endregion

        public virtual Visibility? Convert(string value, object parameter, CultureInfo culture)
        {
            if (value == null)
                return NullVisibility;

            if (value.Length == 0)
                return EmptyVisibility;

            if (value.Any(c => !Char.IsWhiteSpace(c)))
                return NonWhitespaceVisibility;

            return WhitespaceVisibility;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(Visibility)) || targetType.Equals(typeof(Visibility?)))
                return Convert(value as string, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
