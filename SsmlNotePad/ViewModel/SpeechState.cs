using System;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    [Flags]
    public enum SpeechState : byte
    {
        NotStarted = 0,
        Paused = 1,
        Speaking = 3,
        Completed = 2,
        Canceled = 4,
        HasFault = 5
    }
}