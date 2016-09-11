using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class MarkupValidator : BackgroundJobWorker<MarkupValidator, ViewModel.XmlValidationStatus>
    {
        private ViewModel.MainWindowVM _viewModel;
        private ObservableCollection<ViewModel.XmlValidationMessageVM> _validationMessageCollection;
        private ViewModel.XmlValidationStatus _result = ViewModel.XmlValidationStatus.None;
        private string _sourceText;

        public MarkupValidator(ViewModel.MainWindowVM viewModel, string sourceText, ObservableCollection<ViewModel.XmlValidationMessageVM> validationMessageCollection)
        {
            _viewModel = viewModel;
            _validationMessageCollection = validationMessageCollection;
            _sourceText = sourceText;
        }

        public override ViewModel.XmlValidationStatus FromActive() { return _result; }
        public override ViewModel.XmlValidationStatus FromCanceled() { return ViewModel.XmlValidationStatus.Warning; }
        public override ViewModel.XmlValidationStatus FromFault(AggregateException exception) { return ViewModel.XmlValidationStatus.Critical; }

        private static string _baseUri = null;
        public static string BaseUri
        {
            get
            {
                if (_baseUri == null)
                    _baseUri = Common.XmlHelper.GetBaseUri();

                return _baseUri;
            }
        }
        private static XmlSchemaSet _schemaSet = null;
        private static XmlSchemaSet SchemaSet
        {
            get
            {
                if (_schemaSet == null)
                    _schemaSet = Common.XmlHelper.CreateSsmlSchemaSet();

                return _schemaSet;
            }
        }
        protected override ViewModel.XmlValidationStatus Run(MarkupValidator previousWorker)
        {
            if (Token.IsCancellationRequested)
                return ViewModel.XmlValidationStatus.None;
            
            if (String.IsNullOrEmpty(_sourceText))
            {
                return CheckGet(_viewModel, () =>
                {
                    if (Token.IsCancellationRequested)
                        return _viewModel.ValidationStatus;
                    _validationMessageCollection.Clear();
                    _validationMessageCollection.Add(new ViewModel.XmlValidationMessageVM("Document is empty.", ViewModel.XmlValidationStatus.Warning));
                    return ViewModel.XmlValidationStatus.Warning;
                });
            }
            
            if (Token.IsCancellationRequested)
                return ViewModel.XmlValidationStatus.None;

            CheckInvoke(_viewModel, () => _validationMessageCollection.Clear());

            try
            {
                using (StringReader stringReader = new StringReader(_sourceText))
                {
                    lock (SchemaSet)
                    {
                        if (Token.IsCancellationRequested)
                            return ViewModel.XmlValidationStatus.None;
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            CheckCharacters = false,
                            Schemas = SchemaSet,
                            DtdProcessing = DtdProcessing.Ignore,
                            ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes | XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings,
                            ValidationType = ValidationType.Schema
                        };
                        settings.ValidationEventHandler += Schema_ValidationEventHandler;
                        SchemaSet.ValidationEventHandler += Schema_ValidationEventHandler;
                        try
                        {
                            using (XmlReader xmlReader = XmlReader.Create(stringReader, settings, BaseUri))
                            {
                                while (xmlReader.Read())
                                {
                                    if (Token.IsCancellationRequested)
                                        break;
                                }
                            }
                        }
                         catch { throw; }
                        finally { SchemaSet.ValidationEventHandler -= Schema_ValidationEventHandler; }
                    }
                }
            }
            catch (XmlSchemaException exception)
            {
                _result = CheckGet(_viewModel, () =>
                {
                    if (!Token.IsCancellationRequested)
                        _validationMessageCollection.Add(new ViewModel.XmlValidationMessageVM(String.Format("Unexpected XML Schema Exception: {0}", exception.Message),
                                exception, new StringLines(_sourceText)));

                    return ViewModel.XmlValidationStatus.Critical;
                });
            }
            catch (XmlException exception)
            {
                _result = CheckGet(_viewModel, () =>
                {
                    if (!Token.IsCancellationRequested)
                        _validationMessageCollection.Add(new ViewModel.XmlValidationMessageVM(String.Format("Unexpected XML Exception: {0}", exception.Message),
                                exception, new StringLines(_sourceText)));

                    return ViewModel.XmlValidationStatus.Critical;
                });
            }
            catch (IOException exception)
            {
                _result = CheckGet(_viewModel, () =>
                {
                    if (!Token.IsCancellationRequested)
                        _validationMessageCollection.Add(new ViewModel.XmlValidationMessageVM(String.Format("Unexpected I/O Exception: {0}", exception.Message), exception));
                    return ViewModel.XmlValidationStatus.Critical;
                });
            }
            catch (Exception exception)
            {
                _result = CheckGet(_viewModel, () =>
                {
                    if (!Token.IsCancellationRequested)
                        _validationMessageCollection.Add(new ViewModel.XmlValidationMessageVM(String.Format("Unexpected Exception: {0}", exception.Message), exception));
                    return ViewModel.XmlValidationStatus.Critical;
                });
            }

            return _result;
        }
        
        private void Schema_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            CheckInvoke(_viewModel, () =>
            {
                if (Token.IsCancellationRequested)
                    return;
                _validationMessageCollection.Add(new ViewModel.XmlValidationMessageVM(e.Message, e.Exception, e.Severity, new StringLines(_sourceText)));
                if (e.Severity == XmlSeverityType.Warning)
                {
                    if (_result != ViewModel.XmlValidationStatus.Error && _result != ViewModel.XmlValidationStatus.Critical)
                        _result = ViewModel.XmlValidationStatus.Warning;
                }
                else if (_result != ViewModel.XmlValidationStatus.Critical)
                    _result = ViewModel.XmlValidationStatus.Error;
            });
        }
    }
}
