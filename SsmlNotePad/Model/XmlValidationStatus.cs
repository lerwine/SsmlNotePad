namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    /// <summary>
    /// Represents an XML validation message status.
    /// </summary>
    public enum XmlValidationStatus
    {
        /// <summary>
        /// Not validated.
        /// </summary>
        None = 0,

        /// <summary>
        /// Validation information message.
        /// </summary>
        Information,

        /// <summary>
        /// Validation warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Validation error.
        /// </summary>
        Error,

        /// <summary>
        /// Critcal validation error.
        /// </summary>
        Critical
    }
}
