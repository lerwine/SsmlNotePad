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
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class MainWindowVM : DependencyObject
    {
        private object _syncRoot = new object();
        private Common.SupercessiveTaskState<string, Model.TextLine[]> _textLines = new Common.SupercessiveTaskState<string, Model.TextLine[]>("", new Model.TextLine[] { new Model.TextLine(1, 0, "", "") });
        private Common.SupercessiveTaskState<Tuple<Model.IndexAndOffset[], Model.TextLine[]>, Model.IndexAndOffset[]> _lineNumbers;
        private Common.SupercessiveTaskState<Model.TextLine[], Common.XmlValidationResult> _xmlValidation;
        
        public MainWindowVM()
        {
            _innerValidationMessages.CollectionChanged += ValidationMessages_CollectionChanged;
            ValidationMessages = new ReadOnlyObservableCollection<ViewModelValidationMessageVM>(_innerValidationMessages);
            AddValidationError(1, 1, ViewModelValidationMessageVM.ValidationMessage_NoXmlData, null, Model.XmlValidationStatus.Warning);
            _lineNumbers = new Common.SupercessiveTaskState<Tuple<Model.IndexAndOffset[], Model.TextLine[]>, 
                Model.IndexAndOffset[]>(new Tuple<Model.IndexAndOffset[], Model.TextLine[]>(new Model.IndexAndOffset[]
                {
                    new Model.IndexAndOffset(1, 0.0)
                }, _textLines.CurrentResult), new Model.IndexAndOffset[] { new Model.IndexAndOffset(1, 0.0) });
            _lineNumbers.OnStateChanged += LineNumbers_OnStateChanged;
            _xmlValidation = new Common.SupercessiveTaskState<Model.TextLine[], Common.XmlValidationResult>(_textLines.CurrentResult, new Common.XmlValidationResult(Model.XmlValidationStatus.Warning, "No XML data provided"));
            _xmlValidation.OnStateChanged += XmlValidation_OnStateChanged;
            _textLines.OnStateChanged += TextLines_OnStateChanged;
            LineNumbers = new ObservableCollection<LineNumberVM>(_lineNumbers.CurrentResult.Select(t => new LineNumberVM(t.Index, t.Top)));
            SsmlTextBox = new TextBox
            {
                AcceptsReturn = true,
                AcceptsTab = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            SetLineWrap();
            SsmlTextBox.LayoutUpdated += SsmlTextBox_LayoutUpdated;
            SsmlTextBox.TextChanged += SsmlTextBox_TextChanged;
            SsmlTextBox.SelectionChanged += SsmlTextBox_SelectionChanged;
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
            // TODO: Implement OnNewDocument Logic
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
            // TODO: Implement OnOpenDocument Logic
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
            // TODO: Implement OnSaveDocument Logic
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
            // TODO: Implement OnSaveAs Logic
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
                    _reformatDocumentCommand = new Command.RelayCommand(OnReformatDocument, false, true);

                return _reformatDocumentCommand;
            }
        }

        protected virtual void OnReformatDocument(object parameter)
        {
            // TODO: Implement OnReformatDocument Logic
        }

        #endregion

        #region CleanUpLineEndings Command Property Members

        private Command.RelayCommand _cleanUpLineEndingsCommand = null;

        public Command.RelayCommand CleanUpLineEndingsCommand
        {
            get
            {
                if (_cleanUpLineEndingsCommand == null)
                    _cleanUpLineEndingsCommand = new Command.RelayCommand(OnCleanUpLineEndings, false, true);

                return _cleanUpLineEndingsCommand;
            }
        }

        protected virtual void OnCleanUpLineEndings(object parameter)
        {
            // TODO: Implement OnCleanUpLineEndings Logic
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

        protected virtual void OnRemoveOuterWhitespace(object parameter)
        {
            // TODO: Implement OnRemoveOuterWhitespace Logic
        }

        #endregion

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
                    _removeEmptyLinesCommand = new Command.RelayCommand(OnRemoveEmptyLines, false, true);

                return _removeEmptyLinesCommand;
            }
        }

        protected virtual void OnRemoveEmptyLines(object parameter)
        {
            // TODO: Implement OnRemoveEmptyLines Logic
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
            // TODO: Implement OnShowValidationMessages Logic
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

        #region ValidationStatus Property Members

        /// <summary>
        /// Defines the name for the <see cref="ValidationStatus"/> dependency property.
        /// </summary>
        public const string PropertyName_ValidationStatus = "ValidationStatus";

        private static readonly DependencyPropertyKey ValidationStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationStatus, typeof(Model.XmlValidationStatus), typeof(MainWindowVM),
            new PropertyMetadata(Model.XmlValidationStatus.Warning));

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
            new PropertyMetadata(ViewModelValidationMessageVM.ValidationMessage_NoXmlData));

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

        private static readonly DependencyPropertyKey ValidationMessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationMessages,
            typeof(ReadOnlyObservableCollection<ViewModelValidationMessageVM>), typeof(MainWindowVM), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ValidationMessages"/> read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationMessagesProperty = ValidationMessagesPropertyKey.DependencyProperty;

        private ObservableCollection<ViewModelValidationMessageVM> _innerValidationMessages = new ObservableCollection<ViewModelValidationMessageVM>();

        /// <summary>
        /// Inner collection for <see cref="ValidationMessages"/>.
        /// </summary>
        protected ObservableCollection<ViewModelValidationMessageVM> InnerValidationMessages { get { return _innerValidationMessages; } }

        /// <summary>
        /// XML validation messages.
        /// </summary>
        public ReadOnlyObservableCollection<ViewModelValidationMessageVM> ValidationMessages
        {
            get { return (ReadOnlyObservableCollection<ViewModelValidationMessageVM>)(GetValue(ValidationMessagesProperty)); }
            private set { SetValue(ValidationMessagesPropertyKey, value); }
        }
        
        /// <summary>
        /// This gets called when an item in <see cref="ValidationMessages"/> is added, removed, changed, moved, or the entire collection is refreshed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Information about the event.</param>
        protected virtual void ValidationMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO: Implement MainWindowVM.ValidationMessages_CollectionChanged(object, NotifyCollectionChangedEventArgs)
        }

        #endregion

        private IEnumerable<Model.IndexAndOffset> GetLineOffsets()
        {
            for (int lineIndex = SsmlTextBox.GetFirstVisibleLineIndex(); lineIndex <= SsmlTextBox.GetLastVisibleLineIndex(); lineIndex++)
                yield return new Model.IndexAndOffset(lineIndex + 1, SsmlTextBox.GetRectFromCharacterIndex(SsmlTextBox.GetCharacterIndexFromLineIndex(lineIndex)).Top);
        }

        private Model.IndexAndOffset[] GenerateLineNumberValues(Tuple<Model.IndexAndOffset[], Model.TextLine[]> offsetsAndLines, CancellationToken cancellationToken)
        {
            return GenerateLineNumberValues(offsetsAndLines.Item1, offsetsAndLines.Item2, cancellationToken);
        }

        private Model.IndexAndOffset[] GenerateLineNumberValues(Model.IndexAndOffset[] visiblineLineIndexesAndOffsets, Model.TextLine[] sourceLines,
            CancellationToken cancellationToken)
        {
            if (visiblineLineIndexesAndOffsets.Length == 0 || sourceLines.Length == 0)
                return new Model.IndexAndOffset[] { new Model.IndexAndOffset(1, 0.0) };
            int currentLineIndex = visiblineLineIndexesAndOffsets[0].Index;
            int lastLineIndex = visiblineLineIndexesAndOffsets[visiblineLineIndexesAndOffsets.Length - 1].Index;
            return sourceLines.SkipWhile(s => s.Index < currentLineIndex).TakeWhile(s => !cancellationToken.IsCancellationRequested && s.Index <= lastLineIndex)
                .Select(s =>
                {
                    while (s.Index > visiblineLineIndexesAndOffsets[currentLineIndex].Index)
                        currentLineIndex++;
                    return new Model.IndexAndOffset(s.LineNumber, visiblineLineIndexesAndOffsets[currentLineIndex].Top);
                }).DefaultIfEmpty(new Model.IndexAndOffset(1, 0.0)).ToArray();
        }

        private void SsmlTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            using (Common.SynchronizedStateChange<Common.SupercedableTaskState<string, Model.TextLine[]>> stateChange = _textLines.ChangeState())
            {
                Model.TextLine[] lines;
                try { lines = stateChange.CurrentState.Result; } catch { lines = _textLines.CurrentResult; }
                if (Dispatcher.CheckAccess())
                    UpdateStatusBarLineAndCol(lines);
                else
                    Dispatcher.Invoke(() => UpdateStatusBarLineAndCol(lines));
            }
        }

        private void SsmlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _textLines.StartNew(SsmlTextBox.Text, (text, cancellationToken) =>
            {
                return Model.TextLine.Split(text, cancellationToken).ToArray();
            });
        }

        private void SsmlTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            _textLines.StartNew(SsmlTextBox.Text, (text, cancellationToken) =>
            {
                return Model.TextLine.Split(text, cancellationToken).ToArray();
            });
            Model.IndexAndOffset[] lineOffsets = (CheckAccess()) ? GetLineOffsets().ToArray() : Dispatcher.Invoke<Model.IndexAndOffset[]>(() => GetLineOffsets().ToArray());
            using (Common.SynchronizedStateChange<Common.SupercedableTaskState<string, Model.TextLine[]>> changeState = _textLines.ChangeState())
            {
                Model.TextLine[] lines;
                try { lines = changeState.CurrentState.Result; } catch { lines = _textLines.CurrentResult; }
                _lineNumbers.StartNew(new Tuple<Model.IndexAndOffset[], Model.TextLine[]>(lineOffsets, lines), GenerateLineNumberValues);
            }
        }

        private void TextLines_OnStateChanged(object sender, Common.SynchronizedStateEventArgs<Common.SupercedableTaskState<string, Model.TextLine[]>> e)
        {
            Model.TextLine[] lines;
            try { lines = e.CurrentState.Result; } catch { lines = _textLines.CurrentResult; }
            _xmlValidation.StartNew(lines, (l, c) => Common.XmlValidationResult.Create(this, c, l));
        }

        private void LineNumbers_OnStateChanged(object sender, Common.SynchronizedStateEventArgs<Common.SupercedableTaskState<Tuple<Model.IndexAndOffset[], Model.TextLine[]>, Model.IndexAndOffset[]>> e)
        {
            Model.IndexAndOffset[] lineOffsets;
            try { lineOffsets = e.CurrentState.Result; } catch { lineOffsets = _lineNumbers.CurrentResult; }
            Model.TextLine[] lines = _textLines.CurrentResult;
            Dispatcher.Invoke(() =>
            {
                UpdateStatusBarLineAndCol(lines);
                int end = (lineOffsets.Length > LineNumbers.Count) ? LineNumbers.Count : lineOffsets.Length;
                for (int i = 0; i < end; i++)
                {
                    LineNumbers[i].Margin = new Thickness(0.0, lineOffsets[i].Top, 0.0, 0.0);
                    LineNumbers[i].Number = lineOffsets[i].Index;
                }
                if (lineOffsets.Length > LineNumbers.Count)
                {
                    for (int i = LineNumbers.Count; i < lineOffsets.Length; i++)
                        LineNumbers.Add(new LineNumberVM(lineOffsets[i].Index, lineOffsets[i].Top));
                }
                else
                {
                    while (LineNumbers.Count > lineOffsets.Length)
                        LineNumbers.RemoveAt(lineOffsets.Length);
                }
            });
        }

        private void XmlValidation_OnStateChanged(object sender, Common.SynchronizedStateEventArgs<Common.SupercedableTaskState<Model.TextLine[], Common.XmlValidationResult>> e)
        {
            Common.XmlValidationResult result;
            try { result = e.CurrentState.Result; } catch { result = _xmlValidation.CurrentResult; }
            Dispatcher.Invoke(() =>
            {
                ValidationToolTip = result.Message;
                ValidationStatus = result.Status;
            });
        }
        
        private void UpdateStatusBarLineAndCol(Model.TextLine[] lines)
        {
            int absIndex = SsmlTextBox.SelectionStart;
            Model.TextLine currentLine = lines.TakeWhile(l => l.Index < absIndex).LastOrDefault();
            if (currentLine == null)
            {
                CurrentLineNumber = 1;
                CurrentColNumber = absIndex + 1;
            }
            else
            {
                CurrentLineNumber = currentLine.LineNumber;
                CurrentColNumber = (absIndex - currentLine.Index) + 1;
            }
        }

        internal void ClearValidationErrors() { _innerValidationMessages.Clear(); }

        internal void AddValidationError(int lineNumber, int linePosition, string message, Exception exception, Model.XmlValidationStatus xmlValidationStatus)
        {
            _innerValidationMessages.Add(new ViewModelValidationMessageVM(PropertyName_ValidationMessages, message, exception, lineNumber, linePosition, (xmlValidationStatus < Model.XmlValidationStatus.Error)));
        }
    }
}