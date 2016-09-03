using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class SpeakText : BackgroundJobWorker<SpeakText, int>
    {
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
        private bool _paused = false;
        private SpeechSynthesizer _speechSynthesizer = null;

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

        protected override int Run(SpeakText previousWorker)
        {
            if (Token.IsCancellationRequested || String.IsNullOrWhiteSpace(_ssml))
                return _characterPosition;

            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            {
                lock (_syncRoot)
                    _speechSynthesizer = speechSynthesizer;
                _viewModel.PauseSpeech += ViewModel_PauseSpeech;
                _viewModel.ResumeSpeech += ViewModel_ResumeSpeech;
                try
                {
                    _rate = speechSynthesizer.Rate;
                    _voice = speechSynthesizer.Voice;
                    _volume = speechSynthesizer.Volume;
                    if (_path != null)
                    {
                        if (_formatInfo != null)
                            speechSynthesizer.SetOutputToWaveFile(_path, _formatInfo);
                        else
                            speechSynthesizer.SetOutputToWaveFile(_path);
                    }
                    else if (_audioDestination != null)
                    {
                        if (_formatInfo != null)
                            speechSynthesizer.SetOutputToAudioStream(_audioDestination, _formatInfo);
                        else
                            speechSynthesizer.SetOutputToWaveStream(_audioDestination);
                    }
                    else
                        speechSynthesizer.SetOutputToDefaultAudioDevice();
                    CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                    {
                        _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                        _viewModel.SetStartedState();
                    }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
                    speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                    speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                    speechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                    speechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
                    speechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;

                    speechSynthesizer.SpeakSsml(_ssml);

                    bool isCanceled;
                    lock (_syncRoot)
                    {
                        isCanceled = _speechSynthesizer == null;
                        _speechSynthesizer = null;
                    }
                    if (isCanceled)
                        CheckInvoke(_viewModel, () => _viewModel.SetCanceledState());
                    else
                        CheckInvoke(_viewModel, () => _viewModel.SetCompletedState());
                }
                catch (Exception exception)
                {
                    lock (_syncRoot)
                    {
                        if (_speechSynthesizer != null)
                            try { speechSynthesizer.SpeakAsyncCancelAll(); } catch { }
                        _speechSynthesizer = null;
                    }

                    CheckInvoke(_viewModel, () => _viewModel.SetErrorState(exception));
                    _characterPosition = -1;
                }
                finally
                {
                    _viewModel.PauseSpeech -= ViewModel_PauseSpeech;
                    _viewModel.ResumeSpeech -= ViewModel_ResumeSpeech;
                    speechSynthesizer.SetOutputToNull();
                    speechSynthesizer.Dispose();
                }
            }

            return _characterPosition;
        }

        private void ViewModel_ResumeSpeech(object sender, EventArgs e)
        {
            if (CheckCancelSpeech())
                return;

            Task.Factory.StartNew(() =>
            {
                lock (_syncRoot)
                {
                    if (_speechSynthesizer == null || !_paused)
                        return;
                    _paused = false;
                    Task.Factory.StartNew((object o) =>
                    {
                        SpeechSynthesizer speechSynthesizer = o as SpeechSynthesizer;
                        if (Token.IsCancellationRequested || !_paused)
                            return;
                        speechSynthesizer.Resume();
                        CheckInvoke(_viewModel, () => _viewModel.SetUnpausedState());
                    }, _speechSynthesizer as object);
                }
            });
        }
        
        private void ViewModel_PauseSpeech(object sender, EventArgs e)
        {
            if (CheckCancelSpeech())
                return;

            Task.Factory.StartNew(() =>
            {
                lock (_syncRoot)
                {
                    if (_speechSynthesizer == null || _paused || Token.IsCancellationRequested)
                        return;
                    _paused = true;
                    Task.Factory.StartNew((object o) =>
                    {
                        SpeechSynthesizer speechSynthesizer = o as SpeechSynthesizer;
                        if (Token.IsCancellationRequested || !_paused)
                            return;
                        speechSynthesizer.Pause();
                        CheckInvoke(_viewModel, () => _viewModel.SetPausedState());
                    }, _speechSynthesizer as object);
                }
            });
        }

        private bool CheckCancelSpeech()
        {
            lock (_syncRoot)
            {
                if (_speechSynthesizer == null)
                    return true;

                if (!Token.IsCancellationRequested)
                    return false;

                Task.Factory.StartNew((object o) =>
                {
                    SpeechSynthesizer speechSynthesizer = o as SpeechSynthesizer;
                    if (_paused)
                    {
                        speechSynthesizer.Resume();
                        _paused = false;
                    }
                    speechSynthesizer.SpeakAsyncCancelAll();
                }, _speechSynthesizer as object);
                _speechSynthesizer = null;
            }

            return true;
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            if (CheckCancelSpeech())
                return;

            lock (_syncRoot)
            {
                _currentText = e.Text;
                _audioPosition = e.AudioPosition;
                _characterPosition = e.CharacterPosition;
                _characterCount = e.CharacterCount;
                CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                {
                    if (CheckCancelSpeech())
                        return;

                    _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                    if (e.Error != null)
                        _viewModel.AddErrorMessage(e.Error);
                }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
            }
        }

        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            if (CheckCancelSpeech())
                return;

            lock (_syncRoot)
            {
                _stressed = e.Emphasis == SynthesizerEmphasis.Stressed;
                _audioPosition = e.AudioPosition;
                CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                {
                    if (CheckCancelSpeech())
                        return;

                    _viewModel.SetPhoneme(e.Phoneme, e.Duration, e.NextPhoneme);
                    _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                    if (e.Error != null)
                        _viewModel.AddErrorMessage(e.Error);
                }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
            }
        }

        private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            if (CheckCancelSpeech())
                return;

            lock (_syncRoot)
            {
                if (CheckCancelSpeech())
                    return;

                _stressed = e.Emphasis == SynthesizerEmphasis.Stressed;
                _audioPosition = e.AudioPosition;
                CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                {
                    _viewModel.SetViseme(e.Viseme, e.Duration, e.NextViseme);
                    _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                    if (e.Error != null)
                        _viewModel.AddErrorMessage(e.Error);
                }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
            }
        }

        private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            if (CheckCancelSpeech())
                return;

            lock (_syncRoot)
            {
                _voice = e.Voice;
                CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                {
                    if (CheckCancelSpeech())
                        return;

                    _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                    if (e.Error != null)
                        _viewModel.AddErrorMessage(e.Error);
                }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
            }
        }

        private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            if (CheckCancelSpeech())
                return;

            lock (_syncRoot)
            {
                _audioPosition = e.AudioPosition;
                CheckInvokeAsync(_viewModel, (audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume) =>
                {
                    if (CheckCancelSpeech())
                        return;

                    _viewModel.SetBookmark(e.Bookmark);
                    _viewModel.Update(audioPosition, currentText, characterPosition, characterCount, rate, voice, stressed, volume);
                    if (e.Error != null)
                        _viewModel.AddErrorMessage(e.Error);
                }, _audioPosition, _currentText, _characterPosition, _characterCount, _rate, _voice, _stressed, _volume, DispatcherPriority.Input);
            }
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
