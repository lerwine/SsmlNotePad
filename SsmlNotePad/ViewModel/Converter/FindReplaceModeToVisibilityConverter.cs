using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(FindReplaceDisplayMode?), typeof(Visibility?))]
    public class FindReplaceModeToVisibilityConverter : DependencyObject, IValueConverter
    {

        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <see cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? NullValue
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(NullValueProperty));
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

        #region NoneValue Property Members

        public const string DependencyPropertyName_NoneValue = "NoneValue";

        /// <summary>
        /// Identifies the <see cref="NoneValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NoneValueProperty = DependencyProperty.Register(DependencyPropertyName_NoneValue, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? NoneValue
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(NoneValueProperty));
                return Dispatcher.Invoke(() => NoneValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NoneValueProperty, value);
                else
                    Dispatcher.Invoke(() => NoneValue = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="NoneValue"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="Visibility?"/> value before the <see cref="NoneValue"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="Visibility?"/> value after the <see cref="NoneValue"/> property was changed.</param>
        protected virtual void NoneValue_PropertyChanged(Visibility? oldValue, Visibility? newValue)
        {
            // TODO: Implement FindReplaceModeToVisibilityConverter.NoneValue_PropertyChanged(Visibility?, Visibility?)
        }

        /// <summary>
        /// This gets called whenever <see cref="NoneValue"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual Visibility? NoneValue_CoerceValue(object baseValue)
        {
            // TODO: Implement FindReplaceModeToVisibilityConverter.NoneValue_CoerceValue(DependencyObject, object)
            return (Visibility?)baseValue;
        }

        #endregion

        #region FindValue Property Members

        public const string DependencyPropertyName_FindValue = "FindValue";

        /// <summary>
        /// Identifies the <see cref="FindValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FindValueProperty = DependencyProperty.Register(DependencyPropertyName_FindValue, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? FindValue
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(FindValueProperty));
                return Dispatcher.Invoke(() => FindValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(FindValueProperty, value);
                else
                    Dispatcher.Invoke(() => FindValue = value);
            }
        }

        #endregion

        #region FindNextValue Property Members

        public const string DependencyPropertyName_FindNextValue = "FindNextValue";

        /// <summary>
        /// Identifies the <see cref="FindNextValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FindNextValueProperty = DependencyProperty.Register(DependencyPropertyName_FindNextValue, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? FindNextValue
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(FindNextValueProperty));
                return Dispatcher.Invoke(() => FindNextValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(FindNextValueProperty, value);
                else
                    Dispatcher.Invoke(() => FindNextValue = value);
            }
        }

        #endregion

        #region ReplaceValue Property Members

        public const string DependencyPropertyName_ReplaceValue = "ReplaceValue";

        /// <summary>
        /// Identifies the <see cref="ReplaceValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReplaceValueProperty = DependencyProperty.Register(DependencyPropertyName_ReplaceValue, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? ReplaceValue
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(ReplaceValueProperty));
                return Dispatcher.Invoke(() => ReplaceValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ReplaceValueProperty, value);
                else
                    Dispatcher.Invoke(() => ReplaceValue = value);
            }
        }

        #endregion

        #region ReplaceNextValue Property Members

        public const string DependencyPropertyName_ReplaceNextValue = "ReplaceNextValue";

        /// <summary>
        /// Identifies the <see cref="ReplaceNextValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReplaceNextValueProperty = DependencyProperty.Register(DependencyPropertyName_ReplaceNextValue, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? ReplaceNextValue
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(ReplaceNextValueProperty));
                return Dispatcher.Invoke(() => ReplaceNextValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ReplaceNextValueProperty, value);
                else
                    Dispatcher.Invoke(() => ReplaceNextValue = value);
            }
        }

        #endregion

        #region FindNotFoundValue Property Members

        public const string DependencyPropertyName_FindNotFoundValue = "FindNotFoundValue";

        /// <summary>
        /// Identifies the <see cref="FindNotFoundValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FindNotFoundValueProperty = DependencyProperty.Register(DependencyPropertyName_FindNotFoundValue, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? FindNotFoundValue
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(FindNotFoundValueProperty));
                return Dispatcher.Invoke(() => FindNotFoundValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(FindNotFoundValueProperty, value);
                else
                    Dispatcher.Invoke(() => FindNotFoundValue = value);
            }
        }

        #endregion

        #region ReplaceNotFoundValue Property Members

        public const string DependencyPropertyName_ReplaceNotFoundValue = "ReplaceNotFoundValue";

        /// <summary>
        /// Identifies the <see cref="ReplaceNotFoundValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReplaceNotFoundValueProperty = DependencyProperty.Register(DependencyPropertyName_ReplaceNotFoundValue, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility? ReplaceNotFoundValue
        {
            get
            {
                if (CheckAccess())
                    return (Visibility?)(GetValue(ReplaceNotFoundValueProperty));
                return Dispatcher.Invoke(() => ReplaceNotFoundValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ReplaceNotFoundValueProperty, value);
                else
                    Dispatcher.Invoke(() => ReplaceNotFoundValue = value);
            }
        }
        
        #endregion

        public Visibility? Convert(FindReplaceDisplayMode? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return NullValue;

            switch (value.Value)
            {
                case FindReplaceDisplayMode.Find:
                    return FindValue;
                case FindReplaceDisplayMode.FindNext:
                    return FindNextValue;
                case FindReplaceDisplayMode.Replace:
                    return ReplaceValue;
                case FindReplaceDisplayMode.ReplaceNext:
                    return ReplaceNextValue;
                case FindReplaceDisplayMode.FindNotFound:
                    return FindNotFoundValue;
                case FindReplaceDisplayMode.ReplaceNotFound:
                    return ReplaceNotFoundValue;
            }

            return NoneValue;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(Visibility)) || targetType.Equals(typeof(Visibility?)))
                return Convert(value as FindReplaceDisplayMode?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
