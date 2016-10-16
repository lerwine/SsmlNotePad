namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    /// <summary>
    /// Validation state for <seealso cref="ValidatingViewModel"/> view model.
    /// </summary>
    public enum ViewModelValidateState
    {
        /// <summary>
        /// View model contains no errors or warnings.
        /// </summary>
        Valid,

        /// <summary>
        /// View model contains one or more errors.
        /// </summary>
        Error,

        /// <summary>
        /// View model contains one or more warnings and no errors.
        /// </summary>
        Warning
    }
}