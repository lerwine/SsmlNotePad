using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class XmlValidationCompleteEventArgs : EventArgs
    {
        public string Text { get; set; }
        public List<Model.TextLine> Lines { get; set; }
        public List<Model.ValidationError> Errors { get; set; }
        public Model.XmlValidationStatus Status { get; set; }
        public string Message { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }

        public XmlValidationCompleteEventArgs(string text, List<Model.TextLine> lines) : this(text, lines, Model.XmlValidationStatus.None, "") { }
        public XmlValidationCompleteEventArgs(string text, List<Model.TextLine> lines, Model.XmlValidationStatus status, string message)
        {
            Text = text;
            Errors = new List<Model.ValidationError>();
            Lines = lines;
            Status = status;
            Message = message;
        }

        public void Xml_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
                WarningCount++;
            else
                ErrorCount++;

            Errors.Add(new Model.ValidationError(e));
        }

        public void SetStatus(Model.XmlValidationStatus status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
