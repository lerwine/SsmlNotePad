using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Erwine.Leonard.T.SsmlNotePad.Model;
using System.Xml.Schema;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class XmlValidationMessage : ProcessMessageVM, Reserved.IPositionalProcessMessageViewModel<ProcessMessageVM, ReadOnlyObservableCollection<ProcessMessageVM>>
    {
        #region LineNumber Property Members

        /// <summary>
        /// Defines the name for the <see cref="LineNumber"/> dependency property.
        /// </summary>
        public const string PropertyName_LineNumber = "LineNumber";

        private static readonly DependencyPropertyKey LineNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LineNumber, typeof(int), typeof(XmlValidationMessage),
            new PropertyMetadata(1));

        /// <summary>
        /// Identifies the <see cref="LineNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineNumberProperty = LineNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// Line number
        /// </summary>
        public int LineNumber
        {
            get { return (int)(GetValue(LineNumberProperty)); }
            private set { SetValue(LineNumberPropertyKey, value); }
        }

        #endregion

        #region ColumnNumber Property Members

        /// <summary>
        /// Defines the name for the <see cref="ColumnNumber"/> dependency property.
        /// </summary>
        public const string PropertyName_ColumnNumber = "ColumnNumber";

        private static readonly DependencyPropertyKey ColumnNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ColumnNumber, typeof(int), typeof(XmlValidationMessage),
            new PropertyMetadata(1));

        /// <summary>
        /// Identifies the <see cref="ColumnNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnNumberProperty = ColumnNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// Column njumber
        /// </summary>
        public int ColumnNumber
        {
            get { return (int)(GetValue(ColumnNumberProperty)); }
            private set { SetValue(ColumnNumberPropertyKey, value); }
        }


        #endregion

        public XmlValidationMessage(string message, MessageLevel level, int lineNumber, int colNumber, Exception exception, DateTime created) :
            base(message, level, exception, created)
        {
            LineNumber = lineNumber;
            ColumnNumber = colNumber;
        }

        public XmlValidationMessage(string message, MessageLevel level, int lineNumber, int colNumber, Exception exception) :
            this(message, level, lineNumber, colNumber, exception, DateTime.Now) { }

        public XmlValidationMessage(MessageLevel level, int lineNumber, int colNumber, Exception exception, DateTime created) :
            this(null, level, lineNumber, colNumber, exception, DateTime.Now) { }

        public XmlValidationMessage(string message, MessageLevel level, XmlSchemaException exception, DateTime created)
            : this(message, level, (exception == null) ? 0 : exception.LineNumber, (exception == null) ? 0 : exception.LinePosition, exception as Exception, created) { }

        public XmlValidationMessage(string message, MessageLevel level, XmlException exception, DateTime created)
            : this(message, level, (exception == null) ? 0 : exception.LineNumber, (exception == null) ? 0 : exception.LinePosition, exception as Exception, created) { }

        public XmlValidationMessage(string message, MessageLevel level, Exception exception, DateTime created) : base(message, level, exception, created)
        {
        }

        public XmlValidationMessage(string message, MessageLevel level, DateTime created) : base(message, level, created)
        {
        }

        public XmlValidationMessage(string message, MessageLevel level, XmlSchemaException exception)
            : this(message, level, (exception == null) ? 0 : exception.LineNumber, (exception == null) ? 0 : exception.LinePosition, exception as Exception) { }

        public XmlValidationMessage(string message, MessageLevel level, XmlException exception)
            : this(message, level, (exception == null) ? 0 : exception.LineNumber, (exception == null) ? 0 : exception.LinePosition, exception as Exception) { }

        public XmlValidationMessage(string message, MessageLevel level, Exception exception) : base(message, level, exception)
        {
        }
        
        public XmlValidationMessage(string message, MessageLevel level) : base(message, level)
        {
        }

        public XmlValidationMessage(MessageLevel level, XmlSchemaException exception, DateTime created)
            : this(level, (exception == null) ? 0 : exception.LineNumber, (exception == null) ? 0 : exception.LinePosition, exception as Exception, created) { }

        public XmlValidationMessage(MessageLevel level, XmlException exception, DateTime created)
            : this(level, (exception == null) ? 0 : exception.LineNumber, (exception == null) ? 0 : exception.LinePosition, exception as Exception, created) { }

        public XmlValidationMessage(MessageLevel level, Exception exception, DateTime created) : base(level, exception, created)
        {
        }
    }
}
