using System.ComponentModel;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class LineMessageVM : NotificationMessageVM
    {
        #region LineNumber Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="LineNumber"/> dependency property.
        /// </summary>
        public const string PropertyName_LineNumber = "LineNumber";

        private static readonly DependencyPropertyKey LineNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LineNumber, typeof(int), typeof(LineMessageVM),
            new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="LineNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineNumberProperty = LineNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// Line number associated with message or 0 for no specific line.
        /// </summary>
        [DefaultValue(0)]
        public int LineNumber
        {
            get { return (int)(GetValue(LineNumberProperty)); }
            private set { SetValue(LineNumberPropertyKey, value); }
        }

        #endregion

        #region ColNumber Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="ColNumber"/> dependency property.
        /// </summary>
        public const string PropertyName_ColNumber = "ColNumber";

        private static readonly DependencyPropertyKey ColNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ColNumber, typeof(int), typeof(LineMessageVM),
            new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="ColNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColNumberProperty = ColNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// Column number associated with message or 0 for no specific column number.
        /// </summary>
        [DefaultValue(0)]
        public int ColNumber
        {
            get { return (int)(GetValue(ColNumberProperty)); }
            private set { SetValue(ColNumberPropertyKey, value); }
        }
        
        #endregion
    }
}