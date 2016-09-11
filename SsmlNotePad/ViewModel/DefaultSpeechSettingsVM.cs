using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class DefaultSpeechSettingsVM : ValidatingViewModel
    {
        #region SelectedVoice Property Members

        public const string DependencyPropertyName_SelectedVoice = "SelectedVoice";

        /// <summary>
        /// Identifies the <seealso cref="SelectedVoice"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedVoiceProperty = DependencyProperty.Register(DependencyPropertyName_SelectedVoice, typeof(VoiceVM), typeof(DefaultSpeechSettingsVM),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DefaultSpeechSettingsVM).SelectedVoice_PropertyChanged((VoiceVM)(e.OldValue), (VoiceVM)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as DefaultSpeechSettingsVM).SelectedVoice_PropertyChanged((VoiceVM)(e.OldValue), (VoiceVM)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as DefaultSpeechSettingsVM).SelectedVoice_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public VoiceVM SelectedVoice
        {
            get
            {
                if (CheckAccess())
                    return (VoiceVM)(GetValue(SelectedVoiceProperty));
                return Dispatcher.Invoke(() => SelectedVoice);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedVoiceProperty, value);
                else
                    Dispatcher.Invoke(() => SelectedVoice = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedVoice"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="VoiceVM"/> value before the <seealso cref="SelectedVoice"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="VoiceVM"/> value after the <seealso cref="SelectedVoice"/> property was changed.</param>
        protected virtual void SelectedVoice_PropertyChanged(VoiceVM oldValue, VoiceVM newValue)
        {
            int index = _voiceOptions.IndexOf(newValue);
            if (index != SelectedIndex)
                SelectedIndex = index;
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedVoice"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual VoiceVM SelectedVoice_CoerceValue(object baseValue)
        {
            VoiceVM vm = baseValue as VoiceVM;
            if (vm != null && !_voiceOptions.Contains(vm))
                return null;

            return vm;
        }

        #endregion

        #region SelectedIndex Property Members

        public const string DependencyPropertyName_SelectedIndex = "SelectedIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedIndex, typeof(int), typeof(DefaultSpeechSettingsVM),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DefaultSpeechSettingsVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as DefaultSpeechSettingsVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as DefaultSpeechSettingsVM).SelectedIndex_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectedIndexProperty));
                return Dispatcher.Invoke(() => SelectedIndex);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedIndexProperty, value);
                else
                    Dispatcher.Invoke(() => SelectedIndex = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="SelectedIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="SelectedIndex"/> property was changed.</param>
        protected virtual void SelectedIndex_PropertyChanged(int oldValue, int newValue)
        {
            if (newValue < 0)
            {
                if (SelectedVoice != null)
                    SelectedVoice = null;
            }
            else if (SelectedVoice == null || !ReferenceEquals(SelectedVoice, _voiceOptions[newValue]))
                SelectedVoice = _voiceOptions[newValue];
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedIndex_CoerceValue(object baseValue)
        {
            int index = (int)baseValue;
            if (index > -1 && index < _voiceOptions.Count)
                return index;

            return -1;
        }

        #endregion

        #region VoiceOptions Property Members

        private ObservableCollection<VoiceVM> _voiceOptions = null;

        public const string PropertyName_VoiceOptions = "VoiceOptions";

        private static readonly DependencyPropertyKey VoiceOptionsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_VoiceOptions, typeof(ReadOnlyObservableCollection<VoiceVM>), typeof(DefaultSpeechSettingsVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="VoiceOptions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VoiceOptionsProperty = VoiceOptionsPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<VoiceVM> VoiceOptions
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<VoiceVM>)(GetValue(VoiceOptionsProperty));
                return Dispatcher.Invoke(() => VoiceOptions);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(VoiceOptionsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => VoiceOptions = value);
            }
        }

        #endregion

        #region RateText Property Members

        public const string DependencyPropertyName_RateText = "RateText";

        /// <summary>
        /// Identifies the <seealso cref="RateText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RateTextProperty = DependencyProperty.Register(DependencyPropertyName_RateText, typeof(string), typeof(DefaultSpeechSettingsVM),
                new PropertyMetadata("0",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DefaultSpeechSettingsVM).RateText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as DefaultSpeechSettingsVM).RateText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as DefaultSpeechSettingsVM).RateText_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string RateText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(RateTextProperty));
                return Dispatcher.Invoke(() => RateText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(RateTextProperty, value);
                else
                    Dispatcher.Invoke(() => RateText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="RateText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="RateText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="RateText"/> property was changed.</param>
        protected virtual void RateText_PropertyChanged(string oldValue, string newValue)
        {
            int rate;
            if (String.IsNullOrWhiteSpace(newValue))
                SetValidation(DependencyPropertyName_RateText, null);
            else if (!Int32.TryParse(newValue, out rate))
                SetValidation(DependencyPropertyName_RateText, "Invalid integer value");
            else if (rate < -10 || rate > 10)
                SetValidation(DependencyPropertyName_RateText, "Rate  must be a value from -10 to 10");
            else
            {
                SetValidation(DependencyPropertyName_RateText, null);
                if (Rate != rate)
                    Rate = rate;
            }

            SaveCommand.IsEnabled = InnerViewModelValidationMessages.Count == 0;
        }

        /// <summary>
        /// This gets called whenever <seealso cref="RateText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string RateText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region Rate Property Members

        public const string DependencyPropertyName_Rate = "Rate";

        /// <summary>
        /// Identifies the <seealso cref="Rate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RateProperty = DependencyProperty.Register(DependencyPropertyName_Rate, typeof(int), typeof(DefaultSpeechSettingsVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DefaultSpeechSettingsVM).Rate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as DefaultSpeechSettingsVM).Rate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as DefaultSpeechSettingsVM).Rate_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public int Rate
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(RateProperty));
                return Dispatcher.Invoke(() => Rate);
            }
            set
            {
                if (CheckAccess())
                    SetValue(RateProperty, value);
                else
                    Dispatcher.Invoke(() => Rate = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Rate"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="Rate"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="Rate"/> property was changed.</param>
        protected virtual void Rate_PropertyChanged(int oldValue, int newValue)
        {
            if (RateText != newValue.ToString())
                RateText = newValue.ToString();
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Rate"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int Rate_CoerceValue(object baseValue)
        {
            int i = (int)baseValue;
            return (i < -10) ? -10 : ((i > 10) ? 10 : i);
        }

        #endregion

        #region VolumeText Property Members

        public const string DependencyPropertyName_VolumeText = "VolumeText";

        /// <summary>
        /// Identifies the <seealso cref="VolumeText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeTextProperty = DependencyProperty.Register(DependencyPropertyName_VolumeText, typeof(string), typeof(DefaultSpeechSettingsVM),
                new PropertyMetadata("0",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DefaultSpeechSettingsVM).VolumeText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as DefaultSpeechSettingsVM).VolumeText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as DefaultSpeechSettingsVM).VolumeText_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string VolumeText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(VolumeTextProperty));
                return Dispatcher.Invoke(() => VolumeText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(VolumeTextProperty, value);
                else
                    Dispatcher.Invoke(() => VolumeText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="VolumeText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="VolumeText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="VolumeText"/> property was changed.</param>
        protected virtual void VolumeText_PropertyChanged(string oldValue, string newValue)
        {
            int volume;
            if (String.IsNullOrWhiteSpace(newValue))
                SetValidation(DependencyPropertyName_VolumeText, null);
            else if (!Int32.TryParse(newValue, out volume))
                SetValidation(DependencyPropertyName_VolumeText, "Invalid integer value");
            else if (volume < 0 || volume > 100)
                SetValidation(DependencyPropertyName_VolumeText, "Volume  must be a value from 0 to 100");
            else
            {
                SetValidation(DependencyPropertyName_VolumeText, null);
                if (Volume != volume)
                    Volume = volume;
            }

            SaveCommand.IsEnabled = InnerViewModelValidationMessages.Count == 0;
        }

        /// <summary>
        /// This gets called whenever <seealso cref="VolumeText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string VolumeText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region Volume Property Members

        public const string DependencyPropertyName_Volume = "Volume";

        /// <summary>
        /// Identifies the <seealso cref="Volume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(DependencyPropertyName_Volume, typeof(int), typeof(DefaultSpeechSettingsVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DefaultSpeechSettingsVM).Volume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as DefaultSpeechSettingsVM).Volume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as DefaultSpeechSettingsVM).Volume_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public int Volume
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(VolumeProperty));
                return Dispatcher.Invoke(() => Volume);
            }
            set
            {
                if (CheckAccess())
                    SetValue(VolumeProperty, value);
                else
                    Dispatcher.Invoke(() => Volume = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Volume"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="Volume"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="Volume"/> property was changed.</param>
        protected virtual void Volume_PropertyChanged(int oldValue, int newValue)
        {
            if (VolumeText != newValue.ToString())
                VolumeText = newValue.ToString();
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Volume"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int Volume_CoerceValue(object baseValue)
        {
            int i = (int)baseValue;
            return (i < 0) ? 0 : ((i > 100) ? 100 : i);
        }

        #endregion

        public static void EnsureSpeechSettings(SpeechSynthesizer speechSynthesizer = null)
        {
            if (speechSynthesizer != null)
            {
                Func<string> func = () =>
                {
                    if (speechSynthesizer.Rate != App.AppSettingsViewModel.DefaultSpeechRate)
                        speechSynthesizer.Rate = App.AppSettingsViewModel.DefaultSpeechRate;
                    if (speechSynthesizer.Volume != App.AppSettingsViewModel.DefaultSpeechVolume)
                        speechSynthesizer.Volume = App.AppSettingsViewModel.DefaultSpeechVolume;
                    return App.AppSettingsViewModel.DefaultVoiceName;
                };
                string voiceName = (App.AppSettingsViewModel.CheckAccess()) ? func() : App.AppSettingsViewModel.Dispatcher.Invoke(func);
                if (speechSynthesizer.Voice == null || speechSynthesizer.Voice.Name != voiceName)
                {
                    try { speechSynthesizer.SelectVoice(voiceName); }
                    catch { }
                }
            }
        }
        
        public DefaultSpeechSettingsVM()
        {
            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            {
                VoiceInfo[] enabledVoices = speechSynthesizer.GetInstalledVoices().Where(v => v.Enabled).Select(v => v.VoiceInfo).ToArray();
                if (speechSynthesizer.Voice != null && !enabledVoices.Any(v => v.Name == speechSynthesizer.Voice.Name))
                    enabledVoices = (new VoiceInfo[] { speechSynthesizer.Voice }).Concat(enabledVoices).ToArray();
                string voiceName = (App.AppSettingsViewModel.CheckAccess()) ? App.AppSettingsViewModel.DefaultVoiceName : App.AppSettingsViewModel.Dispatcher.Invoke(() => App.AppSettingsViewModel.DefaultVoiceName);
                VoiceInfo selectedVoice = enabledVoices.FirstOrDefault(v => v.Name != null && String.Compare(v.Name.Trim(), voiceName.Trim(), true) == 0);
                if (selectedVoice == null && (selectedVoice = enabledVoices.FirstOrDefault(v => v.Id != null && String.Compare(v.Id.Trim(), voiceName.Trim(), true) == 0)) == null)
                    selectedVoice = enabledVoices.FirstOrDefault();
                VoiceVM selectedVm = null;
                _voiceOptions = new ObservableCollection<VoiceVM>();
                foreach (VoiceInfo v in enabledVoices)
                {
                    VoiceVM vm = new VoiceVM(v);
                    if (selectedVoice != null && ReferenceEquals(v, selectedVoice))
                        selectedVm = vm;
                    _voiceOptions.Add(vm);
                }
                VoiceOptions = new ReadOnlyObservableCollection<VoiceVM>(_voiceOptions);
                SelectedVoice = selectedVm;
                Rate = Properties.Settings.Default.DefaultSpeechRate;
                Volume = Properties.Settings.Default.DefaultSpeechVolume;
            }
        }

        #region Save Command Property Members

        private Command.RelayCommand _saveCommand = null;

        public Command.RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new Command.RelayCommand(OnSave);

                return _saveCommand;
            }
        }

        protected virtual void OnSave(object parameter)
        {
            Action action = () =>
            {
                if (App.AppSettingsViewModel.DefaultSpeechRate != Rate)
                    App.AppSettingsViewModel.DefaultSpeechRate = Rate;
                if (App.AppSettingsViewModel.DefaultSpeechVolume != Volume)
                    App.AppSettingsViewModel.DefaultSpeechVolume = Volume;
                string s = (SelectedVoice == null) ? "" : SelectedVoice.Name;
                if (App.AppSettingsViewModel.DefaultVoiceName != s)
                    App.AppSettingsViewModel.DefaultVoiceName = s;
            };
            if (App.AppSettingsViewModel.CheckAccess())
                action();
            else
                App.AppSettingsViewModel.Dispatcher.Invoke(action);

            View.DefaultSpeechSettingsWindow window = App.GetWindowByDataContext<View.DefaultSpeechSettingsWindow, DefaultSpeechSettingsVM>(this);
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        #endregion

        #region Cancel Command Property Members

        private Command.RelayCommand _cancelCommand = null;

        public Command.RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new Command.RelayCommand(OnCancel);

                return _cancelCommand;
            }
        }

        protected virtual void OnCancel(object parameter)
        {
            View.DefaultSpeechSettingsWindow window = App.GetWindowByDataContext<View.DefaultSpeechSettingsWindow, DefaultSpeechSettingsVM>(this);
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        #endregion
    }
}