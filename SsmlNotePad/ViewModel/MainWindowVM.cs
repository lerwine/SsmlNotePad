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

        public MainWindowVM()
        {
            LineNumbers = new ObservableCollection<LineNumberVM>();
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
                    _newDocumentCommand = new Command.RelayCommand(OnNewDocument);

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
                    _openDocumentCommand = new Command.RelayCommand(OnOpenDocument);

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
                    _saveDocumentCommand = new Command.RelayCommand(OnSaveDocument);

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
                    _saveAsCommand = new Command.RelayCommand(OnSaveAs);

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
                    _pasteEncodedCommand = new Command.RelayCommand(OnPasteEncoded);

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
                    _reformatDocumentCommand = new Command.RelayCommand(OnReformatDocument);

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
                    _cleanUpLineEndingsCommand = new Command.RelayCommand(OnCleanUpLineEndings);

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
                    _removeOuterWhitespaceCommand = new Command.RelayCommand(OnRemoveOuterWhitespace);

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
                    _joinLinesCommand = new Command.RelayCommand(OnJoinLines);

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
                    _removeEmptyLinesCommand = new Command.RelayCommand(OnRemoveEmptyLines);

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
                    _removeConsecutiveEmptyLinesCommand = new Command.RelayCommand(OnRemoveConsecutiveEmptyLines);

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
                    _selectCurrentTagCommand = new Command.RelayCommand(OnSelectCurrentTag);

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
                    _selectTagContentsCommand = new Command.RelayCommand(OnSelectTagContents);

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
                    _goToLineCommand = new Command.RelayCommand(OnGoToLine);

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
                    _findTextCommand = new Command.RelayCommand(OnFindText);

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
                    _findNextCommand = new Command.RelayCommand(OnFindNext);

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
                    _replaceTextCommand = new Command.RelayCommand(OnReplaceText);

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
                    _insertFemaleVoiceCommand = new Command.RelayCommand(OnInsertFemaleVoice);

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
                    _insertMaleVoiceCommand = new Command.RelayCommand(OnInsertMaleVoice);

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
                    _insertGenderNeutralVoiceCommand = new Command.RelayCommand(OnInsertGenderNeutralVoice);

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
                    _insertParagraphCommand = new Command.RelayCommand(OnInsertParagraph);

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
                    _insertSentenceCommand = new Command.RelayCommand(OnInsertSentence);

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
                    _substitutionCommand = new Command.RelayCommand(OnSubstitution);

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
                    _spellOutCommand = new Command.RelayCommand(OnSpellOut);

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
                    _sayAsCommand = new Command.RelayCommand(OnSayAs);

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
                    _autoReplaceCommand = new Command.RelayCommand(OnAutoReplace);

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
                    _insertBookmarkCommand = new Command.RelayCommand(OnInsertBookmark);

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
                    _insertAudioFileCommand = new Command.RelayCommand(OnInsertAudioFile);

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
                    _dictateCommand = new Command.RelayCommand(OnDictate);

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
                    _speakAllTextCommand = new Command.RelayCommand(OnSpeakAllText);

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
                    _exportAsWavCommand = new Command.RelayCommand(OnExportAsWav);

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
                    _defaultSynthSettingsCommand = new Command.RelayCommand(OnDefaultSynthSettings);

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
                    _aboutSsmlNotePadCommand = new Command.RelayCommand(OnAboutSsmlNotePad);

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
                    _showValidationMessagesCommand = new Command.RelayCommand(OnShowValidationMessages);

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
                    _showFileSaveMessagesCommand = new Command.RelayCommand(OnShowFileSaveMessages);

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

        public const string DependencyPropertyName_SelectAfterInsert = "SelectAfterInsert";

        /// <summary>
        /// Identifies the <see cref="SelectAfterInsert"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectAfterInsertProperty = DependencyProperty.Register(DependencyPropertyName_SelectAfterInsert, typeof(bool), typeof(MainWindowVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Whether to select text after it is auto-inserted.
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
        
        #endregion

        #region LineWrapEnabled Property Members

        public const string DependencyPropertyName_LineWrapEnabled = "LineWrapEnabled";

        /// <summary>
        /// Identifies the <see cref="LineWrapEnabled"/> dependency property.
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
        /// Boolean value indicating whether line wrapping is enabled.
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
        /// This gets called after the value associated with the <see cref="LineWrapEnabled"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <see cref="LineWrapEnabled"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <see cref="LineWrapEnabled"/> property was changed.</param>
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

        public const string PropertyName_LineNumbers = "LineNumbers";

        private static readonly DependencyPropertyKey LineNumbersPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LineNumbers, typeof(ObservableCollection<LineNumberVM>), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="LineNumbers"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineNumbersProperty = LineNumbersPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<LineNumberVM> LineNumbers
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<LineNumberVM>)(GetValue(LineNumbersProperty));
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

        #region FileSaveStatus Property Members

        public const string PropertyName_FileSaveStatus = "FileSaveStatus";

        private static readonly DependencyPropertyKey FileSaveStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileSaveStatus, typeof(FileSaveStatus), typeof(MainWindowVM),
                new PropertyMetadata(FileSaveStatus.New));

        /// <summary>
        /// Identifies the <see cref="FileSaveStatus"/> dependency property.
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

        #region FileSaveToolBarMessage Property Members

        public const string PropertyName_FileSaveToolBarMessage = "FileSaveToolBarMessage";

        private static readonly DependencyPropertyKey FileSaveToolBarMessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FileSaveToolBarMessage, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="FileSaveToolBarMessage"/> dependency property.
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

        #region ValidationStatus Property Members

        public const string PropertyName_ValidationStatus = "ValidationStatus";

        private static readonly DependencyPropertyKey ValidationStatusPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ValidationStatus, typeof(XmlValidationStatus), typeof(MainWindowVM),
                new PropertyMetadata(XmlValidationStatus.None));

        /// <summary>
        /// Identifies the <see cref="ValidationStatus"/> dependency property.
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
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ValidationToolTip"/> dependency property.
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

        #region CurrentLineNumber Property Members

        public const string PropertyName_CurrentLineNumber = "CurrentLineNumber";

        private static readonly DependencyPropertyKey CurrentLineNumberPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CurrentLineNumber, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(1));

        /// <summary>
        /// Identifies the <see cref="CurrentLineNumber"/> dependency property.
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
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="CurrentColNumber"/> dependency property.
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

        #region SsmlTextBox Property Members

        public const string PropertyName_SsmlTextBox = "SsmlTextBox";

        private static readonly DependencyPropertyKey SsmlTextBoxPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlTextBox, typeof(TextBox), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SsmlTextBox"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlTextBoxProperty = SsmlTextBoxPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public TextBox SsmlTextBox
        {
            get
            {
                if (CheckAccess())
                    return (TextBox)(GetValue(SsmlTextBoxProperty));
                return Dispatcher.Invoke(() => SsmlTextBox);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SsmlTextBoxPropertyKey, value);
                else
                    Dispatcher.Invoke(() => SsmlTextBox = value);
            }
        }

        #endregion

        private void SsmlTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SsmlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SsmlTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}