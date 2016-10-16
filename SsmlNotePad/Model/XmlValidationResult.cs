using System;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class XmlValidationResult
    {
        public XmlValidationStatus Status { get; private set; }

        public string Message { get; private set; }

        public XmlDocument Xaml { get; private set; }
        
        public XmlValidationResult(XmlValidationStatus status, string message, XmlDocument xaml)
        {
            Status = status;
            Message = (String.IsNullOrWhiteSpace(message)) ? status.ToString() : message;
            Xaml = xaml ?? new XmlDocument();
        }
    }
}