using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class XmlValidator
    {
        private object _syncRoot = new object();
        private string _text = "";
        Task<XmlValidationResult> _validateXml;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private List<Tuple<object, GetXmlValidationStatusHandler>> _onValidateXmlComplete = new List<Tuple<object, GetXmlValidationStatusHandler>>();
        ViewModel.MainWindowVM _mainWindowVM;
        TextLineBackgroundParser _linesParser;

        public XmlValidator()
        {
            _validateXml = Task<XmlValidationResult>.FromResult<XmlValidationResult>(new XmlValidationResult(XmlValidationStatus.Warning, "No XML data provided.", new XmlDocument()));
        }

        public void GetStatus(object state, GetXmlValidationStatusHandler onValidateXmlComplete)
        {
            lock (_syncRoot)
            {
                if (_validateXml.IsCompleted)
                    Task.Factory.StartNew(o =>
                    {
                        object[] args = o as object[];
                        GetXmlValidationStatusHandler h = args[0] as GetXmlValidationStatusHandler;
                        h(args[1], args[2] as string, args[3] as Task<XmlValidationResult>);
                    }, new object[] { onValidateXmlComplete, state, _text, _validateXml });
                else
                    _onValidateXmlComplete.Add(new Tuple<object, GetXmlValidationStatusHandler>(state, onValidateXmlComplete));
            }
        }

        private void ValidateXmlCompleted(Task<XmlValidationResult> task)
        {
            lock (_syncRoot)
            {
                if (task.Id != _validateXml.Id || task.IsCanceled)
                    return;

                foreach (Tuple<object, GetXmlValidationStatusHandler> handler in _onValidateXmlComplete)
                {
                    task.ContinueWith((t, o) =>
                    {
                        object[] args = o as object[];
                        Tuple<object, GetXmlValidationStatusHandler> h = args[0] as Tuple<object, GetXmlValidationStatusHandler>;
                        GetXmlValidationStatusHandler onValidateXmlComplete = h.Item2;
                        onValidateXmlComplete(h.Item1, args[1] as string, t);
                    }, new object[] { handler, _text });
                }
                _onValidateXmlComplete.Clear();
            }
        }

        public void SetText(string text, ViewModel.MainWindowVM vm, TextLineBackgroundParser linesParser)
        {
            lock (_syncRoot)
            {
                if (_text == text)
                    return;
                _mainWindowVM = vm;
                _linesParser = linesParser;
                linesParser.Text = text;
                _text = text;
                if (_validateXml.IsCompleted)
                {
                    try { _tokenSource.Dispose(); }
                    catch { }
                }
                else
                {
                    Task.Factory.StartNew(o =>
                    {
                        object[] args = o as object[];
                        CancellationTokenSource cts = args[0] as CancellationTokenSource;
                        if (!cts.IsCancellationRequested)
                            cts.Cancel();
                        Task<XmlValidationResult> t = args[1] as Task<XmlValidationResult>;
                        try
                        {
                            if (!t.IsCompleted)
                                t.Wait(1000);
                        }
                        finally { cts.Dispose(); }
                    }, new object[] { _tokenSource, _validateXml });
                }
                _tokenSource = new CancellationTokenSource();
                _validateXml = Task<XmlValidationResult>.Factory.StartNew(ValidateXml, _tokenSource.Token);
                _validateXml.ContinueWith(ValidateXmlCompleted);
            }
        }

        public XmlValidationResult ValidateXml()
        {
            if (String.IsNullOrWhiteSpace(_linesParser.Text))
                return new XmlValidationResult(XmlValidationStatus.Warning, "XML text is empty", null);

            XmlDocument xmlDocument = new XmlDocument();

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    CheckCharacters = false,
                    DtdProcessing = DtdProcessing.Ignore,
                    Schemas = new XmlSchemaSet(),
                    ValidationType = ValidationType.Schema
                };

                settings.ValidationEventHandler += Settings_ValidationEventHandler;
                using (StringReader stringReader = new StringReader(_linesParser.Text))
                {
                    using (XmlReader xmlreader = XmlReader.Create(stringReader, settings))
                        xmlDocument.Load(xmlreader);
                }
                
                throw new NotImplementedException();
            }
            catch (XmlSchemaException exception)
            {
                return new XmlValidationResult(XmlValidationStatus.Critical, String.Format("Line {0}, Column {1} - A fatal error has occurred: {2}", exception.LineNumber, exception.LinePosition, exception.Message), xmlDocument);
            }
            catch (XmlException exception)
            {
                return new XmlValidationResult(XmlValidationStatus.Critical, String.Format("Line {0}, Column {1} - A fatal error has occurred: {2}", exception.LineNumber, exception.LinePosition, exception.Message), xmlDocument);
            }
            catch (Exception exception)
            {
                throw new NotImplementedException();
            }
        }

        private void Settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    public delegate void GetXmlValidationStatusHandler(object state, string text, Task<XmlValidationResult> task);
}
