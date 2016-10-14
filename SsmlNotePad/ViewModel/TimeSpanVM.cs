using System;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class TimeSpanVM : DependencyObject
    {
        #region Days Property Members

        public const string PropertyName_Days = "Days";

        private static readonly DependencyPropertyKey DaysPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Days, typeof(int), typeof(TimeSpanVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="Days"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DaysProperty = DaysPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int Days
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(DaysProperty));
                return Dispatcher.Invoke(() => Days);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DaysPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Days = value);
            }
        }

        #endregion

        #region Hours Property Members

        public const string PropertyName_Hours = "Hours";

        private static readonly DependencyPropertyKey HoursPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Hours, typeof(int), typeof(TimeSpanVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="Hours"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HoursProperty = HoursPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int Hours
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(HoursProperty));
                return Dispatcher.Invoke(() => Hours);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(HoursPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Hours = value);
            }
        }

        #endregion

        #region HoursTotal Property Members

        public const string DependencyPropertyName_HoursTotal = "HoursTotal";

        /// <summary>
        /// Identifies the <seealso cref="HoursTotal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HoursTotalProperty = DependencyProperty.Register(DependencyPropertyName_HoursTotal, typeof(long), typeof(TimeSpanVM),
                new PropertyMetadata(0L,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as TimeSpanVM).HoursTotal_PropertyChanged((long)(e.OldValue), (long)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as TimeSpanVM).HoursTotal_PropertyChanged((long)(e.OldValue), (long)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as TimeSpanVM).HoursTotal_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public long HoursTotal
        {
            get
            {
                if (CheckAccess())
                    return (long)(GetValue(HoursTotalProperty));
                return Dispatcher.Invoke(() => HoursTotal);
            }
            set
            {
                if (CheckAccess())
                    SetValue(HoursTotalProperty, value);
                else
                    Dispatcher.Invoke(() => HoursTotal = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="HoursTotal"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="long"/> value before the <seealso cref="HoursTotal"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="long"/> value after the <seealso cref="HoursTotal"/> property was changed.</param>
        protected virtual void HoursTotal_PropertyChanged(long oldValue, long newValue)
        {
            // TODO: Implement TimeSpanVM.HoursTotal_PropertyChanged(long, long)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="HoursTotal"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual long HoursTotal_CoerceValue(object baseValue)
        {
            // TODO: Implement TimeSpanVM.HoursTotal_CoerceValue(DependencyObject, object)
            return (long)baseValue;
        }

        #endregion

        #region Minutes Property Members

        public const string PropertyName_Minutes = "Minutes";

        private static readonly DependencyPropertyKey MinutesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Minutes, typeof(int), typeof(TimeSpanVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="Minutes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinutesProperty = MinutesPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int Minutes
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(MinutesProperty));
                return Dispatcher.Invoke(() => Minutes);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(MinutesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Minutes = value);
            }
        }

        #endregion

        #region Seconds Property Members

        public const string PropertyName_Seconds = "Seconds";

        private static readonly DependencyPropertyKey SecondsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Seconds, typeof(int), typeof(TimeSpanVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="Seconds"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondsProperty = SecondsPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int Seconds
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SecondsProperty));
                return Dispatcher.Invoke(() => Seconds);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SecondsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Seconds = value);
            }
        }

        #endregion

        #region Milliseconds Property Members

        public const string PropertyName_Milliseconds = "Milliseconds";

        private static readonly DependencyPropertyKey MillisecondsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Milliseconds, typeof(int), typeof(TimeSpanVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="Milliseconds"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MillisecondsProperty = MillisecondsPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int Milliseconds
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(MillisecondsProperty));
                return Dispatcher.Invoke(() => Milliseconds);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(MillisecondsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Milliseconds = value);
            }
        }

        #endregion

        #region Text Property Members

        public const string PropertyName_Text = "Text";

        private static readonly DependencyPropertyKey TextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Text, typeof(string), typeof(TimeSpanVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(TextProperty));
                return Dispatcher.Invoke(() => Text);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(TextPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Text = value);
            }
        }

        #endregion

        internal void SetTimeSpan(TimeSpan timeSpan)
        {
            Days = timeSpan.Days;
            Hours = timeSpan.Hours;
            HoursTotal = ((long)(timeSpan.Days) * 24L) + (long)(timeSpan.Hours);
            Minutes = timeSpan.Minutes;
            Seconds = timeSpan.Seconds;
            Milliseconds = timeSpan.Milliseconds;
            Text = String.Format("{0}:{1:D2}:{2:D2}.{3:D6}", HoursTotal, Minutes, Seconds, Milliseconds);
        }
    }
}