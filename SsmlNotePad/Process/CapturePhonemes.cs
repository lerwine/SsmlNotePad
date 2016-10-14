using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class CapturePhonemes
    {
        public bool IsCancellationRequested
        {
            get
            {
                lock (_syncRoot)
                    return _tokenSource != null && _tokenSource.IsCancellationRequested;
            }
        }

        public void CancelAfter(int millisecondsDelay)
        {
            lock (_syncRoot)
            {
                if (_tokenSource != null)
                    _tokenSource.CancelAfter(millisecondsDelay);
            }
        }

        public void CancelAfter(TimeSpan delay)
        {
            lock (_syncRoot)
            {
                if (_tokenSource != null)
                    _tokenSource.CancelAfter(delay);
            }
        }

        public void Cancel()
        {
            lock (_syncRoot)
            {
                if (_tokenSource != null)
                    _tokenSource.Cancel();
            }
        }

        private object _syncRoot = new object();
        private string _ssml;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private LinkedList<PhoneticGroupInfo> _groups = new LinkedList<PhoneticGroupInfo>();
        private LinkedList<PhonemeReachedEventArgs> _phonemes = new LinkedList<PhonemeReachedEventArgs>();
        int _currentPosition = 0, _nextPosition = 0;
        string _text = "";
        
        public Task<PhoneticGroupInfo[]> Task { get; private set; }

        public CapturePhonemes(string ssml)
        {
            _ssml = ssml;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            Task = Task<PhoneticGroupInfo[]>.Factory.StartNew(_GetResult, _token);
            Task.ContinueWith(t =>
            {
                lock (_syncRoot)
                {
                    _tokenSource.Dispose();
                    _tokenSource = null;
                }
            });
        }

        private PhoneticGroupInfo[] _GetResult()
        {
            XmlDocument xmlDocument = new XmlDocument();
            try { xmlDocument.LoadXml(_ssml.Trim()); } catch { }
            if (xmlDocument.DocumentElement == null)
            {
                xmlDocument.AppendChild(xmlDocument.CreateElement("speak", Markup.SsmlSchemaNamespaceURI));
                try { xmlDocument.DocumentElement.InnerXml = _ssml; } catch { xmlDocument.InnerText = _ssml; }
            }

            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            {
                ViewModel.AppSettingsVM settings = App.AppSettingsViewModel;
                settings.Dispatcher.Invoke(() =>
                {
                    if (!String.IsNullOrWhiteSpace(settings.DefaultVoiceName))
                        try { speechSynthesizer.SelectVoice(settings.DefaultVoiceName); } catch { }
                    speechSynthesizer.Rate = settings.DefaultSpeechRate;
                    speechSynthesizer.Volume = settings.DefaultSpeechVolume;
                });
                speechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                speechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                speechSynthesizer.SpeakSsml(xmlDocument.DocumentElement.OuterXml);
            }

            PhoneticGroupInfo[] groups = _groups.OrderBy(a => a.CharacterPosition).ThenBy(a => a.AudioPosition).ToArray();
            if (groups.Length > 0)
            {
                int index = 0;
                foreach (PhonemeReachedEventArgs arg in _phonemes.OrderBy(p => p.AudioPosition))
                {
                    index = groups.Skip(index).SkipWhile(a => a.AudioPosition < arg.AudioPosition).Count() - 1;
                    groups[index].Add(arg);
                }
            }

            return groups;
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            lock (_groups)
            {
                if (e.CharacterPosition < _nextPosition && e.Text == _text)
                    return;

                _currentPosition = e.CharacterPosition;
                _nextPosition = e.CharacterPosition + e.CharacterCount;
                _text = e.Text;
                _groups.AddLast(new PhoneticGroupInfo(e));
            }
        }
        
        private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            lock (_phonemes)
                _phonemes.AddLast(e);
        }
    }
}
