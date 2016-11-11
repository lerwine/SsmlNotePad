using System;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public enum MessageLevel : byte
    {
        /// <summary>
        /// Informational notification
        /// </summary>
        Information = 2,

        /// <summary>
        /// User alert notification.
        /// </summary>
        Alert = 3,

        /// <summary>
        /// Warning message
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Error message for recoverable event.
        /// </summary>
        Error = 5,

        /// <summary>
        /// Error message for unexpected event.
        /// </summary>
        Critical = 6,

        /// <summary>
        /// Diagnostic informational message.
        /// </summary>
        Diagnostic = 0,

        /// <summary>
        /// Verbose message.
        /// </summary>
        Verbose = 1
    }
}
