using System;
using System.Windows;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class XmlValidationMessageVM : ExceptionMessageVM
    {
        public const string ValidationMessage_NoXmlData = "No XML data has been defined.";

        #region LineNumber Property Members

        /// <summary>
        /// Defines the name for the <see cref="LineNumber"/> dependency property.
        /// </summary>
        public const string PropertyName_LineNumber = "LineNumber";

        private static readonly DependencyPropertyKey LineNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LineNumber, typeof(int), typeof(XmlValidationMessageVM),
            new PropertyMetadata(1/*, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                (d as XmlValidationMessageVM).LineNumber_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))*/));

        /// <summary>
        /// Identifies the <see cref="LineNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineNumberProperty = LineNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// Current line number at start of selection or caret.
        /// </summary>
        public int LineNumber
        {
            get { return (int)(GetValue(LineNumberProperty)); }
            private set { SetValue(LineNumberPropertyKey, value); }
        }

        #endregion

        #region LinePosition Property Members

        /// <summary>
        /// Defines the name for the <see cref="LinePosition"/> dependency property.
        /// </summary>
        public const string PropertyName_LinePosition = "LinePosition";

        private static readonly DependencyPropertyKey LinePositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LinePosition, typeof(int), typeof(XmlValidationMessageVM),
            new PropertyMetadata(1/*, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                (d as XmlValidationMessageVM).LinePosition_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))*/));

        /// <summary>
        /// Identifies the <see cref="LinePosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LinePositionProperty = LinePositionPropertyKey.DependencyProperty;

        /// <summary>
        /// Current line number at start of selection or caret.
        /// </summary>
        public int LinePosition
        {
            get { return (int)(GetValue(LinePositionProperty)); }
            private set { SetValue(LinePositionPropertyKey, value); }
        }

        #endregion

        public XmlValidationMessageVM(Model.Workers.XmlValidationResult validationResult)
            : this(validationResult.Message, validationResult.Exception, validationResult.LineNumber, validationResult.LinePosition,
                  validationResult.Status == Model.XmlValidationStatus.Warning) { }

        public void UpdateFrom(Model.Workers.XmlValidationResult validationResult)
        {
            UpdateFrom(validationResult.Message, validationResult.Exception, validationResult.Status == Model.XmlValidationStatus.Warning);
            LineNumber = validationResult.LineNumber;
            LinePosition = validationResult.LinePosition;
        }

        public XmlValidationMessageVM(XmlException exception) : this(exception, false) { }

        public XmlValidationMessageVM(XmlException exception, bool isWarning) : this(exception, (exception == null) ? 1 : exception.LineNumber, (exception == null) ? 1 : exception.LinePosition, isWarning) { }

        public XmlValidationMessageVM(XmlSchemaException exception) : this(exception, false) { }

        public XmlValidationMessageVM(XmlSchemaException exception, bool isWarning) : this(exception, (exception == null) ? 1 : exception.LineNumber, (exception == null) ? 1 : exception.LinePosition, isWarning) { }

        public XmlValidationMessageVM(Exception exception, int lineNumber, int linePosition) : this(exception, lineNumber, linePosition, false) { }

        public XmlValidationMessageVM(Exception exception, int lineNumber, int linePosition, bool isWarning) : this((exception == null) ? "" : exception.Message, exception, lineNumber, linePosition, isWarning) { }

        public XmlValidationMessageVM(string message, Exception exception, int lineNumber, int linePosition) : this(message, exception, lineNumber, linePosition, false) { }

        public XmlValidationMessageVM(string message, Exception exception, int lineNumber, int linePosition, bool isWarning)
            : base(message, exception, isWarning)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }
    }
}