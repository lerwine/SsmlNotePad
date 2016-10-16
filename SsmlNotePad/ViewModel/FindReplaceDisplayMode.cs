using System;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    /// <summary>
    /// Represents a display mode for showing find/replace windows.
    /// </summary>
    public enum FindReplaceDisplayMode
    {
        /// <summary>
        /// Do not diplay window.
        /// </summary>
        None,

        /// <summary>
        /// Display for find.
        /// </summary>
        Find,

        /// <summary>
        /// Display for find next
        /// </summary>
        FindNext,

        /// <summary>
        /// Display for find and replace.
        /// </summary>
        Replace,

        /// <summary>
        /// Display for replace next.
        /// </summary>
        ReplaceNext,

        /// <summary>
        /// Display result of find match not found.
        /// </summary>
        FindNotFound,

        /// <summary>
        /// Display result of replace or replace match not found.
        /// </summary>
        ReplaceNotFound
    }
}
