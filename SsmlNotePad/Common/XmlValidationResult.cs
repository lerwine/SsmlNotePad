using System;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class XmlValidationResult
    {
        public const string Message_NoXml = "No XML data was found.";
        public const string Message_Success = "Validation completed successfully.";

        public Model.XmlValidationStatus Status { get; private set; }

        public string Message { get; private set; }
        
        public XmlValidationResult(Model.XmlValidationStatus status, string message)
        {
            Status = status;
            Message = (String.IsNullOrWhiteSpace(message)) ? status.ToString() : message;
        }

        public static XmlValidationResult Create(ViewModel.MainWindowVM viewModel, CancellationToken token, Model.TextLine[] lines)
        {
            XmlValidator validator = new XmlValidator(viewModel, token, lines);
            return validator.GetResult();
        }
    }
}