using System;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ValueAndDurationQueueVM<T> : DependencyObject
    {
        public ValueAndDurationQueueVM() { Duration = new ViewModel.TimeSpanVM(); }

        #region CurrentValue Property Members

        public const string PropertyName_CurrentValue = "CurrentValue";

        private static readonly DependencyPropertyKey CurrentValuePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentValue, typeof(T), typeof(ValueAndDurationQueueVM<T>),
                new PropertyMetadata(default(T)));

        /// <summary>
        /// Identifies the <seealso cref="CurrentValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty = CurrentValuePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public T CurrentValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(CurrentValueProperty));
                return Dispatcher.Invoke(() => CurrentValue);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(CurrentValuePropertyKey, value);
                else
                    Dispatcher.Invoke(() => CurrentValue = value);
            }
        }

        #endregion

        #region Duration Property Members

        public const string PropertyName_Duration = "Duration";

        private static readonly DependencyPropertyKey DurationPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Duration, typeof(TimeSpanVM), typeof(ValueAndDurationQueueVM<T>),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Duration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DurationPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpanVM Duration
        {
            get
            {
                if (CheckAccess())
                    return (TimeSpanVM)(GetValue(DurationProperty));
                return Dispatcher.Invoke(() => Duration);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DurationPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Duration = value);
            }
        }

        #endregion

        #region NextValue Property Members

        public const string PropertyName_NextValue = "NextValue";

        private static readonly DependencyPropertyKey NextValuePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_NextValue, typeof(T), typeof(ValueAndDurationQueueVM<T>),
                new PropertyMetadata(default(T)));

        /// <summary>
        /// Identifies the <seealso cref="NextValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NextValueProperty = NextValuePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public T NextValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(NextValueProperty));
                return Dispatcher.Invoke(() => NextValue);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(NextValuePropertyKey, value);
                else
                    Dispatcher.Invoke(() => NextValue = value);
            }
        }

        #endregion

        internal void SetQueue(T currentValue, TimeSpan duration, T nextValue)
        {
            CurrentValue = currentValue;
            Duration.SetTimeSpan(duration);
            NextValue = nextValue;
        }
    }
}