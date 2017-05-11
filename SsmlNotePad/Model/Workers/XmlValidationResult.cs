using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Model.Workers
{
    public class XmlValidationResult
    {
        public const string Message_NoXml = "No XML data was found.";
        public const string Message_Success = "Validation completed successfully.";

        public int LineNumber { get; set; }

        public int LinePosition { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public XmlValidationStatus Status { get; set; }

        public XmlValidationResult(ValidationEventArgs e)
        {
            Initialize(e.Exception.LineNumber, e.Exception.LinePosition, e.Message, e.Exception, (e.Severity == XmlSeverityType.Warning) ? XmlValidationStatus.Warning : XmlValidationStatus.Error);
        }

        public XmlValidationResult(XmlSchemaException exception)
        {
            Initialize(exception.LineNumber, exception.LinePosition, exception.Message, exception, XmlValidationStatus.Critical);
        }

        public XmlValidationResult(XmlException exception)
        {
            Initialize(exception.LineNumber, exception.LinePosition, exception.Message, exception, XmlValidationStatus.Critical);
        }

        public XmlValidationResult(Exception exception)
        {
            Initialize(0, 0, exception.Message, exception, XmlValidationStatus.Critical);
        }

        public XmlValidationResult(int lineNumber, int linePosition, string message, Exception exception, XmlValidationStatus status)
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

        public static Task<XmlValidationResult[]> ValidateAsync(Task<TextLine[]> parseLinesTask, CancellationToken token)
        {
            return parseLinesTask.ContinueWith(_ValidateAsync, token);
        }

        private static XmlValidationResult[] _ValidateAsync(Task<TextLine[]> parseLinesTask, object obj)
        {
            CancellationToken token = (CancellationToken)obj;
            if (token.IsCancellationRequested)
                return new XmlValidationResult[0];
            XmlValidationEventCatcher catcher = new XmlValidationEventCatcher(String.Join(Environment.NewLine, parseLinesTask.Result.Select(t => t.LineContent)), token);

            if (token.IsCancellationRequested)
                return new XmlValidationResult[0];

            return catcher.Errors.ToArray();
        }

        public class XmlValidationEventCatcher
        {
            public List<XmlValidationResult> Errors { get; set; }

            public XmlValidationEventCatcher(string sourceText, CancellationToken token)
            {
                Errors = new List<XmlValidationResult>();
                try
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        CheckCharacters = false,
                        DtdProcessing = DtdProcessing.Ignore,
                        Schemas = new XmlSchemaSet(),
                        ValidationType = ValidationType.Schema,
                        ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes | XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessSchemaLocation
                    };
                    settings.Schemas.Add(Markup.SsmlSchemaNamespaceURI, App.AppSettingsViewModel.Dispatcher.Invoke(() => App.AppSettingsViewModel.SsmlSchemaCoreFilePath));
                    settings.Schemas.Add(Markup.SsmlSchemaNamespaceURI, App.AppSettingsViewModel.Dispatcher.Invoke(() => App.AppSettingsViewModel.SsmlSchemaFilePath));
                    settings.Schemas.ValidationEventHandler += Xml_ValidationEventHandler;
                    settings.ValidationEventHandler += Xml_ValidationEventHandler;
                    using (StringReader stringReader = new StringReader(sourceText))
                    {
                        using (XmlReader xmlreader = XmlReader.Create(stringReader, settings))
                        {
                            while (xmlreader.Read())
                            {
                                if (token.IsCancellationRequested)
                                    break;
                            }
                        }
                    }
                }
                catch (XmlSchemaException exception)
                {
                    Errors.Add(new XmlValidationResult(exception));
                }
                catch (XmlException exception)
                {
                    Errors.Add(new XmlValidationResult(exception));
                }
                catch (Exception exception)
                {
                    Errors.Add(new XmlValidationResult(exception));
                }
            }

            private void Xml_ValidationEventHandler(object sender, ValidationEventArgs e)
            {
                Errors.Add(new XmlValidationResult(e));
            }
        }
    }
}