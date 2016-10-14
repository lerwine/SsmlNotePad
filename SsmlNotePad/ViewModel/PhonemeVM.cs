using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Erwine.Leonard.T.SsmlNotePad.Process;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class PhonemeVM : DependencyObject
    {
        #region Phoneme Property Members

        public const string DependencyPropertyName_Phoneme = "Phoneme";

        /// <summary>
        /// Identifies the <seealso cref="Phoneme"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhonemeProperty = DependencyProperty.Register(DependencyPropertyName_Phoneme, typeof(string), typeof(PhonemeVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as PhonemeVM).Phoneme_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as PhonemeVM).Phoneme_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as PhonemeVM).Phoneme_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string Phoneme
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PhonemeProperty));
                return Dispatcher.Invoke(() => Phoneme);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PhonemeProperty, value);
                else
                    Dispatcher.Invoke(() => Phoneme = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Phoneme"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Phoneme"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Phoneme"/> property was changed.</param>
        protected virtual void Phoneme_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement PhonemeVM.Phoneme_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Phoneme"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Phoneme_CoerceValue(object baseValue)
        {
            // TODO: Implement PhonemeVM.Phoneme_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region IsStressed Property Members

        public const string DependencyPropertyName_IsStressed = "IsStressed";

        /// <summary>
        /// Identifies the <seealso cref="IsStressed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsStressedProperty = DependencyProperty.Register(DependencyPropertyName_IsStressed, typeof(bool), typeof(PhonemeVM),
                new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public bool IsStressed
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(IsStressedProperty));
                return Dispatcher.Invoke(() => IsStressed);
            }
            set
            {
                if (CheckAccess())
                    SetValue(IsStressedProperty, value);
                else
                    Dispatcher.Invoke(() => IsStressed = value);
            }
        }

        #endregion

        #region Duration Property Members

        public const string PropertyName_Duration = "Duration";

        private static readonly DependencyPropertyKey DurationPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Duration, typeof(TimeSpanVM), typeof(PhonemeVM),
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

        #region RelativePosition Property Members

        public const string PropertyName_RelativePosition = "RelativePosition";

        private static readonly DependencyPropertyKey RelativePositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_RelativePosition, typeof(TimeSpanVM), typeof(PhonemeVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="RelativePosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RelativePositionProperty = RelativePositionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpanVM RelativePosition
        {
            get
            {
                if (CheckAccess())
                    return (TimeSpanVM)(GetValue(RelativePositionProperty));
                return Dispatcher.Invoke(() => RelativePosition);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(RelativePositionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => RelativePosition = value);
            }
        }

        #endregion

        public PhonemeVM()
        {
            Duration = new TimeSpanVM();
            RelativePosition = new TimeSpanVM();
        }

        public PhonemeVM(PhonemeInfo phoneme)
            : this()
        {
            Phoneme = phoneme.Phoneme;
            IsStressed = phoneme.IsStressed;
            Duration.SetTimeSpan(phoneme.Duration);
            RelativePosition.SetTimeSpan(phoneme.RelativePosition);
        }
    }
}