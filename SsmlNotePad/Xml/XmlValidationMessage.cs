using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Xml
{
    public class XmlValidationMessage
    {
        public string Message { get; private set; }

        public string Details { get; private set; }

        public Exception Exception { get; private set; }

        public XmlSeverityType Severity { get; private set; }

        public int LineNumber { get; private set; }

        public int LinePosition { get; private set; }

        public XmlValidationMessage(string message, string details, Exception exception, XmlSeverityType severity, int lineNumber, int linePosition,
            Text.TextLineInfo[] lines)
        {
            if (exception == null || String.IsNullOrWhiteSpace(exception.Message))
            {
                if (String.IsNullOrWhiteSpace(message))
                {
                    if (String.IsNullOrWhiteSpace(details))
                        message = severity.ToString("F");
                    else
                    {
                        string s = message;
                        message = details;
                        details = s;
                    }
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(message))
                    message = exception.Message;
                else if (String.IsNullOrWhiteSpace(details))
                    details = exception.Message;
            }

            Message = message;
            Details = details ?? "";
            Exception = exception;
            Severity = severity;
            LineNumber = lineNumber;
            LinePosition = linePosition;
            // TODO: Set context lines
        }

        public static XmlValidationMessage Create(string message, Exception exception, XmlSeverityType severity, int lineNumber, int linePosition, Text.TextLineInfo[] lines)
        {
            return new XmlValidationMessage(message, null, exception, XmlSeverityType.Error, lineNumber, linePosition, lines);
        }

        //public static XmlValidationMessage Create(string message, Exception exception, int lineNumber, int linePosition, Text.TextLineInfo[] lines)
        //{
        //    return Create(message, exception, XmlSeverityType.Error, lineNumber, linePosition, lines);
        //}

        public static XmlValidationMessage Create(string message, XmlSeverityType severity, int lineNumber, int linePosition, Text.TextLineInfo[] lines)
        {
            return Create(message, null as Exception, severity, lineNumber, linePosition, lines);
        }

        //public static XmlValidationMessage Create(string message, int lineNumber, int linePosition, Text.TextLineInfo[] lines,
        //    params XmlValidationMessage[] innerErrors)
        //{
        //    return Create(message, null as Exception, XmlSeverityType.Error, lineNumber, linePosition, lines);
        //}

        public static XmlValidationMessage Create(string message, XmlSchemaException exception, XmlSeverityType severity, Text.TextLineInfo[] lines)
        {
            if (exception == null)
                return Create(message, exception, severity, 0, 0, lines);

            return Create(message, exception, severity, exception.LineNumber, exception.LinePosition, lines);
        }
        
        public static XmlValidationMessage Create(XmlSchemaException exception, int defaultLineNumber, int defaultLinePosition, Text.TextLineInfo[] lines)
        {
            if (exception == null)
                return Create("XmlSchemaException", exception, XmlSeverityType.Error, defaultLineNumber, defaultLinePosition, lines);

            if (exception.LineNumber < 1 || exception.LineNumber > lines.Length)
                return Create(exception.Message, exception, XmlSeverityType.Error, defaultLineNumber, defaultLinePosition, lines);

            return Create(exception.Message, exception, XmlSeverityType.Error, exception.LineNumber, exception.LinePosition, lines);
        }

        public static XmlValidationMessage Create(XmlException exception, int defaultLineNumber, int defaultLinePosition, Text.TextLineInfo[] lines)
        {
            if (exception == null)
                return Create("XmlException", exception, XmlSeverityType.Error, defaultLineNumber, defaultLinePosition, lines);

            if (exception.LineNumber < 1 || exception.LineNumber > lines.Length)
                return Create(exception.Message, exception, XmlSeverityType.Error, defaultLineNumber, defaultLinePosition, lines);

            return Create(exception.Message, exception, XmlSeverityType.Error, exception.LineNumber, exception.LinePosition, lines);
        }
        
        public static XmlValidationMessage Create(AggregateException exception, int defaultLineNumber, int defaultLinePosition, Text.TextLineInfo[] lines)
        {
            if (exception == null)
                return Create("AggregateException", exception, XmlSeverityType.Error, defaultLineNumber, defaultLinePosition, lines);

            return Create(exception.Message, exception, XmlSeverityType.Error, defaultLineNumber, defaultLinePosition, lines);
        }

        public static XmlValidationMessage Create(WarningException exception, int lineNumber, int linePosition, Text.TextLineInfo[] lines)
        {
            if (exception == null)
                return Create("WarningException", exception, XmlSeverityType.Warning, lineNumber, linePosition, lines);

            return Create(exception.Message, exception, XmlSeverityType.Warning, lineNumber, linePosition, lines);
        }

        public static XmlValidationMessage Create(Exception exception, int lineNumber, int linePosition, Text.TextLineInfo[] lines)
        {
            if (exception == null)
                return Create("Exception", exception, XmlSeverityType.Error, lineNumber, linePosition, lines);

            return Create(exception.Message, exception, XmlSeverityType.Error, lineNumber, linePosition, lines);
        }

        public static XmlValidationMessage Create(ValidationEventArgs args, Text.TextLineInfo[] lines)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            return Create(args.Message, args.Exception, args.Severity, lines);
        }
    }
}