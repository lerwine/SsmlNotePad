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
    public class PhonemeResultVM : DependencyObject
    {
        #region Text Property Members

        public const string DependencyPropertyName_Text = "Text";

        /// <summary>
        /// Identifies the <seealso cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(DependencyPropertyName_Text, typeof(string), typeof(PhonemeResultVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as PhonemeResultVM).Text_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as PhonemeResultVM).Text_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as PhonemeResultVM).Text_CoerceValue(baseValue)));

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
            set
            {
                if (CheckAccess())
                    SetValue(TextProperty, value);
                else
                    Dispatcher.Invoke(() => Text = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Text"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Text"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Text"/> property was changed.</param>
        protected virtual void Text_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement PhonemeResultVM.Text_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Text"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Text_CoerceValue(object baseValue)
        {
            // TODO: Implement PhonemeResultVM.Text_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region PhonemeText Property Members

        public const string DependencyPropertyName_PhonemeText = "PhonemeText";

        /// <summary>
        /// Identifies the <seealso cref="PhonemeText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhonemeTextProperty = DependencyProperty.Register(DependencyPropertyName_PhonemeText, typeof(string), typeof(PhonemeResultVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as PhonemeResultVM).PhonemeText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as PhonemeResultVM).PhonemeText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as PhonemeResultVM).PhonemeText_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string PhonemeText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PhonemeTextProperty));
                return Dispatcher.Invoke(() => PhonemeText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PhonemeTextProperty, value);
                else
                    Dispatcher.Invoke(() => PhonemeText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="PhonemeText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="PhonemeText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="PhonemeText"/> property was changed.</param>
        protected virtual void PhonemeText_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement PhonemeResultVM.PhonemeText_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="PhonemeText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string PhonemeText_CoerceValue(object baseValue)
        {
            // TODO: Implement PhonemeResultVM.PhonemeText_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region AudioPosition Property Members

        public const string PropertyName_AudioPosition = "AudioPosition";

        private static readonly DependencyPropertyKey AudioPositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_AudioPosition, typeof(TimeSpanVM), typeof(PhonemeResultVM),
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

        #region CharacterPosition Property Members

        public const string DependencyPropertyName_CharacterPosition = "CharacterPosition";

        /// <summary>
        /// Identifies the <seealso cref="CharacterPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterPositionProperty = DependencyProperty.Register(DependencyPropertyName_CharacterPosition, typeof(int), typeof(PhonemeResultVM),
                new PropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public int CharacterPosition
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(CharacterPositionProperty));
                return Dispatcher.Invoke(() => CharacterPosition);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CharacterPositionProperty, value);
                else
                    Dispatcher.Invoke(() => CharacterPosition = value);
            }
        }

        #endregion

        #region Phonemes Property Members

        private ObservableCollection<PhonemeVM> _phonemes = new ObservableCollection<PhonemeVM>();

        public const string PropertyName_Phonemes = "Phonemes";

        private static readonly DependencyPropertyKey PhonemesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Phonemes, typeof(ReadOnlyObservableCollection<PhonemeVM>), typeof(PhonemeResultVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Phonemes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhonemesProperty = PhonemesPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<PhonemeVM> Phonemes
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<PhonemeVM>)(GetValue(PhonemesProperty));
                return Dispatcher.Invoke(() => Phonemes);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(PhonemesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Phonemes = value);
            }
        }

        #endregion

        public PhonemeResultVM()
        {
            AudioPosition = new TimeSpanVM();
            Phonemes = new ReadOnlyObservableCollection<PhonemeVM>(_phonemes);
        }

        public PhonemeResultVM(PhoneticGroupInfo phoneticGroupInfo)
            : this()
        {
            AudioPosition.SetTimeSpan(phoneticGroupInfo.AudioPosition);
            CharacterPosition = phoneticGroupInfo.CharacterPosition;
            Text = phoneticGroupInfo.Text;
            foreach (PhonemeInfo phoneme in phoneticGroupInfo)
                _phonemes.Add(new PhonemeVM(phoneme));
            PhonemeText = String.Join(" ", phoneticGroupInfo.Select(g => g.Phoneme).ToArray());
        }
    }
}