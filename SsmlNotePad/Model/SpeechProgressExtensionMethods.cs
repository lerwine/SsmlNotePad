using System;
using System.Linq;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public static class SpeechProgressExtensionMethods
    {
        public static SpeechProcessStateFlags ToProcessState(this SpeechProgressState value) { return (SpeechProcessStateFlags)((int)value); }

        public static SpeechProcessStateFlags? ToProcessState(this SpeechProgressState? value) { return (value.HasValue) ? value.Value.ToProcessState() as SpeechProcessStateFlags? : null; }

        public static bool IsCompleted(this SpeechProgressState value) { return value.ToProcessState().HasFlag(SpeechProcessStateFlags.Completed); }

        public static bool IsCanceled(this SpeechProgressState value) { return value.ToProcessState().HasFlag(SpeechProcessStateFlags.Cancel); }

        public static bool IsFaulted(this SpeechProgressState value) { return value.ToProcessState().HasFlag(SpeechProcessStateFlags.Fault); }

        public static bool IsInProgress(this SpeechProgressState value) { return value.ToProcessState().HasFlag(SpeechProcessStateFlags.InProgress); }

        public static bool IsSpeaking(this SpeechProgressState value) { return value.ToProcessState().HasFlag(SpeechProcessStateFlags.Speaking); }
    }
}
