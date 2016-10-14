using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class SpeechSynthesis
    {
        private object _syncRoot = new object();
        private ManualResetEvent _speechCompleted = new ManualResetEvent(false);
        private string _ssml;
        private View.SpeechProgressWindow _progressWindow = null;
        private ViewModel.SpeechProgressVM _progressViewModel = null;
        private SpeechSynthesizer _speechSynthesizer = null;
        private bool _result = false;
        private ViewModel.SpeechState _currentState = ViewModel.SpeechState.NotStarted;
        public Task ActiveTask { get; private set; }

        public static SpeechSynthesis ForAudioStream(string ssml, Stream audioDestination, SpeechAudioFormatInfo formatInfo, Window owner = null)
        {
            if (audioDestination == null)
                throw new ArgumentNullException("audioDestination");

            return new SpeechSynthesis(ssml, null, audioDestination, formatInfo, owner);
        }

        public static SpeechSynthesis ForWaveStream(string ssml, Stream audioDestination, Window owner = null)
        {
            if (audioDestination == null)
                throw new ArgumentNullException("audioDestination");

            return new SpeechSynthesis(ssml, null, audioDestination, null, owner);
        }

        public static SpeechSynthesis ForWaveFile(string ssml, string path, SpeechAudioFormatInfo formatInfo, Window owner = null)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            return new SpeechSynthesis(ssml, path, null, formatInfo, owner);
        }

        public static SpeechSynthesis ForWaveFile(string ssml, string path, Window owner = null)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            return new SpeechSynthesis(ssml, path, null, null, owner);
        }

        public static SpeechSynthesis ForDefaultAudioDevice(string ssml, Window owner = null)
        {
            return new SpeechSynthesis(ssml, null, null, null, owner);
        }

        private SpeechSynthesis(string ssml, string path, Stream audioDestination, SpeechAudioFormatInfo formatInfo, Window owner)
        {
            _ssml = ssml ?? "";
            ActiveTask = Task.Factory.StartNew(() =>
            {
                using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
                {
                    _speechSynthesizer = speechSynthesizer;
                    ViewModel.AppSettingsVM settings = App.AppSettingsViewModel;
                    settings.Dispatcher.Invoke(() =>
                    {
                        if (!String.IsNullOrWhiteSpace(settings.DefaultVoiceName))
                            try { speechSynthesizer.SelectVoice(settings.DefaultVoiceName); } catch { }
                        speechSynthesizer.Rate = settings.DefaultSpeechRate;
                        speechSynthesizer.Volume = settings.DefaultSpeechVolume;
                    });
                    try
                    {
                        if (audioDestination != null)
                        {
                            if (formatInfo == null)
                                speechSynthesizer.SetOutputToWaveStream(audioDestination);
                            else
                                speechSynthesizer.SetOutputToAudioStream(audioDestination, formatInfo);
                        }
                        else if (path == null)
                            speechSynthesizer.SetOutputToDefaultAudioDevice();
                        else if (formatInfo == null)
                            speechSynthesizer.SetOutputToWaveFile(path);
                        else
                            speechSynthesizer.SetOutputToWaveFile(path, formatInfo);

                        Action<Window> action = o =>
                        {
                            _progressWindow = new View.SpeechProgressWindow();
                            _progressWindow.Owner = o;
                            _progressViewModel = _progressWindow.DataContext as ViewModel.SpeechProgressVM;
                            if (_progressViewModel == null)
                            {
                                _progressViewModel = new ViewModel.SpeechProgressVM();
                                _progressWindow.DataContext = _progressViewModel;
                            }
                            if (audioDestination != null)
                                _progressViewModel.OutputDestination = "Audio Stream";
                            else if (path == null)
                                _progressViewModel.OutputDestination = "Default audio device";
                            else
                                _progressViewModel.OutputDestination = path;
                                _progressViewModel.Voice.SetVoice(speechSynthesizer.Voice);
                            _progressViewModel.Rate = speechSynthesizer.Rate;
                            _progressViewModel.Volume = speechSynthesizer.Volume;
                            _progressWindow.Closing += ProgressWindow_Closing;
                        };

                        if (owner == null)
                            App.Current.Dispatcher.Invoke(() =>
                                action(App.Current.MainWindow));
                        else
                            owner.Dispatcher.Invoke(() => action(owner));

                        speechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
                        speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                        speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                        speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                        speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
                        speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
                        speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
                        _progressViewModel.PauseSpeech += ProgressViewModel_PauseSpeech;
                        _progressViewModel.ResumeSpeech += ProgressViewModel_ResumeSpeech;
                        _progressViewModel.CancelSpeech += ProgressViewModel_CancelSpeech;
                        speechSynthesizer.SpeakSsmlAsync(_ssml);
                        _progressWindow.Dispatcher.Invoke(() => _progressWindow.ShowDialog());
                    }
                    catch (Exception exception)
                    {
                        Action action = () =>
                        {
                            lock (_syncRoot)
                            {
                                _progressViewModel.ResumeSpeechCommand.IsEnabled = false;
                                _progressViewModel.PauseSpeechCommand.IsEnabled = false;
                                _progressViewModel.ToggleSpeechCommand.IsEnabled = false;
                                _progressViewModel.CurrentState = ViewModel.SpeechState.Faulted;
                            }
                            _progressViewModel.AddException(exception, ViewModel.MessageSeverity.Critical);
                        };

                        if (_progressViewModel.CheckAccess())
                            action();
                        else
                            _progressViewModel.Dispatcher.Invoke(action);
                    }
                    finally
                    {
                        try { speechSynthesizer.SetOutputToNull(); } catch { /* Okay to ignore, making sure any file handles are released */ }
                        Task task;
                        lock (_syncRoot)
                            task = (_speechCompleted != null) ? Task.Factory.StartNew(o => { try { (o as ManualResetEvent).WaitOne(new TimeSpan(0, 0, 15)); } catch { } }, _speechCompleted) : null;

                        if (task != null)
                            task.Wait();
                    }
                }

                if (_speechCompleted != null)
                    try { _speechCompleted.Dispose(); } finally { _speechCompleted = null; }
            });
        }

        private void ProgressViewModel_PauseSpeech(object sender, EventArgs e)
        {
            lock (_syncRoot)
            {
                if (_currentState != ViewModel.SpeechState.Speaking)
                    return;

                _currentState = ViewModel.SpeechState.Paused;
            }

            _speechSynthesizer.Pause();

            Action action = () =>
            {
                _progressViewModel.PauseSpeechCommand.IsEnabled = false;
                _progressViewModel.ResumeSpeechCommand.IsEnabled = true;
                _progressViewModel.CurrentState = _currentState;
            };

            if (_progressViewModel.CheckAccess())
                action();
            else
                _progressViewModel.Dispatcher.Invoke(action);
        }

        private void ProgressViewModel_ResumeSpeech(object sender, EventArgs e)
        {
            lock (_syncRoot)
            {
                if (_currentState != ViewModel.SpeechState.Paused)
                    return;

                _currentState = ViewModel.SpeechState.Speaking;
            }

            _speechSynthesizer.Resume();

            Action action = () =>
            {
                _progressViewModel.ResumeSpeechCommand.IsEnabled = false;
                _progressViewModel.PauseSpeechCommand.IsEnabled = true;
                _progressViewModel.CurrentState = _currentState;
            };

            if (_progressViewModel.CheckAccess())
                action();
            else
                _progressViewModel.Dispatcher.Invoke(action);
        }

        private void ProgressViewModel_CancelSpeech(object sender, EventArgs e)
        {
            bool shouldResume = false;
            lock (_syncRoot)
            {
                if (_currentState == ViewModel.SpeechState.Paused)
                {
                    _progressViewModel.ResumeSpeechCommand.IsEnabled = false;
                    shouldResume = true;
                }
                else if (_currentState != ViewModel.SpeechState.Speaking)
                    return;
                
                _currentState = ViewModel.SpeechState.Canceled;
            }

            Action action = () =>
            {
                _progressViewModel.PauseSpeechCommand.IsEnabled = false;
                _progressViewModel.ResumeSpeechCommand.IsEnabled = false;
                _progressViewModel.CurrentState = _currentState;
            };

            if (_progressViewModel.CheckAccess())
                action();
            else
                _progressViewModel.Dispatcher.Invoke(action);

            if (shouldResume)
            {
                _speechSynthesizer.Resume();
                Thread.Sleep(100);
            }

            _speechSynthesizer.SpeakAsyncCancelAll();
        }

        private void ProgressWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.SpeechProgressVM vm;
            Action action = () =>
            {
                _progressViewModel.PauseSpeech -= ProgressViewModel_PauseSpeech;
                _progressViewModel.ResumeSpeech -= ProgressViewModel_ResumeSpeech;
                _progressViewModel.CancelSpeech -= ProgressViewModel_CancelSpeech;
                _progressViewModel.PauseSpeechCommand.IsEnabled = false;
                _progressViewModel.ResumeSpeechCommand.IsEnabled = false;
                _progressViewModel.CancelSpeechCommand.IsEnabled = false;
                _progressViewModel.ToggleSpeechCommand.IsEnabled = false;
                if (_currentState == ViewModel.SpeechState.Paused || _currentState == ViewModel.SpeechState.Speaking)
                    _progressViewModel.CurrentState = ViewModel.SpeechState.Canceled;
            };
            
            Task task;
            lock (_syncRoot)
            {
                if (_progressViewModel == null)
                    return;
                vm = _progressViewModel;
                if (_currentState == ViewModel.SpeechState.Paused)
                {
                    _progressViewModel.ResumeSpeechCommand.IsEnabled = false;
                    _speechSynthesizer.Resume();
                    _currentState = ViewModel.SpeechState.Speaking;
                    Thread.Sleep(100);
                }

                if (_currentState != ViewModel.SpeechState.Speaking)
                    task = null;
                else
                {
                    _currentState = ViewModel.SpeechState.Canceled;
                    task = Task.Factory.StartNew(o =>
                    {
                        try
                        {
                            ManualResetEvent manualResetEvent = o as ManualResetEvent;
                            if (manualResetEvent != null)
                                manualResetEvent.WaitOne(new TimeSpan(0, 0, 15));
                        }
                        catch { }
                    }, _speechCompleted);
                }
            }

            if (_progressViewModel.CheckAccess())
                action();
            else
                _progressViewModel.Dispatcher.Invoke(action);
            
            if (task != null)
            {
                _speechSynthesizer.SpeakAsyncCancelAll();
                task.Wait();
            }
        }
        
        private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            ViewModel.SpeechProgressVM vm;
            lock (_syncRoot)
            {
                if ((vm = _progressViewModel) == null || (_currentState != ViewModel.SpeechState.NotStarted && _currentState != ViewModel.SpeechState.Paused))
                    return;

                _currentState = ViewModel.SpeechState.Speaking;
            }

            Action action = () =>
            {
                vm.CurrentState = _currentState;
                if (e.Error != null)
                    vm.AddException(e.Error);
                if (_currentState == ViewModel.SpeechState.Speaking && !e.Cancelled)
                {
                    vm.CancelSpeechCommand.IsEnabled = true;
                    vm.PauseSpeechCommand.IsEnabled = true;
                }
            };

            if (vm.CheckAccess())
                action();
            else
                vm.Dispatcher.Invoke(action);
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            ViewModel.SpeechProgressVM vm = _progressViewModel;
            if (vm == null)
                return;

            int totalCount = (e.CharacterPosition > _ssml.Length) ? e.CharacterPosition : _ssml.Length;
            int percentComplete = (e.CharacterPosition * 100) / totalCount;
            Action action = () =>
            {
                vm.AudioPosition.SetTimeSpan(e.AudioPosition);
                vm.PercentComplete = percentComplete;
                vm.CurrentText = e.Text;
                if (e.Error != null)
                    vm.AddException(e.Error);
            };

            if (vm.CheckAccess())
                action();
            else
                vm.Dispatcher.Invoke(action);
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            ViewModel.SpeechProgressVM vm = _progressViewModel;
            if (vm == null)
                return;
            
            Action action = () =>
            {
                vm.AudioPosition.SetTimeSpan(e.AudioPosition);
                if (e.Error != null)
                    vm.AddException(e.Error);
            };

            if (vm.CheckAccess())
                action();
            else
                vm.Dispatcher.Invoke(action);
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            ViewModel.SpeechProgressVM vm = _progressViewModel;
            if (vm == null)
                return;

            Action action = () =>
            {
                vm.AudioPosition.SetTimeSpan(e.AudioPosition);
                if (e.Error != null)
                    vm.AddException(e.Error);
            };

            if (vm.CheckAccess())
                action();
            else
                vm.Dispatcher.Invoke(action);
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            ViewModel.SpeechProgressVM vm = _progressViewModel;
            if (vm == null)
                return;

            Action action = () =>
            {
                vm.Voice.SetVoice(e.Voice);
                if (e.Error != null)
                    vm.AddException(e.Error);
            };

            if (vm.CheckAccess())
                action();
            else
                vm.Dispatcher.Invoke(action);
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            ViewModel.SpeechProgressVM vm = _progressViewModel;
            if (vm == null)
                return;

            Action action = () =>
            {
                vm.AddBookmark(e.AudioPosition, e.Bookmark);
                if (e.Error != null)
                    _progressViewModel.AddException(e.Error);
            };

            if (vm.CheckAccess())
                action();
            else
                vm.Dispatcher.Invoke(action);
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            ManualResetEvent manualResetEvent;
            ViewModel.SpeechProgressVM vm;
            lock (_syncRoot)
            {
                vm = _progressViewModel;
                if (e.Error != null && _currentState != ViewModel.SpeechState.Canceled)
                    _currentState = ViewModel.SpeechState.Faulted;
                else if (e.Cancelled && _currentState != ViewModel.SpeechState.Canceled && _currentState != ViewModel.SpeechState.Faulted)
                    _currentState = ViewModel.SpeechState.Canceled;
                else if (_currentState == ViewModel.SpeechState.Speaking || _currentState == ViewModel.SpeechState.Paused)
                    _currentState = ViewModel.SpeechState.Completed;
                manualResetEvent = _speechCompleted;
                _speechCompleted = null;
            }

            Func<bool> func = () =>
            {
                if (e.Error != null)
                    vm.AddException(e.Error);
                if (vm.CurrentState != _currentState)
                    vm.CurrentState = _currentState;
                vm.CancelSpeechCommand.IsEnabled = false;
                vm.PauseSpeechCommand.IsEnabled = false;
                vm.ResumeSpeechCommand.IsEnabled = false;
                vm.ToggleSpeechCommand.IsEnabled = false;
                if (_currentState != ViewModel.SpeechState.Completed)
                    return false;

                vm.PercentComplete = 100;
                return vm.Messages.Count == 0;
            };
            if (vm != null && ((vm.CheckAccess()) ? func() : vm.Dispatcher.Invoke(func)))
            {
                _progressViewModel = null;
                Task.Factory.StartNew(() => _progressWindow.Dispatcher.Invoke(() => _progressWindow.Close()));
            }

            if (manualResetEvent != null)
            {
                manualResetEvent.Set();
                Thread.Sleep(10);
                manualResetEvent.Dispose();
            }
        }
    }
}
