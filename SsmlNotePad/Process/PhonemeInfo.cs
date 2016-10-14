using System;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class PhonemeInfo
    {
        public string Phoneme { get; private set; }
        public TimeSpan RelativePosition { get; private set; }
        public TimeSpan Duration { get; private set; }
        public bool IsStressed { get; private set; }
        public PhonemeInfo(string phoneme, TimeSpan relativePosition, TimeSpan duration, bool isStressed)
        {
            Phoneme = phoneme;
            RelativePosition = relativePosition;
            Duration = duration;
            IsStressed = isStressed;
        }
    }
}