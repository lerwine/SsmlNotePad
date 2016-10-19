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

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class XmlValidator
    {
        private object _syncRoot = new object();
        Model.TextLine[] _lines;
        private CancellationToken _token;
        ViewModel.MainWindowVM _mainWindowVM;
        private int _errorCount = 0;
        private int _warningCount = 0;

        public XmlValidator(ViewModel.MainWindowVM viewModel, CancellationToken token, Model.TextLine[] lines)
        {
            _mainWindowVM = viewModel;
            _token = token;
            _lines = lines ?? new Model.TextLine[0];
        }

        internal XmlValidationResult GetResult()
        {
            string text = String.Join("", _lines.Select(l => l.LineContent + l.LineEnding));

            if (_token.IsCancellationRequested)
                return new XmlValidationResult(Model.XmlValidationStatus.None, "Validation canceled");

            _mainWindowVM.Dispatcher.Invoke(() => _mainWindowVM.ClearValidationErrors());

            if (String.IsNullOrWhiteSpace(text))
                return new XmlValidationResult(Model.XmlValidationStatus.Warning, XmlValidationResult.Message_NoXml);
            
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
                settings.Schemas.Add(Markup.SsmlSchemaNamespaceURI, App.AppSettingsViewModel.SsmlSchemaCoreFilePath);
                settings.Schemas.Add(Markup.SsmlSchemaNamespaceURI, App.AppSettingsViewModel.SsmlSchemaFilePath);
                settings.Schemas.ValidationEventHandler += Settings_ValidationEventHandler;
                settings.ValidationEventHandler += Settings_ValidationEventHandler;
                using (StringReader stringReader = new StringReader(text))
                {
                    using (XmlReader xmlreader = XmlReader.Create(stringReader, settings))
                    {
                        while (xmlreader.Read())
                        {
                            if (_token.IsCancellationRequested)
                                break;
                        }
                    }
                }

                if (_token.IsCancellationRequested)
                    return new XmlValidationResult(Model.XmlValidationStatus.None, "Validation canceled");

                if (_errorCount == 0)
                {
                    if (_warningCount == 0)
                        return new XmlValidationResult(Model.XmlValidationStatus.Information, XmlValidationResult.Message_Success);

                    if (_warningCount == 1)
                        return new XmlValidationResult(Model.XmlValidationStatus.Warning, "Validation complete with 1 warning.");

                    return new XmlValidationResult(Model.XmlValidationStatus.Warning, String.Format("Validation complete with {0} warnings.", _warningCount));
                }

                if (_errorCount == 1)
                {
                    if (_warningCount == 0)
                        return new XmlValidationResult(Model.XmlValidationStatus.Error, "There was 1 error detected.");

                    if (_warningCount == 1)
                        return new XmlValidationResult(Model.XmlValidationStatus.Error, "There is 1 error and 1 warning detected.");

                    return new XmlValidationResult(Model.XmlValidationStatus.Error, String.Format("There was 1 error and {0} warnings detected.", _warningCount));
                }

                if (_warningCount == 0)
                    return new XmlValidationResult(Model.XmlValidationStatus.Error, String.Format("There were {0} errors detected.", _errorCount));

                if (_warningCount == 1)
                    return new XmlValidationResult(Model.XmlValidationStatus.Error, String.Format("There were {0} errors and 1 warning detected.", _errorCount));

                return new XmlValidationResult(Model.XmlValidationStatus.Error, String.Format("There was {0} errors and {1} warnings detected.", _errorCount, _warningCount));
            }
            catch (XmlSchemaException exception)
            {
                _mainWindowVM.Dispatcher.Invoke(() => _mainWindowVM.AddValidationError(exception.LineNumber, exception.LinePosition, exception.Message, exception, Model.XmlValidationStatus.Critical));
                return new XmlValidationResult(Model.XmlValidationStatus.Critical, String.Format("Line {0}, Column {1} - A fatal error has occurred: {2}", exception.LineNumber, exception.LinePosition, exception.Message));
            }
            catch (XmlException exception)
            {
                _mainWindowVM.Dispatcher.Invoke(() => _mainWindowVM.AddValidationError(exception.LineNumber, exception.LinePosition, exception.Message, exception, Model.XmlValidationStatus.Critical));
                return new XmlValidationResult(Model.XmlValidationStatus.Critical, String.Format("Line {0}, Column {1} - A fatal error has occurred: {2}", exception.LineNumber, exception.LinePosition, exception.Message));
            }
            catch (Exception exception)
            {
                _mainWindowVM.Dispatcher.Invoke(() => _mainWindowVM.AddValidationError(0, 0, exception.Message, exception, Model.XmlValidationStatus.Critical));
                return new XmlValidationResult(Model.XmlValidationStatus.Critical, String.Format("An unexpected fatal error has occurred: {0}", exception.Message));
            }
        }

        //public void GetStatus(object state, GetXmlValidationStatusHandler onValidateXmlComplete)
        //{
        //    lock (_syncRoot)
        //    {
        //        if (_validateXml.IsCompleted)
        //            Task.Factory.StartNew(o =>
        //            {
        //                object[] args = o as object[];
        //                GetXmlValidationStatusHandler h = args[0] as GetXmlValidationStatusHandler;
        //                h(args[1], args[2] as string, args[3] as Task<XmlValidationResult>);
        //            }, new object[] { onValidateXmlComplete, state, _text, _validateXml });
        //        else
        //            _onValidateXmlComplete.Add(new Tuple<object, GetXmlValidationStatusHandler>(state, onValidateXmlComplete));
        //    }
        //}

        //private void ValidateXmlCompleted(Task<XmlValidationResult> task)
        //{
        //    lock (_syncRoot)
        //    {
        //        if (task.Id != _validateXml.Id || task.IsCanceled)
        //            return;

        //        foreach (Tuple<object, GetXmlValidationStatusHandler> handler in _onValidateXmlComplete)
        //        {
        //            task.ContinueWith((t, o) =>
        //            {
        //                object[] args = o as object[];
        //                Tuple<object, GetXmlValidationStatusHandler> h = args[0] as Tuple<object, GetXmlValidationStatusHandler>;
        //                GetXmlValidationStatusHandler onValidateXmlComplete = h.Item2;
        //                onValidateXmlComplete(h.Item1, args[1] as string, t);
        //            }, new object[] { handler, _text });
        //        }
        //        _onValidateXmlComplete.Clear();
        //    }
        //}

        //public void SetText(string text, ViewModel.MainWindowVM vm, TextLineBackgroundParser linesParser)
        //{
        //    lock (_syncRoot)
        //    {
        //        if (_text == text)
        //            return;
        //        _mainWindowVM = vm;
        //        _linesParser = linesParser;
        //        linesParser.Text = text;
        //        _text = text;
        //        if (_validateXml.IsCompleted)
        //        {
        //            try { _tokenSource.Dispose(); }
        //            catch { }
        //        }
        //        else
        //        {
        //            Task.Factory.StartNew(o =>
        //            {
        //                object[] args = o as object[];
        //                CancellationTokenSource cts = args[0] as CancellationTokenSource;
        //                if (!cts.IsCancellationRequested)
        //                    cts.Cancel();
        //                Task<XmlValidationResult> t = args[1] as Task<XmlValidationResult>;
        //                try
        //                {
        //                    if (!t.IsCompleted)
        //                        t.Wait(1000);
        //                }
        //                finally { cts.Dispose(); }
        //            }, new object[] { _tokenSource, _validateXml });
        //        }
        //        _tokenSource = new CancellationTokenSource();
        //        _validateXml = Task<XmlValidationResult>.Factory.StartNew(ValidateXml, _tokenSource.Token);
        //        _validateXml.ContinueWith(ValidateXmlCompleted);
        //    }
        //}

        //public XmlValidationResult ValidateXml()
        //{
        //    if (String.IsNullOrWhiteSpace(_linesParser.Text))
        //    {
        //        _mainWindowVM.Dispatcher.Invoke(() =>
        //        {
        //            _mainWindowVM.ClearValidationErrors();
        //            _mainWindowVM.AddValidationError(1, 1, "XML text is empty", null, Model.XmlValidationStatus.Warning);
        //        });
        //        return new XmlValidationResult(Model.XmlValidationStatus.Warning, "XML text is empty", null);
        //    }

        //    XmlDocument xmlDocument = new XmlDocument();

        //    try
        //    {
        //        XmlReaderSettings settings = new XmlReaderSettings
        //        {
        //            CheckCharacters = false,
        //            DtdProcessing = DtdProcessing.Ignore,
        //            Schemas = new XmlSchemaSet(),
        //            ValidationType = ValidationType.Schema
        //        };
        //        settings.Schemas.Add(Markup.SsmlSchemaNamespaceURI, App.AppSettingsViewModel.SsmlSchemaCoreFileName);
        //        settings.Schemas.Add(Markup.SsmlSchemaNamespaceURI, App.AppSettingsViewModel.SsmlSchemaFileName);
        //        settings.Schemas.ValidationEventHandler += Settings_ValidationEventHandler;
        //        settings.ValidationEventHandler += Settings_ValidationEventHandler;
        //        using (StringReader stringReader = new StringReader(_linesParser.Text))
        //        {
        //            using (XmlReader xmlreader = XmlReader.Create(stringReader, settings))
        //                xmlDocument.Load(xmlreader);
        //        }

        //        return new XmlValidationResult(Model.XmlValidationStatus.None, "Validation completed.", xmlDocument);
        //    }
        //    catch (XmlSchemaException exception)
        //    {
        //        _mainWindowVM.Dispatcher.Invoke(() => _mainWindowVM.AddValidationError(exception.LineNumber, exception.LinePosition, exception.Message, exception, Model.XmlValidationStatus.Critical));
        //        return new XmlValidationResult(Model.XmlValidationStatus.Critical, String.Format("Line {0}, Column {1} - A fatal error has occurred: {2}", exception.LineNumber, exception.LinePosition, exception.Message), xmlDocument);
        //    }
        //    catch (XmlException exception)
        //    {
        //        _mainWindowVM.Dispatcher.Invoke(() => _mainWindowVM.AddValidationError(exception.LineNumber, exception.LinePosition, exception.Message, exception, Model.XmlValidationStatus.Critical));
        //        return new XmlValidationResult(Model.XmlValidationStatus.Critical, String.Format("Line {0}, Column {1} - A fatal error has occurred: {2}", exception.LineNumber, exception.LinePosition, exception.Message), xmlDocument);
        //    }
        //    catch (Exception exception)
        //    {
        //        _mainWindowVM.Dispatcher.Invoke(() => _mainWindowVM.AddValidationError(0, 0, exception.Message, exception, Model.XmlValidationStatus.Critical));
        //        return new XmlValidationResult(Model.XmlValidationStatus.Critical, String.Format("An unexpected fatal error has occurred: {0}", exception.Message), xmlDocument);
        //    }
        //}

        private void Settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
                _warningCount++;
            else
                _errorCount++;

            _mainWindowVM.Dispatcher.Invoke(() => _mainWindowVM.AddValidationError(e.Exception.LineNumber, e.Exception.LinePosition, e.Message, e.Exception, (e.Severity == XmlSeverityType.Warning) ? Model.XmlValidationStatus.Warning :Model. XmlValidationStatus.Error));
        }
    }
}
