using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    /// <summary>
    /// Represents state flags for the speech synthesis process.
    /// </summary>
    [Flags]
    public enum SpeechProcessStateFlags
    {
        /// <summary>
        /// Speech synthesizer process has not been started.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Speech synthesizer process has completed.
        /// </summary>
        Completed = 0x10,

        /// <summary>
        /// Speech synthesizer process is active.
        /// </summary>
        InProgress = 0x08,

        /// <summary>
        /// Speech synthesizer is producing speech.
        /// </summary>
        Speaking = 0x04,

        /// <summary>
        /// Speech synthesizer in a fault state.
        /// </summary>
        Fault = 0x02,

        /// <summary>
        /// Speech synthesizer is a cancellation state.
        /// </summary>
        Cancel = 0x01
    }
}
