using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class SpeechProgressVM : DependencyObject
    {
        private object _syncRoot = new object();

        private Process.BackgroundJobManager<Process.SpeakText, int> _speechProcess = null;

        public SpeechProgressVM()
        {
            Voice = new VoiceVM();
            AudioPosition = new TimeSpanVM();
            Phoneme = new ValueAndDurationQueueVM<string>();
            Viseme = new ValueAndDurationQueueVM<int>();
            Messages = new ReadOnlyObservableCollection<SpeechMessageVM>(_messages);
        }

        public void Start(string ssml, Stream audioDestination, SpeechAudioFormatInfo formatInfo)
        {
            if (CheckAccess())
                OutputDestination = "(Stream)";
            else
                Dispatcher.Invoke(() => OutputDestination = "(Stream)");
            Start(new Process.SpeakText(ssml, this, audioDestination, formatInfo));
        }

        public void Start(string ssml, Stream audioDestination)
        {
            if (CheckAccess())
                OutputDestination = "(Stream)";
            else
                Dispatcher.Invoke(() => OutputDestination = "(Stream)");
            Start(new Process.SpeakText(ssml, this, audioDestination));
        }

        public void Start(string ssml, string path, SpeechAudioFormatInfo formatInfo)
        {
            if (CheckAccess())
                OutputDestination = path;
            else
                Dispatcher.Invoke(() => OutputDestination = path);
            Start(new Process.SpeakText(ssml, this, path, formatInfo));
        }

        public void Start(string ssml, string path)
        {
            if (CheckAccess())
                OutputDestination = path;
            else
                Dispatcher.Invoke(() => OutputDestination = path);
            Start(new Process.SpeakText(ssml, this, path));
        }

        public void Start(string ssml)
        {
            if (CheckAccess())
                OutputDestination = "(Default audio device)";
            else
                Dispatcher.Invoke(() => OutputDestination = "(Default audio device)");

            Start(new Process.SpeakText(ssml, this));
        }

        private void Start(Process.SpeakText worker)
        {
            lock (_syncRoot)
            {
                if (_speechProcess == null)
                    _speechProcess = new Process.BackgroundJobManager<Process.SpeakText, int>(worker);
                else
                    _speechProcess.Replace(worker);
            }
        }

        #region OutputDestination Property Members

        public const string PropertyName_OutputDestination = "OutputDestination";

        private static readonly DependencyPropertyKey OutputDestinationPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_OutputDestination, typeof(string), typeof(SpeechProgressVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="OutputDestination"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutputDestinationProperty = OutputDestinationPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string OutputDestination
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(OutputDestinationProperty));
                return Dispatcher.Invoke(() => OutputDestination);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(OutputDestinationPropertyKey, value);
                else
                    Dispatcher.Invoke(() => OutputDestination = value);
            }
        }

        #endregion

        #region PauseSpeech Command Property Members

        public event EventHandler PauseSpeech;

        private Command.RelayCommand _pauseSpeechCommand = null;

        public Command.RelayCommand PauseSpeechCommand
        {
            get
            {
                if (_pauseSpeechCommand == null)
                    _pauseSpeechCommand = new Command.RelayCommand(OnPauseSpeech, false, true);

                return _pauseSpeechCommand;
            }
        }

        protected virtual void OnPauseSpeech(object parameter)
        {
            PauseSpeech?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ResumeSpeech Command Property Members

        public event EventHandler ResumeSpeech;

        private Command.RelayCommand _resumeSpeechCommand = null;

        public Command.RelayCommand ResumeSpeechCommand
        {
            get
            {
                if (_resumeSpeechCommand == null)
                    _resumeSpeechCommand = new Command.RelayCommand(OnResumeSpeech, false, true);

                return _resumeSpeechCommand;
            }
        }

        protected virtual void OnResumeSpeech(object parameter)
        {
            ResumeSpeech?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ToggleSpeech Command Property Members

        private Command.RelayCommand _toggleSpeechCommand = null;

        public Command.RelayCommand ToggleSpeechCommand
        {
            get
            {
                if (_toggleSpeechCommand == null)
                    _toggleSpeechCommand = new Command.RelayCommand(OnToggleSpeech, false, true);

                return _toggleSpeechCommand;
            }
        }

        protected virtual void OnToggleSpeech(object parameter)
        {
            if (_resumeSpeechCommand.IsEnabled)
                OnResumeSpeech(parameter);
            else if (_pauseSpeechCommand.IsEnabled)
                OnPauseSpeech(parameter);
        }

        #endregion

        #region CancelSpeech Command Property Members
        
        private Command.RelayCommand _cancelSpeechCommand = null;

        public Command.RelayCommand CancelSpeechCommand
        {
            get
            {
                if (_cancelSpeechCommand == null)
                    _cancelSpeechCommand = new Command.RelayCommand(OnCancelSpeech, false, true);

                return _cancelSpeechCommand;
            }
        }

        protected virtual void OnCancelSpeech(object parameter)
        {
            if (!_cancelSpeechCommand.IsEnabled)
                return;

            _speechProcess.Cancel();
            _cancelSpeechCommand.IsEnabled = false;
            _resumeSpeechCommand.IsEnabled = false;
            _pauseSpeechCommand.IsEnabled = false;
        }

        #endregion

        #region VoiceDetailsVisible Property Members

        public const string PropertyName_VoiceDetailsVisible = "VoiceDetailsVisible";

        private static readonly DependencyPropertyKey VoiceDetailsVisiblePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_VoiceDetailsVisible, typeof(bool), typeof(SpeechProgressVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="VoiceDetailsVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VoiceDetailsVisibleProperty = VoiceDetailsVisiblePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public bool VoiceDetailsVisible
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(VoiceDetailsVisibleProperty));
                return Dispatcher.Invoke(() => VoiceDetailsVisible);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(VoiceDetailsVisiblePropertyKey, value);
                else
                    Dispatcher.Invoke(() => VoiceDetailsVisible = value);
            }
        }

        #endregion

        #region ShowVoiceDetails Command Property Members

        private Command.RelayCommand _showVoiceDetailsCommand = null;

        public Command.RelayCommand ShowVoiceDetailsCommand
        {
            get
            {
                if (this._showVoiceDetailsCommand == null)
                    this._showVoiceDetailsCommand = new Command.RelayCommand(this.OnShowVoiceDetails);

                return this._showVoiceDetailsCommand;
            }
        }

        protected virtual void OnShowVoiceDetails(object parameter) { VoiceDetailsVisible = true; }

        #endregion

        #region HideVoiceDetails Command Property Members

        private Command.RelayCommand _hideVoiceDetailsCommand = null;

        public Command.RelayCommand HideVoiceDetailsCommand
        {
            get
            {
                if (this._hideVoiceDetailsCommand == null)
                    this._hideVoiceDetailsCommand = new Command.RelayCommand(this.OnHideVoiceDetails);

                return this._hideVoiceDetailsCommand;
            }
        }

        protected virtual void OnHideVoiceDetails(object parameter) { VoiceDetailsVisible = false; }

        #endregion

        #region Voice Property Members

        public const string PropertyName_Voice = "Voice";

        private static readonly DependencyPropertyKey VoicePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Voice, typeof(VoiceVM), typeof(SpeechProgressVM),
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

        public const string PropertyName_Volume = "Volume";

        private static readonly DependencyPropertyKey VolumePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Volume, typeof(int), typeof(SpeechProgressVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="Volume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = VolumePropertyKey.DependencyProperty;

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
            private set
            {
                if (CheckAccess())
                    SetValue(VolumePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Volume = value);
            }
        }

        #endregion

        #region Stressed Property Members

        public const string PropertyName_Stressed = "Stressed";

        private static readonly DependencyPropertyKey StressedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Stressed, typeof(bool), typeof(SpeechProgressVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="Stressed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StressedProperty = StressedPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public bool Stressed
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(StressedProperty));
                return Dispatcher.Invoke(() => Stressed);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(StressedPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Stressed = value);
            }
        }

        #endregion

        #region Rate Property Members

        public const string PropertyName_Rate = "Rate";

        private static readonly DependencyPropertyKey RatePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Rate, typeof(int), typeof(SpeechProgressVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="Rate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RateProperty = RatePropertyKey.DependencyProperty;

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
            private set
            {
                if (CheckAccess())
                    SetValue(RatePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Rate = value);
            }
        }

        #endregion

        #region LatestBookmark Property Members

        public const string PropertyName_LatestBookmark = "LatestBookmark";

        private static readonly DependencyPropertyKey LatestBookmarkPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LatestBookmark, typeof(string), typeof(SpeechProgressVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="LatestBookmark"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LatestBookmarkProperty = LatestBookmarkPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string LatestBookmark
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LatestBookmarkProperty));
                return Dispatcher.Invoke(() => LatestBookmark);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(LatestBookmarkPropertyKey, value);
                else
                    Dispatcher.Invoke(() => LatestBookmark = value);
            }
        }

        #endregion

        #region AudioPosition Property Members

        public const string PropertyName_AudioPosition = "AudioPosition";

        private static readonly DependencyPropertyKey AudioPositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_AudioPosition, typeof(TimeSpanVM), typeof(SpeechProgressVM),
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

        public const string PropertyName_CharacterPosition = "CharacterPosition";

        private static readonly DependencyPropertyKey CharacterPositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CharacterPosition, typeof(int), typeof(SpeechProgressVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="CharacterPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterPositionProperty = CharacterPositionPropertyKey.DependencyProperty;

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
            private set
            {
                if (CheckAccess())
                    SetValue(CharacterPositionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => CharacterPosition = value);
            }
        }

        #endregion

        #region CharacterCount Property Members

        public const string PropertyName_CharacterCount = "CharacterCount";

        private static readonly DependencyPropertyKey CharacterCountPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CharacterCount, typeof(int), typeof(SpeechProgressVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="CharacterCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterCountProperty = CharacterCountPropertyKey.DependencyProperty;

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
            private set
            {
                if (CheckAccess())
                    SetValue(CharacterCountPropertyKey, value);
                else
                    Dispatcher.Invoke(() => CharacterCount = value);
            }
        }

        #endregion

        #region Phoneme Property Members

        public const string PropertyName_Phoneme = "Phoneme";

        private static readonly DependencyPropertyKey PhonemePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Phoneme, typeof(ValueAndDurationQueueVM<string>), typeof(SpeechProgressVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Phoneme"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhonemeProperty = PhonemePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ValueAndDurationQueueVM<string> Phoneme
        {
            get
            {
                if (CheckAccess())
                    return (ValueAndDurationQueueVM<string>)(GetValue(PhonemeProperty));
                return Dispatcher.Invoke(() => Phoneme);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(PhonemePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Phoneme = value);
            }
        }

        #endregion

        #region Viseme Property Members

        public const string PropertyName_Viseme = "Viseme";

        private static readonly DependencyPropertyKey VisemePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Viseme, typeof(ValueAndDurationQueueVM<int>), typeof(SpeechProgressVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Viseme"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisemeProperty = VisemePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ValueAndDurationQueueVM<int> Viseme
        {
            get
            {
                if (CheckAccess())
                    return (ValueAndDurationQueueVM<int>)(GetValue(VisemeProperty));
                return Dispatcher.Invoke(() => Viseme);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(VisemePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Viseme = value);
            }
        }

        #endregion

        #region CurrentText Property Members

        public const string PropertyName_CurrentText = "CurrentText";

        private static readonly DependencyPropertyKey CurrentTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentText, typeof(string), typeof(SpeechProgressVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="CurrentText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentTextProperty = CurrentTextPropertyKey.DependencyProperty;

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
            private set
            {
                if (CheckAccess())
                    SetValue(CurrentTextPropertyKey, value);
                else
                    Dispatcher.Invoke(() => CurrentText = value);
            }
        }

        #endregion

        #region Messages Property Members

        private ObservableCollection<SpeechMessageVM> _messages = new ObservableCollection<SpeechMessageVM>();

        public const string PropertyName_Messages = "Messages";

        private static readonly DependencyPropertyKey MessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Messages, typeof(ReadOnlyObservableCollection<SpeechMessageVM>), typeof(SpeechProgressVM),
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

        #region CurrentState Property Members

        public const string PropertyName_CurrentState = "CurrentState";

        private static readonly DependencyPropertyKey CurrentStatePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentState, typeof(SpeechState), typeof(SpeechProgressVM),
                new PropertyMetadata(SpeechState.NotStarted));

        /// <summary>
        /// Identifies the <seealso cref="CurrentState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentStateProperty = CurrentStatePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public SpeechState CurrentState
        {
            get
            {
                if (CheckAccess())
                    return (SpeechState)(GetValue(CurrentStateProperty));
                return Dispatcher.Invoke(() => CurrentState);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(CurrentStatePropertyKey, value);
                else
                    Dispatcher.Invoke(() => CurrentState = value);
            }
        }

        #endregion

        #region PercentComplete Property Members

        public const string PropertyName_PercentComplete = "PercentComplete";

        private static readonly DependencyPropertyKey PercentCompletePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PercentComplete, typeof(int), typeof(SpeechProgressVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="PercentComplete"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PercentCompleteProperty = PercentCompletePropertyKey.DependencyProperty;

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
            private set
            {
                if (CheckAccess())
                    SetValue(PercentCompletePropertyKey, value);
                else
                    Dispatcher.Invoke(() => PercentComplete = value);
            }
        }

        #endregion

        internal void Update(TimeSpan audioPosition, string text, int characterPosition, int characterCount, int rate, VoiceInfo voice, bool stressed, int volume)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => Update(audioPosition, text, characterPosition, characterCount, rate, voice, stressed, volume), DispatcherPriority.Input);
                return;
            }

            AudioPosition.SetTimeSpan(audioPosition);
            CurrentText = text ?? "";
            CharacterPosition = characterPosition;
            CharacterCount = characterCount;
            PercentComplete = (characterPosition * 100) / characterCount;
            Rate = rate;
            Voice.SetVoice(voice);
            Stressed = stressed;
            Volume = volume;
        }

        internal void AddErrorMessage(Exception error, MessageSeverity severity = MessageSeverity.Error)
        {
            if (CheckAccess())
            {
                CurrentState = CurrentState | SpeechState.HasFault;
                _messages.Add(SpeechMessageVM.Create(error, severity));
                if (_messages.Count > 128)
                    _messages.RemoveAt(0);
            }
            else
                Dispatcher.Invoke(() => AddErrorMessage(error), DispatcherPriority.Input);
        }

        internal void SetPhoneme(string phoneme, TimeSpan duration, string nextPhoneme)
        {
            if (CheckAccess())
                Phoneme.SetQueue(phoneme, duration, nextPhoneme);
            else
                Dispatcher.Invoke(() => SetPhoneme(phoneme, duration, nextPhoneme), DispatcherPriority.Input);
        }

        internal void SetViseme(int viseme, TimeSpan duration, int nextViseme)
        {
            if (CheckAccess())
                Viseme.SetQueue(viseme, duration, nextViseme);
            else
                Dispatcher.Invoke(() => SetViseme(viseme, duration, nextViseme), DispatcherPriority.Input);
        }

        internal void SetBookmark(string bookmark)
        {
            if (CheckAccess())
            {
                LatestBookmark = bookmark ?? "";
                _messages.Add(SpeechMessageVM.Create("Bookmark reached", bookmark));
                if (_messages.Count > 128)
                    _messages.RemoveAt(0);
            }
            else
                Dispatcher.Invoke(() => SetBookmark(bookmark), DispatcherPriority.Input);
        }

        internal void SetStartedState()
        {
            if (CheckAccess())
            {
                CurrentState = SpeechState.Speaking;
                _cancelSpeechCommand.IsEnabled = true;
                _pauseSpeechCommand.IsEnabled = true;
                _resumeSpeechCommand.IsEnabled = false;
                _toggleSpeechCommand.IsEnabled = true;
            }
            else
                Dispatcher.Invoke(() => SetStartedState(), DispatcherPriority.Input);
        }

        internal void SetCompletedState()
        {
            if (CheckAccess())
            {
                _cancelSpeechCommand.IsEnabled = false;
                _pauseSpeechCommand.IsEnabled = false;
                _resumeSpeechCommand.IsEnabled = false;
                _toggleSpeechCommand.IsEnabled = false;
                if (CurrentState.HasFlag(SpeechState.Canceled))
                    CurrentState = SpeechState.Completed | ((CurrentState.HasFlag(SpeechState.HasFault)) ? SpeechState.Canceled | SpeechState.HasFault : SpeechState.Canceled);
                else
                    CurrentState = (CurrentState.HasFlag(SpeechState.HasFault)) ? SpeechState.Completed | SpeechState.HasFault : SpeechState.Completed;
            }
            else
                Dispatcher.Invoke(() => SetCompletedState(), DispatcherPriority.Input);
        }

        internal void SetCanceledState()
        {
            if (CheckAccess())
            {
                _cancelSpeechCommand.IsEnabled = false;
                _pauseSpeechCommand.IsEnabled = false;
                _resumeSpeechCommand.IsEnabled = false;
                _toggleSpeechCommand.IsEnabled = false;
                CurrentState = SpeechState.Canceled | ((CurrentState.HasFlag(SpeechState.HasFault)) ? SpeechState.Completed | SpeechState.HasFault : SpeechState.Completed);
            }
            else
                Dispatcher.Invoke(() => SetCanceledState(), DispatcherPriority.Input);
        }

        internal void SetErrorState(Exception exception)
        {
            if (CheckAccess())
            {
                _cancelSpeechCommand.IsEnabled = false;
                _pauseSpeechCommand.IsEnabled = false;
                _resumeSpeechCommand.IsEnabled = false;
                _toggleSpeechCommand.IsEnabled = false;
                CurrentState = SpeechState.HasFault | ((CurrentState.HasFlag(SpeechState.Canceled)) ? SpeechState.Completed | SpeechState.Canceled : SpeechState.Completed);
                _messages.Add(SpeechMessageVM.Create(exception, MessageSeverity.Critical));
                if (_messages.Count > 128)
                    _messages.RemoveAt(0);
            }
            else
                Dispatcher.Invoke(() => SetErrorState(exception), DispatcherPriority.Input);
        }

        internal void SetPausedState()
        {
            if (CheckAccess())
            {
                _pauseSpeechCommand.IsEnabled = false;
                _resumeSpeechCommand.IsEnabled = true;
                if (CurrentState.HasFlag(SpeechState.Canceled))
                    CurrentState = SpeechState.Paused | ((CurrentState.HasFlag(SpeechState.HasFault)) ? SpeechState.Canceled | SpeechState.HasFault : SpeechState.Canceled);
                else
                    CurrentState = (CurrentState.HasFlag(SpeechState.HasFault)) ? SpeechState.Paused | SpeechState.HasFault : SpeechState.Paused;
            }
            else
                Dispatcher.Invoke(() => SetCompletedState(), DispatcherPriority.Input);
        }

        internal void SetUnpausedState()
        {
            if (CheckAccess())
            {
                _pauseSpeechCommand.IsEnabled = true;
                _resumeSpeechCommand.IsEnabled = false;
                if (CurrentState.HasFlag(SpeechState.Canceled))
                    CurrentState = SpeechState.Speaking | ((CurrentState.HasFlag(SpeechState.HasFault)) ? SpeechState.Canceled | SpeechState.HasFault : SpeechState.Canceled);
                else
                    CurrentState = (CurrentState.HasFlag(SpeechState.HasFault)) ? SpeechState.Speaking | SpeechState.HasFault : SpeechState.Speaking;
            }
            else
                Dispatcher.Invoke(() => SetCompletedState(), DispatcherPriority.Input);
        }
    }
}