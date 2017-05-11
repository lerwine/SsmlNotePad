using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class XmlValidator
    {
        private Model.TextLine[] _lineInfo;
        private Model.Workers.TextChangeArgs _args;
        private List<Model.ValidationError> _errors = new List<Model.ValidationError>();

        public Model.ValidationError[] GetResult()
        {
            //if (_args.Token.IsCancellationRequested)
            //    return new Model.ValidationError[0];

            if (String.IsNullOrWhiteSpace(_args.SourceText))
                return new Model.ValidationError[]
                {
                    new Model.ValidationError(1, 1, ViewModel.XmlValidationMessageVM.ValidationMessage_NoXmlData, null, Model.XmlValidationStatus.Warning)
                };

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
                using (StringReader stringReader = new StringReader(_args.SourceText))
                {
                    using (XmlReader xmlreader = XmlReader.Create(stringReader, settings))
                    {
                        while (xmlreader.Read())
                        {
                            //if (_args.Token.IsCancellationRequested)
                            //    break;
                        }
                    }
                }
            }
            catch (XmlSchemaException exception)
            {
                _errors.Add(new Model.ValidationError(exception));
            }
            catch (XmlException exception)
            {
                _errors.Add(new Model.ValidationError(exception));
            }
            catch (Exception exception)
            {
                _errors.Add(new Model.ValidationError(exception));
            }

            //if (_args.Token.IsCancellationRequested)
            //    return new Model.ValidationError[0];

            return _errors.ToArray();
        }

        private void Xml_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            _errors.Add(new Model.ValidationError(e));
        }

        public XmlValidator(Model.TextLine[] lineInfo, Model.Workers.TextChangeArgs args)
        {
            _lineInfo = lineInfo;
            _args = args;
        }
        
        public static Task<Model.ValidationError[]> ValidateAsync(Task<Model.TextLine[]> parseLinesTask, Model.Workers.TextChangeArgs args)
        {
            //return parseLinesTask.ContinueWith(Validate, args, args.Token);
            return parseLinesTask.ContinueWith(Validate, args);
        }

        private static Model.ValidationError[] Validate(Task<Model.TextLine[]> parseLinesTask, object args)
        {
            Model.Workers.TextChangeArgs textChangArgs = args as Model.Workers.TextChangeArgs;

#if DEBUG
            //System.Diagnostics.Debug.WriteLine("{0} GetLines: textChangArgs.Token.IsCancellationRequested = {1}; parseLinesTask.IsCanceled = {2}; parseLinesTask.IsFaulted = {3}", Task.CurrentId, textChangArgs.Token.IsCancellationRequested, parseLinesTask.IsCanceled, parseLinesTask.IsFaulted);
            System.Diagnostics.Debug.WriteLine("{0} GetLines: parseLinesTask.IsCanceled = {1}; parseLinesTask.IsFaulted = {2}", Task.CurrentId, parseLinesTask.IsCanceled, parseLinesTask.IsFaulted);
#endif
            //if (textChangArgs.Token.IsCancellationRequested || parseLinesTask.IsFaulted || parseLinesTask.IsCanceled)
            if (parseLinesTask.IsFaulted || parseLinesTask.IsCanceled)
                    return new Model.ValidationError[0];
            XmlValidator validator = new XmlValidator(parseLinesTask.Result, textChangArgs);
            return validator.GetResult();
        }
    }
}
