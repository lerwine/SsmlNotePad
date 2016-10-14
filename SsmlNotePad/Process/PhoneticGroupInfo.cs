using System;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class PhoneticGroupInfo : List<PhonemeInfo>
    {
        public string Text { get; private set; }
        public int CharacterPosition { get; private set; }
        public TimeSpan AudioPosition { get; private set; }

        public void Add(PhonemeReachedEventArgs args)
        {
            Add(new PhonemeInfo(args.Phoneme, args.AudioPosition - AudioPosition, args.Duration, args.Emphasis == SynthesizerEmphasis.Stressed));
        }

        public PhoneticGroupInfo(SpeakProgressEventArgs args)
        {
            AudioPosition = args.AudioPosition;
            CharacterPosition = args.CharacterPosition;
            Text = args.Text;
        }
    }
}