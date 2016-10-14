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
    [ValueConversion(typeof(SpeechState), typeof(int))]
    public class SpeechStateToIntConverter : DependencyObject, IValueConverter
    {
        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <seealso cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(int?), typeof(SpeechStateToIntConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public int? NullValue
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(NullValueProperty));
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

        #region FaultedValue Property Members

        public const string DependencyPropertyName_FaultedValue = "FaultedValue";

        /// <summary>
        /// Identifies the <seealso cref="FaultedValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FaultedValueProperty = DependencyProperty.Register(DependencyPropertyName_FaultedValue, typeof(int?), typeof(SpeechStateToIntConverter),
                new PropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public int? FaultedValue
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(FaultedValueProperty));
                return Dispatcher.Invoke(() => FaultedValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(FaultedValueProperty, value);
                else
                    Dispatcher.Invoke(() => FaultedValue = value);
            }
        }
        
        #endregion

        #region CanceledValue Property Members

        public const string DependencyPropertyName_CanceledValue = "CanceledValue";

        /// <summary>
        /// Identifies the <seealso cref="CanceledValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanceledValueProperty = DependencyProperty.Register(DependencyPropertyName_CanceledValue, typeof(int?), typeof(SpeechStateToIntConverter),
                new PropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public int? CanceledValue
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(CanceledValueProperty));
                return Dispatcher.Invoke(() => CanceledValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CanceledValueProperty, value);
                else
                    Dispatcher.Invoke(() => CanceledValue = value);
            }
        }
        
        #endregion

        #region PausedValue Property Members

        public const string DependencyPropertyName_PausedValue = "PausedValue";

        /// <summary>
        /// Identifies the <seealso cref="PausedValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PausedValueProperty = DependencyProperty.Register(DependencyPropertyName_PausedValue, typeof(int?), typeof(SpeechStateToIntConverter),
                new PropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public int? PausedValue
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(PausedValueProperty));
                return Dispatcher.Invoke(() => PausedValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PausedValueProperty, value);
                else
                    Dispatcher.Invoke(() => PausedValue = value);
            }
        }
        
        #endregion

        #region NormalValue Property Members

        public const string DependencyPropertyName_NormalValue = "NormalValue";

        /// <summary>
        /// Identifies the <seealso cref="NormalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalValueProperty = DependencyProperty.Register(DependencyPropertyName_NormalValue, typeof(int?), typeof(SpeechStateToIntConverter),
                new PropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public int? NormalValue
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(NormalValueProperty));
                return Dispatcher.Invoke(() => NormalValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NormalValueProperty, value);
                else
                    Dispatcher.Invoke(() => NormalValue = value);
            }
        }
        

        #endregion

        public int? Convert(SpeechState? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return NullValue;

            switch (value.Value)
            {
                case SpeechState.Faulted:
                    return FaultedValue;
                case SpeechState.Canceled:
                    return CanceledValue;
                case SpeechState.Paused:
                    return PausedValue;
            }

            return NormalValue;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null || targetType.Equals(typeof(int)) || targetType.Equals(typeof(int?)))
                return Convert(value as SpeechState?, parameter, culture);

            return System.Convert.ChangeType(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
