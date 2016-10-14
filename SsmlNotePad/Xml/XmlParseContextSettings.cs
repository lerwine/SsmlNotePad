using System;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Xml
{
    public class XmlParseContextSettings
    {
        public bool CheckCharacters { get; set; }
        public int LineNumberOffset { get; set; }
        public int LinePositionOffset { get; set; }
        public XmlSchemaSet Schemas { get; set; }

        public XmlReaderSettings ToXmlReaderSettings()
        {
            XmlReaderSettings result = new XmlReaderSettings
            {
                CheckCharacters = CheckCharacters,
                LineNumberOffset = LineNumberOffset,
                LinePositionOffset = LinePositionOffset,
                ValidationType = (Schemas == null) ? ValidationType.None : ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes | XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ReportValidationWarnings
            };
            if (Schemas != null)
                result.Schemas = Schemas;
            return result;
        }
    }
}