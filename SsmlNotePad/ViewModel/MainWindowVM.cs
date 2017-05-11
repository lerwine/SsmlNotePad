using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class MainWindowVM : DependencyObject
    {
        private object _syncRoot = new object();
        private Model.Workers.SsmlChangedJob _ssmlChangedJob = null;

        public MainWindowVM()
        {
            ValidationMessages = new ObservableCollection<XmlValidationMessageVM>();
            LineNumbers = new ObservableCollection<LineNumberVM>();
            LineNumbers.Add(new LineNumberVM(1, 0.0));
            SsmlTextBox = new TextBox
            {
                AcceptsReturn = true,
                AcceptsTab = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            _ssmlChangedJob = new Model.Workers.SsmlChangedJob(this);
            SetLineWrap();
            SsmlTextBox.Text = Markup.BlankSsmlDocument;
        }
        
        #region Commands

        #region File

        #region NewDocument Command Property Members

        private Command.RelayCommand _newDocumentCommand = null;

        public Command.RelayCommand NewDocumentCommand
        {
            get
            {
                if (_newDocumentCommand == null)
                    _newDocumentCommand = new Command.RelayCommand(OnNewDocument, false, true);

                return _newDocumentCommand;
            }
        }

        protected virtual void OnNewDocument(object parameter)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action<object>(OnNewDocument), parameter);
                return;
            }

            if (FileSaveStatus != Model.FileSaveStatus.New && FileSaveStatus != Model.FileSaveStatus.SaveSuccess)
            {
                switch (MessageBox.Show(App.Current.MainWindow, (CurrentFullPath.Length == 0) ? "New file has not been saved. Would you like to save changes?" : String.Format("{0} has not been saved. Would you like to save changes?", CurrentFullPath), "Save Changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel))
                {
                    case MessageBoxResult.Yes:
                        if (!SaveCurrentDocument())
                            return;
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            FileSaveStatus = Model.FileSaveStatus.New;
            FileSaveToolBarMessage = "";
            CurrentFileName = "";
            CurrentFullPath = "";
            SsmlTextBox.Text = Markup.BlankSsmlDocument;
        }

        #endregion

        #region OpenDocument Command Property Members

        private Command.RelayCommand _openDocumentCommand = null;

        public Command.RelayCommand OpenDocumentCommand
        {
            get
            {
                if (_openDocumentCommand == null)
                    _openDocumentCommand = new Command.RelayCommand(OnOpenDocument, false, true);

                return _openDocumentCommand;
            }
        }

        protected virtual void OnOpenDocument(object parameter)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action<object>(OnOpenDocument), parameter);
                return;
            }

            if (FileSaveStatus != Model.FileSaveStatus.New && FileSaveStatus != Model.FileSaveStatus.SaveSuccess)
            {
                switch (MessageBox.Show(App.Current.MainWindow, (CurrentFullPath.Length == 0) ? "New file has not been saved. Would you like to save changes?" : String.Format("{0} has not been saved. Would you like to save changes?", CurrentFullPath), "Save Changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel))
                {
                    case MessageBoxResult.Yes:
                        if (!SaveCurrentDocument())
                            return;
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            string path = CurrentFullPath;
            if (String.IsNullOrEmpty(path))
                path = App.AppSettingsViewModel.LastSsmlFilePath;
            if (!String.IsNullOrEmpty(path))
                path = Path.GetDirectoryName(path);
            if (String.IsNullOrEmpty(path) || !Directory.Exists(path))
                path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            OpenFileDialog dialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                DefaultExt = App.AppSettingsViewModel.SsmlFileExtension,
                Filter = String.Format("{0} (*{1})|*{1}|XML Files (*.xml)|*.*|All Files (*.*)|*.*", App.AppSettingsViewModel.SsmlFileTypeDescriptionShort, App.AppSettingsViewModel.SsmlFileExtension),
                InitialDirectory = path,
                RestoreDirectory = true,
                Title = "Open SSML File"
            };

            bool? dialogResult = dialog.ShowDialog(App.Current.MainWindow);
            if (dialogResult.HasValue && dialogResult.Value)
            {
                SsmlTextBox.Text = File.ReadAllText(dialog.FileName);
                FileSaveStatus = Model.FileSaveStatus.SaveSuccess;
                FileSaveToolBarMessage = "";
                CurrentFileName = Path.GetFileName(dialog.FileName);
                CurrentFullPath = Path.GetFullPath(dialog.FileName);
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
                    _saveDocumentCommand = new Command.RelayCommand(OnSaveDocument, false, true);

                return _saveDocumentCommand;
            }
        }

        protected virtual void OnSaveDocument(object parameter)
        {
            SaveCurrentDocument();
        }

        #endregion

        #region SaveAs Command Property Members

        private Command.RelayCommand _saveAsCommand = null;

        public Command.RelayCommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                    _saveAsCommand = new Command.RelayCommand(OnSaveAs, false, true);

                return _saveAsCommand;
            }
        }

        protected virtual void OnSaveAs(object parameter)
        {
            SaveCurrentDocumentAs();
        }

        #endregion
        
        #endregion

        #region Edit

        #region PasteEncoded Command Property Members

        private Command.RelayCommand _pasteEncodedCommand = null;

        public Command.RelayCommand PasteEncodedCommand
        {
            get
            {
                if (_pasteEncodedCommand == null)
                    _pasteEncodedCommand = new Command.RelayCommand(OnPasteEncoded, false, true);

                return _pasteEncodedCommand;
            }
        }

        protected virtual void OnPasteEncoded(object parameter)
        {
            // TODO: Implement OnPasteEncoded Logic
        }

        #endregion

        #region ReformatDocument Command Property Members

        private Command.RelayCommand _reformatDocumentCommand = null;

        public Command.RelayCommand ReformatDocumentCommand
        {
            get
            {
                if (_reformatDocumentCommand == null)
                    _reformatDocumentCommand = new Command.RelayCommand(OnReformatDocument, false);

                return _reformatDocumentCommand;
            }
        }

        protected virtual void OnReformatDocument(object parameter)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(SsmlTextBox.Text.Trim());
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    XmlWriterSettings settings = new XmlWriterSettings { CheckCharacters = false, Indent = true, Encoding = Encoding.UTF8 };
                    using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
                    {
                        xmlDocument.WriteTo(writer);
                        writer.Flush();
                        string text = settings.Encoding.GetString(memoryStream.ToArray());
                        if (text != SsmlTextBox.Text)
                            SsmlTextBox.Text = text;
                    }

                }
            } catch { }
        }

        #endregion

        #region CleanUpLineEndings Command Property Members

        private Command.RelayCommand _cleanUpLineEndingsCommand = null;

        public Command.RelayCommand CleanUpLineEndingsCommand
        {
            get
            {
                if (_cleanUpLineEndingsCommand == null)
                    _cleanUpLineEndingsCommand = new Command.RelayCommand(OnCleanUpLineEndings, false);

                return _cleanUpLineEndingsCommand;
            }
        }

        protected virtual void OnCleanUpLineEndings(object parameter)
        {
            string startText, endText;
            string text = SplitTextSelection(out startText, out endText).Trim();
            text = SsmlTextBox.Text.Trim();
            if (text.Length > 0)
                text = String.Join(Environment.NewLine, Model.TextLine.Split(text).Select(l => l.LineContent.TrimEnd())).Trim();
            text = startText + text + endText;
            if (text != SsmlTextBox.Text)
                SsmlTextBox.Text = text;
        }

        #endregion

        #region RemoveOuterWhitespace Command Property Members

        private Command.RelayCommand _removeOuterWhitespaceCommand = null;

        public Command.RelayCommand RemoveOuterWhitespaceCommand
        {
            get
            {
                if (_removeOuterWhitespaceCommand == null)
                    _removeOuterWhitespaceCommand = new Command.RelayCommand(OnRemoveOuterWhitespace, false, true);

                return _removeOuterWhitespaceCommand;
            }
        }

        private string SplitTextSelection(out string startText, out string endText)
        {
            if (CurrentSelectionLength == 0)
            {
                startText = "";
                endText = "";
                return SsmlTextBox.Text;
            }
            if (CurrentSelectionStart == 0)
            {
                startText = "";
                if (CurrentSelectionLength == SsmlTextBox.Text.Length)
                {
                    endText = "";
                    return SsmlTextBox.Text;
                }
                endText = SsmlTextBox.Text.Substring(CurrentSelectionLength);
                return SsmlTextBox.Text.Substring(0, CurrentSelectionLength);
            }

            startText = SsmlTextBox.Text.Substring(0, CurrentSelectionStart);
            if (CurrentSelectionStart + CurrentSelectionLength == SsmlTextBox.Text.Length)
            {
                endText = "";
                return SsmlTextBox.Text.Substring(CurrentSelectionStart);
            }

            endText = SsmlTextBox.Text.Substring(CurrentSelectionStart + CurrentSelectionLength);
            return SsmlTextBox.Text.Substring(CurrentSelectionStart, CurrentSelectionLength);
        }
        protected virtual void OnRemoveOuterWhitespace(object parameter)
        {
            string startText, endText;
            string text = SplitTextSelection(out startText, out endText).Trim();
            text = SsmlTextBox.Text.Trim();
            if (text.Length > 0)
                text = String.Join(Environment.NewLine, Model.TextLine.Split(text).Select(l => l.LineContent.Trim())).Trim();
            text = startText + text + endText;
            if (text != SsmlTextBox.Text)
                SsmlTextBox.Text = text;
        }

        #endregion

        public void DisableSelectionBasedCommands()
        {
            CleanUpLineEndingsCommand.IsEnabled = false;
            RemoveOuterWhitespaceCommand.IsEnabled = false;
            //JoinLinesCommand.IsEnabled = false;
            RemoveEmptyLinesCommand.IsEnabled = false;
            //RemoveConsecutiveEmptyLinesCommand.IsEnabled = false;
            //SelectTagContentsCommand.IsEnabled = false;
        }

        public void UpdateSelection(int selectionStart, int selectionLength, int currentLineNumber, int currentColNumber)
        {
            CurrentSelectionStart = selectionStart;
            CurrentSelectionLength = selectionLength;
            CurrentLineNumber = currentLineNumber;
            CurrentColNumber = currentColNumber;
            CleanUpLineEndingsCommand.IsEnabled = true;
            RemoveOuterWhitespaceCommand.IsEnabled = true;
            //JoinLinesCommand.IsEnabled = true;
            RemoveEmptyLinesCommand.IsEnabled = true;
            //RemoveConsecutiveEmptyLinesCommand.IsEnabled = true;
            //SelectTagContentsCommand.IsEnabled = true;
        }

        #region JoinLines Command Property Members

        private Command.RelayCommand _joinLinesCommand = null;

        public Command.RelayCommand JoinLinesCommand
        {
            get
            {
                if (_joinLinesCommand == null)
                    _joinLinesCommand = new Command.RelayCommand(OnJoinLines, false, true);

                return _joinLinesCommand;
            }
        }

        protected virtual void OnJoinLines(object parameter)
        {
            // TODO: Implement OnJoinLines Logic
        }

        #endregion

        #region RemoveEmptyLines Command Property Members

        private Command.RelayCommand _removeEmptyLinesCommand = null;

        public Command.RelayCommand RemoveEmptyLinesCommand
        {
            get
            {
                if (_removeEmptyLinesCommand == null)
                    _removeEmptyLinesCommand = new Command.RelayCommand(OnRemoveEmptyLines, false);

                return _removeEmptyLinesCommand;
            }
        }

        protected virtual void OnRemoveEmptyLines(object parameter)
        {
            string startText, endText;
            string text = SplitTextSelection(out startText, out endText).Trim();
            text = SsmlTextBox.Text.Trim();
            if (text.Length > 0)
                text = String.Join(Environment.NewLine, Model.TextLine.Split(text).Where(l => !String.IsNullOrWhiteSpace(l.LineContent)).Select(l => l.LineContent));
            text = startText + text + endText;
            if (text != SsmlTextBox.Text)
                SsmlTextBox.Text = text;
        }

        #endregion

        #region RemoveConsecutiveEmptyLines Command Property Members

        private Command.RelayCommand _removeConsecutiveEmptyLinesCommand = null;

        public Command.RelayCommand RemoveConsecutiveEmptyLinesCommand
        {
            get
            {
                if (_removeConsecutiveEmptyLinesCommand == null)
                    _removeConsecutiveEmptyLinesCommand = new Command.RelayCommand(OnRemoveConsecutiveEmptyLines, false, true);

                return _removeConsecutiveEmptyLinesCommand;
            }
        }

        protected virtual void OnRemoveConsecutiveEmptyLines(object parameter)
        {
            // TODO: Implement OnRemoveConsecutiveEmptyLines Logic
        }

        #endregion

        #region SelectCurrentTag Command Property Members

        private Command.RelayCommand _selectCurrentTagCommand = null;

        public Command.RelayCommand SelectCurrentTagCommand
        {
            get
            {
                if (_selectCurrentTagCommand == null)
                    _selectCurrentTagCommand = new Command.RelayCommand(OnSelectCurrentTag, false, true);

                return _selectCurrentTagCommand;
            }
        }

        protected virtual void OnSelectCurrentTag(object parameter)
        {
            // TODO: Implement OnSelectCurrentTag Logic
        }

        #endregion

        #region SelectTagContents Command Property Members

        private Command.RelayCommand _selectTagContentsCommand = null;

        public Command.RelayCommand SelectTagContentsCommand
        {
            get
            {
                if (_selectTagContentsCommand == null)
                    _selectTagContentsCommand = new Command.RelayCommand(OnSelectTagContents, false, true);

                return _selectTagContentsCommand;
            }
        }

        protected virtual void OnSelectTagContents(object parameter)
        {
            // TODO: Implement OnSelectTagContents Logic
        }

        #endregion

        #region GoToLine Command Property Members

        private Command.RelayCommand _goToLineCommand = null;

        public Command.RelayCommand GoToLineCommand
        {
            get
            {
                if (_goToLineCommand == null)
                    _goToLineCommand = new Command.RelayCommand(OnGoToLine, false, true);

                return _goToLineCommand;
            }
        }

        protected virtual void OnGoToLine(object parameter)
        {
            // TODO: Implement OnGoToLine Logic
        }

        #endregion

        #region FindText Command Property Members

        private Command.RelayCommand _findTextCommand = null;

        public Command.RelayCommand FindTextCommand
        {
            get
            {
                if (_findTextCommand == null)
                    _findTextCommand = new Command.RelayCommand(OnFindText, false, true);

                return _findTextCommand;
            }
        }

        protected virtual void OnFindText(object parameter)
        {
            // TODO: Implement OnFindText Logic
        }

        #endregion

        #region FindNext Command Property Members

        private Command.RelayCommand _findNextCommand = null;

        public Command.RelayCommand FindNextCommand
        {
            get
            {
                if (_findNextCommand == null)
                    _findNextCommand = new Command.RelayCommand(OnFindNext, false, true);

                return _findNextCommand;
            }
        }

        protected virtual void OnFindNext(object parameter)
        {
            // TODO: Implement OnFindNext Logic
        }

        #endregion

        #region ReplaceText Command Property Members

        private Command.RelayCommand _replaceTextCommand = null;

        public Command.RelayCommand ReplaceTextCommand
        {
            get
            {
                if (_replaceTextCommand == null)
                    _replaceTextCommand = new Command.RelayCommand(OnReplaceText, false, true);

                return _replaceTextCommand;
            }
        }

        protected virtual void OnReplaceText(object parameter)
        {
            // TODO: Implement OnReplaceText Logic
        }

        #endregion

        #endregion

        #region Insert

        #region InsertFemaleVoice Command Property Members

        private Command.RelayCommand _insertFemaleVoiceCommand = null;

        public Command.RelayCommand InsertFemaleVoiceCommand
        {
            get
            {
                if (_insertFemaleVoiceCommand == null)
                    _insertFemaleVoiceCommand = new Command.RelayCommand(OnInsertFemaleVoice, false, true);

                return _insertFemaleVoiceCommand;
            }
        }

        protected virtual void OnInsertFemaleVoice(object parameter)
        {
            // TODO: Implement OnInsertFemaleVoice Logic
        }

        #endregion

        #region InsertMaleVoice Command Property Members

        private Command.RelayCommand _insertMaleVoiceCommand = null;

        public Command.RelayCommand InsertMaleVoiceCommand
        {
            get
            {
                if (_insertMaleVoiceCommand == null)
                    _insertMaleVoiceCommand = new Command.RelayCommand(OnInsertMaleVoice, false, true);

                return _insertMaleVoiceCommand;
            }
        }

        protected virtual void OnInsertMaleVoice(object parameter)
        {
            // TODO: Implement OnInsertMaleVoice Logic
        }

        #endregion

        #region InsertGenderNeutralVoice Command Property Members

        private Command.RelayCommand _insertGenderNeutralVoiceCommand = null;

        public Command.RelayCommand InsertGenderNeutralVoiceCommand
        {
            get
            {
                if (_insertGenderNeutralVoiceCommand == null)
                    _insertGenderNeutralVoiceCommand = new Command.RelayCommand(OnInsertGenderNeutralVoice, false, true);

                return _insertGenderNeutralVoiceCommand;
            }
        }

        protected virtual void OnInsertGenderNeutralVoice(object parameter)
        {
            // TODO: Implement OnInsertGenderNeutralVoice Logic
        }

        #endregion

        #region InsertParagraph Command Property Members

        private Command.RelayCommand _insertParagraphCommand = null;

        public Command.RelayCommand InsertParagraphCommand
        {
            get
            {
                if (_insertParagraphCommand == null)
                    _insertParagraphCommand = new Command.RelayCommand(OnInsertParagraph, false, true);

                return _insertParagraphCommand;
            }
        }

        protected virtual void OnInsertParagraph(object parameter)
        {
            // TODO: Implement OnInsertParagraph Logic
        }

        #endregion

        #region InsertSentence Command Property Members

        private Command.RelayCommand _insertSentenceCommand = null;

        public Command.RelayCommand InsertSentenceCommand
        {
            get
            {
                if (_insertSentenceCommand == null)
                    _insertSentenceCommand = new Command.RelayCommand(OnInsertSentence, false, true);

                return _insertSentenceCommand;
            }
        }

        protected virtual void OnInsertSentence(object parameter)
        {
            // TODO: Implement OnInsertSentence Logic
        }

        #endregion

        #region Substitution Command Property Members

        private Command.RelayCommand _substitutionCommand = null;

        public Command.RelayCommand SubstitutionCommand
        {
            get
            {
                if (_substitutionCommand == null)
                    _substitutionCommand = new Command.RelayCommand(OnSubstitution, false, true);

                return _substitutionCommand;
            }
        }

        protected virtual void OnSubstitution(object parameter)
        {
            // TODO: Implement OnSubstitution Logic
        }

        #endregion

        #region SpellOut Command Property Members

        private Command.RelayCommand _spellOutCommand = null;

        public Command.RelayCommand SpellOutCommand
        {
            get
            {
                if (_spellOutCommand == null)
                    _spellOutCommand = new Command.RelayCommand(OnSpellOut, false, true);

                return _spellOutCommand;
            }
        }

        protected virtual void OnSpellOut(object parameter)
        {
            // TODO: Implement OnSpellOut Logic
        }

        #endregion

        #region SayAs Command Property Members

        private Command.RelayCommand _sayAsCommand = null;

        public Command.RelayCommand SayAsCommand
        {
            get
            {
                if (_sayAsCommand == null)
                    _sayAsCommand = new Command.RelayCommand(OnSayAs, false, true);

                return _sayAsCommand;
            }
        }

        protected virtual void OnSayAs(object parameter)
        {
            // TODO: Implement OnSayAs Logic
        }

        #endregion

        #region AutoReplace Command Property Members

        private Command.RelayCommand _autoReplaceCommand = null;

        public Command.RelayCommand AutoReplaceCommand
        {
            get
            {
                if (_autoReplaceCommand == null)
                    _autoReplaceCommand = new Command.RelayCommand(OnAutoReplace, false, true);

                return _autoReplaceCommand;
            }
        }

        protected virtual void OnAutoReplace(object parameter)
        {
            // TODO: Implement OnAutoReplace Logic
        }

        #endregion

        #region InsertBookmark Command Property Members

        private Command.RelayCommand _insertBookmarkCommand = null;

        public Command.RelayCommand InsertBookmarkCommand
        {
            get
            {
                if (_insertBookmarkCommand == null)
                    _insertBookmarkCommand = new Command.RelayCommand(OnInsertBookmark, false, true);

                return _insertBookmarkCommand;
            }
        }

        protected virtual void OnInsertBookmark(object parameter)
        {
            // TODO: Implement OnInsertBookmark Logic
        }

        #endregion

        #region InsertAudioFile Command Property Members

        private Command.RelayCommand _insertAudioFileCommand = null;

        public Command.RelayCommand InsertAudioFileCommand
        {
            get
            {
                if (_insertAudioFileCommand == null)
                    _insertAudioFileCommand = new Command.RelayCommand(OnInsertAudioFile, false, true);

                return _insertAudioFileCommand;
            }
        }

        protected virtual void OnInsertAudioFile(object parameter)
        {
            // TODO: Implement OnInsertAudioFile Logic
        }

        #endregion

        #region Dictate Command Property Members

        private Command.RelayCommand _dictateCommand = null;

        public Command.RelayCommand DictateCommand
        {
            get
            {
                if (_dictateCommand == null)
                    _dictateCommand = new Command.RelayCommand(OnDictate, false, true);

                return _dictateCommand;
            }
        }

        protected virtual void OnDictate(object parameter)
        {
            // TODO: Implement OnDictate Logic
        }

        #endregion

        #endregion

        #region Generate

        #region SpeakAllText Command Property Members

        private Command.RelayCommand _speakAllTextCommand = null;

        public Command.RelayCommand SpeakAllTextCommand
        {
            get
            {
                if (_speakAllTextCommand == null)
                    _speakAllTextCommand = new Command.RelayCommand(OnSpeakAllText, false, true);

                return _speakAllTextCommand;
            }
        }

        protected virtual void OnSpeakAllText(object parameter)
        {
            // TODO: Implement OnSpeakAllText Logic
        }

        #endregion

        #region ExportAsWav Command Property Members

        private Command.RelayCommand _exportAsWavCommand = null;

        public Command.RelayCommand ExportAsWavCommand
        {
            get
            {
                if (_exportAsWavCommand == null)
                    _exportAsWavCommand = new Command.RelayCommand(OnExportAsWav, false, true);

                return _exportAsWavCommand;
            }
        }

        protected virtual void OnExportAsWav(object parameter)
        {
            // TODO: Implement OnExportAsWav Logic
        }

        #endregion

        #endregion

        #region DefaultSynthSettings Command Property Members

        private Command.RelayCommand _defaultSynthSettingsCommand = null;

        public Command.RelayCommand DefaultSynthSettingsCommand
        {
            get
            {
                if (_defaultSynthSettingsCommand == null)
                    _defaultSynthSettingsCommand = new Command.RelayCommand(OnDefaultSynthSettings, false, true);

                return _defaultSynthSettingsCommand;
            }
        }

        protected virtual void OnDefaultSynthSettings(object parameter)
        {
            // TODO: Implement OnDefaultSynthSettings Logic
        }

        #endregion

        #region AboutSsmlNotePad Command Property Members

        private Command.RelayCommand _aboutSsmlNotePadCommand = null;

        public Command.RelayCommand AboutSsmlNotePadCommand
        {
            get
            {
                if (_aboutSsmlNotePadCommand == null)
                    _aboutSsmlNotePadCommand = new Command.RelayCommand(OnAboutSsmlNotePad, false, true);

                return _aboutSsmlNotePadCommand;
            }
        }

        protected virtual void OnAboutSsmlNotePad(object parameter)
        {
            // TODO: Implement OnAboutSsmlNotePad Logic
        }

        #endregion

        #region ShowValidationMessages Command Property Members

        private Command.RelayCommand _showValidationMessagesCommand = null;

        public Command.RelayCommand ShowValidationMessagesCommand
        {
            get
            {
                if (_showValidationMessagesCommand == null)
                    _showValidationMessagesCommand = new Command.RelayCommand(OnShowValidationMessages, false, true);

                return _showValidationMessagesCommand;
            }
        }

        protected virtual void OnShowValidationMessages(object parameter)
        {
            if (ValidationMessages.Count > 0)
                DisplayErrorsPopup = true;
        }

        #endregion

        #region HideValidationMessages Command Property Members

        private Command.RelayCommand _hideValidationMessagesCommand = null;

        public Command.RelayCommand HideValidationMessagesCommand
        {
            get
            {
                if (_hideValidationMessagesCommand == null)
                    _hideValidationMessagesCommand = new Command.RelayCommand(OnHideValidationMessages, false, true);

                return _hideValidationMessagesCommand;
            }
        }

        protected virtual void OnHideValidationMessages(object parameter)
        {
            DisplayErrorsPopup = false;
        }

        #endregion

        #region ShowFileSaveMessages Command Property Members

        private Command.RelayCommand _showFileSaveMessagesCommand = null;

        public Command.RelayCommand ShowFileSaveMessagesCommand
        {
            get
            {
                if (_showFileSaveMessagesCommand == null)
                    _showFileSaveMessagesCommand = new Command.RelayCommand(OnShowFileSaveMessages, false, true);

                return _showFileSaveMessagesCommand;
            }
        }

        protected virtual void OnShowFileSaveMessages(object parameter)
        {
            // TODO: Implement OnShowFileSaveMessages Logic
        }

        #endregion

        #endregion

        #region SelectAfterInsert Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="SelectAfterInsert"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SelectAfterInsert = "SelectAfterInsert";

        /// <summary>
        /// Identifies the <see cref="SelectAfterInsert"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectAfterInsertProperty = DependencyProperty.Register(DependencyPropertyName_SelectAfterInsert, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(true));

        /// <summary>
        /// Indicates whether the content should be selected after it is inserted.
        /// </summary>
        public bool SelectAfterInsert
        {
            get { return (bool)(GetValue(SelectAfterInsertProperty)); }
            set { SetValue(SelectAfterInsertProperty, value); }
        }

        #endregion

        #region DisplayErrorsPopup Property Members

        /// <summary>
        /// Defines the name for the <see cref="DisplayErrorsPopup"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_DisplayErrorsPopup = "DisplayErrorsPopup";

        /// <summary>
        /// Identifies the <see cref="DisplayErrorsPopup"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayErrorsPopupProperty = DependencyProperty.Register(DependencyPropertyName_DisplayErrorsPopup, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Indicates whether errors grid in is shown.
        /// </summary>
        public bool DisplayErrorsPopup
        {
            get { return (bool)(GetValue(DisplayErrorsPopupProperty)); }
            set { SetValue(DisplayErrorsPopupProperty, value); }
        }
        
        #endregion
        
        #region LineWrapEnabled Property Members

        /// <summary>
        /// Defines the name for the <see cref="LineWrapEnabled"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_LineWrapEnabled = "LineWrapEnabled";

        /// <summary>
        /// Identifies the <see cref="LineWrapEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineWrapEnabledProperty = DependencyProperty.Register(DependencyPropertyName_LineWrapEnabled, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(true,
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as MainWindowVM).LineWrapEnabled_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// Indicates whether line wrapping is enabled in <see cref="SsmlTextBox"/>.
        /// </summary>
        public bool LineWrapEnabled
        {
            get { return (bool)(GetValue(LineWrapEnabledProperty)); }
            set { SetValue(LineWrapEnabledProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="LineWrapEnabled"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="LineWrapEnabled"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="LineWrapEnabled"/> property was changed.</param>
        protected virtual void LineWrapEnabled_PropertyChanged(bool oldValue, bool newValue) { SetLineWrap(); }

        private void SetLineWrap()
        {
            if (LineWrapEnabled)
            {
                SsmlTextBox.TextWrapping = TextWrapping.Wrap;
                SsmlTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            else
            {
                SsmlTextBox.TextWrapping = TextWrapping.NoWrap;
                SsmlTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }

        #endregion

        #region LineNumbers Property Members

        /// <summary>
        /// Defines the name for the <see cref="LineNumbers"/> dependency property.
        /// </summary>
        public const string PropertyName_LineNumbers = "LineNumbers";

        private static readonly DependencyPropertyKey LineNumbersPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LineNumbers, typeof(ObservableCollection<LineNumberVM>), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="LineNumbers"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineNumbersProperty = LineNumbersPropertyKey.DependencyProperty;

        /// <summary>
        /// Contains line numbers to be displayed which are associated with the displayed text from <see cref="SsmlTextBox"/>.
        /// </summary>
        public ObservableCollection<LineNumberVM> LineNumbers
        {
            get { return (ObservableCollection<LineNumberVM>)(GetValue(LineNumbersProperty)); }
            private set { SetValue(LineNumbersPropertyKey, value); }
        }

        #endregion

        #region FileSaveStatus Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="FileSaveStatus"/> dependency property.
        /// </summary>
        public const string PropertyName_FileSaveStatus = "FileSaveStatus";

        private static readonly DependencyPropertyKey FileSaveStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileSaveStatus, typeof(Model.FileSaveStatus), typeof(MainWindowVM),
            new PropertyMetadata(Model.FileSaveStatus.New));

        /// <summary>
        /// Identifies the <see cref="FileSaveStatus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FileSaveStatusProperty = FileSaveStatusPropertyKey.DependencyProperty;

        /// <summary>
        /// Status of last file operation.
        /// </summary>
        public Model.FileSaveStatus FileSaveStatus
        {
            get { return (Model.FileSaveStatus)(GetValue(FileSaveStatusProperty)); }
            private set { SetValue(FileSaveStatusPropertyKey, value); }
        }
        
        #endregion

        #region FileSaveToolBarMessage Property Members

        /// <summary>
        /// Defines the name for the <see cref="FileSaveToolBarMessage"/> dependency property.
        /// </summary>
        public const string PropertyName_FileSaveToolBarMessage = "FileSaveToolBarMessage";

        /// <summary>
        /// Defines the value for the <see cref="FileSaveToolBarMessage"/> dependency property when the associate file is new.
        /// </summary>
        public const string FileSaveMessage_NewFile = "File not saved.";

        private static readonly DependencyPropertyKey FileSaveToolBarMessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileSaveToolBarMessage, typeof(string), typeof(MainWindowVM),
            new PropertyMetadata(FileSaveMessage_NewFile));

        /// <summary>
        /// Identifies the <see cref="FileSaveToolBarMessage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FileSaveToolBarMessageProperty = FileSaveToolBarMessagePropertyKey.DependencyProperty;

        /// <summary>
        /// Status message for last file operation.
        /// </summary>
        public string FileSaveToolBarMessage
        {
            get { return GetValue(FileSaveToolBarMessageProperty) as string; }
            private set { SetValue(FileSaveToolBarMessagePropertyKey, value); }
        }

        #endregion

        #region CurrentFileName Property Members

        /// <summary>
        /// Defines the name for the <see cref="CurrentFileName"/> dependency property.
        /// </summary>
        public const string PropertyName_CurrentFileName = "CurrentFileName";
        
        private static readonly DependencyPropertyKey CurrentFileNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentFileName, typeof(string), typeof(MainWindowVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="CurrentFileName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentFileNameProperty = CurrentFileNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Name of currently loaded file.
        /// </summary>
        public string CurrentFileName
        {
            get { return GetValue(CurrentFileNameProperty) as string; }
            private set { SetValue(CurrentFileNamePropertyKey, value); }
        }

        #endregion

        #region CurrentFullPath Property Members

        /// <summary>
        /// Defines the name for the <see cref="CurrentFullPath"/> dependency property.
        /// </summary>
        public const string PropertyName_CurrentFullPath = "CurrentFullPath";

        private static readonly DependencyPropertyKey CurrentFullPathPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentFullPath, typeof(string), typeof(MainWindowVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="CurrentFullPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentFullPathProperty = CurrentFullPathPropertyKey.DependencyProperty;

        /// <summary>
        /// Full path of currently loaded file.
        /// </summary>
        public string CurrentFullPath
        {
            get { return GetValue(CurrentFullPathProperty) as string; }
            private set { SetValue(CurrentFullPathPropertyKey, value); }
        }

        #endregion

        #region ValidationStatus Property Members

        /// <summary>
        /// Defines the name for the <see cref="ValidationStatus"/> dependency property.
        /// </summary>
        public const string PropertyName_ValidationStatus = "ValidationStatus";

        private static readonly DependencyPropertyKey ValidationStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationStatus, typeof(Model.XmlValidationStatus), typeof(MainWindowVM),
            new PropertyMetadata(Model.XmlValidationStatus.None));

        /// <summary>
        /// Identifies the <see cref="ValidationStatus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationStatusProperty = ValidationStatusPropertyKey.DependencyProperty;

        /// <summary>
        /// Current validation status.
        /// </summary>
        public Model.XmlValidationStatus ValidationStatus
        {
            get { return (Model.XmlValidationStatus)(GetValue(ValidationStatusProperty)); }
            private set { SetValue(ValidationStatusPropertyKey, value); }
        }
        
        #endregion

        #region ValidationToolTip Property Members

        /// <summary>
        /// Defines the name for the <see cref="ValidationToolTip"/> dependency property.
        /// </summary>
        public const string PropertyName_ValidationToolTip = "ValidationToolTip";

        private static readonly DependencyPropertyKey ValidationToolTipPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationToolTip, typeof(string), typeof(MainWindowVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ValidationToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationToolTipProperty = ValidationToolTipPropertyKey.DependencyProperty;

        /// <summary>
        /// Tooltip to display for XML validation.
        /// </summary>
        public string ValidationToolTip
        {
            get { return GetValue(ValidationToolTipProperty) as string; }
            private set { SetValue(ValidationToolTipPropertyKey, value); }
        }
        
        #endregion

        #region CurrentLineNumber Property Members

        /// <summary>
        /// Defines the name for the <see cref="CurrentLineNumber"/> dependency property.
        /// </summary>
        public const string PropertyName_CurrentLineNumber = "CurrentLineNumber";

        private static readonly DependencyPropertyKey CurrentLineNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentLineNumber, typeof(int), typeof(MainWindowVM),
            new PropertyMetadata(1/*, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                (d as MainWindowVM).CurrentLineNumber_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))*/));

        /// <summary>
        /// Identifies the <see cref="CurrentLineNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentLineNumberProperty = CurrentLineNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// Current line number at start of selection or caret.
        /// </summary>
        public int CurrentLineNumber
        {
            get { return (int)(GetValue(CurrentLineNumberProperty)); }
            private set { SetValue(CurrentLineNumberPropertyKey, value); }
        }
        
        #endregion

        #region CurrentColNumber Property Members

        /// <summary>
        /// Defines the name for the <see cref="CurrentColNumber"/> dependency property.
        /// </summary>
        public const string PropertyName_CurrentColNumber = "CurrentColNumber";

        private static readonly DependencyPropertyKey CurrentColNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentColNumber, typeof(int), typeof(MainWindowVM),
            new PropertyMetadata(1));

        /// <summary>
        /// Identifies the <see cref="CurrentColNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentColNumberProperty = CurrentColNumberPropertyKey.DependencyProperty;

        /// <summary>
        /// Current column number at start of selection or caret.
        /// </summary>
        public int CurrentColNumber
        {
            get { return (int)(GetValue(CurrentColNumberProperty)); }
            private set { SetValue(CurrentColNumberPropertyKey, value); }
        }

        #endregion

        #region CurrentSelectionStart Property Members

        /// <summary>
        /// Defines the name for the <see cref="CurrentSelectionStart"/> dependency property.
        /// </summary>
        public const string PropertyName_CurrentSelectionStart = "CurrentSelectionStart";

        private static readonly DependencyPropertyKey CurrentSelectionStartPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentSelectionStart, typeof(int), typeof(MainWindowVM),
            new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="CurrentSelectionStart"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentSelectionStartProperty = CurrentSelectionStartPropertyKey.DependencyProperty;

        /// <summary>
        /// Current start index of selection or caret.
        /// </summary>
        public int CurrentSelectionStart
        {
            get { return (int)(GetValue(CurrentSelectionStartProperty)); }
            private set { SetValue(CurrentSelectionStartPropertyKey, value); }
        }

        #endregion

        #region CurrentSelectionLength Property Members

        /// <summary>
        /// Defines the name for the <see cref="CurrentSelectionLength"/> dependency property.
        /// </summary>
        public const string PropertyName_CurrentSelectionLength = "CurrentSelectionLength";

        private static readonly DependencyPropertyKey CurrentSelectionLengthPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentSelectionLength, typeof(int), typeof(MainWindowVM),
            new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="CurrentSelectionLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentSelectionLengthProperty = CurrentSelectionLengthPropertyKey.DependencyProperty;

        /// <summary>
        /// Current length of selection.
        /// </summary>
        public int CurrentSelectionLength
        {
            get { return (int)(GetValue(CurrentSelectionLengthProperty)); }
            private set { SetValue(CurrentSelectionLengthPropertyKey, value); }
        }

        #endregion

        #region SsmlTextBox Property Members

        /// <summary>
        /// Defines the name for the <see cref="SsmlTextBox"/> dependency property.
        /// </summary>
        public const string PropertyName_SsmlTextBox = "SsmlTextBox";

        private static readonly DependencyPropertyKey SsmlTextBoxPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlTextBox, typeof(TextBox), typeof(MainWindowVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SsmlTextBox"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlTextBoxProperty = SsmlTextBoxPropertyKey.DependencyProperty;

        /// <summary>
        /// Markup editor textbox
        /// </summary>
        public TextBox SsmlTextBox
        {
            get { return (TextBox)(GetValue(SsmlTextBoxProperty)); }
            private set { SetValue(SsmlTextBoxPropertyKey, value); }
        }

        #endregion

        #region ValidationMessages Property Members

        /// <summary>
        /// Defines the name for the <see cref="ValidationMessages"/> dependency property.
        /// </summary>
        public const string PropertyName_ValidationMessages = "ValidationMessages";

        private static readonly DependencyPropertyKey ValidationMessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationMessages, typeof(ObservableCollection<XmlValidationMessageVM>), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ValidationMessages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationMessagesProperty = ValidationMessagesPropertyKey.DependencyProperty;

        /// <summary>
        /// Contains validation messages which are associated with the displayed text from <see cref="SsmlTextBox"/>.
        /// </summary>
        public ObservableCollection<XmlValidationMessageVM> ValidationMessages
        {
            get { return (ObservableCollection<XmlValidationMessageVM>)(GetValue(ValidationMessagesProperty)); }
            private set { SetValue(ValidationMessagesPropertyKey, value); }
        }

        #endregion

        public bool SaveCurrentDocument()
        {
            if (FileSaveStatus == Model.FileSaveStatus.SaveSuccess)
                return true;

            if (FileSaveStatus == Model.FileSaveStatus.New || !File.Exists(CurrentFullPath))
                return SaveCurrentDocumentAs();

            try { File.WriteAllText(CurrentFullPath, SsmlTextBox.Text); }
            catch (Exception exception)
            {
                FileSaveStatus = Model.FileSaveStatus.SaveError;
                FileSaveToolBarMessage = exception.Message;
                return false;
            }
            FileSaveStatus = Model.FileSaveStatus.SaveSuccess;
            FileSaveToolBarMessage = "";
            return true;
        }

        public bool SaveCurrentDocumentAs()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                DefaultExt = App.AppSettingsViewModel.SsmlFileExtension,
                Filter = String.Format("{0} (*{1})|*{1}|XML Files (*.xml)|*.*|All Files (*.*)|*.*", App.AppSettingsViewModel.SsmlFileTypeDescriptionShort, App.AppSettingsViewModel.SsmlFileExtension),
                RestoreDirectory = true,
                Title = "Open SSML File"
            };
            string path = CurrentFullPath;
            if (String.IsNullOrEmpty(path))
            {
                path = App.AppSettingsViewModel.LastSsmlFilePath;
                if (!String.IsNullOrEmpty(path))
                    path = Path.GetDirectoryName(path);
                dialog.InitialDirectory = (String.IsNullOrEmpty(path) || !Directory.Exists(path)) ?  Environment.GetFolderPath(Environment.SpecialFolder.Personal) : path;
            }
            else
            {
                if (File.Exists(path))
                    dialog.FileName = path;
                path = Path.GetDirectoryName(path);
                dialog.InitialDirectory = (Directory.Exists(path)) ? path : Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            bool? dialogResult = dialog.ShowDialog(App.Current.MainWindow);
            if (!(dialogResult.HasValue && dialogResult.Value))
                return false;

            try { File.WriteAllText(dialog.FileName, SsmlTextBox.Text); }
            catch (Exception exception)
            {
                FileSaveStatus = Model.FileSaveStatus.SaveError;
                FileSaveToolBarMessage = exception.Message;
                return false;
            }
            FileSaveStatus = Model.FileSaveStatus.SaveSuccess;
            FileSaveToolBarMessage = "";
            CurrentFileName = Path.GetFileName(dialog.FileName);
            CurrentFullPath = Path.GetFullPath(dialog.FileName);
            return true;
        }
    }
}