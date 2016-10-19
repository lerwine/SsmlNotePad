using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.ViewModel;
using System.ComponentModel;
using System.Diagnostics;
using Erwine.Leonard.T.SsmlNotePad.Model;
using System.Windows.Controls;
using System.Windows;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for OtherViewModels
    /// </summary>
    [TestClass]
    public class MainViewModelUnitTest
    {
        public MainViewModelUnitTest() { }

        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void MainViewModelConstructorTestMethod()
        {
            MainWindowVM target = new MainWindowVM();
            
            Assert.IsNotNull(target.AboutSsmlNotePadCommand);
            Assert.IsFalse(target.AboutSsmlNotePadCommand.AllowSimultaneousExecute);
            // TODO: Implement AboutSsmlNotePadCommand
            Assert.IsFalse(target.AboutSsmlNotePadCommand.IsEnabled);

            Assert.IsNotNull(target.AutoReplaceCommand);
            Assert.IsFalse(target.AutoReplaceCommand.AllowSimultaneousExecute);
            // TODO: Implement AutoReplaceCommand
            Assert.IsFalse(target.AutoReplaceCommand.IsEnabled);


            Assert.IsNotNull(target.CleanUpLineEndingsCommand);
            Assert.IsFalse(target.CleanUpLineEndingsCommand.AllowSimultaneousExecute);
            // TODO: Implement CleanUpLineEndingsCommand
            Assert.IsFalse(target.CleanUpLineEndingsCommand.IsEnabled);

            Assert.IsNotNull(target.CurrentColNumber);
            Assert.AreEqual(1, target.CurrentColNumber);

            Assert.IsNotNull(target.CurrentLineNumber);
            Assert.AreEqual(1, target.CurrentLineNumber);

            Assert.IsNotNull(target.DefaultSynthSettingsCommand);
            Assert.IsFalse(target.DefaultSynthSettingsCommand.AllowSimultaneousExecute);
            // TODO: Implement DefaultSynthSettingsCommand
            Assert.IsFalse(target.DefaultSynthSettingsCommand.IsEnabled);

            Assert.IsNotNull(target.DictateCommand);
            Assert.IsFalse(target.DictateCommand.AllowSimultaneousExecute);
            // TODO: Implement DictateCommand
            Assert.IsFalse(target.DictateCommand.IsEnabled);

            Assert.IsNotNull(target.ExportAsWavCommand);
            Assert.IsFalse(target.ExportAsWavCommand.AllowSimultaneousExecute);
            // TODO: Implement ExportAsWavCommand
            Assert.IsFalse(target.ExportAsWavCommand.IsEnabled);

            Assert.IsNotNull(target.FileSaveStatus);
            Assert.AreEqual(FileSaveStatus.New, target.FileSaveStatus);

            Assert.IsNotNull(target.FileSaveToolBarMessage);
            Assert.AreEqual("File not saved.", target.FileSaveToolBarMessage);

            Assert.IsNotNull(target.FindNextCommand);
            Assert.IsFalse(target.FindNextCommand.AllowSimultaneousExecute);
            // TODO: Implement FindNextCommand
            Assert.IsFalse(target.FindNextCommand.IsEnabled);

            Assert.IsNotNull(target.FindTextCommand);
            Assert.IsFalse(target.FindTextCommand.AllowSimultaneousExecute);
            // TODO: Implement FindTextCommand
            Assert.IsFalse(target.FindTextCommand.IsEnabled);

            Assert.IsNotNull(target.GoToLineCommand);
            Assert.IsFalse(target.GoToLineCommand.AllowSimultaneousExecute);
            // TODO: Implement GoToLineCommand
            Assert.IsFalse(target.GoToLineCommand.IsEnabled);

            Assert.IsNotNull(target.InsertAudioFileCommand);
            Assert.IsFalse(target.InsertAudioFileCommand.AllowSimultaneousExecute);
            // TODO: Implement InsertAudioFileCommand
            Assert.IsFalse(target.InsertAudioFileCommand.IsEnabled);

            Assert.IsNotNull(target.InsertBookmarkCommand);
            Assert.IsFalse(target.InsertBookmarkCommand.AllowSimultaneousExecute);
            // TODO: Implement InsertBookmarkCommand
            Assert.IsFalse(target.InsertBookmarkCommand.IsEnabled);

            Assert.IsNotNull(target.InsertFemaleVoiceCommand);
            Assert.IsFalse(target.InsertFemaleVoiceCommand.AllowSimultaneousExecute);
            // TODO: Implement InsertFemaleVoiceCommand
            Assert.IsFalse(target.InsertFemaleVoiceCommand.IsEnabled);

            Assert.IsNotNull(target.InsertGenderNeutralVoiceCommand);
            Assert.IsFalse(target.InsertGenderNeutralVoiceCommand.AllowSimultaneousExecute);
            // TODO: Implement InsertGenderNeutralVoiceCommand
            Assert.IsFalse(target.InsertGenderNeutralVoiceCommand.IsEnabled);

            Assert.IsNotNull(target.InsertMaleVoiceCommand);
            Assert.IsFalse(target.InsertMaleVoiceCommand.AllowSimultaneousExecute);
            // TODO: Implement InsertMaleVoiceCommand
            Assert.IsFalse(target.InsertMaleVoiceCommand.IsEnabled);

            Assert.IsNotNull(target.InsertParagraphCommand);
            Assert.IsFalse(target.InsertParagraphCommand.AllowSimultaneousExecute);
            // TODO: Implement InsertParagraphCommand
            Assert.IsFalse(target.InsertParagraphCommand.IsEnabled);

            Assert.IsNotNull(target.InsertSentenceCommand);
            Assert.IsFalse(target.InsertSentenceCommand.AllowSimultaneousExecute);
            // TODO: Implement InsertSentenceCommand
            Assert.IsFalse(target.InsertSentenceCommand.IsEnabled);

            Assert.IsNotNull(target.JoinLinesCommand);
            Assert.IsFalse(target.JoinLinesCommand.AllowSimultaneousExecute);
            // TODO: Implement JoinLinesCommand
            Assert.IsFalse(target.JoinLinesCommand.IsEnabled);

            Assert.IsNotNull(target.LineNumbers);
            Assert.AreEqual(1, target.LineNumbers.Count);
            Assert.IsNotNull(target.LineNumbers[0]);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Top);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Left);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Right);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Bottom);
            Assert.AreEqual(1, target.LineNumbers[0].Number);

            Assert.IsTrue(target.LineWrapEnabled);

            Assert.IsNotNull(target.NewDocumentCommand);
            Assert.IsFalse(target.NewDocumentCommand.AllowSimultaneousExecute);
            // TODO: Implement NewDocumentCommand
            Assert.IsFalse(target.NewDocumentCommand.IsEnabled);

            Assert.IsNotNull(target.OpenDocumentCommand);
            Assert.IsFalse(target.OpenDocumentCommand.AllowSimultaneousExecute);
            // TODO: Implement OpenDocumentCommand
            Assert.IsFalse(target.OpenDocumentCommand.IsEnabled);

            Assert.IsNotNull(target.PasteEncodedCommand);
            Assert.IsFalse(target.PasteEncodedCommand.AllowSimultaneousExecute);
            // TODO: Implement PasteEncodedCommand
            Assert.IsFalse(target.PasteEncodedCommand.IsEnabled);

            Assert.IsNotNull(target.ReformatDocumentCommand);
            Assert.IsFalse(target.ReformatDocumentCommand.AllowSimultaneousExecute);
            // TODO: Implement ReformatDocumentCommand
            Assert.IsFalse(target.ReformatDocumentCommand.IsEnabled);

            Assert.IsNotNull(target.RemoveConsecutiveEmptyLinesCommand);
            Assert.IsFalse(target.RemoveConsecutiveEmptyLinesCommand.AllowSimultaneousExecute);
            // TODO: Implement RemoveConsecutiveEmptyLinesCommand
            Assert.IsFalse(target.RemoveConsecutiveEmptyLinesCommand.IsEnabled);

            Assert.IsNotNull(target.RemoveEmptyLinesCommand);
            Assert.IsFalse(target.RemoveEmptyLinesCommand.AllowSimultaneousExecute);
            // TODO: Implement RemoveEmptyLinesCommand
            Assert.IsFalse(target.RemoveEmptyLinesCommand.IsEnabled);

            Assert.IsNotNull(target.RemoveOuterWhitespaceCommand);
            Assert.IsFalse(target.RemoveOuterWhitespaceCommand.AllowSimultaneousExecute);
            // TODO: Implement RemoveOuterWhitespaceCommand
            Assert.IsFalse(target.RemoveOuterWhitespaceCommand.IsEnabled);

            Assert.IsNotNull(target.ReplaceTextCommand);
            Assert.IsFalse(target.ReplaceTextCommand.AllowSimultaneousExecute);
            // TODO: Implement ReplaceTextCommand
            Assert.IsFalse(target.ReplaceTextCommand.IsEnabled);

            Assert.IsNotNull(target.SaveAsCommand);
            Assert.IsFalse(target.SaveAsCommand.AllowSimultaneousExecute);
            // TODO: Implement SaveAsCommand
            Assert.IsFalse(target.SaveAsCommand.IsEnabled);

            Assert.IsNotNull(target.SaveDocumentCommand);
            Assert.IsFalse(target.SaveDocumentCommand.AllowSimultaneousExecute);
            // TODO: Implement SaveDocumentCommand
            Assert.IsFalse(target.SaveDocumentCommand.IsEnabled);

            Assert.IsNotNull(target.SayAsCommand);
            Assert.IsFalse(target.SayAsCommand.AllowSimultaneousExecute);
            // TODO: Implement SayAsCommand
            Assert.IsFalse(target.SayAsCommand.IsEnabled);

            Assert.IsTrue(target.SelectAfterInsert);

            Assert.IsNotNull(target.SelectCurrentTagCommand);
            Assert.IsFalse(target.SelectCurrentTagCommand.AllowSimultaneousExecute);
            // TODO: Implement SelectCurrentTagCommand
            Assert.IsFalse(target.SelectCurrentTagCommand.IsEnabled);

            Assert.IsNotNull(target.SelectTagContentsCommand);
            Assert.IsFalse(target.SelectTagContentsCommand.AllowSimultaneousExecute);
            // TODO: Implement SelectTagContentsCommand
            Assert.IsFalse(target.SelectTagContentsCommand.IsEnabled);

            Assert.IsNotNull(target.ShowFileSaveMessagesCommand);
            Assert.IsFalse(target.ShowFileSaveMessagesCommand.AllowSimultaneousExecute);
            // TODO: Implement ShowFileSaveMessagesCommand
            Assert.IsFalse(target.ShowFileSaveMessagesCommand.IsEnabled);

            Assert.IsNotNull(target.SpeakAllTextCommand);
            Assert.IsFalse(target.SpeakAllTextCommand.AllowSimultaneousExecute);
            // TODO: Implement SpeakAllTextCommand
            Assert.IsFalse(target.SpeakAllTextCommand.IsEnabled);

            Assert.IsNotNull(target.SpellOutCommand);
            Assert.IsFalse(target.SpellOutCommand.AllowSimultaneousExecute);
            // TODO: Implement SpellOutCommand
            Assert.IsFalse(target.SpellOutCommand.IsEnabled);

            Assert.IsNotNull(target.SsmlTextBox);
            Assert.AreEqual("", target.SsmlTextBox.Text);
            Assert.IsTrue(target.SsmlTextBox.AcceptsReturn);
            Assert.IsTrue(target.SsmlTextBox.AcceptsTab);
            Assert.IsTrue(target.SsmlTextBox.IsEnabled);
            Assert.IsFalse(target.SsmlTextBox.IsReadOnly);
            Assert.IsTrue(target.SsmlTextBox.IsTabStop);
            Assert.IsTrue(target.SsmlTextBox.IsUndoEnabled);
            Assert.AreEqual(ScrollBarVisibility.Disabled, target.SsmlTextBox.HorizontalScrollBarVisibility);
            Assert.AreEqual(TextWrapping.Wrap, target.SsmlTextBox.TextWrapping);
            Assert.AreEqual(ScrollBarVisibility.Auto, target.SsmlTextBox.VerticalScrollBarVisibility);
            Assert.AreEqual(Visibility.Visible, target.SsmlTextBox.Visibility);

            Assert.IsNotNull(target.SubstitutionCommand);
            Assert.IsFalse(target.SubstitutionCommand.AllowSimultaneousExecute);
            // TODO: Implement SubstitutionCommand
            Assert.IsFalse(target.SubstitutionCommand.IsEnabled);

            Assert.IsNotNull(target.ValidationMessages);
            Assert.AreEqual(1, target.ValidationMessages.Count);
            Assert.IsNotNull(target.ValidationMessages[0]);
            Assert.AreEqual("", target.ValidationMessages[0].Details);
            Assert.IsTrue(target.ValidationMessages[0].IsWarning);
            Assert.AreEqual("Line 1, Column 1: No SSML markup defined.", target.ValidationMessages[0].Message);

            Assert.IsNotNull(target.ValidationStatus);
            Assert.AreEqual(XmlValidationStatus.Warning, target.ValidationStatus);

            Assert.IsNotNull(target.ValidationToolTip);
            Assert.AreEqual("No SSML markup defined.", target.ValidationToolTip);
        }

        [TestMethod]
        public void MainViewModelLineWrapTestMethod()
        {
            MainWindowVM target = new MainWindowVM();
            bool expectedLineWrapEnabled = false;
            target.LineWrapEnabled = false;
            bool actualLineWrapEnabled = target.LineWrapEnabled;
            Assert.AreEqual(expectedLineWrapEnabled, actualLineWrapEnabled);
            int expectedLineNumber = 1;
            Assert.AreEqual(expectedLineNumber, target.CurrentLineNumber);
            int expectedColNumber = 1;
            Assert.AreEqual(expectedColNumber, target.CurrentColNumber);
            int expectedLineNumberCount = 1;
            Assert.AreEqual(expectedLineNumberCount, target.LineNumbers.Count);
            Assert.IsNotNull(target.LineNumbers[0]);
            double expectedLineNumber0MarginTop = 0.0;
            Assert.AreEqual(expectedLineNumber0MarginTop, target.LineNumbers[0].Margin.Top);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Left);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Right);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Bottom);
            int expectedLineNumber0Number = 1;
            Assert.AreEqual(expectedLineNumber0Number, target.LineNumbers[0].Number);
            ScrollBarVisibility expectedScrollBarVisibility = ScrollBarVisibility.Auto;
            Assert.AreEqual(expectedScrollBarVisibility, target.SsmlTextBox.HorizontalScrollBarVisibility);
            TextWrapping expectedTextWrapping = TextWrapping.NoWrap;
            Assert.AreEqual(expectedTextWrapping, target.SsmlTextBox.TextWrapping);

            expectedLineWrapEnabled = true;
            target.LineWrapEnabled = true;
            actualLineWrapEnabled = target.LineWrapEnabled;
            Assert.AreEqual(expectedLineWrapEnabled, actualLineWrapEnabled);
            Assert.AreEqual(expectedLineNumber, target.CurrentLineNumber);
            Assert.AreEqual(expectedColNumber, target.CurrentColNumber);
            Assert.AreEqual(expectedLineNumberCount, target.LineNumbers.Count);
            Assert.IsNotNull(target.LineNumbers[0]);
            Assert.AreEqual(expectedLineNumber0MarginTop, target.LineNumbers[0].Margin.Top);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Left);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Right);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Bottom);
            Assert.AreEqual(expectedLineNumber0Number, target.LineNumbers[0].Number);
            expectedScrollBarVisibility = ScrollBarVisibility.Disabled;
            Assert.AreEqual(expectedScrollBarVisibility, target.SsmlTextBox.HorizontalScrollBarVisibility);
            expectedTextWrapping = TextWrapping.Wrap;
            Assert.AreEqual(expectedTextWrapping, target.SsmlTextBox.TextWrapping);

            expectedLineWrapEnabled = false;
            target.LineWrapEnabled = false;
            actualLineWrapEnabled = target.LineWrapEnabled;
            Assert.AreEqual(expectedLineWrapEnabled, actualLineWrapEnabled);
            Assert.AreEqual(expectedLineNumber, target.CurrentLineNumber);
            Assert.AreEqual(expectedColNumber, target.CurrentColNumber);
            Assert.AreEqual(expectedLineNumberCount, target.LineNumbers.Count);
            Assert.IsNotNull(target.LineNumbers[0]);
            Assert.AreEqual(expectedLineNumber0MarginTop, target.LineNumbers[0].Margin.Top);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Left);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Right);
            Assert.AreEqual(0.0, target.LineNumbers[0].Margin.Bottom);
            Assert.AreEqual(expectedLineNumber0Number, target.LineNumbers[0].Number);
            expectedScrollBarVisibility = ScrollBarVisibility.Auto;
            Assert.AreEqual(expectedScrollBarVisibility, target.SsmlTextBox.HorizontalScrollBarVisibility);
            expectedTextWrapping = TextWrapping.NoWrap;
            Assert.AreEqual(expectedTextWrapping, target.SsmlTextBox.TextWrapping);
        }
    }
}
