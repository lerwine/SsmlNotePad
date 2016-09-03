using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class XmlValidationMessageVM : DependencyObject
    {
        #region Message Property Members

        public const string PropertyName_Message = "Message";

        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Message, typeof(string), typeof(XmlValidationMessageVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(MessageProperty));
                return Dispatcher.Invoke(() => Message);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(MessagePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Message = value);
            }
        }

        #endregion

        #region LineNumber Property Members

        public const string PropertyName_LineNumber = "LineNumber";

        private static readonly DependencyPropertyKey LineNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LineNumber, typeof(int), typeof(XmlValidationMessageVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="LineNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineNumberProperty = LineNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int LineNumber
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(LineNumberProperty));
                return Dispatcher.Invoke(() => LineNumber);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(LineNumberPropertyKey, value);
                else
                    Dispatcher.Invoke(() => LineNumber = value);
            }
        }

        #endregion

        #region LinePosition Property Members

        public const string PropertyName_LinePosition = "LinePosition";

        private static readonly DependencyPropertyKey LinePositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LinePosition, typeof(int), typeof(XmlValidationMessageVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="LinePosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LinePositionProperty = LinePositionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int LinePosition
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(LinePositionProperty));
                return Dispatcher.Invoke(() => LinePosition);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(LinePositionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => LinePosition = value);
            }
        }

        #endregion

        #region InnerErrors Property Members

        private ObservableViewModelCollection<XmlValidationMessageVM> _innerErrors = new ObservableViewModelCollection<XmlValidationMessageVM>();

        public const string PropertyName_InnerErrors = "InnerErrors";

        private static readonly DependencyPropertyKey InnerErrorsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_InnerErrors, typeof(ReadOnlyObservableViewModelCollection<XmlValidationMessageVM>), typeof(XmlValidationMessageVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="InnerErrors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerErrorsProperty = InnerErrorsPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableViewModelCollection<XmlValidationMessageVM> InnerErrors
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableViewModelCollection<XmlValidationMessageVM>)(GetValue(InnerErrorsProperty));
                return Dispatcher.Invoke(() => InnerErrors);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(InnerErrorsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => InnerErrors = value);
            }
        }

        #endregion

        #region ValidationStatus Property Members

        public const string PropertyName_ValidationStatus = "ValidationStatus";

        private static readonly DependencyPropertyKey ValidationStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationStatus, typeof(XmlValidationStatus), typeof(XmlValidationMessageVM),
                new PropertyMetadata(XmlValidationStatus.Error));

        /// <summary>
        /// Identifies the <seealso cref="ValidationStatus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationStatusProperty = ValidationStatusPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public XmlValidationStatus ValidationStatus
        {
            get
            {
                if (CheckAccess())
                    return (XmlValidationStatus)(GetValue(ValidationStatusProperty));
                return Dispatcher.Invoke(() => ValidationStatus);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ValidationStatusPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ValidationStatus = value);
            }
        }

        #endregion

        #region ReferenceIndex Property Members

        public const string PropertyName_ReferenceIndex = "ReferenceIndex";

        private static readonly DependencyPropertyKey ReferenceIndexPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ReferenceIndex, typeof(int), typeof(XmlValidationMessageVM),
                new PropertyMetadata(-1));

        /// <summary>
        /// Identifies the <seealso cref="ReferenceIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReferenceIndexProperty = ReferenceIndexPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int ReferenceIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(ReferenceIndexProperty));
                return Dispatcher.Invoke(() => ReferenceIndex);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ReferenceIndexPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ReferenceIndex = value);
            }
        }

        #endregion

        #region ContextLines Property Members

        private ObservableViewModelCollection<string> _contextLines = new ObservableViewModelCollection<string>();

        public const string PropertyName_ContextLines = "ContextLines";

        private static readonly DependencyPropertyKey ContextLinesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ContextLines, typeof(ReadOnlyObservableViewModelCollection<string>), typeof(XmlValidationMessageVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="ContextLines"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextLinesProperty = ContextLinesPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableViewModelCollection<string> ContextLines
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableViewModelCollection<string>)(GetValue(ContextLinesProperty));
                return Dispatcher.Invoke(() => ContextLines);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ContextLinesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ContextLines = value);
            }
        }

        #endregion

        #region StackTrace Property Members

        public const string PropertyName_StackTrace = "StackTrace";

        private static readonly DependencyPropertyKey StackTracePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_StackTrace, typeof(string), typeof(XmlValidationMessageVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="StackTrace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StackTraceProperty = StackTracePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string StackTrace
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(StackTraceProperty));
                return Dispatcher.Invoke(() => StackTrace);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(StackTracePropertyKey, value);
                else
                    Dispatcher.Invoke(() => StackTrace = value);
            }
        }

        #endregion

        public XmlValidationMessageVM()
        {
            InnerErrors = new ReadOnlyObservableViewModelCollection<XmlValidationMessageVM>(_innerErrors);
            ContextLines = new ReadOnlyObservableViewModelCollection<string>(_contextLines);
        }

        public XmlValidationMessageVM(string message, Exception exception, int lineNumber, int linePosition, 
            StringLines sourceText, XmlValidationStatus status = XmlValidationStatus.Error)
        {
            InnerErrors = new ReadOnlyObservableViewModelCollection<XmlValidationMessageVM>(_innerErrors);
            ContextLines = new ReadOnlyObservableViewModelCollection<string>(_contextLines);

            ValidationStatus = status;

            if (String.IsNullOrWhiteSpace(message))
            {
                if (exception == null || String.IsNullOrWhiteSpace(exception.Message))
                    Message = status.ToString("F");
                else
                    Message = exception.Message;
            }
            else
                Message = message;

            LineNumber = lineNumber;
            LinePosition = linePosition;

            if (lineNumber > 0 && sourceText != null && lineNumber <= sourceText.Count)
            {
                int start = lineNumber - 3;
                int refIndex = 2;
                int end = lineNumber + 1;
                if (start < 0)
                {
                    refIndex += start;
                    start = 0;
                }
                for (int i = start; i < end; i++)
                    _contextLines.Add(sourceText[i]);
                ReferenceIndex = refIndex;
            }

            if (exception == null)
                return;

            try
            {
                StackTrace = exception.StackTrace;
            }
            catch { }

            if (exception is AggregateException)
            {
                AggregateException ae = exception as AggregateException;
                IEnumerable<Exception> exceptions = (ae.InnerException == null) ? null : new Exception[] { ae.InnerException };
                if (exceptions == null)
                    exceptions = ae.InnerExceptions;
                else
                    exceptions = exceptions.Concat(ae.InnerExceptions.Where(e => e != null && 
                        !ReferenceEquals(e, ae.InnerException)));
                foreach (Exception e in exceptions)
                    _innerErrors.Add(Create(e, sourceText, status));
            }
            else if (exception.InnerException != null)
                _innerErrors.Add(Create(exception.InnerException, sourceText, status));
        }

        private static XmlValidationMessageVM Create(Exception exception, StringLines sourceText, XmlValidationStatus status)
        {
            if (exception is XmlSchemaException)
                return new XmlValidationMessageVM(null, exception as XmlSchemaException, sourceText, status);

            if (exception is XmlException)
                return new XmlValidationMessageVM(null, exception as XmlException, sourceText, status);

            return new XmlValidationMessageVM(null, exception, status, sourceText);
        }

        public XmlValidationMessageVM(string message, XmlSchemaException exception, XmlSeverityType severity, 
                StringLines sourceText)
            : this(message, exception, sourceText, (severity == XmlSeverityType.Error) ? XmlValidationStatus.Error : 
                  XmlValidationStatus.Warning) { }

        public XmlValidationMessageVM(string message, XmlSchemaException exception, StringLines sourceText, 
                XmlValidationStatus status = XmlValidationStatus.Error)
            : this(message, exception, exception.LineNumber, exception.LinePosition, sourceText, status) { }

        public XmlValidationMessageVM(string message, XmlException exception, StringLines sourceText, 
                XmlValidationStatus status = XmlValidationStatus.Error)
            : this(message, exception, exception.LineNumber, exception.LinePosition, sourceText, status) { }
        
        public XmlValidationMessageVM(string message, Exception exception, 
                XmlValidationStatus status = XmlValidationStatus.Error, StringLines sourceText = null)
            : this(message, exception, -1, -1, null, status) { }

        public XmlValidationMessageVM(string message, XmlValidationStatus status = XmlValidationStatus.Error) : 
            this(message, null, status) { }
    }
}