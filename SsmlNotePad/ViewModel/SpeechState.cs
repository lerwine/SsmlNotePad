using System;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public enum SpeechState : byte
    {
        NotStarted = 0,
        Speaking = 1,
        Paused = 2,
        Completed = 3,
        Canceled = 4,
        Faulted = 5
    }
}