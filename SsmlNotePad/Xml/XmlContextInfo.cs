using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Xml
{
    public class XmlContextInfo : IReadOnlyList<XmlNodeContext>
    {
        private XmlNodeContext[] _nodes = null;
        private List<XmlValidationMessage> _validationMessages = new List<XmlValidationMessage>();
        private Text.TextLineInfo[] _lines;

        public ReadOnlyCollection<Text.TextLineInfo> Lines { get; private set; }

        public ReadOnlyCollection<XmlValidationMessage> ValidationMessages { get; private set; }

        private void Initialize(string text, XmlParseContextSettings settings)
        {
            ValidationMessages = new ReadOnlyCollection<XmlValidationMessage>(_validationMessages);
            _lines = Text.TextLineInfo.Load(text).ToArray();
            Lines = new ReadOnlyCollection<Text.TextLineInfo>(_lines);
            if (String.IsNullOrWhiteSpace(text))
            {
                _validationMessages.Add(XmlValidationMessage.Create("Text contains no XML.", XmlSeverityType.Warning, 1, 1, _lines));
                return;
            }

            int lastLineNumber = 1, lastLinePosition = 1;
            XmlReaderSettings xmlReaderSettings = settings.ToXmlReaderSettings();
            xmlReaderSettings.ValidationEventHandler += Xml_ValidationEventHandler;
            if (xmlReaderSettings.Schemas.Count > 0 || xmlReaderSettings.ValidationType == ValidationType.Schema)
                xmlReaderSettings.ValidationEventHandler += Xml_ValidationEventHandler;
            Action<int, int> updateLineInfo = (int n, int p) =>
            {
                lastLineNumber = n;
                lastLinePosition = p;
            };
            try
            {
                using (StringReader stringReader = new StringReader(text))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
                        _nodes = XmlNodeContext.Load(xmlReader, _lines, updateLineInfo).ToArray();
                }
            }
            catch (XmlSchemaException exception)
            {
                _validationMessages.Add(XmlValidationMessage.Create(exception, lastLineNumber, lastLinePosition, _lines));
            }
            catch (XmlException exception)
            {
                _validationMessages.Add(XmlValidationMessage.Create(exception, lastLineNumber, lastLinePosition, _lines));
            }
            catch (WarningException exception)
            {
                _validationMessages.Add(XmlValidationMessage.Create(exception, lastLineNumber, lastLinePosition, _lines));
            }
            catch (AggregateException exception)
            {
                _validationMessages.Add(XmlValidationMessage.Create(exception, lastLineNumber, lastLinePosition, _lines));
            }
            catch (Exception exception)
            {
                _validationMessages.Add(XmlValidationMessage.Create(exception, lastLineNumber, lastLinePosition, _lines));
            }
            finally
            {
                xmlReaderSettings.ValidationEventHandler -= Xml_ValidationEventHandler;
                if (xmlReaderSettings.Schemas.Count > 0 || xmlReaderSettings.ValidationType == ValidationType.Schema)
                    xmlReaderSettings.ValidationEventHandler -= Xml_ValidationEventHandler;
                if (_nodes == null)
                    _nodes = new XmlNodeContext[0];
            }
        }
        
        private void Xml_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            _validationMessages.Add(XmlValidationMessage.Create(e, _lines));
        }

        private void Initialize(TextReader textReader, XmlParseContextSettings settings)
        {
            Initialize(textReader.ReadToEnd(), settings);
        }

        public XmlContextInfo(string text, XmlParseContextSettings settings)
        {
            Initialize(text, settings);
        }

        public XmlContextInfo(TextReader textReader, XmlParseContextSettings settings)
        {
            if (textReader == null)
                throw new ArgumentNullException("textReader");

            Initialize(textReader, settings);
        }

        public XmlContextInfo(Stream stream, bool detectEncodingFromByteOrderMarks, XmlParseContextSettings settings)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (StreamReader streamReader = new StreamReader(stream, detectEncodingFromByteOrderMarks))
                Initialize(streamReader, settings);
        }

        public XmlContextInfo(Stream stream, Encoding encoding, XmlParseContextSettings settings)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (StreamReader streamReader = new StreamReader(stream, encoding))
                Initialize(streamReader, settings);
        }

        public XmlContextInfo(Stream stream, XmlParseContextSettings settings)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (StreamReader streamReader = new StreamReader(stream))
                Initialize(streamReader, settings);
        }

        public XmlNodeContext this[int index] { get { return _nodes[index]; } }

        public int Count { get { return _nodes.Length; } }

        IEnumerator<XmlNodeContext> IEnumerable<XmlNodeContext>.GetEnumerator() { return _nodes.ToList().GetEnumerator(); }

        public IEnumerator GetEnumerator() { return _nodes.GetEnumerator(); }
    }
}
