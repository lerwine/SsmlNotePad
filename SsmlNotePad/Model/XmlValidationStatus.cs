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
        None,

        /// <summary>
        /// Critcal validation error.
        /// </summary>
        Critical,

        /// <summary>
        /// Validation error.
        /// </summary>
        Error,

        /// <summary>
        /// Validation warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Validation information message.
        /// </summary>
        Information
    }
}
