using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class SpeakText : BackgroundJobWorker<SpeakText, int>
    {
        public enum SpeechState
        {
            NotStarted,
            Speaking,
            Paused,
            Cancelling,
            Canceled,
            Error
        }

        private object _syncRoot = new object();
        private ViewModel.SpeechProgressVM _viewModel;
        private string _ssml;
        private int _characterPosition = 0;
        private int _characterCount = 0;
        private TimeSpan _audioPosition = TimeSpan.Zero;
        private int _rate = 0;
        private VoiceInfo _voice;
        private int _volume = 0;
        private SpeechAudioFormatInfo _formatInfo = null;
        private Stream _audioDestination = null;
        private string _path = null;
        private string _currentText = "";
        private bool _stressed = false;
        private SpeechState _state = SpeechState.NotStarted;
        private SpeechSynthesizer _activeSpeechSynthesizer = null;

        public SpeakText(string ssml, ViewModel.SpeechProgressVM viewModel, Stream audioDestination, SpeechAudioFormatInfo formatInfo)
            : this(ssml, viewModel, audioDestination)
        {
            if (formatInfo == null)
                throw new ArgumentNullException("formatInfo");

            _formatInfo = formatInfo;
        }

        public SpeakText(string ssml, ViewModel.SpeechProgressVM viewModel, Stream audioDestination)
            : this(ssml, viewModel)
        {
            if (audioDestination == null)
                throw new ArgumentNullException("audioDestination");

            _audioDestination = audioDestination;
        }

        public SpeakText(string ssml, ViewModel.SpeechProgressVM viewModel, string path, SpeechAudioFormatInfo formatInfo)
            : this(ssml, viewModel, path)
        {
            if (formatInfo == null)
                throw new ArgumentNullException("formatInfo");

            _formatInfo = formatInfo;
        }

        public SpeakText(string ssml, ViewModel.SpeechProgressVM viewModel, string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            
            _path = path;
        }

        public SpeakText(string ssml, ViewModel.SpeechProgressVM viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            
            _ssml = ssml;
            _characterCount = (ssml ?? "").Length;
            _viewModel = viewModel;
        }

        private bool TryInvoke(Func<bool> func)
        {
            if (CheckCancelSpeech())
                return false;

            try { return func(); }
            catch (Exception exception)
            {
                Logger.WriteLine("Unexpected exception: {0}", exception.ToString());
                try
                {
                    lock (_syncRoot)
                    {
                        switch (_state)
                        {
                            case SpeechState.Paused:
                                try
                                {
                                    _activeSpeechSynthesizer.Resume();
                                    Thread.Sleep(100);
                                    _activeSpeechSynthesizer.SpeakAsyncCancelAll();
                                }
                                catch { }
                                break;
                            case SpeechState.Speaking:
                                try { _activeSpeechSynthesizer.SpeakAsyncCancelAll(); } catch { }
                                break;
                        }
                        _state = SpeechState.Error;
                    }

                    CheckInvoke(_viewModel, () => _viewModel.SetErrorState(exception));
                }
                catch { }
                return false;
            }
        }

        protected override int Run(SpeakText previousWorker)
        {
            if (Token.IsCancellationRequested || String.IsNullOrWhiteSpace(_ssml))
                return _characterPosition;

            _activeSpeechSynthesizer = new SpeechSynthesizer();
            TryInvoke(() =>
            {
                Logger.WriteLine("Speech synthesizer created.");
                _viewModel.PauseSpeech += ViewModel_PauseSpeech;
                _viewModel.ResumeSpeech += ViewModel_ResumeSpeech;
                _rate = _activeSpeechSynthesizer.Rate;
                _voice = _activeSpeechSynthesizer.Voice;
                _volume = _activeSpeechSynthesizer.Volume;
                if (_path != null)
                {
                    if (_formatInfo != null)
                    {
                        Logger.WriteLine("speechSynthesizer.SetOutputToWaveFile({0}, _formatInfo);", _path);
                        _activeSpeechSynthesizer.SetOutputToWaveFile(_path, _formatInfo);
                    }
                    else
                    {
                        Logger.WriteLine("speechSynthesizer.SetOutputToWaveFile({0});", _path);
                        _activeSpeechSynthesizer.SetOutputToWaveFile(_path);
                    }
                }
                else if (_audioDestination != null)
                {
                    if (_formatInfo != null)
                    {
                        Logger.WriteLine("speechSynthesizer.SetOutputToAudioStream(_audioDestination, _formatInfo);");
                        _activeSpeechSynthesizer.SetOutputToAudioStream(_audioDestination, _formatInfo);
                    }
                    else
                    {
                        Logger.WriteLine("speechSynthesizer.SetOutputToAudioStream(_audioDestination);");
                        _activeSpeechSynthesizer.SetOutputToWaveStream(_audioDestination);
                    }
                }
                else
                {
                    Logger.WriteLine("speechSynthesizer.SetOutputToDefaultAudioDevice();");
                    _activeSpeechSynthesizer.SetOutputToDefaultAudioDevice();
                }

                Logger.WriteLine("Update view model: audioPosition = {0}, currentText = {1}, characterPosition = {2}, characterCount = {3}, rate = {4}, voice = {5}, stressed = {6}, volume = {7})", _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume);
                CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                {
                    _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                    _viewModel.SetStartedState();
                }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
                _activeSpeechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                _activeSpeechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                _activeSpeechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                _activeSpeechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
                _activeSpeechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;

                Logger.WriteLine("Starting speech synthesis");
                _state = SpeechState.Speaking;
                _activeSpeechSynthesizer.SpeakSsml(_ssml);
                Logger.WriteLine("Speech synthesis completed");

                if (Token.IsCancellationRequested)
                {
                    _state = SpeechState.Canceled;
                    CheckInvoke(_viewModel, () => _viewModel.SetCanceledState());
                    return false;
                }

                _state = SpeechState.Error;
                CheckInvoke(_viewModel, () => _viewModel.SetCompletedState());
                return true;
            });

            _viewModel.PauseSpeech -= ViewModel_PauseSpeech;
            _viewModel.ResumeSpeech -= ViewModel_ResumeSpeech;
            _activeSpeechSynthesizer.SetOutputToNull();
            if (_activeSpeechSynthesizer.State == SynthesizerState.Ready)
            {
                _activeSpeechSynthesizer.Dispose();
                _activeSpeechSynthesizer = null;
            }
            else
                _activeSpeechSynthesizer.SpeakCompleted += ActiveSpeechSynthesizer_SpeakCompleted;

            return _characterPosition;
        }

        private void ActiveSpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                _activeSpeechSynthesizer.SpeakCompleted -= ActiveSpeechSynthesizer_SpeakCompleted;
                _activeSpeechSynthesizer.Dispose();
                _activeSpeechSynthesizer = null;
            });
        }

        private void ViewModel_ResumeSpeech(object sender, EventArgs e)
        {
            if (TryInvoke(() => {
                lock (_syncRoot)
                {
                    if (_state != SpeechState.Paused)
                        return false;
                    Logger.WriteLine("Resuming speech");
                    _activeSpeechSynthesizer.Resume();
                    _state = SpeechState.Speaking;
                }
                return true;
            }))
                CheckInvoke(_viewModel, () => _viewModel.SetUnpausedState());
        }
        
        private void ViewModel_PauseSpeech(object sender, EventArgs e)
        {
            if (TryInvoke(() => {
                lock (_syncRoot)
                {
                    if (_state != SpeechState.Speaking)
                        return false;
                    Logger.WriteLine("Pausing speech");
                    _activeSpeechSynthesizer.Resume();
                    _state = SpeechState.Paused;
                }
                return true;
            }))
                CheckInvoke(_viewModel, () => _viewModel.SetPausedState());
        }

        private bool CheckCancelSpeech()
        {
            Logger.WriteLine("Locking for checking speech status");
            lock (_syncRoot)
            {
                switch (_state)
                {
                    case SpeechState.Canceled:
                    case SpeechState.Cancelling:
                    case SpeechState.Error:
                        return true;
                }

                if (!Token.IsCancellationRequested)
                    return false;

                bool shouldResume = (_state == SpeechState.Paused);
                _state = SpeechState.Cancelling;
                Logger.WriteLine("New cancel state detected. Queuing speech cancellation.");
                Task.Factory.StartNew((object o) =>
                {
                    if ((bool)o)
                    {
                        Logger.WriteLine("Unpausing before cancelling");
                        _activeSpeechSynthesizer.Resume();
                        Thread.Sleep(100);
                    }
                    Logger.WriteLine("Cancel all speech");
                    _activeSpeechSynthesizer.SpeakAsyncCancelAll();
                }, shouldResume);
            }

            _viewModel.SetCanceledState();

            return true;
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            TryInvoke(() =>
            {
                Task task;
                lock (_syncRoot)
                {
                    _currentText = e.Text;
                    _audioPosition = e.AudioPosition;
                    _characterPosition = e.CharacterPosition;
                    _characterCount = e.CharacterCount;
                    task = Task.Factory.StartNew(() =>
                    {
                        CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                        {
                            if (CheckCancelSpeech())
                                return;

                            _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                            if (e.Error != null)
                                _viewModel.AddErrorMessage(e.Error);
                        }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
                    });
                }
                task.Wait();
                return true;
            });
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            TryInvoke(() =>
            {
                Task task;
                lock (_syncRoot)
                {
                    _stressed = e.Emphasis == SynthesizerEmphasis.Stressed;
                    _audioPosition = e.AudioPosition;
                    task = Task.Factory.StartNew(() =>
                    {
                        CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                        {
                            if (CheckCancelSpeech())
                                return;

                            _viewModel.SetPhoneme(e.Phoneme, e.Duration, e.NextPhoneme);
                            _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                            if (e.Error != null)
                                _viewModel.AddErrorMessage(e.Error);
                        }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
                    });
                }
                task.Wait();
                return true;
            });
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            TryInvoke(() =>
            {
                Task task;
                lock (_syncRoot)
                {
                    _stressed = e.Emphasis == SynthesizerEmphasis.Stressed;
                    _audioPosition = e.AudioPosition;
                    task = Task.Factory.StartNew(() =>
                    {
                        CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                        {
                            _viewModel.SetViseme(e.Viseme, e.Duration, e.NextViseme);
                            _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                            if (e.Error != null)
                                _viewModel.AddErrorMessage(e.Error);
                        }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
                    });
                }
                task.Wait();
                return true;
            });
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            TryInvoke(() =>
            {
                Task task;
                lock (_syncRoot)
                {
                    _voice = e.Voice;
                    task = Task.Factory.StartNew(() =>
                    {
                        CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                        {
                            if (CheckCancelSpeech())
                                return;

                            _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                            if (e.Error != null)
                                _viewModel.AddErrorMessage(e.Error);
                        }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
                    });
                }
                task.Wait();
                return true;
            });
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            TryInvoke(() =>
            {
                Task task;
                lock (_syncRoot)
                {
                    _audioPosition = e.AudioPosition;
                    task = Task.Factory.StartNew(() =>
                    {
                        CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                        {
                            if (CheckCancelSpeech())
                                return;

                            _viewModel.SetBookmark(e.Bookmark);
                            _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                            if (e.Error != null)
                                _viewModel.AddErrorMessage(e.Error);
                        }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
                    });
                }
                task.Wait();
                return true;
            });
        }
        
        public override int FromActive() { return _characterPosition; }

        public override int FromCanceled()
        {
            _characterPosition = -1;
            return _characterPosition;
        }

        public override int FromFault(AggregateException exception)
        {
            _characterPosition = -1;
            return _characterPosition;
        }
    }
}
