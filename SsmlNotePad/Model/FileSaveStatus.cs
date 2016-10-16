namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    /// <summary>
    /// Represents the saved state of a file.
    /// </summary>
    public enum FileSaveStatus
    {
        /// <summary>
        /// File is new and has never been saved.
        /// </summary>
        New,

        /// <summary>
        /// File has previously been saved to disk, but has been modified.
        /// </summary>
        Modified,

        /// <summary>
        /// File has been successfuly saved to disk and has not been modified.
        /// </summary>
        SaveSuccess,

        /// <summary>
        /// An error has occurred while trying to save file to disk.
        /// </summary>
        SaveError,

        /// <summary>
        /// A warning has been issued while saving file to disk.
        /// </summary>
        SaveWarning
    }
}
