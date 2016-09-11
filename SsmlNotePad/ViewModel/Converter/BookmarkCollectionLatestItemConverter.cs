using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(ReadOnlyObservableCollection<string>), typeof(string))]
    public class BookmarkCollectionLatestItemConverter : DependencyObject, IValueConverter
    {
        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <seealso cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(string), typeof(BookmarkCollectionLatestItemConverter),
                new PropertyMetadata(""));

        /// <summary>
        /// 
        /// </summary>
        public string NullValue
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(NullValueProperty));
                return Dispatcher.Invoke(() => NullValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NullValueProperty, value);
                else
                    Dispatcher.Invoke(() => NullValue = value);
            }
        }
        
        #endregion

        #region EmptyValue Property Members

        public const string DependencyPropertyName_EmptyValue = "EmptyValue";

        /// <summary>
        /// Identifies the <seealso cref="EmptyValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyValueProperty = DependencyProperty.Register(DependencyPropertyName_EmptyValue, typeof(string), typeof(BookmarkCollectionLatestItemConverter),
                new PropertyMetadata(""));

        /// <summary>
        /// 
        /// </summary>
        public string EmptyValue
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(EmptyValueProperty));
                return Dispatcher.Invoke(() => EmptyValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(EmptyValueProperty, value);
                else
                    Dispatcher.Invoke(() => EmptyValue = value);
            }
        }

        #endregion

        public string Convert(ReadOnlyObservableCollection<string> value, object parameter, CultureInfo culture)
        {
            if (value == null)
                return NullValue;

            if (value.Count == 0)
                return EmptyValue;

            string latest = value.LastOrDefault();
            return (latest == null) ? NullValue : (latest.Trim().Length == 0) ? EmptyValue : latest;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value as ReadOnlyObservableCollection<string>, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
