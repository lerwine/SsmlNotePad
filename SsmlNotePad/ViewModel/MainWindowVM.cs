using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Input;

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
            _validationMessages.CollectionChanged += ValidationMessages_CollectionChanged;
            PhonemeResults = new ReadOnlyObservableCollection<PhonemeResultVM>(_phonemeResult);
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
            // BUG: wait never returns
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

        #region GetPhonemes Command Property Members

        private Command.RelayCommand _getPhonemesCommand = null;

        public Command.RelayCommand GetPhonemesCommand
        {
            get
            {
                if (_getPhonemesCommand == null)
                    _getPhonemesCommand = new Command.RelayCommand(OnGetPhonemes);

                return _getPhonemesCommand;
            }
        }

        protected virtual void OnGetPhonemes(object parameter)
        {
            if (SelectionLength == 0)
                return;
            int index = SelectionStart;
            if (index >= Text.Length)
                return;
            int length = (index + SelectionLength > Text.Length) ? Text.Length - index : SelectionLength;

            string s = Text.Substring(index, length);
            if (s.Trim().Length == 0)
                return;

            Process.CapturePhonemes cp = new Process.CapturePhonemes(s);
            try
            {
                _phonemeResult.Clear();
                foreach (Process.PhoneticGroupInfo grp in cp.Task.Result)
                    _phonemeResult.Add(new PhonemeResultVM(grp));
                PhonemesVisible = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(App.Current.MainWindow, "Error", String.Format("Unexpected error while getting phonemes: {0}", exception.Message), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region RemoveConsecutiveEmptyLines Command Property Members

        private Command.RelayCommand _removeConsecutiveEmptyLinesCommand = null;

        public Command.RelayCommand RemoveConsecutiveEmptyLinesCommand
        {
            get
            {
                if (_removeConsecutiveEmptyLinesCommand == null)
                    _removeConsecutiveEmptyLinesCommand = new Command.RelayCommand(OnRemoveConsecutiveEmptyLines);

                return _removeConsecutiveEmptyLinesCommand;
            }
        }

        protected virtual void OnRemoveConsecutiveEmptyLines(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            textBox.SelectedText = Common.XmlHelper.RemoveConsecutiveEmptyLines(textBox.SelectedText);
            textBox.UpdateLayout();
        }

        #endregion

        #region RemoveEmptyLines Command Property Members

        private Command.RelayCommand _removeEmptyLinesCommand = null;

        public Command.RelayCommand RemoveEmptyLinesCommand
        {
            get
            {
                if (_removeEmptyLinesCommand == null)
                    _removeEmptyLinesCommand = new Command.RelayCommand(OnRemoveEmptyLines);

                return _removeEmptyLinesCommand;
            }
        }

        protected virtual void OnRemoveEmptyLines(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            textBox.SelectedText = Common.XmlHelper.RemoveEmptyLines(textBox.SelectedText);
            textBox.UpdateLayout();
        }

        #endregion

        #region JoinLines Command Property Members

        private Command.RelayCommand _joinLinesCommand = null;

        public Command.RelayCommand JoinLinesCommand
        {
            get
            {
                if (_joinLinesCommand == null)
                    _joinLinesCommand = new Command.RelayCommand(OnJoinLines);

                return _joinLinesCommand;
            }
        }

        protected virtual void OnJoinLines(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            textBox.SelectedText = Common.XmlHelper.JoinLines(textBox.SelectedText);
            textBox.UpdateLayout();
        }

        #endregion

        #region RemoveOuterWhitespace Command Property Members

        private Command.RelayCommand _removeOuterWhitespaceCommand = null;

        public Command.RelayCommand RemoveOuterWhitespaceCommand
        {
            get
            {
                if (_removeOuterWhitespaceCommand == null)
                    _removeOuterWhitespaceCommand = new Command.RelayCommand(OnRemoveOuterWhitespace);

                return _removeOuterWhitespaceCommand;
            }
        }

        protected virtual void OnRemoveOuterWhitespace(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            textBox.SelectedText = Common.XmlHelper.RemoveOuterWhitespace(textBox.SelectedText);
            textBox.UpdateLayout();
        }

        #endregion

        #region CleanUpLineEndings Command Property Members

        private Command.RelayCommand _cleanUpLineEndingsCommand = null;

        public Command.RelayCommand CleanUpLineEndingsCommand
        {
            get
            {
                if (_cleanUpLineEndingsCommand == null)
                    _cleanUpLineEndingsCommand = new Command.RelayCommand(OnCleanUpLineEndings);

                return _cleanUpLineEndingsCommand;
            }
        }

        protected virtual void OnCleanUpLineEndings(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            textBox.SelectedText = Common.XmlHelper.CleanUpLineEndings(textBox.SelectedText);
            textBox.UpdateLayout();
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

            string pattern = textBox.SelectedText;
            if (pattern.Trim().Length > 0)
                Pattern = (UseRegularExpressions) ? Regex.Escape(pattern) : pattern;
            FindReplaceMode = FindReplaceDisplayMode.Find;
        }

        #endregion
        
        #region FindNext Command Property Members

        private Command.RelayCommand _findNextCommand = null;

        public Command.RelayCommand FindNextCommand
        {
            get
            {
                if (_findNextCommand == null)
                    _findNextCommand = new Command.RelayCommand(OnFindNext);

                return _findNextCommand;
            }
        }

        protected virtual void OnFindNext(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            string selectedText;
            if (SelectionLength == 0 || SelectionStart >= Text.Length)
                selectedText = "";
            else
                selectedText = Text.Substring(SelectionStart, ((SelectionStart + SelectionLength) > Text.Length) ? Text.Length - SelectionStart : SelectionLength);
            
            switch (FindReplaceMode)
            {
                case FindReplaceDisplayMode.None:
                    if (selectedText.Trim().Length > 0)
                        Pattern = (UseRegularExpressions) ? Regex.Escape(selectedText) : selectedText;
                    FindReplaceMode = FindReplaceDisplayMode.Find;
                    return;
                case FindReplaceDisplayMode.Find:
                    if (UseRegularExpressions && SearchExpression == null)
                        return;
                    FindReplaceMode = FindReplaceDisplayMode.FindNext;
                    break;
                case FindReplaceDisplayMode.Replace:
                    if (UseRegularExpressions && SearchExpression == null)
                        return;
                    FindReplaceMode = FindReplaceDisplayMode.ReplaceNext;
                    break;
                case FindReplaceDisplayMode.ReplaceNext:
                    if (SelectionLength == 0)
                        return;
                    Text = Text.Substring(0, SelectionStart) + ReplacementText + Text.Substring(SelectionStart + SelectionLength);
                    break;
            }

            int index = SelectionStart + SelectionLength;
            if (index >= Text.Length)
            {
                index = 0;
                SelectionStart = 0;
                SelectionLength = 0;
            }
            int length;
            if (UseRegularExpressions)
            {
                Match m = SearchExpression.Match(Text, SelectionStart + SelectionLength);
                if (m.Success)
                {
                    index = m.Index;
                    length = m.Length;
                }
                else
                {
                    index = -1;
                    length = 0;
                }
            }
            else
            {
                index = Text.IndexOf(Pattern, SelectionStart + SelectionLength, (CaseSensitiveMatch) ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
                length = Pattern.Length;
            }
            if (index == -1)
            {
                if (SelectionStart == 0)
                {
                    MessageBox.Show(App.Current.MainWindow, "Match not found", "Match not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    FindReplaceMode = (FindReplaceMode == FindReplaceDisplayMode.FindNext) ? FindReplaceDisplayMode.FindNotFound : FindReplaceDisplayMode.ReplaceNotFound;
                    return;
                }
                index = 0;
                if (UseRegularExpressions)
                {
                    Match m = SearchExpression.Match(Text, SelectionStart + SelectionLength);
                    if (m.Success)
                    {
                        index = m.Index;
                        length = m.Length;
                    }
                    else
                    {
                        index = -1;
                        length = 0;
                    }
                }
                else
                {
                    index = Text.IndexOf(Pattern, SelectionStart + SelectionLength, (CaseSensitiveMatch) ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
                    length = Pattern.Length;
                }
                if (index == -1)
                {
                    MessageBox.Show(App.Current.MainWindow, "Match not found", "Match not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    FindReplaceMode = (FindReplaceMode == FindReplaceDisplayMode.FindNext) ? FindReplaceDisplayMode.FindNotFound : FindReplaceDisplayMode.ReplaceNotFound;
                    return;
                }
            }
            SelectionStart = index;
            SelectionLength = length;
            textBox.SelectionStart = SelectionStart;
            textBox.SelectionLength = SelectionLength;
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

            string pattern = textBox.SelectedText;
            if (pattern.Trim().Length > 0)
                Pattern = (UseRegularExpressions) ? Regex.Escape(pattern) : pattern;
            FindReplaceMode = FindReplaceDisplayMode.Replace;
        }

        #endregion
        
        #region SkipNext Command Property Members

        private Command.RelayCommand _skipNextCommand = null;

        public Command.RelayCommand SkipNextCommand
        {
            get
            {
                if (_skipNextCommand == null)
                    _skipNextCommand = new Command.RelayCommand(OnSkipNext);

                return _skipNextCommand;
            }
        }

        protected virtual void OnSkipNext(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            string selectedText;
            if (SelectionLength == 0 || SelectionStart >= Text.Length)
                selectedText = "";
            else
                selectedText = Text.Substring(SelectionStart, ((SelectionStart + SelectionLength) > Text.Length) ? Text.Length - SelectionStart : SelectionLength);
            
            int index = SelectionStart + SelectionLength;
            if (index >= Text.Length)
            {
                index = 0;
                SelectionStart = 0;
                SelectionLength = 0;
            }
            int length;
            if (UseRegularExpressions)
            {
                Match m = SearchExpression.Match(Text, SelectionStart + SelectionLength);
                if (m.Success)
                {
                    index = m.Index;
                    length = m.Length;
                }
                else
                {
                    index = -1;
                    length = 0;
                }
            }
            else
            {
                index = Text.IndexOf(Pattern, SelectionStart + SelectionLength, (CaseSensitiveMatch) ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
                length = Pattern.Length;
            }
            if (index == -1)
            {
                if (SelectionStart == 0)
                {
                    MessageBox.Show(App.Current.MainWindow, "Match not found", "Match not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    FindReplaceMode = FindReplaceDisplayMode.ReplaceNotFound;
                    return;
                }
                index = 0;
                if (UseRegularExpressions)
                {
                    Match m = SearchExpression.Match(Text, SelectionStart + SelectionLength);
                    if (m.Success)
                    {
                        index = m.Index;
                        length = m.Length;
                    }
                    else
                    {
                        index = -1;
                        length = 0;
                    }
                }
                else
                {
                    index = Text.IndexOf(Pattern, SelectionStart + SelectionLength, (CaseSensitiveMatch) ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
                    length = Pattern.Length;
                }
                if (index == -1)
                {
                    MessageBox.Show(App.Current.MainWindow, "Match not found", "Match not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    FindReplaceMode = FindReplaceDisplayMode.ReplaceNotFound;
                    return;
                }
            }
            SelectionStart = index;
            SelectionLength = length;
            textBox.SelectionStart = SelectionStart;
            textBox.SelectionLength = SelectionLength;
        }

        #endregion

        #region FindReplaceCancel Command Property Members

        private Command.RelayCommand _findReplaceCancelCommand = null;

        public Command.RelayCommand FindReplaceCancelCommand
        {
            get
            {
                if (_findReplaceCancelCommand == null)
                    _findReplaceCancelCommand = new Command.RelayCommand(OnFindReplaceCancel);

                return _findReplaceCancelCommand;
            }
        }

        protected virtual void OnFindReplaceCancel(object parameter)
        {
            FindReplaceMode = FindReplaceDisplayMode.None;
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

        #region AutoReplace Command Property Members

        private Command.RelayCommand _autoReplaceCommand = null;

        public Command.RelayCommand AutoReplaceCommand
        {
            get
            {
                if (_autoReplaceCommand == null)
                    _autoReplaceCommand = new Command.RelayCommand(OnAutoReplace);

                return _autoReplaceCommand;
            }
        }

        protected virtual void OnAutoReplace(object parameter)
        {
            TextBox textBox = parameter as TextBox;

            textBox.SelectedText = Common.XmlHelper.AutoReplace(textBox.SelectedText);
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

        #region SpeakAllText Command Property Members

        private Command.RelayCommand _speakAllTextCommand = null;

        public Command.RelayCommand SpeakAllTextCommand
        {
            get
            {
                if (_speakAllTextCommand == null)
                    _speakAllTextCommand = new Command.RelayCommand(OnSpeakAllText);

                return _speakAllTextCommand;
            }
        }

        protected virtual void OnSpeakAllText(object parameter)
        {
            Process.SpeechSynthesis.ForDefaultAudioDevice(Text);
        }

        #endregion

        #region ExportAsWav Command Property Members

        private Command.RelayCommand _exportAsWavCommand = null;

        public Command.RelayCommand ExportAsWavCommand
        {
            get
            {
                if (_exportAsWavCommand == null)
                    _exportAsWavCommand = new Command.RelayCommand(OnExportAsWav);

                return _exportAsWavCommand;
            }
        }

        protected virtual void OnExportAsWav(object parameter)
        {
            Window owner = App.GetWindowByDataContext<MainWindow, MainWindowVM>(this) ?? App.Current.MainWindow;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.OverwritePrompt = true;
            string fileName;
            if (String.IsNullOrEmpty(FileSaveLocation))
                fileName = "";
            else
                try { fileName = Path.GetFileNameWithoutExtension(FileSaveLocation) + ".wav"; } catch { fileName = ""; }
            if (Common.FileUtility.InvokeWavFileDialog(dialog, owner, fileName))
                Process.SpeechSynthesis.ForWaveFile(Text, dialog.FileName, owner);
        }

        #endregion

        #endregion

        #region Dependency Properties

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

        #region PhonemesVisible Property Members

        public const string DependencyPropertyName_PhonemesVisible = "PhonemesVisible";

        /// <summary>
        /// Identifies the <seealso cref="PhonemesVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhonemesVisibleProperty = DependencyProperty.Register(DependencyPropertyName_PhonemesVisible, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(false,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).PhonemesVisible_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).PhonemesVisible_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).PhonemesVisible_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public bool PhonemesVisible
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(PhonemesVisibleProperty));
                return Dispatcher.Invoke(() => PhonemesVisible);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PhonemesVisibleProperty, value);
                else
                    Dispatcher.Invoke(() => PhonemesVisible = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="PhonemesVisible"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="PhonemesVisible"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="PhonemesVisible"/> property was changed.</param>
        protected virtual void PhonemesVisible_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement MainWindowVM.PhonemesVisible_PropertyChanged(bool, bool)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="PhonemesVisible"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual bool PhonemesVisible_CoerceValue(object baseValue)
        {
            // TODO: Implement MainWindowVM.PhonemesVisible_CoerceValue(DependencyObject, object)
            return (bool)baseValue;
        }

        #endregion

        #region PhonemeResults Property Members

        private ObservableCollection<PhonemeResultVM> _phonemeResult = new ObservableCollection<PhonemeResultVM>();

        public const string PropertyName_PhonemeResults = "PhonemeResults";

        private static readonly DependencyPropertyKey PhonemeResultsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PhonemeResults, typeof(ReadOnlyObservableCollection<PhonemeResultVM>), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="PhonemeResults"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhonemeResultsProperty = PhonemeResultsPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<PhonemeResultVM> PhonemeResults
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<PhonemeResultVM>)(GetValue(PhonemeResultsProperty));
                return Dispatcher.Invoke(() => PhonemeResults);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(PhonemeResultsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => PhonemeResults = value);
            }
        }

        #endregion

        #region FindReplaceMode Property Members

        public const string PropertyName_FindReplaceMode = "FindReplaceMode";

        private static readonly DependencyPropertyKey FindReplaceModePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FindReplaceMode, typeof(FindReplaceDisplayMode), typeof(MainWindowVM),
                new PropertyMetadata(FindReplaceDisplayMode.None,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).FindReplaceMode_PropertyChanged((FindReplaceDisplayMode)(e.OldValue), (FindReplaceDisplayMode)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).FindReplaceMode_PropertyChanged((FindReplaceDisplayMode)(e.OldValue), (FindReplaceDisplayMode)(e.NewValue)));
                }));

        /// <summary>
        /// Identifies the <seealso cref="FindReplaceMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FindReplaceModeProperty = FindReplaceModePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public FindReplaceDisplayMode FindReplaceMode
        {
            get
            {
                if (CheckAccess())
                    return (FindReplaceDisplayMode)(GetValue(FindReplaceModeProperty));
                return Dispatcher.Invoke(() => FindReplaceMode);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(FindReplaceModePropertyKey, value);
                else
                    Dispatcher.Invoke(() => FindReplaceMode = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="FindReplaceMode"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="FindReplaceMode"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="FindReplaceMode"/> property was changed.</param>
        protected virtual void FindReplaceMode_PropertyChanged(FindReplaceDisplayMode oldValue, FindReplaceDisplayMode newValue)
        {
            UpdateSearchCommands();
        }

        #endregion

        #region CaseSensitiveMatch Property Members

        public const string DependencyPropertyName_CaseSensitiveMatch = "CaseSensitiveMatch";

        /// <summary>
        /// Identifies the <seealso cref="CaseSensitiveMatch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaseSensitiveMatchProperty = DependencyProperty.Register(DependencyPropertyName_CaseSensitiveMatch, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(false,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).CaseSensitiveMatch_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).CaseSensitiveMatch_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue)));
                }));

        /// <summary>
        /// Whether searches are case-sensitive
        /// </summary>
        public bool CaseSensitiveMatch
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(CaseSensitiveMatchProperty));
                return Dispatcher.Invoke(() => CaseSensitiveMatch);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CaseSensitiveMatchProperty, value);
                else
                    Dispatcher.Invoke(() => CaseSensitiveMatch = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="CaseSensitiveMatch"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="CaseSensitiveMatch"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="CaseSensitiveMatch"/> property was changed.</param>
        protected virtual void CaseSensitiveMatch_PropertyChanged(bool oldValue, bool newValue)
        {
            if (UseRegularExpressions)
                ValidatePattern();
        }

        #endregion

        #region UseRegularExpressions Property Members

        public const string DependencyPropertyName_UseRegularExpressions = "UseRegularExpressions";

        /// <summary>
        /// Identifies the <seealso cref="UseRegularExpressions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseRegularExpressionsProperty = DependencyProperty.Register(DependencyPropertyName_UseRegularExpressions, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(false,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).UseRegularExpressions_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).UseRegularExpressions_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue)));
                }));

        /// <summary>
        /// 
        /// </summary>
        public bool UseRegularExpressions
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(UseRegularExpressionsProperty));
                return Dispatcher.Invoke(() => UseRegularExpressions);
            }
            set
            {
                if (CheckAccess())
                    SetValue(UseRegularExpressionsProperty, value);
                else
                    Dispatcher.Invoke(() => UseRegularExpressions = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="UseRegularExpressions"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="UseRegularExpressions"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="UseRegularExpressions"/> property was changed.</param>
        protected virtual void UseRegularExpressions_PropertyChanged(bool oldValue, bool newValue)
        {
            if (newValue)
                ValidatePattern();
            else
                PatternValidationText = "";
        }

        #endregion

        #region SearchExpression Property Members

        public const string PropertyName_SearchExpression = "SearchExpression";

        private static readonly DependencyPropertyKey SearchExpressionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SearchExpression, typeof(Regex), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SearchExpression"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchExpressionProperty = SearchExpressionPropertyKey.DependencyProperty;

        /// <summary>
        /// Regular expression object for find or replace
        /// </summary>
        public Regex SearchExpression
        {
            get
            {
                if (CheckAccess())
                    return (Regex)(GetValue(SearchExpressionProperty));
                return Dispatcher.Invoke(() => SearchExpression);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SearchExpressionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => SearchExpression = value);
            }
        }

        #endregion

        #region Pattern Property Members

        public const string DependencyPropertyName_Pattern = "Pattern";

        /// <summary>
        /// Identifies the <seealso cref="Pattern"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PatternProperty = DependencyProperty.Register(DependencyPropertyName_Pattern, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).Pattern_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).Pattern_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).Pattern_CoerceValue(baseValue)));

        /// <summary>
        /// Pattern for use with find or replace
        /// </summary>
        public string Pattern
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PatternProperty));
                return Dispatcher.Invoke(() => Pattern);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PatternProperty, value);
                else
                    Dispatcher.Invoke(() => Pattern = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Pattern"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Pattern"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Pattern"/> property was changed.</param>
        protected virtual void Pattern_PropertyChanged(string oldValue, string newValue)
        {
            if (UseRegularExpressions)
                ValidatePattern();
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Pattern"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Pattern_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        private void ValidatePattern()
        {
            if (Pattern.Length != 0)
            {
                if (!UseRegularExpressions)
                    PatternValidationText = "";
                else
                {
                    try
                    {
                        SearchExpression = new Regex(Pattern, (CaseSensitiveMatch) ? RegexOptions.Compiled : RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        PatternValidationText = "";
                    }
                    catch (Exception exception)
                    {
                        SearchExpression = null;
                        PatternValidationText = exception.Message;
                    }
                }
            }
            else
                PatternValidationText = "Pattern text cannot be empty.";

            UpdateSearchCommands();
        }

        private void UpdateSearchCommands()
        {
            if (PatternValidationText.Length != 0)
            {
                FindNextCommand.IsEnabled = false;
                SkipNextCommand.IsEnabled = false;
                return;
            }

            switch (FindReplaceMode)
            {
                case FindReplaceDisplayMode.Find:
                case FindReplaceDisplayMode.FindNext:
                case FindReplaceDisplayMode.Replace:
                    FindNextCommand.IsEnabled = true;
                    SkipNextCommand.IsEnabled = false;
                    break;
                case FindReplaceDisplayMode.ReplaceNext:
                    FindNextCommand.IsEnabled = true;
                    SkipNextCommand.IsEnabled = true;
                    break;
                default:
                    FindNextCommand.IsEnabled = false;
                    SkipNextCommand.IsEnabled = false;
                    break;
            }
        }

        #region ReplacementText Property Members

        public const string DependencyPropertyName_ReplacementText = "ReplacementText";

        /// <summary>
        /// Identifies the <seealso cref="ReplacementText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReplacementTextProperty = DependencyProperty.Register(DependencyPropertyName_ReplacementText, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).ReplacementText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).ReplacementText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).ReplacementText_CoerceValue(baseValue)));

        /// <summary>
        /// Text to replace for replace command
        /// </summary>
        public string ReplacementText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(ReplacementTextProperty));
                return Dispatcher.Invoke(() => ReplacementText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ReplacementTextProperty, value);
                else
                    Dispatcher.Invoke(() => ReplacementText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="ReplacementText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="ReplacementText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="ReplacementText"/> property was changed.</param>
        protected virtual void ReplacementText_PropertyChanged(string oldValue, string newValue) { }

        /// <summary>
        /// This gets called whenever <seealso cref="ReplacementText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string ReplacementText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region PatternValidationText Property Members

        public const string PropertyName_PatternValidationText = "PatternValidationText";

        private static readonly DependencyPropertyKey PatternValidationTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PatternValidationText, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="PatternValidationText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PatternValidationTextProperty = PatternValidationTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Displays error message when Pattern does not represent a valid regular expression.
        /// </summary>
        public string PatternValidationText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PatternValidationTextProperty));
                return Dispatcher.Invoke(() => PatternValidationText);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(PatternValidationTextPropertyKey, value);
                else
                    Dispatcher.Invoke(() => PatternValidationText = value);
            }
        }

        #endregion

        #region Text Property Members

        public event EventHandler TextChanged;

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
        /// SSML markup text
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

            //TextChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Text"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Text_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        public event EventHandler SelectionChanged;

        #region SelectionStart Property Members

        private bool _isSelectionChanging = false;

        public const string DependencyPropertyName_SelectionStart = "SelectionStart";

        /// <summary>
        /// Identifies the <seealso cref="SelectionStart"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(DependencyPropertyName_SelectionStart, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).SelectionStart_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).SelectionStart_PropertyChanged((int)(e.OldValue),
                            (int)(e.NewValue)));
                }));

        /// <summary>
        /// Start index of selection
        /// </summary>
        public int SelectionStart
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectionStartProperty));
                return Dispatcher.Invoke(() => SelectionStart);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectionStartProperty, value);
                else
                    Dispatcher.Invoke(() => SelectionStart = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectionStart"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="Text"/> 
        /// property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="Text"/> 
        /// property was changed.</param>
        protected virtual void SelectionStart_PropertyChanged(int oldValue, int newValue)
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
                return;
            
        }

        #endregion

        #region SelectionLength Property Members

        public const string DependencyPropertyName_SelectionLength = "SelectionLength";

        /// <summary>
        /// Identifies the <seealso cref="SelectionLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register(DependencyPropertyName_SelectionLength, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).SelectionLength_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).SelectionLength_PropertyChanged((int)(e.OldValue),
                            (int)(e.NewValue)));
                }));

        /// <summary>
        /// 
        /// </summary>
        public int SelectionLength
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectionLengthProperty));
                return Dispatcher.Invoke(() => SelectionLength);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectionLengthProperty, value);
                else
                    Dispatcher.Invoke(() => SelectionLength = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectionLength"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="Text"/> 
        /// property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="Text"/> 
        /// property was changed.</param>
        protected virtual void SelectionLength_PropertyChanged(int oldValue, int newValue)
        {
            object syncRoot = _syncRoot;
            if (syncRoot == null)
                return;

            lock (syncRoot)
            {
            }
        }

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
        /// Whether line wrapis to be enabled.
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
        /// Whether to select text after inserting
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

        public const string DependencyPropertyName_CurrentLineNumber = "CurrentLineNumber";

        /// <summary>
        /// Identifies the <seealso cref="CurrentLineNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentLineNumberProperty = DependencyProperty.Register(DependencyPropertyName_CurrentLineNumber, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).CurrentLineNumber_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).CurrentLineNumber_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).CurrentLineNumber_CoerceValue(baseValue)));

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
            set
            {
                if (CheckAccess())
                    SetValue(CurrentLineNumberProperty, value);
                else
                    Dispatcher.Invoke(() => CurrentLineNumber = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="CurrentLineNumber"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="CurrentLineNumber"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="CurrentLineNumber"/> property was changed.</param>
        protected virtual void CurrentLineNumber_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement MainWindowVM.CurrentLineNumber_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="CurrentLineNumber"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int CurrentLineNumber_CoerceValue(object baseValue)
        {
            int i = (int)baseValue;
            return (i < 1) ? 1 : i;
        }

        #endregion

        #region CurrentColNumber Property Members

        public const string DependencyPropertyName_CurrentColNumber = "CurrentColNumber";

        /// <summary>
        /// Identifies the <seealso cref="CurrentColNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentColNumberProperty = DependencyProperty.Register(DependencyPropertyName_CurrentColNumber, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).CurrentColNumber_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).CurrentColNumber_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).CurrentColNumber_CoerceValue(baseValue)));

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
            set
            {
                if (CheckAccess())
                    SetValue(CurrentColNumberProperty, value);
                else
                    Dispatcher.Invoke(() => CurrentColNumber = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="CurrentColNumber"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="CurrentColNumber"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="CurrentColNumber"/> property was changed.</param>
        protected virtual void CurrentColNumber_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement MainWindowVM.CurrentColNumber_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="CurrentColNumber"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int CurrentColNumber_CoerceValue(object baseValue)
        {
            int i = (int)baseValue;
            return (i < 1) ? 1 : i;
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
            
            MessageBoxResult canReplace = MessageBox.Show(App.Current.MainWindow, "The current document has modifications that have not been saved.\r\nDo you want to save those changes?", 
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
            //object syncRoot = _syncRoot;
            //if (syncRoot == null)
            //    return;
            
            //lock (syncRoot)
            //{
            //    if (_lineNumbersUpdated)
            //        return;
            //    _lineNumbersUpdated = true;
            //    int characterIndex = textBox.SelectionStart;
            //    int currentLine = textBox.GetLineIndexFromCharacterIndex(characterIndex);
            //    CurrentLineNumber = currentLine + 1;
            //    if (currentLine > -1)
            //        CurrentColNumber = (characterIndex - textBox.GetCharacterIndexFromLineIndex(currentLine)) + 1;
            //    if (_lineNumberUpdater == null)
            //        _lineNumberUpdater = new Process.BackgroundJobManager<Process.LineNumberGenerator, int>(new Process.LineNumberGenerator(textBox, _lineNumbers));
            //    else
            //        _lineNumberUpdater.Replace(new Process.LineNumberGenerator(textBox, _lineNumbers));
            //}
        }
    }
}
