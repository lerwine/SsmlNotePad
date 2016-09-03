using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(FileSaveStatus?), typeof(Style))]
    public class FileSaveStatusToStyleConverter : DependencyObject, IValueConverter
    {
        #region NullStyle Property Members

        public const string DependencyPropertyName_NullStyle = "NullStyle";

        /// <summary>
        /// Identifies the <seealso cref="NullStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullStyleProperty = DependencyProperty.Register(DependencyPropertyName_NullStyle, typeof(Style), typeof(FileSaveStatusToStyleConverter),
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

        #region NewStyle Property Members

        public const string DependencyPropertyName_NewStyle = "NewStyle";

        /// <summary>
        /// Identifies the <seealso cref="NewStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NewStyleProperty = DependencyProperty.Register(DependencyPropertyName_NewStyle, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Style NewStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(NewStyleProperty));
                return Dispatcher.Invoke(() => NewStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NewStyleProperty, value);
                else
                    Dispatcher.Invoke(() => NewStyle = value);
            }
        }
        
        #endregion

        #region ModifiedStyle Property Members

        public const string DependencyPropertyName_ModifiedStyle = "ModifiedStyle";

        /// <summary>
        /// Identifies the <seealso cref="ModifiedStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModifiedStyleProperty = DependencyProperty.Register(DependencyPropertyName_ModifiedStyle, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Style ModifiedStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(ModifiedStyleProperty));
                return Dispatcher.Invoke(() => ModifiedStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ModifiedStyleProperty, value);
                else
                    Dispatcher.Invoke(() => ModifiedStyle = value);
            }
        }
        
        #endregion

        #region SaveErrorStyle Property Members

        public const string DependencyPropertyName_SaveErrorStyle = "SaveErrorStyle";

        /// <summary>
        /// Identifies the <seealso cref="SaveErrorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaveErrorStyleProperty = DependencyProperty.Register(DependencyPropertyName_SaveErrorStyle, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Style SaveErrorStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(SaveErrorStyleProperty));
                return Dispatcher.Invoke(() => SaveErrorStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SaveErrorStyleProperty, value);
                else
                    Dispatcher.Invoke(() => SaveErrorStyle = value);
            }
        }

        #endregion

        #region SaveWarningStyle Property Members

        public const string DependencyPropertyName_SaveWarningStyle = "SaveWarningStyle";

        /// <summary>
        /// Identifies the <seealso cref="SaveWarningStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaveWarningStyleProperty = DependencyProperty.Register(DependencyPropertyName_SaveWarningStyle, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as FileSaveStatusToStyleConverter).SaveWarningStyle_PropertyChanged((Style)(e.OldValue), (Style)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as FileSaveStatusToStyleConverter).SaveWarningStyle_PropertyChanged((Style)(e.OldValue), (Style)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as FileSaveStatusToStyleConverter).SaveWarningStyle_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public Style SaveWarningStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(SaveWarningStyleProperty));
                return Dispatcher.Invoke(() => SaveWarningStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SaveWarningStyleProperty, value);
                else
                    Dispatcher.Invoke(() => SaveWarningStyle = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SaveWarningStyle"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="Style"/> value before the <seealso cref="SaveWarningStyle"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="Style"/> value after the <seealso cref="SaveWarningStyle"/> property was changed.</param>
        protected virtual void SaveWarningStyle_PropertyChanged(Style oldValue, Style newValue)
        {
            // TODO: Implement FileSaveStatusToStyleConverter.SaveWarningStyle_PropertyChanged(Style, Style)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SaveWarningStyle"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual Style SaveWarningStyle_CoerceValue(object baseValue)
        {
            // TODO: Implement FileSaveStatusToStyleConverter.SaveWarningStyle_CoerceValue(DependencyObject, object)
            return (Style)baseValue;
        }

        #endregion

        #region SaveSuccessStyle Property Members

        public const string DependencyPropertyName_SaveSuccessStyle = "SaveSuccessStyle";

        /// <summary>
        /// Identifies the <seealso cref="SaveSuccessStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaveSuccessStyleProperty = DependencyProperty.Register(DependencyPropertyName_SaveSuccessStyle, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as FileSaveStatusToStyleConverter).SaveSuccessStyle_PropertyChanged((Style)(e.OldValue), (Style)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as FileSaveStatusToStyleConverter).SaveSuccessStyle_PropertyChanged((Style)(e.OldValue), (Style)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as FileSaveStatusToStyleConverter).SaveSuccessStyle_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public Style SaveSuccessStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(SaveSuccessStyleProperty));
                return Dispatcher.Invoke(() => SaveSuccessStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SaveSuccessStyleProperty, value);
                else
                    Dispatcher.Invoke(() => SaveSuccessStyle = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SaveSuccessStyle"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="Style"/> value before the <seealso cref="SaveSuccessStyle"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="Style"/> value after the <seealso cref="SaveSuccessStyle"/> property was changed.</param>
        protected virtual void SaveSuccessStyle_PropertyChanged(Style oldValue, Style newValue)
        {
            // TODO: Implement FileSaveStatusToStyleConverter.SaveSuccessStyle_PropertyChanged(Style, Style)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SaveSuccessStyle"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual Style SaveSuccessStyle_CoerceValue(object baseValue)
        {
            // TODO: Implement FileSaveStatusToStyleConverter.SaveSuccessStyle_CoerceValue(DependencyObject, object)
            return (Style)baseValue;
        }

        #endregion

        public Style Convert(FileSaveStatus? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return NullStyle;

            switch (value.Value)
            {
                case FileSaveStatus.New:
                    return NewStyle;
                case FileSaveStatus.Modified:
                    return ModifiedStyle;
                case FileSaveStatus.SaveError:
                    return SaveErrorStyle;
                case FileSaveStatus.SaveWarning:
                    return SaveWarningStyle;
            }

            return SaveSuccessStyle;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(Style)))
                return Convert(value as FileSaveStatus?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
