using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class SpeechGenerationStatusVM : DependencyObject
    {
        public SpeechGenerationStatusVM()
        {
            Messages = new ReadOnlyObservableCollection<SpeechMessageVM>(_messages);
            Voice = new VoiceVM();
            BookmarksReached = new ReadOnlyObservableCollection<string>(_bookmarksReached);
            AudioPosition = new TimeSpanVM();
            BookmarkAudioPosition = new TimeSpanVM();
        }

        #region Voice Property Members

        public const string PropertyName_Voice = "Voice";

        private static readonly DependencyPropertyKey VoicePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Voice, typeof(VoiceVM), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Voice"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VoiceProperty = VoicePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public VoiceVM Voice
        {
            get
            {
                if (CheckAccess())
                    return (VoiceVM)(GetValue(VoiceProperty));
                return Dispatcher.Invoke(() => Voice);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(VoicePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Voice = value);
            }
        }

        #endregion

        #region Volume Property Members

        public const string DependencyPropertyName_Volume = "Volume";

        /// <summary>
        /// Identifies the <seealso cref="Volume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(DependencyPropertyName_Volume, typeof(int), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechGenerationStatusVM).Volume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechGenerationStatusVM).Volume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechGenerationStatusVM).Volume_CoerceValue(baseValue)));

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
        protected virtual void Volume_PropertyChanged(int oldValue, int newValue) { }

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
        
        #region Rate Property Members

        public const string DependencyPropertyName_Rate = "Rate";

        /// <summary>
        /// Identifies the <seealso cref="Rate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RateProperty = DependencyProperty.Register(DependencyPropertyName_Rate, typeof(int), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechGenerationStatusVM).Rate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechGenerationStatusVM).Rate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechGenerationStatusVM).Rate_CoerceValue(baseValue)));

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
        protected virtual void Rate_PropertyChanged(int oldValue, int newValue) { }

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

        #region RecentBookmarkCount Property Members

        public const string DependencyPropertyName_RecentBookmarkCount = "RecentBookmarkCount";

        /// <summary>
        /// Identifies the <seealso cref="RecentBookmarkCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecentBookmarkCountProperty = DependencyProperty.Register(DependencyPropertyName_RecentBookmarkCount, typeof(int), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechGenerationStatusVM).RecentBookmarkCount_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechGenerationStatusVM).RecentBookmarkCount_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechGenerationStatusVM).RecentBookmarkCount_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public int RecentBookmarkCount
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(RecentBookmarkCountProperty));
                return Dispatcher.Invoke(() => RecentBookmarkCount);
            }
            set
            {
                if (CheckAccess())
                    SetValue(RecentBookmarkCountProperty, value);
                else
                    Dispatcher.Invoke(() => RecentBookmarkCount = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="RecentBookmarkCount"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="RecentBookmarkCount"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="RecentBookmarkCount"/> property was changed.</param>
        protected virtual void RecentBookmarkCount_PropertyChanged(int oldValue, int newValue)
        {
            if (newValue < 1)
                return;

            lock (_bookmarksReached)
            {
                while (_bookmarksReached.Count > newValue)
                    _bookmarksReached.RemoveAt(0);
            }
        }

        /// <summary>
        /// This gets called whenever <seealso cref="RecentBookmarkCount"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int RecentBookmarkCount_CoerceValue(object baseValue)
        {
            int recentBookmarkCount = (int)baseValue;
            return (recentBookmarkCount < 0) ? 0 : recentBookmarkCount;
        }

        #endregion

        #region BookmarksReached Property Members

        private ObservableCollection<string> _bookmarksReached = new ObservableCollection<string>();

        public const string PropertyName_BookmarksReached = "BookmarksReached";

        private static readonly DependencyPropertyKey BookmarksReachedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BookmarksReached, typeof(ReadOnlyObservableCollection<string>), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="BookmarksReached"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BookmarksReachedProperty = BookmarksReachedPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<string> BookmarksReached
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<string>)(GetValue(BookmarksReachedProperty));
                return Dispatcher.Invoke(() => BookmarksReached);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(BookmarksReachedPropertyKey, value);
                else
                    Dispatcher.Invoke(() => BookmarksReached = value);
            }
        }

        #endregion

        #region AudioPosition Property Members

        public const string PropertyName_AudioPosition = "AudioPosition";

        private static readonly DependencyPropertyKey AudioPositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_AudioPosition, typeof(TimeSpanVM), typeof(SpeechGenerationStatusVM),
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
        public static readonly DependencyProperty CharacterPositionProperty = DependencyProperty.Register(DependencyPropertyName_CharacterPosition, typeof(int), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechGenerationStatusVM).CharacterPosition_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechGenerationStatusVM).CharacterPosition_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                }));

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

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="CharacterPosition"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="CharacterPosition"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="CharacterPosition"/> property was changed.</param>
        protected virtual void CharacterPosition_PropertyChanged(int oldValue, int newValue) { }
        
        #endregion

        #region CharacterCount Property Members

        public const string DependencyPropertyName_CharacterCount = "CharacterCount";

        /// <summary>
        /// Identifies the <seealso cref="CharacterCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterCountProperty = DependencyProperty.Register(DependencyPropertyName_CharacterCount, typeof(int), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechGenerationStatusVM).CharacterCount_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechGenerationStatusVM).CharacterCount_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                }));

        /// <summary>
        /// 
        /// </summary>
        public int CharacterCount
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(CharacterCountProperty));
                return Dispatcher.Invoke(() => CharacterCount);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CharacterCountProperty, value);
                else
                    Dispatcher.Invoke(() => CharacterCount = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="CharacterCount"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="CharacterCount"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="CharacterCount"/> property was changed.</param>
        protected virtual void CharacterCount_PropertyChanged(int oldValue, int newValue) { }
        
        #endregion

        #region PercentComplete Property Members

        public const string DependencyPropertyName_PercentComplete = "PercentComplete";

        /// <summary>
        /// Identifies the <seealso cref="PercentComplete"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PercentCompleteProperty = DependencyProperty.Register(DependencyPropertyName_PercentComplete, typeof(int), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechGenerationStatusVM).PercentComplete_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechGenerationStatusVM).PercentComplete_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechGenerationStatusVM).PercentComplete_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public int PercentComplete
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(PercentCompleteProperty));
                return Dispatcher.Invoke(() => PercentComplete);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PercentCompleteProperty, value);
                else
                    Dispatcher.Invoke(() => PercentComplete = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="PercentComplete"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="PercentComplete"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="PercentComplete"/> property was changed.</param>
        protected virtual void PercentComplete_PropertyChanged(int oldValue, int newValue) { }

        /// <summary>
        /// This gets called whenever <seealso cref="PercentComplete"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int PercentComplete_CoerceValue(object baseValue)
        {
            int i = (int)baseValue;
            return (i < 0) ? 0 : ((i > 100) ? 100 : i);
        }

        #endregion

        #region CurrentText Property Members

        public const string DependencyPropertyName_CurrentText = "CurrentText";

        /// <summary>
        /// Identifies the <seealso cref="CurrentText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentTextProperty = DependencyProperty.Register(DependencyPropertyName_CurrentText, typeof(string), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechGenerationStatusVM).CurrentText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechGenerationStatusVM).CurrentText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechGenerationStatusVM).CurrentText_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string CurrentText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(CurrentTextProperty));
                return Dispatcher.Invoke(() => CurrentText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CurrentTextProperty, value);
                else
                    Dispatcher.Invoke(() => CurrentText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="CurrentText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="CurrentText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="CurrentText"/> property was changed.</param>
        protected virtual void CurrentText_PropertyChanged(string oldValue, string newValue) { }

        /// <summary>
        /// This gets called whenever <seealso cref="CurrentText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string CurrentText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion
        
        #region BookmarkAudioPosition Property Members

        public const string PropertyName_BookmarkAudioPosition = "BookmarkAudioPosition";

        private static readonly DependencyPropertyKey BookmarkAudioPositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BookmarkAudioPosition, typeof(TimeSpanVM), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="BookmarkAudioPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BookmarkAudioPositionProperty = BookmarkAudioPositionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpanVM BookmarkAudioPosition
        {
            get
            {
                if (CheckAccess())
                    return (TimeSpanVM)(GetValue(BookmarkAudioPositionProperty));
                return Dispatcher.Invoke(() => BookmarkAudioPosition);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(BookmarkAudioPositionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => BookmarkAudioPosition = value);
            }
        }

        #endregion
        
        #region Status Property Members

        public const string PropertyName_Status = "Status";

        private static readonly DependencyPropertyKey StatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Status, typeof(SpeechState), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(SpeechState.NotStarted));

        /// <summary>
        /// Identifies the <seealso cref="Status"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = StatusPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public SpeechState Status
        {
            get
            {
                if (CheckAccess())
                    return (SpeechState)(GetValue(StatusProperty));
                return Dispatcher.Invoke(() => Status);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(StatusPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Status = value);
            }
        }

        #endregion

        #region Messages Property Members

        private ObservableCollection<SpeechMessageVM> _messages = new ObservableCollection<SpeechMessageVM>();

        public const string PropertyName_Messages = "Messages";

        private static readonly DependencyPropertyKey MessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Messages, typeof(ReadOnlyObservableCollection<SpeechMessageVM>), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Messages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessagesProperty = MessagesPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<SpeechMessageVM> Messages
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<SpeechMessageVM>)(GetValue(MessagesProperty));
                return Dispatcher.Invoke(() => Messages);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(MessagesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Messages = value);
            }
        }

        #endregion

        internal void SetMessage(string eventName, string message, MessageSeverity severity = MessageSeverity.Information)
        {
            _messages.Add(SpeechMessageVM.Create(eventName, message, severity));
        }

        internal void SetCurrentBookmark(string bookmark, TimeSpan audioPosition)
        {
            _bookmarksReached.Add(bookmark);
            BookmarkAudioPosition.SetTimeSpan(audioPosition);
        }

        internal void SetFault(Exception exception) { _messages.Add(SpeechMessageVM.Create(exception)); }

        #region SpeakSelectedText Command Property Members

        private Command.RelayCommand _speakSelectedTextCommand = null;

        public Command.RelayCommand SpeakSelectedTextCommand
        {
            get
            {
                if (_speakSelectedTextCommand == null)
                    _speakSelectedTextCommand = new Command.RelayCommand(OnSpeakSelectedText);

                return _speakSelectedTextCommand;
            }
        }

        protected virtual void OnSpeakSelectedText(object parameter)
        {
            MainWindowVM viewModel = App.MainWindowViewModel;
            if (viewModel != null)
                Process.SpeechGenerationManager2.StartOutputToDefaultAudioDevice(this, viewModel.SelectedText, true);
        }

        #endregion

        #region SpeakAllText Command Property Members

        private Command.RelayCommand _speakAllTextCommand = null;

        public Command.RelayCommand SpeakAllTextCommand
        {
            get
            {
                if (_speakAllTextCommand == null)
                    _speakAllTextCommand = new Command.RelayCommand(OnSpeakAllText);

                return _speakAllTextCommand;
            }
        }

        protected virtual void OnSpeakAllText(object parameter)
        {
            MainWindowVM viewModel = App.MainWindowViewModel;
            if (viewModel != null)
                Process.SpeechGenerationManager2.StartOutputToDefaultAudioDevice(this, (viewModel.CheckAccess()) ? viewModel.Text : viewModel.Dispatcher.Invoke(() => viewModel.Text));
        }

        #endregion

        #region ExportToWavFile Command Property Members

        private Command.RelayCommand _exportToWavFileCommand = null;

        public Command.RelayCommand ExportToWavFileCommand
        {
            get
            {
                if (_exportToWavFileCommand == null)
                    _exportToWavFileCommand = new Command.RelayCommand(OnExportToWavFile);

                return _exportToWavFileCommand;
            }
        }

        protected virtual void OnExportToWavFile(object parameter)
        {
            MainWindowVM viewModel = App.MainWindowViewModel;
            if (viewModel == null)
                return;

            Window owner = App.GetWindowByDataContext<MainWindow, MainWindowVM>(viewModel) ?? App.Current.MainWindow;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.OverwritePrompt = true;
            string fileName;
            if (String.IsNullOrEmpty(viewModel.FileSaveLocation))
                fileName = "";
            else
                try { fileName = Path.GetFileNameWithoutExtension(viewModel.FileSaveLocation) + ".wav"; } catch { fileName = ""; }
            if (Common.FileUtility.InvokeWavFileDialog(dialog, owner, fileName))
                Process.SpeechGenerationManager2.StartOutputToWaveFile(this, viewModel.Text, dialog.FileName);
        }

        #endregion

        #region Pause Command Property Members

        private Command.RelayCommand _pauseCommand = null;

        public Command.RelayCommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                    _pauseCommand = new Command.RelayCommand(OnPause, false, true);

                return _pauseCommand;
            }
        }

        protected virtual void OnPause(object parameter) { Process.SpeechGenerationManager2.PauseSpeech(); }

        #endregion

        #region CancelButtonText Property Members

        public const string PropertyName_CancelButtonText = "CancelButtonText";

        private static readonly DependencyPropertyKey CancelButtonTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CancelButtonText, typeof(string), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata("Cancel"));

        /// <summary>
        /// Identifies the <seealso cref="CancelButtonText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelButtonTextProperty = CancelButtonTextPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string CancelButtonText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(CancelButtonTextProperty));
                return Dispatcher.Invoke(() => CancelButtonText);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(CancelButtonTextPropertyKey, value);
                else
                    Dispatcher.Invoke(() => CancelButtonText = value);
            }
        }

        #endregion

        #region Resume Command Property Members

        private Command.RelayCommand _resumeCommand = null;

        public Command.RelayCommand ResumeCommand
        {
            get
            {
                if (_resumeCommand == null)
                    _resumeCommand = new Command.RelayCommand(OnResume, false, true);

                return _resumeCommand;
            }
        }

        protected virtual void OnResume(object parameter) { Process.SpeechGenerationManager2.ResumeSpeech(); }

        #endregion

        #region Cancel Command Property Members

        private Command.RelayCommand _cancelCommand = null;

        public Command.RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new Command.RelayCommand(OnCancel, false, true);

                return _cancelCommand;
            }
        }

        protected virtual void OnCancel(object parameter)
        {
            if (Status.HasFlag(SpeechState.Canceled) || Status.HasFlag(SpeechState.Completed))
                IsBusy = false;
            else
                Process.SpeechGenerationManager2.CancelSpeech();
        }

        #endregion

        #region IsBusy Property Members

        public const string PropertyName_IsBusy = "IsBusy";

        private static readonly DependencyPropertyKey IsBusyPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsBusy, typeof(bool), typeof(SpeechGenerationStatusVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="IsBusy"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = IsBusyPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public bool IsBusy
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(IsBusyProperty));
                return Dispatcher.Invoke(() => IsBusy);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(IsBusyPropertyKey, value);
                else
                    Dispatcher.Invoke(() => IsBusy = value);
            }
        }

        #endregion

        internal void SetWarning(Exception error, TimeSpan audioPosition) { _messages.Add(SpeechMessageVM.Create(error, MessageSeverity.Warning)); }

        internal void SetStatus(Process.SpeechGenerationStatus status)
        {
            switch (status)
            {
                case Process.SpeechGenerationStatus.Canceled:
                case Process.SpeechGenerationStatus.Completed:
                case Process.SpeechGenerationStatus.ErrorAborted:
                    SpeakAllTextCommand.IsEnabled = true;
                    SpeakSelectedTextCommand.IsEnabled = true;
                    ExportToWavFileCommand.IsEnabled = true;
                    PauseCommand.IsEnabled = false;
                    ResumeCommand.IsEnabled = false;
                    CancelCommand.IsEnabled = false;
                    break;
                case Process.SpeechGenerationStatus.Resuming:
                    SpeakAllTextCommand.IsEnabled = false;
                    SpeakSelectedTextCommand.IsEnabled = false;
                    SpeakSelectedTextCommand.IsEnabled = false;
                    PauseCommand.IsEnabled = false;
                    ResumeCommand.IsEnabled = false;
                    CancelCommand.IsEnabled = true;
                    break;
                case Process.SpeechGenerationStatus.Paused:
                    SpeakAllTextCommand.IsEnabled = false;
                    SpeakSelectedTextCommand.IsEnabled = false;
                    ExportToWavFileCommand.IsEnabled = false;
                    PauseCommand.IsEnabled = false;
                    ResumeCommand.IsEnabled = true;
                    CancelCommand.IsEnabled = true;
                    break;
                case Process.SpeechGenerationStatus.Speaking:
                case Process.SpeechGenerationStatus.SpeakInitiated:
                    SpeakAllTextCommand.IsEnabled = false;
                    SpeakSelectedTextCommand.IsEnabled = false;
                    SpeakSelectedTextCommand.IsEnabled = false;
                    PauseCommand.IsEnabled = true;
                    ResumeCommand.IsEnabled = false;
                    CancelCommand.IsEnabled = true;
                    break;
                default:
                    SpeakAllTextCommand.IsEnabled = false;
                    SpeakSelectedTextCommand.IsEnabled = false;
                    SpeakSelectedTextCommand.IsEnabled = false;
                    PauseCommand.IsEnabled = false;
                    ResumeCommand.IsEnabled = false;
                    CancelCommand.IsEnabled = false;
                    break;
            }
            switch (status)
            {
                case Process.SpeechGenerationStatus.Canceled:
                case Process.SpeechGenerationStatus.CancelInitiated:
                    Status = SpeechState.Canceled;
                    break;
                case Process.SpeechGenerationStatus.Completed:
                case Process.SpeechGenerationStatus.Completing:
                    Status = SpeechState.Completed;
                    break;
                case Process.SpeechGenerationStatus.Resuming:
                case Process.SpeechGenerationStatus.Speaking:
                case Process.SpeechGenerationStatus.SpeakInitiated:
                    Status = SpeechState.Speaking;
                    break;
                case Process.SpeechGenerationStatus.ErrorAborted:
                case Process.SpeechGenerationStatus.ErrorAbortInitiated:
                    Status = SpeechState.HasFault;
                    break;
                case Process.SpeechGenerationStatus.Paused:
                    Status = SpeechState.Paused;
                    break;
                default:
                    Status = SpeechState.NotStarted;
                    break;
            }
            switch (status)
            {
                case Process.SpeechGenerationStatus.NotStarted:
                    IsBusy = false;
                    break;
                case Process.SpeechGenerationStatus.ErrorAborted:
                    IsBusy = true;
                    CancelCommand.IsEnabled = true;
                    CancelButtonText = "Close";
                    break;
                case Process.SpeechGenerationStatus.Completed:
                case Process.SpeechGenerationStatus.Canceled:
                    if (Messages.Any(m => m.Severity == MessageSeverity.Critical || m.Severity == MessageSeverity.Error || m.Severity == MessageSeverity.Warning))
                    {
                        IsBusy = true;
                        CancelCommand.IsEnabled = true;
                        CancelButtonText = "Close";
                    }
                    else
                        IsBusy = false;
                    break;
                default:
                    IsBusy = true;
                    break;
            }
        }
    }
}
