using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class SpeechGenerationManager
    {
        private SpeechProcessController _currentController = null;

        public abstract class SpeechInvocationSettings : ICloneable
        {
            public event EventHandler<SpeechStatusEventArgs> StateChanged;
            public event EventHandler<SpeakStartedEventArgs> SpeakStarted;
            public event EventHandler<SpeakProgressEventArgs> SpeakProgress;
            public event EventHandler<VisemeReachedEventArgs> VisemeReached;
            public event EventHandler<PhonemeReachedEventArgs> PhonemeReached;
            public event EventHandler<VoiceChangeEventArgs> VoiceChange;
            public event EventHandler<BookmarkReachedEventArgs> BookmarkReached;
            public event EventHandler<SpeechStatusEventArgs> SpeakCompleted;

            public string SsmlMarkup { get; private set; }

            public bool MarkukpMayBePartial { get; private set; }

            protected SpeechInvocationSettings(string ssmlMarkup, bool mayBePartial)
            {
                if (ssmlMarkup == null)
                    throw new ArgumentNullException("ssmlMarkup");

                SsmlMarkup = ssmlMarkup;
                MarkukpMayBePartial = mayBePartial;
            }

            public static SpeechInvocationSettings ForDefaultAudioDevice(string ssmlMarkup, bool mayBePartial = false)
            {
                return new OutputToDefaultAudioDeviceSettings(ssmlMarkup, mayBePartial);
            }

            public static SpeechInvocationSettings ForWaveFile(string path, string ssmlMarkup, bool mayBePartial = false)
            {
                return new OutputToWaveFileSettings(path, ssmlMarkup, mayBePartial);
            }

            public static SpeechInvocationSettings ForWaveFile(string path, SpeechAudioFormatInfo formatInfo, string ssmlMarkup, bool mayBePartial = false)
            {
                return new OutputToWaveFileSettings(path, ssmlMarkup, formatInfo, mayBePartial);
            }

            public static SpeechInvocationSettings ForWaveStream(Stream audioDestination, string ssmlMarkup, bool mayBePartial = false)
            {
                return new OutputToAudioStreamSettings(audioDestination, ssmlMarkup, mayBePartial);
            }

            public static SpeechInvocationSettings ForAudioStream(Stream audioDestination, SpeechAudioFormatInfo formatInfo, string ssmlMarkup, bool mayBePartial = false)
            {
                return new OutputToAudioStreamSettings(audioDestination, ssmlMarkup, formatInfo, mayBePartial);
            }

            protected abstract SpeechInvocationSettings CreateClone();

            public SpeechInvocationSettings Clone()
            {
                SpeechInvocationSettings clone = CreateClone();
                clone.StateChanged = StateChanged;
                clone.SpeakStarted = SpeakStarted;
                clone.SpeakProgress = SpeakProgress;
                clone.VisemeReached = VisemeReached;
                clone.PhonemeReached = PhonemeReached;
                clone.VoiceChange = VoiceChange;
                clone.BookmarkReached = BookmarkReached;
                clone.SpeakCompleted = SpeakCompleted;
                return clone;
            }

            object ICloneable.Clone() { return Clone(); }
        }

        public abstract class SpeechInvocationSettings<TImplemented> : SpeechInvocationSettings
            where TImplemented : SpeechInvocationSettings<TImplemented>
        {
            protected SpeechInvocationSettings(string ssmlMarkup, bool mayBePartial) : base(ssmlMarkup, mayBePartial) { }

            protected override SpeechInvocationSettings CreateClone() { return CloneCurrent(); }

            protected abstract TImplemented CloneCurrent();
        }

        public abstract class OutputToWaveSettings<TImplemented> : SpeechInvocationSettings<TImplemented>
            where TImplemented : SpeechInvocationSettings<TImplemented>
        {
            public SpeechAudioFormatInfo FormatInfo { get; private set; }

            protected OutputToWaveSettings(string ssmlMarkup, bool mayBePartial) : base(ssmlMarkup, mayBePartial) { }

            protected OutputToWaveSettings(string ssmlMarkup, SpeechAudioFormatInfo formatInfo, bool mayBePartial)
                : base(ssmlMarkup, mayBePartial)
            {
                if (formatInfo == null)
                    throw new ArgumentNullException("formatInfo");

                FormatInfo = formatInfo;
            }
            protected abstract override TImplemented CloneCurrent();
        }

        public class OutputToWaveFileSettings : OutputToWaveSettings<OutputToWaveFileSettings>
        {
            public string AudioDestination { get; private set; }
            
            public OutputToWaveFileSettings(string path, string ssmlMarkup, bool mayBePartial = false)
                : base(ssmlMarkup, mayBePartial)
            {
                if (path == null)
                    throw new ArgumentNullException("path");

                if (path.Trim().Length == 0)
                    throw new ArgumentException("Path cannot be empty", "path");

                AudioDestination = path;
            }

            public OutputToWaveFileSettings(string path, string ssmlMarkup, SpeechAudioFormatInfo formatInfo, bool mayBePartial = false)
                : base(ssmlMarkup, formatInfo, mayBePartial)
            {
                if (path == null)
                    throw new ArgumentNullException("path");

                if (path.Trim().Length == 0)
                    throw new ArgumentException("Path cannot be empty", "path");

                AudioDestination = path;
            }

            protected override OutputToWaveFileSettings CloneCurrent()
            {
                if (FormatInfo == null)
                    return new OutputToWaveFileSettings(AudioDestination, SsmlMarkup, MarkukpMayBePartial);

                return new OutputToWaveFileSettings(AudioDestination, SsmlMarkup, FormatInfo, MarkukpMayBePartial);
            }
        }

        public class OutputToAudioStreamSettings : OutputToWaveSettings<OutputToAudioStreamSettings>
        {
            public Stream AudioDestination { get; private set; }

            public OutputToAudioStreamSettings(Stream audioDestination, string ssmlMarkup, bool mayBePartial = false)
                : base(ssmlMarkup, mayBePartial)
            {
                if (audioDestination == null)
                    throw new ArgumentNullException("audioDestination");

                AudioDestination = audioDestination;
            }

            public OutputToAudioStreamSettings(Stream audioDestination, string ssmlMarkup, SpeechAudioFormatInfo formatInfo, bool mayBePartial = false)
                : base(ssmlMarkup, formatInfo, mayBePartial)
            {
                if (audioDestination == null)
                    throw new ArgumentNullException("audioDestination");

                AudioDestination = audioDestination;
            }

            protected override OutputToAudioStreamSettings CloneCurrent()
            {
                if (FormatInfo == null)
                    return new OutputToAudioStreamSettings(AudioDestination, SsmlMarkup, MarkukpMayBePartial);

                return new OutputToAudioStreamSettings(AudioDestination, SsmlMarkup, FormatInfo, MarkukpMayBePartial);
            }
        }

        public class OutputToDefaultAudioDeviceSettings : SpeechInvocationSettings<OutputToDefaultAudioDeviceSettings>
        {
            public OutputToDefaultAudioDeviceSettings(string ssmlMarkup, bool mayBePartial = false) : base(ssmlMarkup, mayBePartial) { }

            protected override OutputToDefaultAudioDeviceSettings CloneCurrent()
            {
                return new OutputToDefaultAudioDeviceSettings(SsmlMarkup, MarkukpMayBePartial);
            }
        }

        public class SpeechStatusEventArgs : EventArgs
        {

        }

        public class SpeechProcessController : IDisposable
        {
            public static SpeechStatusEventArgs PauseSpeechAsync()
            {
                throw new NotImplementedException();
            }

            public static SpeechStatusEventArgs ResumeSpeechAsync()
            {
                throw new NotImplementedException();
            }

            public static SpeechStatusEventArgs CancelSpeechAsync()
            {
                throw new NotImplementedException();
            }

            public static SpeechStatusEventArgs CancelSpeechAsync(TimeSpan abortAfter)
            {
                throw new NotImplementedException();
            }

            public static SpeechStatusEventArgs GetStatusAsync()
            {
                throw new NotImplementedException();
            }

            private static object _controllerChainSyncRoot = new object();
            private object _syncRoot = new object();
            private static SpeechProcessController _currentController = null;
            private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
            private CancellationTokenSource _tokenSource = new CancellationTokenSource();
            private SpeechProcessWorker _worker;
            private Task<SpeechStatusEventArgs> _task;
            private AutoResetEvent _speechCompletedEvent = new AutoResetEvent(false);
            private bool _isDisposed = false;
            private SpeechInvocationSettings _speechInvocationSettings;

            public SpeechProcessController(SpeechInvocationSettings speechInvocationSettings)
            {
                _speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
                _worker = new SpeechProcessWorker(speechInvocationSettings, _speechSynthesizer, _tokenSource.Token);
            }

            private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
            {
                Task<SpeechStatusEventArgs> task;
                lock (_syncRoot)
                {
                    task = _task;
                    if (_isDisposed || task == null)
                        return;
                    _task = null;
                }
                _speechCompletedEvent.Set();
                _task.Wait(new TimeSpan(0, 0, 10));
                if (!_task.IsCompleted)
                    _tokenSource.Cancel();
                try
                {
                    _worker.RaiseSpeakCompleted(e, task.Result, null);
                }
                catch (Exception exception)
                {
                    _worker.RaiseSpeakCompleted(e, task.Result, exception);
                }
            }

            public static SpeechStatusEventArgs SpeakAsync(SpeechInvocationSettings settings)
            {
                if (settings == null)
                    throw new ArgumentNullException("settings");

                lock (_controllerChainSyncRoot)
                {
                    SpeechProcessController currentController = _currentController;
                    _currentController = new SpeechProcessController(settings.Clone());
                    if (currentController != null)
                    {
                        if (currentController._task.IsCompleted)
                            currentController.Dispose();
                        else
                        {
                            Task.Factory.StartNew(o =>
                            {
                                SpeechProcessController c = o as SpeechProcessController;
                                lock (c._syncRoot)
                                {
                                    if (c._speechCompletedEvent != null)
                                        return;
                                    c._speechCompletedEvent = new AutoResetEvent(false);
                                    c._speechSynthesizer.SpeakAsyncCancelAll();
                                    c._speechCompletedEvent.WaitOne(new TimeSpan(0, 0, 10));
                                    c.Dispose();
                                }
                            }, currentController);
                        }
                    }
                    // TODO: Start new task
                }
            }

            #region IDisposable Support

            protected virtual void Dispose(bool disposing)
            {
                if (!_isDisposed)
                {
                    if (disposing)
                    {
                        _speechSynthesizer.Dispose();
                        _tokenSource.Dispose();
                        if (_task != null)
                            _task.Dispose();
                        if (_speechCompletedEvent != null)
                            _speechCompletedEvent.Dispose();
                    }
                    
                    _isDisposed = true;
                }
            }
            
            public void Dispose() { Dispose(true); }

            #endregion
        }

        public class SpeechProcessWorker
        {
            private SpeechInvocationSettings _speechInvocationSettings;
            private CancellationToken token;

            public SpeechProcessWorker(SpeechInvocationSettings speechInvocationSettings, SpeechSynthesizer speechSynthesizer, CancellationToken token)
            {
                _speechInvocationSettings = speechInvocationSettings;
                this.token = token;
                speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
                speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
                speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                speechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
                speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
            }

            private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
            {
                _speechInvocationSettings.InvokeVoiceChange(this, e);
            }

            private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
            {
                _speechInvocationSettings.InvokeVisemeReached(this, e);
            }

            private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
            {
                _speechInvocationSettings.InvokeSpeakStarted(this, e);
            }

            private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
            {
                throw new NotImplementedException();
            }

            private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
            {
                throw new NotImplementedException();
            }

            private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
            {
                throw new NotImplementedException();
            }

            private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
            {
                throw new NotImplementedException();
            }

            internal Task<SpeechStatusEventArgs> Start()
            {
                throw new NotImplementedException();
            }

            internal SpeechStatusEventArgs GetCurrentStatus()
            {
                throw new NotImplementedException();
            }
        }
    }

    public class SpeechGenerationManager2
    {
        private static object _syncRoot = new object();
        private static Task<SpeechGenerationManager2> _activeTask = null;
        private static SpeechGenerationManager2 _activeManager = null;

        private ViewModel.SpeechGenerationStatusVM _viewModel;
        private string _ssml;
        private bool _mayBePartial;
        private SpeechSynthesizer _speechSynthesizer;
        private TimeSpan _lastAudioPosition = TimeSpan.Zero;
        private SpeechGenerationStatus _status = SpeechGenerationStatus.NotStarted;
        private AutoResetEvent _speechCompletedEvent = new AutoResetEvent(false);

        public SpeechGenerationManager2(ViewModel.SpeechGenerationStatusVM viewModel, string ssml, bool mayBePartial)
        {
            _viewModel = viewModel;
            _ssml = ssml ?? "";
            _mayBePartial = mayBePartial;
        }

        private static Task<SpeechGenerationManager2> StartOutput(ViewModel.SpeechGenerationStatusVM viewModel, string ssml, bool mayBePartial, Action<SpeechGenerationManager2, SpeechSynthesizer> onInitialize)
        {
            Func<object, SpeechGenerationManager2> func = o =>
            {
                SpeechGenerationManager2 speechGenerationManager = o as SpeechGenerationManager2;
                try
                {
                    using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
                    {
                        (o as Action<SpeechGenerationManager2, SpeechSynthesizer>)?.Invoke(speechGenerationManager, speechSynthesizer);
                        speechGenerationManager._speechCompletedEvent.Dispose()
                        speechGenerationManager.Speak(speechSynthesizer);
                        speechSynthesizer.SetOutputToAudioStream
                    }
                }
                catch (Exception exception)
                {
                    speechGenerationManager.SetUnexpectedError(exception);
                }
                return speechGenerationManager;
            };
            lock (_syncRoot)
            {
                if (_activeTask == null || _activeTask.IsCompleted)
                {
                    _activeManager = new SpeechGenerationManager2(viewModel, ssml, mayBePartial);
                    _activeTask = Task<SpeechGenerationManager2>.Factory.StartNew(func, _activeManager);
                }
                else
                {
                    if (!_activeManager.IsCancellationRequested)
                        _activeManager.CancelAsync();

                    _activeManager = new SpeechGenerationManager2(viewModel, ssml, mayBePartial);
                    _activeTask = _activeTask.ContinueWith((t, o) =>
                    {
                        if (!t.IsCompleted)
                            try { t.Wait(); } catch { }
                        object[] parameters = o as object[];
                        Func<object, SpeechGenerationManager2> f = (Func<object, SpeechGenerationManager2>)(parameters[1]);
                        return f(parameters[0]);
                    }, new object[] { _activeManager, func });
                }
                return _activeTask;
            }
        }

        private void InvokeViewModel(Action action)
        {
            if (_viewModel.CheckAccess())
                action();
            else
                _viewModel.Dispatcher.Invoke(action);
        }

        private TResult InvokeViewModel<TResult>(Func<TResult> func)
        {
            if (_viewModel.CheckAccess())
                return func();

            return _viewModel.Dispatcher.Invoke(func);
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            if (!e.Cancelled)
                InvokeViewModel(() => _viewModel.Voice.SetVoice(e.Voice));

            SetStatus(e);
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            SetStatus(e, e.AudioPosition);
        }

        private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            switch (_status)
            {
                case SpeechGenerationStatus.NotStarted:
                case SpeechGenerationStatus.Initializing:
                case SpeechGenerationStatus.SpeakInitiated:
                case SpeechGenerationStatus.Paused:
                case SpeechGenerationStatus.Resuming:
                    SetStatus(SpeechGenerationStatus.Speaking, _lastAudioPosition, e);
                    break;
                default:
                    SetStatus(e, _lastAudioPosition);
                    break;
            }
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            if (!e.Cancelled)
            {
                int percentComplete = (e.CharacterPosition * 100) / ((e.CharacterPosition > _ssml.Length) ? e.CharacterPosition : _ssml.Length);
                InvokeViewModel(() =>
                {
                    _viewModel.PercentComplete = percentComplete;
                    _viewModel.CharacterPosition = e.CharacterPosition;
                    _viewModel.CharacterCount = e.CharacterCount;
                    _viewModel.CurrentText = e.Text;
                });
            }

            SetStatus(e, e.AudioPosition);
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            switch (_status)
            {
                case SpeechGenerationStatus.Initializing:
                case SpeechGenerationStatus.NotStarted:
                case SpeechGenerationStatus.Paused:
                case SpeechGenerationStatus.Resuming:
                case SpeechGenerationStatus.Speaking:
                case SpeechGenerationStatus.SpeakInitiated:
                    SetStatus(SpeechGenerationStatus.Completing, _lastAudioPosition, e);
                    break;
                default:
                    SetStatus(e, _lastAudioPosition);
                    break;
            }
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            SetStatus(e, e.AudioPosition);
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            if (!e.Cancelled)
                InvokeViewModel(() => _viewModel.SetCurrentBookmark(e.Bookmark, e.AudioPosition));

            SetStatus(e, e.AudioPosition);
        }

        public static void StartOutputToAudioStream(ViewModel.SpeechGenerationStatusVM viewModel, string text, Stream audioDestination, SpeechAudioFormatInfo formatInfo, bool mayBePartial = false)
        {
            StartOutput(viewModel, text, mayBePartial, (m, s) => s.SetOutputToAudioStream(audioDestination, formatInfo));
        }

        public static void StartOutputToDefaultAudioDevice(ViewModel.SpeechGenerationStatusVM viewModel, string text, bool mayBePartial = false)
        {
            StartOutput(viewModel, text, mayBePartial, (m, s) => s.SetOutputToDefaultAudioDevice());
        }

        public static void StartOutputToWaveFile(ViewModel.SpeechGenerationStatusVM viewModel, string text, string path, bool mayBePartial = false)
        {
            StartOutput(viewModel, text, mayBePartial, (m, s) => s.SetOutputToWaveFile(path));
        }

        public static void StartOutputToWaveFile(ViewModel.SpeechGenerationStatusVM viewModel, string text, string path, SpeechAudioFormatInfo formatInfo, bool mayBePartial = false)
        {
            StartOutput(viewModel, text, mayBePartial, (m, s) => s.SetOutputToWaveFile(path, formatInfo));
        }

        public static void StartOutputToWaveStream(ViewModel.SpeechGenerationStatusVM viewModel, string text, Stream audioDestination, bool mayBePartial = false)
        {
            StartOutput(viewModel, text, mayBePartial, (m, s) => s.SetOutputToWaveStream(audioDestination));
        }

        private void Speak(SpeechSynthesizer speechSynthesizer)
        {
            _speechSynthesizer = speechSynthesizer;
            SetStatus(SpeechGenerationStatus.Initializing, TimeSpan.Zero);
            speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
            speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
            speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
            speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
            speechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
            speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
            speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
            try
            {
                string errorMessage;
                bool isValid = Common.XmlHelper.ValidateSsml(_ssml, false, out errorMessage);
                if (!isValid && _mayBePartial && Common.XmlHelper.TryCreateFromPartial(_ssml, out _ssml))
                    isValid = Common.XmlHelper.ValidateSsml(_ssml, false, out errorMessage);

                if (isValid)
                {
                    SetStatus(SpeechGenerationStatus.SpeakInitiated, TimeSpan.Zero);
                    speechSynthesizer.SpeakSsml(_ssml);
                    switch (_status)
                    {
                        case SpeechGenerationStatus.CancelInitiated:
                            SetStatus(SpeechGenerationStatus.Canceled, _lastAudioPosition);
                            break;
                        case SpeechGenerationStatus.ErrorAbortInitiated:
                            SetStatus(SpeechGenerationStatus.ErrorAborted, _lastAudioPosition);
                            break;
                        case SpeechGenerationStatus.Completed:
                        case SpeechGenerationStatus.Canceled:
                        case SpeechGenerationStatus.ErrorAborted:
                            break;
                        default:
                            SetStatus(SpeechGenerationStatus.Completed, _lastAudioPosition);
                            break;
                    }
                }
                else
                {
                    SetStatus(SpeechGenerationStatus.ErrorAborted, _lastAudioPosition);
                    InvokeViewModel(() => _viewModel.SetMessage("SSML Validation Failure", errorMessage, ViewModel.MessageSeverity.Critical));
                }
            }
            catch (Exception exception)
            {
                InvokeViewModel(() => _viewModel.SetFault(exception));
            }
            finally
            {
                speechSynthesizer.BookmarkReached -= SpeechSynthesizer_BookmarkReached;
                speechSynthesizer.PhonemeReached -= SpeechSynthesizer_PhonemeReached;
                speechSynthesizer.SpeakCompleted -= SpeechSynthesizer_SpeakCompleted;
                speechSynthesizer.SpeakProgress -= SpeechSynthesizer_SpeakProgress;
                speechSynthesizer.SpeakStarted -= SpeechSynthesizer_SpeakStarted;
                speechSynthesizer.VisemeReached -= SpeechSynthesizer_VisemeReached;
                speechSynthesizer.VoiceChange -= SpeechSynthesizer_VoiceChange;
            }
        }

        private void SetStatus(AsyncCompletedEventArgs args, TimeSpan audioPosition)
        {
            switch (_status)
            {
                case SpeechGenerationStatus.CancelInitiated:
                case SpeechGenerationStatus.Canceled:
                case SpeechGenerationStatus.ErrorAbortInitiated:
                case SpeechGenerationStatus.ErrorAborted:
                    if (!args.Cancelled)
                        try { _speechSynthesizer.SpeakAsyncCancelAll(); } catch { }
                    SetStatus(args, audioPosition);
                    break;
            }

            if (args.Error != null)
                InvokeViewModel(() => _viewModel.SetFault(args.Error));
        }

        private void SetStatus(AsyncCompletedEventArgs args) { SetStatus(args, _lastAudioPosition); }

        private void SetStatus(SpeechGenerationStatus status, TimeSpan audioPosition, AsyncCompletedEventArgs args = null)
        {
            InvokeViewModel(() =>
            {
                if (args == null || !args.Cancelled)
                {
                    _viewModel.SetStatus(status);
                    _viewModel.AudioPosition.SetTimeSpan(audioPosition);
                }
                if (args != null && args.Error != null)
                    _viewModel.SetFault(args.Error);
            });
        }

        public bool IsCancellationRequested
        {
            get
            {
                switch (_status)
                {
                    case SpeechGenerationStatus.Canceled:
                    case SpeechGenerationStatus.CancelInitiated:
                    case SpeechGenerationStatus.ErrorAborted:
                    case SpeechGenerationStatus.ErrorAbortInitiated:
                        return true;
                }

                return false;
            }
        }

        public void CancelAsync()
        {
            lock (_syncRoot)
            {
                switch (_status)
                {
                    case SpeechGenerationStatus.Canceled:
                    case SpeechGenerationStatus.CancelInitiated:
                    case SpeechGenerationStatus.ErrorAborted:
                    case SpeechGenerationStatus.ErrorAbortInitiated:
                        return;
                }
                _status = SpeechGenerationStatus.CancelInitiated;
                Task.Factory.StartNew(o => (o as SpeechSynthesizer).SpeakAsyncCancelAll(), _speechSynthesizer);
            }

            SetStatus(_status, _lastAudioPosition);
        }

        private void SetUnexpectedError(Exception exception)
        {
            SetStatus(SpeechGenerationStatus.ErrorAborted, _lastAudioPosition);
            _viewModel.SetFault(exception);
        }

        internal static void PauseSpeech()
        {
            lock (_syncRoot)
            {
                if (_activeManager == null)
                    return;

                switch (_activeManager._status)
                {
                    case SpeechGenerationStatus.Initializing:
                    case SpeechGenerationStatus.NotStarted:
                    case SpeechGenerationStatus.Resuming:
                    case SpeechGenerationStatus.Speaking:
                    case SpeechGenerationStatus.SpeakInitiated:
                        _activeManager._speechSynthesizer.Pause();
                        _activeManager._status = SpeechGenerationStatus.Paused;
                        break;
                    default:
                        return;
                }
            }

            _activeManager.SetStatus(_activeManager._status, _activeManager._lastAudioPosition);
        }

        internal static void CancelSpeech()
        {
            SpeechGenerationManager2 mgr = _activeManager;
            if (mgr != null)
                mgr.CancelAsync();
        }

        internal static void ResumeSpeech()
        {
            lock (_syncRoot)
            {
                if (_activeManager == null)
                    return;

                switch (_activeManager._status)
                {
                    case SpeechGenerationStatus.Paused:
                        _activeManager._speechSynthesizer.Resume();
                        _activeManager._status = SpeechGenerationStatus.Resuming;
                        break;
                    default:
                        return;
                }
            }

            _activeManager.SetStatus(_activeManager._status, _activeManager._lastAudioPosition);
        }
    }
}
