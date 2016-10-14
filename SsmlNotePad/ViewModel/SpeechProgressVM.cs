using System;
using System.Collections.Generic;
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
        
        public SpeechProgressVM()
        {
            Voice = new VoiceVM();
            AudioPosition = new TimeSpanVM();
            BookmarksReached = new ReadOnlyObservableCollection<ViewModel.PositionalValueVM<string>>(_bookmarksReached);
            LatestBookmark = new ViewModel.PositionalValueVM<string>(TimeSpan.Zero, "");
            Messages = new ReadOnlyObservableCollection<SpeechMessageVM>(_messages);
        }

        #region OutputDestination Property Members

        public const string DependencyPropertyName_OutputDestination = "OutputDestination";

        /// <summary>
        /// Identifies the <seealso cref="OutputDestination"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutputDestinationProperty = DependencyProperty.Register(DependencyPropertyName_OutputDestination, typeof(string), typeof(SpeechProgressVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechProgressVM).OutputDestination_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechProgressVM).OutputDestination_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechProgressVM).OutputDestination_CoerceValue(baseValue)));

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
            set
            {
                if (CheckAccess())
                    SetValue(OutputDestinationProperty, value);
                else
                    Dispatcher.Invoke(() => OutputDestination = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="OutputDestination"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="OutputDestination"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="OutputDestination"/> property was changed.</param>
        protected virtual void OutputDestination_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement SpeechProgressVM.OutputDestination_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="OutputDestination"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string OutputDestination_CoerceValue(object baseValue)
        {
            // TODO: Implement SpeechProgressVM.OutputDestination_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
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

        public event EventHandler CancelSpeech;

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
            CancelSpeech?.Invoke(this, EventArgs.Empty);
        }

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

        public const string DependencyPropertyName_Volume = "Volume";

        /// <summary>
        /// Identifies the <seealso cref="Volume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(DependencyPropertyName_Volume, typeof(int), typeof(SpeechProgressVM),
                new PropertyMetadata(100,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechProgressVM).Volume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechProgressVM).Volume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechProgressVM).Volume_CoerceValue(baseValue)));

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
            int? i = baseValue as int?;
            if (!i.HasValue || i.Value < 0)
                return 0;

            return (i.Value > 100) ? 100 : i.Value;
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

        public const string DependencyPropertyName_Rate = "Rate";

        /// <summary>
        /// Identifies the <seealso cref="Rate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RateProperty = DependencyProperty.Register(DependencyPropertyName_Rate, typeof(int), typeof(SpeechProgressVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechProgressVM).Rate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechProgressVM).Rate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechProgressVM).Rate_CoerceValue(baseValue)));

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
            int? i = baseValue as int?;
            if (!i.HasValue)
                return 0;

            if (i.Value < -10)
                return -10;

            return (i.Value > 10) ? 10 : i.Value;
        }

        #endregion

        #region LatestBookmark Property Members

        public const string PropertyName_LatestBookmark = "LatestBookmark";

        private static readonly DependencyPropertyKey LatestBookmarkPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LatestBookmark, typeof(PositionalValueVM<string>), typeof(SpeechProgressVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="LatestBookmark"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LatestBookmarkProperty = LatestBookmarkPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public PositionalValueVM<string> LatestBookmark
        {
            get
            {
                if (CheckAccess())
                    return (PositionalValueVM<string>)(GetValue(LatestBookmarkProperty));
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

        #region BookmarksReached Property Members

        public void AddBookmark(TimeSpan audioPosition, string name)
        {
            LatestBookmark.AudioPosition.SetTimeSpan(audioPosition);
            LatestBookmark.Value = name ?? "";
            _bookmarksReached.Add(new ViewModel.PositionalValueVM<string>(audioPosition, name ?? ""));
        }

        private ObservableCollection<PositionalValueVM<string>> _bookmarksReached = new ObservableCollection<PositionalValueVM<string>>();

        public const string PropertyName_BookmarksReached = "BookmarksReached";

        private static readonly DependencyPropertyKey BookmarksReachedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BookmarksReached, typeof(ReadOnlyObservableCollection<PositionalValueVM<string>>), typeof(SpeechProgressVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="BookmarksReached"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BookmarksReachedProperty = BookmarksReachedPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<PositionalValueVM<string>> BookmarksReached
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<PositionalValueVM<string>>)(GetValue(BookmarksReachedProperty));
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

        #region CurrentText Property Members

        public const string DependencyPropertyName_CurrentText = "CurrentText";

        /// <summary>
        /// Identifies the <seealso cref="CurrentText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentTextProperty = DependencyProperty.Register(DependencyPropertyName_CurrentText, typeof(string), typeof(SpeechProgressVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechProgressVM).CurrentText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechProgressVM).CurrentText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechProgressVM).CurrentText_CoerceValue(baseValue)));

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
        protected virtual void CurrentText_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement SpeechProgressVM.CurrentText_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="CurrentText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string CurrentText_CoerceValue(object baseValue)
        {
            // TODO: Implement SpeechProgressVM.CurrentText_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Messages Property Members

        private ObservableCollection<SpeechMessageVM> _messages = new ObservableCollection<SpeechMessageVM>();

        public void AddException(Exception exception, MessageSeverity severity = MessageSeverity.Error)
        {
            Action action = () => _messages.Add(SpeechMessageVM.Create(exception, severity));
            if (CheckAccess())
                action();
            else
                Dispatcher.Invoke(action);
        }

        public void AddMessage(string eventName, string message, MessageSeverity severity = MessageSeverity.Information)
        {
            Action action = () => _messages.Add(SpeechMessageVM.Create(eventName, message, severity));
            if (CheckAccess())
                action();
            else
                Dispatcher.Invoke(action);
        }

        public void AddMessage(string eventName, string message, IEnumerable<string> eventDetail, MessageSeverity severity = MessageSeverity.Information)
        {
            Action action = () => _messages.Add(SpeechMessageVM.Create(eventName, message, severity, eventDetail));
            if (CheckAccess())
                action();
            else
                Dispatcher.Invoke(action);
        }
        
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

        public const string DependencyPropertyName_CurrentState = "CurrentState";

        /// <summary>
        /// Identifies the <seealso cref="CurrentState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentStateProperty = DependencyProperty.Register(DependencyPropertyName_CurrentState, typeof(SpeechState), typeof(SpeechProgressVM),
                new PropertyMetadata(SpeechState.NotStarted,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechProgressVM).CurrentState_PropertyChanged((SpeechState)(e.OldValue), (SpeechState)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechProgressVM).CurrentState_PropertyChanged((SpeechState)(e.OldValue), (SpeechState)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechProgressVM).CurrentState_CoerceValue(baseValue)));

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
            set
            {
                if (CheckAccess())
                    SetValue(CurrentStateProperty, value);
                else
                    Dispatcher.Invoke(() => CurrentState = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="CurrentState"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="SpeechState"/> value before the <seealso cref="CurrentState"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="SpeechState"/> value after the <seealso cref="CurrentState"/> property was changed.</param>
        protected virtual void CurrentState_PropertyChanged(SpeechState oldValue, SpeechState newValue) { }

        /// <summary>
        /// This gets called whenever <seealso cref="CurrentState"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual SpeechState CurrentState_CoerceValue(object baseValue)
        {
            SpeechState? speechState = baseValue as SpeechState?;
            return (speechState.HasValue) ? speechState.Value : SpeechState.NotStarted;
        }

        #endregion

        #region PercentComplete Property Members

        public const string DependencyPropertyName_PercentComplete = "PercentComplete";

        /// <summary>
        /// Identifies the <seealso cref="PercentComplete"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PercentCompleteProperty = DependencyProperty.Register(DependencyPropertyName_PercentComplete, typeof(int), typeof(SpeechProgressVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechProgressVM).PercentComplete_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechProgressVM).PercentComplete_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SpeechProgressVM).PercentComplete_CoerceValue(baseValue)));

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
            int? i = baseValue as int?;
            if (!i.HasValue || i.Value < 0)
                return 0;

            return (i.Value > 100) ? 100 : i.Value;
        }

        #endregion
    }
}