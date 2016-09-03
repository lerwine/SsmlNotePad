using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(object), typeof(Visibility?))]
    public class ObjectToVisibilityConverter : DependencyObject, IValueConverter
    {
        #region NullVisibility Property Members

        public const string DependencyPropertyName_NullVisibility = "NullVisibility";

        /// <summary>
        /// Identifies the <seealso cref="NullVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullVisibilityProperty = DependencyProperty.Register(DependencyPropertyName_NullVisibility, typeof(Visibility?), typeof(ObjectToVisibilityConverter),
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

        #region NonNullVisibility Property Members

        public const string DependencyPropertyName_NonNullVisibility = "NonNullVisibility";

        /// <summary>
        /// Identifies the <seealso cref="NonNullVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NonNullVisibilityProperty = DependencyProperty.Register(DependencyPropertyName_NonNullVisibility, typeof(Visibility?), typeof(ObjectToVisibilityConverter),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? NonNullVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(NonNullVisibilityProperty));
                return Dispatcher.Invoke(() => NonNullVisibility);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NonNullVisibilityProperty, value);
                else
                    Dispatcher.Invoke(() => NonNullVisibility = value);
            }
        }

        #endregion

        public virtual Visibility? Convert(object value, object parameter, CultureInfo culture)
        {
            if (value == null)
                return NullVisibility;

            return NonNullVisibility;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(Visibility)) || targetType.Equals(typeof(Visibility?)))
                return Convert(value, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
