namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    /// <summary>
    /// Alert level for a <seealso cref="IProcessMessageHost{TCollection, TItem}"/> object.
    /// </summary>
    public enum AlertLevel
    {
        /// <summary>
        /// <seealso cref="IProcessMessageHost{TCollection, TItem}.Messages"/> is empty.
        /// </summary>
        None,

        /// <summary>
        /// <seealso cref="IProcessMessageHost{TCollection, TItem}.Messages"/> is not empty, and all <seealso cref="IProcessMessage"/> items have a
        /// <seealso cref="IProcessMessage.Level"/> property value set to <seealso cref="MessageLevel.Diagnostic"/>.
        /// </summary>
        Diagnostic,

        /// <summary>
        /// At least one <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value set to <seealso cref="MessageLevel.Verbose"/>
        /// and no other <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value higher than that.
        /// </summary>
        Verbose,

        /// <summary>
        /// At least one <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value set to <seealso cref="MessageLevel.Information"/>
        /// and no other <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value higher than that.
        /// </summary>
        Information,

        /// <summary>
        /// At least one <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value set to <seealso cref="MessageLevel.Alert"/>
        /// and no other <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value higher than that.
        /// </summary>
        Alert,

        /// <summary>
        /// At least one <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value set to <seealso cref="MessageLevel.Warning"/>
        /// and no other <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value higher than that.
        /// </summary>
        Warning,

        /// <summary>
        /// At least one <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value set to <seealso cref="MessageLevel.Error"/>
        /// or <seealso cref="MessageLevel.Critical"/>.
        /// </summary>
        Error,

        /// <summary>
        /// At least one <seealso cref="IProcessMessage"/> items have a <seealso cref="IProcessMessage.Level"/> property value set to <seealso cref="MessageLevel.Critical"/>.
        /// </summary>
        Critical = 5
    }
}