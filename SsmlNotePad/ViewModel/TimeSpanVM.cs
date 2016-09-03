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

        internal void SetTimeSpan(TimeSpan timeSpan)
        {
            Days = timeSpan.Days;
            Hours = timeSpan.Hours;
            Minutes = timeSpan.Minutes;
            Seconds = timeSpan.Seconds;
            Milliseconds = timeSpan.Milliseconds;
        }
    }
}