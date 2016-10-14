using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class PositionalValueVM<T> : DependencyObject
    {
        public PositionalValueVM() { AudioPosition = new ViewModel.TimeSpanVM(); }

        public PositionalValueVM(TimeSpan audioPosition, T value)
            : this()
        {
            AudioPosition.SetTimeSpan(audioPosition);
            Value = value;
        }

        #region Value Property Members

        public const string DependencyPropertyName_Value = "Value";

        /// <summary>
        /// Identifies the <seealso cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(DependencyPropertyName_Value, typeof(T), typeof(PositionalValueVM<T>),
                new PropertyMetadata(default(T)));

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(ValueProperty));
                return Dispatcher.Invoke(() => Value);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ValueProperty, value);
                else
                    Dispatcher.Invoke(() => Value = value);
            }
        }

        #endregion

        #region AudioPosition Property Members

        public const string PropertyName_AudioPosition = "AudioPosition";

        private static readonly DependencyPropertyKey AudioPositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_AudioPosition, typeof(TimeSpanVM), typeof(PositionalValueVM<T>),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="AudioPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AudioPositionProperty = AudioPositionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpanVM AudioPosition
        {
            get
            {
                if (CheckAccess())
                    return (TimeSpanVM)(GetValue(AudioPositionProperty));
                return Dispatcher.Invoke(() => AudioPosition);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(AudioPositionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => AudioPosition = value);
            }
        }

        #endregion

    }
}
