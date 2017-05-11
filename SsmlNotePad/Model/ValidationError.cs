using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class ValidationError
    {
        public const string Message_NoXml = "No XML data was found.";
        public const string Message_Success = "Validation completed successfully.";

        public int LineNumber { get; set; }

        public int LinePosition { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public XmlValidationStatus Status { get; set; }

        public ValidationError(ValidationEventArgs e)
        {
            Initialize(e.Exception.LineNumber, e.Exception.LinePosition, e.Message, e.Exception, (e.Severity == XmlSeverityType.Warning) ? XmlValidationStatus.Warning : XmlValidationStatus.Error);
        }

        public ValidationError(XmlSchemaException exception)
        {
            Initialize(exception.LineNumber, exception.LinePosition, exception.Message, exception, XmlValidationStatus.Critical);
        }

        public ValidationError(XmlException exception)
        {
            Initialize(exception.LineNumber, exception.LinePosition, exception.Message, exception, XmlValidationStatus.Critical);
        }

        public ValidationError(Exception exception)
        {
            Initialize(0, 0, exception.Message, exception, XmlValidationStatus.Critical);
        }

        public ValidationError(int lineNumber, int linePosition, string message, Exception exception, XmlValidationStatus status)
        {
            Initialize(lineNumber, linePosition, message, exception, status);
        }

        private void Initialize(int lineNumber, int linePosition, string message, Exception exception, XmlValidationStatus status)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
            Message = message;
            Exception = exception;
            Status = status;
        }
    }
}
