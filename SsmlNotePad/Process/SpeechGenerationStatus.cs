namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public enum SpeechGenerationStatus
    {
        NotStarted,
        Initializing,
        SpeakInitiated,
        Speaking,
        Paused,
        Resuming,
        Completing,
        CancelInitiated,
        Canceled,
        ErrorAbortInitiated,
        ErrorAborted,
        Completed
    }
}