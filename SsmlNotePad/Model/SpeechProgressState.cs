using System;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    /// <summary>
    /// Represents the current state of progress for speech generation.
    /// </summary>
    public enum SpeechProgressState
    {
        /// <summary>
        /// Speech synthesizer has not been started.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.None"/>.</remarks>
        NotStarted = 0x0000,

        /// <summary>
        /// Speech synthesizer is currently speaking, no faults have been encountered and it is not in a state of being canceled.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.InProgress"/> <code>|</code> <see cref="SpeechProcessStateFlags.Speaking"/>.</remarks>
        SpeakingNormal = 0x000C,

        /// <summary>
        /// Speech synthesizer is currently speaking, at least one fault has been encountered and it is not in a state of being canceled.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.InProgress"/> <code>|</code> <see cref="SpeechProcessStateFlags.Speaking"/> <code>|</code> <see cref="SpeechProcessStateFlags.Fault"/>.</remarks>
        SpeakingWithFault = 0x000E,

        /// <summary>
        /// Speech synthesizer is currently paused and no faults have been encountered.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.InProgress"/>.</remarks>
        PausedNormal = 0x0008,

        /// <summary>
        /// Speech synthesizer is currently paused and at least one fault has been encountered.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.InProgress"/> <code>|</code> <see cref="SpeechProcessStateFlags.Fault"/>.</remarks>
        PausedWithFault = 0x000A,

        /// <summary>
        /// Speech synthesizer is currently speaking and is in the state of being canceled.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.InProgress"/> <code>|</code> <see cref="SpeechProcessStateFlags.Speaking"/> <code>|</code> <see cref="SpeechProcessStateFlags.Cancel"/>.</remarks>
        Cancelling = 0x000D,

        /// <summary>
        /// Speech synthesizer has completed successfully, no faults have been encountered and was not canceled.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.Completed"/>.</remarks>
        CompletedSuccess = 0x0010,

        /// <summary>
        /// Speech synthesizer has completed, at least one fault has been encountered, and was not canceled.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.Completed"/> <code>|</code> <see cref="SpeechProcessStateFlags.Fault"/>.</remarks>
        CompletedWithFault = 0x0012,

        /// <summary>
        /// Speech synthesizer has completed and was canceled.
        /// </summary>
        /// <remarks>Equivalent to <see cref="SpeechProcessStateFlags.Completed"/> <code>|</code> <see cref="SpeechProcessStateFlags.Cancel"/>.</remarks>
        Canceled = 0x0011
    }
}
