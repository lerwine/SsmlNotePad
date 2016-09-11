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
                new PropertyMetadata(null));

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

        #endregion

        #region SaveSuccessStyle Property Members

        public const string DependencyPropertyName_SaveSuccessStyle = "SaveSuccessStyle";

        /// <summary>
        /// Identifies the <seealso cref="SaveSuccessStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaveSuccessStyleProperty = DependencyProperty.Register(DependencyPropertyName_SaveSuccessStyle, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

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
