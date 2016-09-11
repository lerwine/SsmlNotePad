using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class MainWindowVM : DependencyObject
    {
        private object _syncRoot = new object();
        private bool _lineNumbersUpdated = false;
        private Process.BackgroundJobManager<Process.LineNumberGenerator, int> _lineNumberUpdater;
        private Process.BackgroundJobManager<Process.MarkupValidator, XmlValidationStatus> _markupValidator = null;

        public MainWindowVM()
        {
            LineNumbers = new ReadOnlyObservableViewModelCollection<LineNumberVM>(_lineNumbers);
            ValidationMessages = new ReadOnlyObservableViewModelCollection<XmlValidationMessageVM>(_validationMessages);
            SpeechGenerationStatus = new SpeechGenerationStatusVM();
            _validationMessages.CollectionChanged += ValidationMessages_CollectionChanged;
            ApplyNewDocument();
        }

        internal void InvalidateLineNumbers()
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
                return;

            lock (syncRoot)
                _lineNumbersUpdated = false;
        }

        internal void ValidateDocument(string text)
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
                return;

            lock (syncRoot)
            {
                if (_markupValidator == null)
                    _markupValidator = new Process.BackgroundJobManager<Process.MarkupValidator, XmlValidationStatus>(new Process.MarkupValidator(this, text, _validationMessages));
                else
                    _markupValidator.Replace(new Process.MarkupValidator(this, text, _validationMessages));
            }
        }

        #region Commands

        #region NewDocument Command Property Members

        private Command.RelayCommand _newDocumentCommand = null;

        public Command.RelayCommand NewDocumentCommand
        {
            get
            {
                if (_newDocumentCommand == null)
                    _newDocumentCommand = new Command.RelayCommand(OnNewDocument);

                return _newDocumentCommand;
            }
        }

        protected virtual void OnNewDocument(object parameter)
        {
            if (VerifyCanReplaceCurrentFile())
                ApplyNewDocument();
        }

        private void ApplyNewDocument()
        {
            _validationMessages.Clear();
            ValidationToolTip = "";
            FileSaveStatus = FileSaveStatus.New;
            FileModified = false;
            FileSaveToolBarMessage = "";
            FileSaveDetailMessage = "";

            Text = Markup.BlankSsmlDocument;
            if (_markupValidator != null)
                _markupValidator.Wait();
            if (FileSaveStatus == FileSaveStatus.Modified)
                FileSaveStatus = FileSaveStatus.New;
            if (FileSaveStatus == FileSaveStatus.New)
            {
                FileSaveToolBarMessage = "";
                FileSaveDetailMessage = "";
            }

            FileModified = false;
        }

        #endregion

        #region OpenDocument Command Property Members

        private Command.RelayCommand _openDocumentCommand = null;

        public Command.RelayCommand OpenDocumentCommand
        {
            get
            {
                if (_openDocumentCommand == null)
                    _openDocumentCommand = new Command.RelayCommand(OnOpenDocument);

                return _openDocumentCommand;
            }
        }

        public string GetValidationMessageSummary()
        {
            if (!_validationMessages.Any(m => m.ValidationStatus == XmlValidationStatus.Critical || m.ValidationStatus == XmlValidationStatus.Error))
                return null;

            var counts = _validationMessages.Aggregate(new { C = 0, E = 0, W = 0 }, (a, m) =>
            {
                switch (m.ValidationStatus)
                {
                    case XmlValidationStatus.Critical:
                        return new { C = a.C + 1, E = a.E, W = a.W };
                    case XmlValidationStatus.Error:
                        return new { C = a.C, E = a.E + 1, W = a.W };
                    case XmlValidationStatus.Warning:
                        return new { C = a.C, E = a.E, W = a.W + 1 };
                }
                return a;
            });
            StringBuilder message = new StringBuilder("There ");
            Action<int, string> addCountVerbiage = (c, n) =>
            {
                message.Append((counts.E == 1) ? "is " : "are ");
                message.Append(counts.E);
                message.Append(" ");
                message.Append(n);
                if (c != 1)
                    message.Append("s");
            };

            if (counts.W == 0)
            {
                if (counts.C == 0)
                    addCountVerbiage(counts.E, "error");
                else
                {
                    addCountVerbiage(counts.C, "critical error");
                    if (counts.E != 0)
                    {
                        message.Append(" and ");
                        addCountVerbiage(counts.C, "other error");
                    }
                }
            }
            else
            {
                if (counts.C == 0)
                {
                    addCountVerbiage(counts.E, "error");
                    message.Append(" and ");
                }
                else
                {
                    addCountVerbiage(counts.C, "critical error");
                    if (counts.E == 0)
                        message.Append(" and ");
                    else
                    {
                        addCountVerbiage(counts.C, "other error");
                        message.Append(", as well as ");
                    }
                }
                addCountVerbiage(counts.W, "warning");
            }

            message.Append("in this document.");
            return message.ToString();
        }

        protected virtual void OnOpenDocument(object parameter)
        {
            if (!VerifyCanReplaceCurrentFile())
                return;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Title = "Open SSML Document";
            if (!Common.FileUtility.InvokeSsmlFileDialog(dialog, App.Current.MainWindow, FileSaveLocation))
                return;

            try
            {
                Text = File.ReadAllText(dialog.FileName);
                FileSaveStatus = FileSaveStatus.SaveSuccess;
                FileSaveToolBarMessage = Path.GetFileName(dialog.FileName);
                FileSaveDetailMessage = String.Format("File loaded from {0}", dialog.FileName);
                FileModified = false;
                FileSaveLocation = dialog.FileName;
            }
            catch (Exception exception)
            {
                FileSaveStatus = FileSaveStatus.SaveError;
                FileSaveToolBarMessage = "Error loading file.";
                FileSaveDetailMessage = String.Format("Error loading from {0}: {1}", dialog.FileName, exception.Message);
            }
        }

        #endregion

        #region SaveDocument Command Property Members

        private Command.RelayCommand _saveDocumentCommand = null;

        public Command.RelayCommand SaveDocumentCommand
        {
            get
            {
                if (_saveDocumentCommand == null)
                    _saveDocumentCommand = new Command.RelayCommand(OnSaveDocument);

                return _saveDocumentCommand;
            }
        }

        protected virtual void OnSaveDocument(object parameter) { Save(parameter); }

        #endregion

        #region SaveAs Command Property Members

        private Command.RelayCommand _saveAsCommand = null;

        public Command.RelayCommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                    _saveAsCommand = new Command.RelayCommand(OnSaveAs);

                return _saveAsCommand;
            }
        }

        protected virtual void OnSaveAs(object parameter) { SaveAs(parameter); }

        #endregion

        #region Exit Methods

        internal void OnClosing(MainWindow mainWindow, CancelEventArgs e)
        {
            e.Cancel = !VerifyCanReplaceCurrentFile();
        }

        internal void OnClosed(EventArgs e)
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
                return;

            Process.BackgroundJobManager<Process.LineNumberGenerator, int> lineNumberUpdater;
            Process.BackgroundJobManager<Process.MarkupValidator, XmlValidationStatus> markupValidator;

            lock (syncRoot)
            {
                _syncRoot = null;
                lineNumberUpdater = _lineNumberUpdater;
                _lineNumberUpdater = null;
                markupValidator = _markupValidator;
                _markupValidator = null;
            }

            if (lineNumberUpdater != null)
            {
                try
                {
                    if (!lineNumberUpdater.IsCompleted)
                    {
                        lineNumberUpdater.Cancel();
                        lineNumberUpdater.Wait(5000);
                    }
                }
                catch { }
            }

            if (markupValidator != null)
            {
                try
                {
                    if (!markupValidator.IsCompleted)
                    {
                        markupValidator.Cancel();
                        markupValidator.Wait(5000);
                    }
                }
                catch { }
            }
        }

        #endregion

        #region PasteEncoded Command Property Members

        private Command.RelayCommand _pasteEncodedCommand = null;

        public Command.RelayCommand PasteEncodedCommand
        {
            get
            {
                if (_pasteEncodedCommand == null)
                    _pasteEncodedCommand = new Command.RelayCommand(OnPasteEncoded);

                return _pasteEncodedCommand;
            }
        }

        protected virtual void OnPasteEncoded(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            if (Clipboard.ContainsText())
            {
                textBox.SelectedText = Common.XmlHelper.XmlEncode(Clipboard.GetText());
                textBox.UpdateLayout();
                textBox.Focus();
            }
        }

        #endregion

        #region ReformatDocument Command Property Members

        private Command.RelayCommand _reformatDocumentCommand = null;

        public Command.RelayCommand ReformatDocumentCommand
        {
            get
            {
                if (_reformatDocumentCommand == null)
                    _reformatDocumentCommand = new Command.RelayCommand(OnReformatDocument);

                return _reformatDocumentCommand;
            }
        }

        protected virtual void OnReformatDocument(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            string errorMessage;
            Text = Common.XmlHelper.ReformatDocument(Text, out errorMessage);
            textBox.UpdateLayout();
            if (errorMessage != null)
                MessageBox.Show(App.Current.MainWindow, errorMessage, "Format Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        #endregion

        #region SelectCurrentTag Command Property Members

        private Command.RelayCommand _selectCurrentTagCommand = null;

        public Command.RelayCommand SelectCurrentTagCommand
        {
            get
            {
                if (_selectCurrentTagCommand == null)
                    _selectCurrentTagCommand = new Command.RelayCommand(OnSelectCurrentTag);

                return _selectCurrentTagCommand;
            }
        }

        protected virtual void OnSelectCurrentTag(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            MessageBox.Show("SelectCurrentTag Command not implemented", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion

        #region SelectTagContents Command Property Members

        private Command.RelayCommand _selectTagContentsCommand = null;

        public Command.RelayCommand SelectTagContentsCommand
        {
            get
            {
                if (_selectTagContentsCommand == null)
                    _selectTagContentsCommand = new Command.RelayCommand(OnSelectTagContents);

                return _selectTagContentsCommand;
            }
        }

        protected virtual void OnSelectTagContents(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            MessageBox.Show("SelectTagContents Command not implemented", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion

        #region GoToLine Command Property Members

        private Command.RelayCommand _goToLineCommand = null;

        public Command.RelayCommand GoToLineCommand
        {
            get
            {
                if (_goToLineCommand == null)
                    _goToLineCommand = new Command.RelayCommand(OnGoToLine);

                return _goToLineCommand;
            }
        }

        protected virtual void OnGoToLine(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            int? lineNumber = textBox.GetLineIndexFromCharacterIndex(textBox.SelectionStart) + 1;
            if (lineNumber < 1)
                lineNumber = null;
            lineNumber = View.GoToLineWindow.GetGotoLine(lineNumber);
            if (lineNumber.HasValue)
            {
                textBox.SelectionLength = 0;
                textBox.SelectionStart = textBox.GetCharacterIndexFromLineIndex(lineNumber.Value - 1);
            }
        }

        #endregion

        #region FindText Command Property Members

        private Command.RelayCommand _findTextCommand = null;

        public Command.RelayCommand FindTextCommand
        {
            get
            {
                if (_findTextCommand == null)
                    _findTextCommand = new Command.RelayCommand(OnFindText);

                return _findTextCommand;
            }
        }

        protected virtual void OnFindText(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            MessageBox.Show("FindText Command not implemented", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion

        #region ReplaceText Command Property Members

        private Command.RelayCommand _replaceTextCommand = null;

        public Command.RelayCommand ReplaceTextCommand
        {
            get
            {
                if (this._replaceTextCommand == null)
                    this._replaceTextCommand = new Command.RelayCommand(this.OnReplaceText);

                return this._replaceTextCommand;
            }
        }

        protected virtual void OnReplaceText(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            MessageBox.Show("ReplaceText Command not implemented", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion

        #region ContextProperties Command Property Members

        private Command.RelayCommand _contextPropertiesCommand = null;

        public Command.RelayCommand ContextPropertiesCommand
        {
            get
            {
                if (this._contextPropertiesCommand == null)
                    this._contextPropertiesCommand = new Command.RelayCommand(this.OnContextProperties);

                return this._contextPropertiesCommand;
            }
        }

        protected virtual void OnContextProperties(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            MessageBox.Show("ContextProperties Command not implemented", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion

        #region InsertParagraph Command Property Members

        private Command.RelayCommand _insertParagraphCommand = null;

        public Command.RelayCommand InsertParagraphCommand
        {
            get
            {
                if (this._insertParagraphCommand == null)
                    this._insertParagraphCommand = new Command.RelayCommand(this.OnInsertParagraph);

                return this._insertParagraphCommand;
            }
        }

        protected virtual void OnInsertParagraph(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            textBox.SelectedText = String.Format("<p>{0}</p>", textBox.SelectedText);
            if (!SelectAfterInsert)
                textBox.SelectionLength = 0;
            else
                textBox.SelectionLength -= 7;
            textBox.SelectionStart += 3;
            textBox.Focus();
        }

        #endregion

        #region InsertSentence Command Property Members

        private Command.RelayCommand _insertSentenceCommand = null;

        public Command.RelayCommand InsertSentenceCommand
        {
            get
            {
                if (this._insertSentenceCommand == null)
                    this._insertSentenceCommand = new Command.RelayCommand(this.OnInsertSentence);

                return this._insertSentenceCommand;
            }
        }

        protected virtual void OnInsertSentence(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            textBox.SelectedText = String.Format("<s>{0}</s>", textBox.SelectedText);
            if (!SelectAfterInsert)
                textBox.SelectionLength = 0;
            else
                textBox.SelectionLength -= 7;
            textBox.SelectionStart += 3;
            textBox.Focus();
        }

        #endregion

        #region Substitution Command Property Members

        private Command.RelayCommand _substitutionCommand = null;

        public Command.RelayCommand SubstitutionCommand
        {
            get
            {
                if (this._substitutionCommand == null)
                    this._substitutionCommand = new Command.RelayCommand(OnSubstitution);

                return this._substitutionCommand;
            }
        }

        protected virtual void OnSubstitution(object parameter)
        {
            TextBox textBox = parameter as TextBox;
            
            int selectionOffset;
            string text, leadingWs, trailingWs;
            text = textBox.SelectedText.ExtractOuterWhitespace(out leadingWs, out trailingWs);
            if (SubstitutionVM.TryGetSubstitution(text, App.GetWindowByDataContext<MainWindow, MainWindowVM>(this), out text, out selectionOffset))
            {
                textBox.SelectedText = leadingWs + text + trailingWs;
                textBox.SelectionStart += (leadingWs.Length + selectionOffset);
                textBox.SelectionLength = 0;
                textBox.Focus();
            }
        }

        #endregion
        
        #region SpellOut Command Property Members

        private Command.RelayCommand _spellOutCommand = null;

        public Command.RelayCommand SpellOutCommand
        {
            get
            {
                if (this._spellOutCommand == null)
                    this._spellOutCommand = new Command.RelayCommand(OnSpellOut);

                return this._spellOutCommand;
            }
        }

        protected virtual void OnSpellOut(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            string text, leadingWs, trailingWs;
            text = textBox.SelectedText.ExtractOuterWhitespace(out leadingWs, out trailingWs);
            textBox.SelectedText = leadingWs + String.Format("<say-as interpret-as=\"characters\">{0}</say-as>", text) + trailingWs;
            if (!SelectAfterInsert)
                textBox.SelectionLength = 0;
            else
                textBox.SelectionLength -= 43;
            textBox.SelectionStart += (leadingWs.Length + 34);
            textBox.Focus();
        }

        #endregion

        #region SayAs Command Property Members

        private Command.RelayCommand _sayAsCommand = null;

        public Command.RelayCommand SayAsCommand
        {
            get
            {
                if (this._sayAsCommand == null)
                    this._sayAsCommand = new Command.RelayCommand(OnSayAs);

                return this._sayAsCommand;
            }
        }

        protected virtual void OnSayAs(object parameter)
        {
            TextBox textBox = parameter as TextBox;
            int selectionOffset;
            string text, leadingWs, trailingWs;
            text = textBox.SelectedText.ExtractOuterWhitespace(out leadingWs, out trailingWs);
            if (SayAsVM.TryGetSayAs(text, App.GetWindowByDataContext<MainWindow, MainWindowVM>(this), out text, out selectionOffset))
            {
                textBox.SelectedText = leadingWs + text + trailingWs;
                textBox.SelectionStart += (leadingWs.Length + selectionOffset);
                textBox.SelectionLength = 0;
                textBox.Focus();
            }
        }

        #endregion

        #region InsertFemaleVoice Command Property Members

        private Command.RelayCommand _insertFemaleVoiceCommand = null;

        public Command.RelayCommand InsertFemaleVoiceCommand
        {
            get
            {
                if (this._insertFemaleVoiceCommand == null)
                    this._insertFemaleVoiceCommand = new Command.RelayCommand(OnInsertFemaleVoice);

                return this._insertFemaleVoiceCommand;
            }
        }

        protected virtual void OnInsertFemaleVoice(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            string text, leadingWs, trailingWs;
            text = textBox.SelectedText.ExtractOuterWhitespace(out leadingWs, out trailingWs);
            textBox.SelectedText = leadingWs + String.Format("<voice gender=\"female\">{0}</voice>", text) + trailingWs;
            if (!SelectAfterInsert)
                textBox.SelectionLength = 0;
            else
                textBox.SelectionLength -= 31;
            textBox.SelectionStart += (23 + leadingWs.Length);
            textBox.Focus();
        }

        #endregion

        #region InsertMaleVoice Command Property Members

        private Command.RelayCommand _insertMaleVoiceCommand = null;

        public Command.RelayCommand InsertMaleVoiceCommand
        {
            get
            {
                if (this._insertMaleVoiceCommand == null)
                    this._insertMaleVoiceCommand = new Command.RelayCommand(OnInsertMaleVoice);

                return this._insertMaleVoiceCommand;
            }
        }

        protected virtual void OnInsertMaleVoice(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            string text, leadingWs, trailingWs;
            text = textBox.SelectedText.ExtractOuterWhitespace(out leadingWs, out trailingWs);
            textBox.SelectedText = leadingWs + String.Format("<voice gender=\"male\">{0}</voice>", text) + trailingWs;
            if (!SelectAfterInsert)
                textBox.SelectionLength = 0;
            else
                textBox.SelectionLength -= 29;
            textBox.SelectionStart += (21 + leadingWs.Length);
            textBox.Focus();
        }

        #endregion

        #region InsertGenderNeutralVoice Command Property Members

        private Command.RelayCommand _insertGenderNeutralVoiceCommand = null;

        public Command.RelayCommand InsertGenderNeutralVoiceCommand
        {
            get
            {
                if (this._insertGenderNeutralVoiceCommand == null)
                    this._insertGenderNeutralVoiceCommand = new Command.RelayCommand(OnInsertGenderNeutralVoice);

                return this._insertGenderNeutralVoiceCommand;
            }
        }

        protected virtual void OnInsertGenderNeutralVoice(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            string text, leadingWs, trailingWs;
            text = textBox.SelectedText.ExtractOuterWhitespace(out leadingWs, out trailingWs);
            textBox.SelectedText = leadingWs + String.Format("<voice gender=\"neutral\">{0}</voice>", text) + trailingWs;
            if (!SelectAfterInsert)
                textBox.SelectionLength = 0;
            else
                textBox.SelectionLength -= 32;
            textBox.SelectionStart += (24 + leadingWs.Length);
            textBox.Focus();
        }

        #endregion

        #region InsertBookmark Command Property Members

        private Command.RelayCommand _insertBookmarkCommand = null;

        public Command.RelayCommand InsertBookmarkCommand
        {
            get
            {
                if (this._insertBookmarkCommand == null)
                    this._insertBookmarkCommand = new Command.RelayCommand(OnInsertBookmark);

                return this._insertBookmarkCommand;
            }
        }

        protected virtual void OnInsertBookmark(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            string text, leadingWs, trailingWs;
            text = textBox.SelectedText.ExtractOuterWhitespace(out leadingWs, out trailingWs);
            textBox.SelectedText = leadingWs + String.Format("<mark name=\"{0}\" />", Common.XmlHelper.XmlEncode(Common.XmlHelper.XmlDecode(text), Common.XmlEncodeOption.DoubleQuotedAttribute)) + trailingWs;
            if (!SelectAfterInsert)
                textBox.SelectionLength = 0;
            else
                textBox.SelectionLength -= 16;
            textBox.SelectionStart += (12 + leadingWs.Length);
            textBox.Focus();
        }

        #endregion

        #region InsertAudioFile Command Property Members

        private Command.RelayCommand _insertAudioFileCommand = null;

        public Command.RelayCommand InsertAudioFileCommand
        {
            get
            {
                if (this._insertAudioFileCommand == null)
                    this._insertAudioFileCommand = new Command.RelayCommand(OnInsertAudioFile);

                return this._insertAudioFileCommand;
            }
        }

        protected virtual void OnInsertAudioFile(object parameter)
        {
            TextBox textBox = parameter as TextBox;
            int selectionOffset;
            string text, leadingWs, trailingWs;
            text = textBox.SelectedText.ExtractOuterWhitespace(out leadingWs, out trailingWs);
            if (AudioFileVM.TryGetAudioFile(text, App.GetWindowByDataContext<MainWindow, MainWindowVM>(this), out text, out selectionOffset))
            {
                textBox.SelectedText = leadingWs + text + trailingWs;
                textBox.SelectionStart += (leadingWs.Length + selectionOffset);
                textBox.SelectionLength = 0;
                textBox.Focus();
            }
        }

        #endregion

        #region Dictate Command Property Members

        private Command.RelayCommand _dictateCommand = null;

        public Command.RelayCommand DictateCommand
        {
            get
            {
                if (this._dictateCommand == null)
                    this._dictateCommand = new Command.RelayCommand(OnDictate);

                return this._dictateCommand;
            }
        }

        protected virtual void OnDictate(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            MessageBox.Show("Dictate Command not implemented", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion
        
        #region DefaultSynthSettings Command Property Members

        private Command.RelayCommand _defaultSynthSettingsCommand = null;

        public Command.RelayCommand DefaultSynthSettingsCommand
        {
            get
            {
                if (this._defaultSynthSettingsCommand == null)
                    this._defaultSynthSettingsCommand = new Command.RelayCommand(this.OnDefaultSynthSettings);

                return this._defaultSynthSettingsCommand;
            }
        }

        protected virtual void OnDefaultSynthSettings(object parameter)
        {
            View.DefaultSpeechSettingsWindow window = new View.DefaultSpeechSettingsWindow();
            Window owner = App.GetWindowByDataContext<MainWindow, MainWindowVM>(this);
            window.Owner = owner ?? App.Current.MainWindow;
            window.ShowDialog();
        }

        #endregion

        #region Help Command Property Members

        private Command.RelayCommand _helpCommand = null;

        public Command.RelayCommand HelpCommand
        {
            get
            {
                if (this._helpCommand == null)
                    this._helpCommand = new Command.RelayCommand(this.OnHelp);

                return this._helpCommand;
            }
        }

        protected virtual void OnHelp(object parameter)
        {
            MessageBox.Show("Help Command not implemented", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion

        #region AboutSsmlNotePad Command Property Members

        private Command.RelayCommand _aboutSsmlNotePadCommand = null;

        public Command.RelayCommand AboutSsmlNotePadCommand
        {
            get
            {
                if (this._aboutSsmlNotePadCommand == null)
                    this._aboutSsmlNotePadCommand = new Command.RelayCommand(this.OnAboutSsmlNotePad);

                return this._aboutSsmlNotePadCommand;
            }
        }

        protected virtual void OnAboutSsmlNotePad(object parameter)
        {
            MessageBox.Show("AboutSsmlNotePad Command not implemented", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion

        #region ShowValidationMessages Command Property Members

        private Command.RelayCommand _showValidationMessagesCommand = null;

        public Command.RelayCommand ShowValidationMessagesCommand
        {
            get
            {
                if (this._showValidationMessagesCommand == null)
                    this._showValidationMessagesCommand = new Command.RelayCommand(this.OnShowValidationMessages);

                return this._showValidationMessagesCommand;
            }
        }

        protected virtual void OnShowValidationMessages(object parameter)
        {
            View.ValidationMessagesWindow window = new View.ValidationMessagesWindow();
            window.ShowDialog();
        }

        #endregion

        #region ShowFileSaveMessages Command Property Members

        private Command.RelayCommand _showFileSaveMessagesCommand = null;

        public Command.RelayCommand ShowFileSaveMessagesCommand
        {
            get
            {
                if (this._showFileSaveMessagesCommand == null)
                    this._showFileSaveMessagesCommand = new Command.RelayCommand(this.OnShowFileSaveMessages);

                return this._showFileSaveMessagesCommand;
            }
        }

        protected virtual void OnShowFileSaveMessages(object parameter)
        {
            MessageBoxImage image;
            string heading;
            switch (FileSaveStatus)
            {
                case FileSaveStatus.Modified:
                    image = MessageBoxImage.Exclamation;
                    heading = "File Modifled";
                    break;
                case FileSaveStatus.New:
                    image = MessageBoxImage.Asterisk;
                    heading = "New file";
                    break;
                case FileSaveStatus.SaveError:
                    image = MessageBoxImage.Error;
                    heading = "File Save Error";
                    break;
                case FileSaveStatus.SaveWarning:
                    image = MessageBoxImage.Warning;
                    heading = "File Save Warning";
                    break;
                default:
                    image = MessageBoxImage.Information;
                    heading = "File successfully saved";
                    break;
            }
            MessageBox.Show(FileSaveDetailMessage, heading, MessageBoxButton.OK, image);
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region SpeechGenerationStatus Property Members

        public const string PropertyName_SpeechGenerationStatus = "SpeechGenerationStatus";

        private static readonly DependencyPropertyKey SpeechGenerationStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SpeechGenerationStatus, typeof(SpeechGenerationStatusVM), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SpeechGenerationStatus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeechGenerationStatusProperty = SpeechGenerationStatusPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public SpeechGenerationStatusVM SpeechGenerationStatus
        {
            get
            {
                if (CheckAccess())
                    return (SpeechGenerationStatusVM)(GetValue(SpeechGenerationStatusProperty));
                return Dispatcher.Invoke(() => SpeechGenerationStatus);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SpeechGenerationStatusPropertyKey, value);
                else
                    Dispatcher.Invoke(() => SpeechGenerationStatus = value);
            }
        }

        #endregion

        #region HostWindow Property Members

        public const string DependencyPropertyName_HostWindow = "HostWindow";

        /// <summary>
        /// Identifies the <seealso cref="HostWindow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HostWindowProperty = DependencyProperty.Register(DependencyPropertyName_HostWindow, typeof(MainWindow), typeof(MainWindowVM),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).HostWindow_PropertyChanged((MainWindow)(e.OldValue), (MainWindow)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).HostWindow_PropertyChanged((MainWindow)(e.OldValue), (MainWindow)(e.NewValue)));
                }));

        /// <summary>
        /// 
        /// </summary>
        public MainWindow HostWindow
        {
            get
            {
                if (CheckAccess())
                    return (MainWindow)(GetValue(HostWindowProperty));
                return Dispatcher.Invoke(() => HostWindow);
            }
            set
            {
                if (CheckAccess())
                    SetValue(HostWindowProperty, value);
                else
                    Dispatcher.Invoke(() => HostWindow = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="HostWindow"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="MainWindow"/> value before the <seealso cref="HostWindow"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="MainWindow"/> value after the <seealso cref="HostWindow"/> property was changed.</param>
        protected virtual void HostWindow_PropertyChanged(MainWindow oldValue, MainWindow newValue)
        {
            bool selectionCommandsEnabled = newValue != null;
            InsertAudioFileCommand.IsEnabled = selectionCommandsEnabled;
            InsertBookmarkCommand.IsEnabled = selectionCommandsEnabled;
            InsertFemaleVoiceCommand.IsEnabled = selectionCommandsEnabled;
            InsertGenderNeutralVoiceCommand.IsEnabled = selectionCommandsEnabled;
            InsertMaleVoiceCommand.IsEnabled = selectionCommandsEnabled;
            InsertMaleVoiceCommand.IsEnabled = selectionCommandsEnabled;
            InsertParagraphCommand.IsEnabled = selectionCommandsEnabled;
            InsertSentenceCommand.IsEnabled = selectionCommandsEnabled;
            SayAsCommand.IsEnabled = selectionCommandsEnabled;
            DictateCommand.IsEnabled = selectionCommandsEnabled;
            SpellOutCommand.IsEnabled = selectionCommandsEnabled;
            SubstitutionCommand.IsEnabled = selectionCommandsEnabled;
            ContextPropertiesCommand.IsEnabled = selectionCommandsEnabled;
            GoToLineCommand.IsEnabled = selectionCommandsEnabled;
            SelectCurrentTagCommand.IsEnabled = selectionCommandsEnabled;
            SelectTagContentsCommand.IsEnabled = selectionCommandsEnabled;
            PasteEncodedCommand.IsEnabled = selectionCommandsEnabled;
        }

        #endregion

        #region LineNumbers Property Members

        private ObservableViewModelCollection<LineNumberVM> _lineNumbers = new ObservableViewModelCollection<LineNumberVM>();

        public const string PropertyName_LineNumbers = "LineNumbers";

        private static readonly DependencyPropertyKey LineNumbersPropertyKey = 
            DependencyProperty.RegisterReadOnly(PropertyName_LineNumbers, 
                typeof(ReadOnlyObservableViewModelCollection<LineNumberVM>), typeof(MainWindowVM), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="LineNumbers"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineNumbersProperty = LineNumbersPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableViewModelCollection<LineNumberVM> LineNumbers
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableViewModelCollection<LineNumberVM>)(GetValue(LineNumbersProperty));
                return Dispatcher.Invoke(() => LineNumbers);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(LineNumbersPropertyKey, value);
                else
                    Dispatcher.Invoke(() => LineNumbers = value);
            }
        }

        #endregion

        #region ValidationMessages Property Members

        private ObservableViewModelCollection<XmlValidationMessageVM> _validationMessages = 
            new ObservableViewModelCollection<XmlValidationMessageVM>();

        public const string PropertyName_ValidationMessages = "ValidationMessages";

        private static readonly DependencyPropertyKey ValidationMessagesPropertyKey = 
            DependencyProperty.RegisterReadOnly(PropertyName_ValidationMessages, 
                typeof(ReadOnlyObservableViewModelCollection<XmlValidationMessageVM>), typeof(MainWindowVM), 
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="ValidationMessages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationMessagesProperty = ValidationMessagesPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableViewModelCollection<XmlValidationMessageVM> ValidationMessages
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableViewModelCollection<XmlValidationMessageVM>)(GetValue(ValidationMessagesProperty));
                return Dispatcher.Invoke(() => ValidationMessages);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ValidationMessagesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ValidationMessages = value);
            }
        }

        private void ValidationMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => ValidationMessages_CollectionChanged(sender, e));
                return;
            }

            string[] lines;
            var statusColl = _validationMessages.Select(m => new { Status = m.ValidationStatus, Message = m.Message }).ToArray();
            var errorMessages = statusColl.Where(s => s.Status == XmlValidationStatus.Critical)
                .Concat(statusColl.Where(s => s.Status == XmlValidationStatus.Error)).ToArray();
            var warningMessages = statusColl.Where(s => s.Status == XmlValidationStatus.Warning).ToArray();
            if (errorMessages.Length > 0)
            {
                var status = errorMessages.Where(m => m.Status == XmlValidationStatus.Critical)
                    .DefaultIfEmpty(errorMessages.First()).First();
                ValidationStatus = status.Status;
                lines = status.Message.SplitLines().ToArray();
                if (errorMessages.Length == 1)
                {
                    if (lines.Length == 1)
                        ValidationToolTip = lines[0];
                    else
                        ValidationToolTip = lines[0].TrimEnd() + " ... (more)";
                }
                else
                {
                    string toolTip = lines[0].TrimEnd() + " ... (more)";
                    if (status.Status == XmlValidationStatus.Critical)
                    {
                        int count = errorMessages.Count(m => m.Status == XmlValidationStatus.Error);
                        if (count == 0)
                        {
                            if (warningMessages.Length == 0)
                                ValidationToolTip = String.Format("{0} Critical: {1}", errorMessages.Length, toolTip);
                            else
                                ValidationToolTip = String.Format("{0} Critical, {1} Warnings: {2}", errorMessages.Length, toolTip);
                        }
                        else if (warningMessages.Length == 0)
                            ValidationToolTip = String.Format("{0} Critical, {1} Errors: {2}", errorMessages.Length - count, count,
                                toolTip);
                        else
                            ValidationToolTip = String.Format("{0} Critical, {1} Errors, {2} Warnings: {3}",
                                errorMessages.Length - count, count, warningMessages.Length, toolTip);
                    }
                    else if (warningMessages.Length == 0)
                        ValidationToolTip = String.Format("{0} Error: {1}", errorMessages.Length, toolTip);
                    else
                        ValidationToolTip = String.Format("{0} Errors, {1} Warnings: {2}", errorMessages.Length, toolTip);
                }
                return;
            }
            if (warningMessages.Length > 0)
            {
                lines = warningMessages.First().Message.SplitLines().ToArray();
                ValidationStatus = XmlValidationStatus.Warning;
                if (warningMessages.Length == 1)
                {
                    if (lines.Length == 1)
                        ValidationToolTip = lines[0];
                    else
                        ValidationToolTip = lines[0].TrimEnd() + " ... (more)";
                }
                else
                    ValidationToolTip = String.Format("{0} Warnings: {1}", lines.Length, lines[0].TrimEnd() + " ... (more)");
                return;
            }

            var messages = statusColl.Where(m => m.Status == XmlValidationStatus.Information).ToArray();
            if (messages.Length == 0)
            {
                ValidationStatus = XmlValidationStatus.None;
                messages = statusColl.Where(m => m.Status == XmlValidationStatus.Information).ToArray();
                if (messages.Length == 0)
                    ValidationToolTip = "Markup is valid.";
                else
                    ValidationToolTip = String.Join(Environment.NewLine, messages.Select(m => m.Message));
                return;
            }

            ValidationStatus = XmlValidationStatus.Information;
            lines = messages.First().Message.SplitLines().ToArray();
            if (messages.Length == 1)
            {
                if (lines.Length == 1)
                    ValidationToolTip = lines[0];
                else
                    ValidationToolTip = lines[0].TrimEnd() + " ... (more)";
            }
            else
                ValidationToolTip = String.Format("{0} Messages: {1}", lines.Length, lines[0].TrimEnd() + " ... (more)");
        }

        #endregion

        #region ValidationStatus Property Members

        public const string PropertyName_ValidationStatus = "ValidationStatus";

        private static readonly DependencyPropertyKey ValidationStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationStatus, typeof(XmlValidationStatus), typeof(MainWindowVM),
                new PropertyMetadata(XmlValidationStatus.Warning));

        /// <summary>
        /// Identifies the <seealso cref="ValidationStatus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationStatusProperty = ValidationStatusPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public XmlValidationStatus ValidationStatus
        {
            get
            {
                if (CheckAccess())
                    return (XmlValidationStatus)(GetValue(ValidationStatusProperty));
                return Dispatcher.Invoke(() => ValidationStatus);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ValidationStatusPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ValidationStatus = value);
            }
        }

        #endregion

        #region ValidationToolTip Property Members

        public const string PropertyName_ValidationToolTip = "ValidationToolTip";

        private static readonly DependencyPropertyKey ValidationToolTipPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationToolTip, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata("Document is empty."));

        /// <summary>
        /// Identifies the <seealso cref="ValidationToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationToolTipProperty = ValidationToolTipPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string ValidationToolTip
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(ValidationToolTipProperty));
                return Dispatcher.Invoke(() => ValidationToolTip);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ValidationToolTipPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ValidationToolTip = value);
            }
        }

        #endregion

        #region Text Property Members

        public const string DependencyPropertyName_Text = "Text";

        /// <summary>
        /// Identifies the <seealso cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(DependencyPropertyName_Text, 
            typeof(string), typeof(MainWindowVM), new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).Text_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).Text_PropertyChanged((string)(e.OldValue), 
                            (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).Text_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(TextProperty));
                return Dispatcher.Invoke(() => Text);
            }
            set
            {
                if (CheckAccess())
                    SetValue(TextProperty, value);
                else
                    Dispatcher.Invoke(() => Text = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Text"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Text"/> 
        /// property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Text"/> 
        /// property was changed.</param>
        protected virtual void Text_PropertyChanged(string oldValue, string newValue)
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
                return;

            lock (syncRoot)
            {
                FileSaveStatus = FileSaveStatus.Modified;
                FileModified = true;
                if (FileSaveLocation == "")
                {
                    FileSaveToolBarMessage = "Unsaved File modified.";
                    FileSaveDetailMessage = FileSaveDetail_NewModified;
                }
                else
                {
                    FileSaveToolBarMessage = String.Format("{0} modified", System.IO.Path.GetFileName(FileSaveLocation));
                    FileSaveDetailMessage = String.Format("{0} has been modified, and has not yet been saved. Full Path: {1}.",
                        Path.GetFileName(FileSaveLocation), FileSaveLocation);
                }
            }
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Text"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Text_CoerceValue(object baseValue)
        {
            return (baseValue as string) ?? "";
        }

        #endregion

        #region SelectedText Property Members

        public const string DependencyPropertyName_SelectedText = "SelectedText";

        /// <summary>
        /// Identifies the <seealso cref="SelectedText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register(DependencyPropertyName_SelectedText, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).SelectedText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).SelectedText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).SelectedText_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string SelectedText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(SelectedTextProperty));
                return Dispatcher.Invoke(() => SelectedText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedTextProperty, value);
                else
                    Dispatcher.Invoke(() => SelectedText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="SelectedText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="SelectedText"/> property was changed.</param>
        protected virtual void SelectedText_PropertyChanged(string oldValue, string newValue) { }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string SelectedText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region FileSaveStatus Property Members

        public const string PropertyName_FileSaveStatus = "FileSaveStatus";

        private static readonly DependencyPropertyKey FileSaveStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileSaveStatus, typeof(FileSaveStatus), typeof(MainWindowVM),
                new PropertyMetadata(FileSaveStatus.New));

        /// <summary>
        /// Identifies the <seealso cref="FileSaveStatus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FileSaveStatusProperty = FileSaveStatusPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public FileSaveStatus FileSaveStatus
        {
            get
            {
                if (CheckAccess())
                    return (FileSaveStatus)(GetValue(FileSaveStatusProperty));
                return Dispatcher.Invoke(() => FileSaveStatus);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(FileSaveStatusPropertyKey, value);
                else
                    Dispatcher.Invoke(() => FileSaveStatus = value);
            }
        }

        #endregion

        #region FileModified Property Members

        public const string PropertyName_FileModified = "FileModified";

        private static readonly DependencyPropertyKey FileModifiedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileModified, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="FileModified"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FileModifiedProperty = FileModifiedPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public bool FileModified
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(FileModifiedProperty));
                return Dispatcher.Invoke(() => FileModified);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(FileModifiedPropertyKey, value);
                else
                    Dispatcher.Invoke(() => FileModified = value);
            }
        }

        #endregion

        #region FileSaveToolBarMessage Property Members

        public const string PropertyName_FileSaveToolBarMessage = "FileSaveToolBarMessage";

        private static readonly DependencyPropertyKey FileSaveToolBarMessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileSaveToolBarMessage, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="FileSaveToolBarMessage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FileSaveToolBarMessageProperty = FileSaveToolBarMessagePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string FileSaveToolBarMessage
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(FileSaveToolBarMessageProperty));
                return Dispatcher.Invoke(() => FileSaveToolBarMessage);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(FileSaveToolBarMessagePropertyKey, value);
                else
                    Dispatcher.Invoke(() => FileSaveToolBarMessage = value);
            }
        }

        #endregion

        #region FileSaveDetailMessage Property Members

        public const string PropertyName_FileSaveDetailMessage = "FileSaveDetailMessage";
        public const string FileSaveDetail_NewDocument = "Document is new and has not been modified.";
        public const string FileSaveDetail_NewModified = "Document has been modified, but never saved.";

        private static readonly DependencyPropertyKey FileSaveDetailMessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileSaveDetailMessage, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="FileSaveDetailMessage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FileSaveDetailMessageProperty = FileSaveDetailMessagePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string FileSaveDetailMessage
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(FileSaveDetailMessageProperty));
                return Dispatcher.Invoke(() => FileSaveDetailMessage);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(FileSaveDetailMessagePropertyKey, value);
                else
                    Dispatcher.Invoke(() => FileSaveDetailMessage = value);
            }
        }

        #endregion

        #region FileSaveLocation Property Members

        public const string PropertyName_FileSaveLocation = "FileSaveLocation";

        private static readonly DependencyPropertyKey FileSaveLocationPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileSaveLocation, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="FileSaveLocation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FileSaveLocationProperty = FileSaveLocationPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string FileSaveLocation
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(FileSaveLocationProperty));
                return Dispatcher.Invoke(() => FileSaveLocation);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(FileSaveLocationPropertyKey, value);
                else
                    Dispatcher.Invoke(() => FileSaveLocation = value);
            }
        }

        #endregion

        #region LineWrapEnabled Property Members

        public const string DependencyPropertyName_LineWrapEnabled = "LineWrapEnabled";

        /// <summary>
        /// Identifies the <seealso cref="LineWrapEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineWrapEnabledProperty = DependencyProperty.Register(DependencyPropertyName_LineWrapEnabled, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(true,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).LineWrapEnabled_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).LineWrapEnabled_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue)));
                }));

        /// <summary>
        /// 
        /// </summary>
        public bool LineWrapEnabled
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(LineWrapEnabledProperty));
                return Dispatcher.Invoke(() => LineWrapEnabled);
            }
            set
            {
                if (CheckAccess())
                    SetValue(LineWrapEnabledProperty, value);
                else
                    Dispatcher.Invoke(() => LineWrapEnabled = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="LineWrapEnabled"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="LineWrapEnabled"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="LineWrapEnabled"/> property was changed.</param>
        protected virtual void LineWrapEnabled_PropertyChanged(bool oldValue, bool newValue) { }

        #endregion

        #region SelectAfterInsert Property Members

        public const string DependencyPropertyName_SelectAfterInsert = "SelectAfterInsert";

        /// <summary>
        /// Identifies the <seealso cref="SelectAfterInsert"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectAfterInsertProperty = DependencyProperty.Register(DependencyPropertyName_SelectAfterInsert, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(false,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).SelectAfterInsert_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).SelectAfterInsert_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue)));
                }));

        /// <summary>
        /// 
        /// </summary>
        public bool SelectAfterInsert
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(SelectAfterInsertProperty));
                return Dispatcher.Invoke(() => SelectAfterInsert);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectAfterInsertProperty, value);
                else
                    Dispatcher.Invoke(() => SelectAfterInsert = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectAfterInsert"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="SelectAfterInsert"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="SelectAfterInsert"/> property was changed.</param>
        protected virtual void SelectAfterInsert_PropertyChanged(bool oldValue, bool newValue) { }

        #endregion

        #region CurrentLineNumber Property Members

        public const string PropertyName_CurrentLineNumber = "CurrentLineNumber";

        private static readonly DependencyPropertyKey CurrentLineNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentLineNumber, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(1));

        /// <summary>
        /// Identifies the <seealso cref="CurrentLineNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentLineNumberProperty = CurrentLineNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int CurrentLineNumber
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(CurrentLineNumberProperty));
                return Dispatcher.Invoke(() => CurrentLineNumber);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(CurrentLineNumberPropertyKey, value);
                else
                    Dispatcher.Invoke(() => CurrentLineNumber = value);
            }
        }

        #endregion

        #region CurrentColNumber Property Members

        public const string PropertyName_CurrentColNumber = "CurrentColNumber";

        private static readonly DependencyPropertyKey CurrentColNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentColNumber, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(1));

        /// <summary>
        /// Identifies the <seealso cref="CurrentColNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentColNumberProperty = CurrentColNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int CurrentColNumber
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(CurrentColNumberProperty));
                return Dispatcher.Invoke(() => CurrentColNumber);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(CurrentColNumberPropertyKey, value);
                else
                    Dispatcher.Invoke(() => CurrentColNumber = value);
            }
        }

        #endregion

        #endregion

        public bool SaveAs(object parameter = null)
        {
            string errorMessage = GetValidationMessageSummary();
            if (errorMessage != null && MessageBox.Show(String.Format("{0}\r\nAre you sure you want to save?", errorMessage), "Invalid Document",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return false;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.Title = "Save SSML Document";
            if (!Common.FileUtility.InvokeSsmlFileDialog(dialog, App.Current.MainWindow, FileSaveLocation))
                return false;
            try
            {
                File.WriteAllText(dialog.FileName, Text, Encoding.UTF8);
                FileModified = false;
                FileSaveLocation = dialog.FileName;
                FileSaveStatus = FileSaveStatus.SaveSuccess;
                FileSaveToolBarMessage = Path.GetFileName(dialog.FileName);
                FileSaveDetailMessage = String.Format("File saved to {0}", dialog.FileName);
            }
            catch (Exception exception)
            {
                FileSaveStatus = FileSaveStatus.SaveError;
                FileSaveToolBarMessage = String.Format("Error saving {0}.", Path.GetFileName(dialog.FileName));
                FileSaveDetailMessage = String.Format("Error saving from {0}: {1}", dialog.FileName, exception.Message);
                return false;
            }

            return true;
        }

        public bool Save(object parameter = null)
        {
            if (FileSaveLocation.Length == 0)
                return SaveAs(parameter);

            if (!File.Exists(FileSaveLocation))
            {
                string dir = Path.GetDirectoryName(FileSaveLocation);
                if (String.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                    return SaveAs(parameter);
            }
            string errorMessage = GetValidationMessageSummary();
            if (errorMessage != null && MessageBox.Show(String.Format("{0}\r\nAre you sure you want to save?", errorMessage), "Invalid Document",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return false;

            try
            {
                File.WriteAllText(FileSaveLocation, Text, Encoding.UTF8);
                FileModified = false;
                FileSaveStatus = FileSaveStatus.SaveSuccess;
                FileSaveToolBarMessage = String.Format("{0} saved.", Path.GetFileName(FileSaveLocation));
                FileSaveDetailMessage = String.Format("File saved to {0}", FileSaveLocation);
            }
            catch (Exception exception)
            {
                FileSaveStatus = FileSaveStatus.SaveError;
                FileSaveToolBarMessage = String.Format("Error saving {0}.", Path.GetFileName(FileSaveLocation));
                FileSaveDetailMessage = String.Format("Error loading from {0}: {1}", FileSaveLocation, exception.Message);
                return false;
            }

            return true;
        }

        private bool VerifyCanReplaceCurrentFile()
        {
            if (!FileModified)
                return true;

            MessageBoxResult canReplace = MessageBox.Show("The current document has modifications that have not been saved.\r\nDo you want to save those changes?", 
                "File Modified", MessageBoxButton.YesNoCancel , MessageBoxImage.Warning);

           switch (canReplace)
            {
                case MessageBoxResult.Yes:
                    return Save();
                case MessageBoxResult.No:
                    return true;
            }

            return false;
        }

        internal void LayoutUpdated(TextBox textBox, EventArgs e)
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
                return;
            
            lock (syncRoot)
            {
                if (_lineNumbersUpdated)
                    return;
                _lineNumbersUpdated = true;
                int characterIndex = textBox.SelectionStart;
                int currentLine = textBox.GetLineIndexFromCharacterIndex(characterIndex);
                CurrentLineNumber = currentLine + 1;
                if (currentLine > -1)
                    CurrentColNumber = (characterIndex - textBox.GetCharacterIndexFromLineIndex(currentLine)) + 1;
                if (_lineNumberUpdater == null)
                    _lineNumberUpdater = new Process.BackgroundJobManager<Process.LineNumberGenerator, int>(new Process.LineNumberGenerator(textBox, _lineNumbers));
                else
                    _lineNumberUpdater.Replace(new Process.LineNumberGenerator(textBox, _lineNumbers));
            }
        }
    }
}
