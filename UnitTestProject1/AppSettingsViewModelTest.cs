using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.ViewModel;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for AppSettingsViewModelTest
    /// </summary>
    [TestClass]
    public class AppSettingsViewModelTest
    {
        public AppSettingsViewModelTest() { }

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
        public void AppSettingsConstructorTestMethod()
        {
            AppSettingsVM target = new AppSettingsVM();
            Assert.IsNotNull(target.BaseUriPath);
            string expectedUriPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(target.GetType().Assembly.Location), "Resources");
            Assert.AreEqual(expectedUriPath, target.BaseUriPath);

            Assert.IsNotNull(target.BlankSsmlFilePath);
            string expected = System.IO.Path.Combine(expectedUriPath, "BlankSsmlDocument.xml");
            Assert.AreEqual(expected, target.BlankSsmlFilePath);

            int expectedValue = 0;
            Assert.AreEqual(expectedValue, target.DefaultSpeechRate);

            expectedValue = 100;
            Assert.AreEqual(expectedValue, target.DefaultSpeechVolume);
            
            Assert.IsNotNull(target.LastAudioPath);
            expected = "";
            Assert.AreEqual(expected, target.LastAudioPath);

            Assert.IsNotNull(target.LastBrowsedSubdirectory);
            Assert.AreEqual(expected, target.LastBrowsedSubdirectory);

            Assert.IsNotNull(target.LastSavedWavPath);
            Assert.AreEqual(expected, target.LastSavedWavPath);

            Assert.IsNotNull(target.LastSsmlFilePath);
            Assert.AreEqual(expected, target.LastSsmlFilePath);

            Assert.IsNotNull(target.PlsFileExtension);
            expected = ".pls";
            Assert.AreEqual(expected, target.PlsFileExtension);

            Assert.IsNotNull(target.PlsFileTypeDescriptionLong);
            expected = "Pronunciation Lexicon Specification File";
            Assert.AreEqual(expected, target.PlsFileTypeDescriptionLong);

            Assert.IsNotNull(target.PlsFileTypeDescriptionLong);
            expected = "Pronunciation Lexicon Specification";
            Assert.AreEqual(expected, target.PlsFileTypeDescriptionShort);

            Assert.IsNotNull(target.SsmlFileExtension);
            expected = ".ssml";
            Assert.AreEqual(expected, target.SsmlFileExtension);

            Assert.IsNotNull(target.SsmlFileTypeDescriptionLong);
            expected = "Speech Synthesis Markup File";
            Assert.AreEqual(expected, target.SsmlFileTypeDescriptionLong);

            Assert.IsNotNull(target.SsmlFileTypeDescriptionShort);
            expected = "Speech Synthesis Markup";
            Assert.AreEqual(expected, target.SsmlFileTypeDescriptionShort);

            Assert.IsNotNull(target.SsmlSchemaFilePath);
            expected = System.IO.Path.Combine(expectedUriPath, "WindowsPhoneSynthesis.xsd");
            Assert.AreEqual(expected, target.SsmlSchemaFilePath);

            Assert.IsNotNull(target.SsmlSchemaCoreFilePath);
            expected = System.IO.Path.Combine(expectedUriPath, "WindowsPhoneSynthesis-core.xsd");
            Assert.AreEqual(expected, target.SsmlSchemaCoreFilePath);

            Assert.AreEqual(ViewModelValidateState.Valid, target.ViewModelValidateState);

            Assert.AreEqual(0, target.ViewModelValidationMessages.Count);
        }

        [TestMethod]
        public void DefaultSpeechRateTestMethod()
        {
            AppSettingsVM target = new AppSettingsVM();
            int actual;
            for (int e = -10; e < 11; e++)
            {
                target.DefaultSpeechRate = e;
                actual = target.DefaultSpeechRate;
                Assert.AreEqual(e, actual);
                Assert.AreEqual(ViewModelValidateState.Valid, target.ViewModelValidateState);
                Assert.AreEqual(0, target.ViewModelValidationMessages.Count);
            }

            int expected = -11;
            string expectedMessage = "Value cannot be less than -10 or greater than 10.";
            string expectedPropertyName = "DefaultSpeechRate";
            target.DefaultSpeechRate = expected;
            actual = target.DefaultSpeechRate;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(1, target.ViewModelValidationMessages.Count);
            Assert.AreEqual("", target.ViewModelValidationMessages[0].Details);
            Assert.IsFalse(target.ViewModelValidationMessages[0].IsWarning);
            Assert.AreEqual(expectedMessage, target.ViewModelValidationMessages[0].Message);
            Assert.AreEqual(expectedPropertyName, target.ViewModelValidationMessages[0].PropertyName);

            expected = -10;
            target.DefaultSpeechRate = expected;
            actual = target.DefaultSpeechRate;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(0, target.ViewModelValidationMessages.Count);

            expected = 11;

            target.DefaultSpeechRate = expected;
            actual = target.DefaultSpeechRate;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(1, target.ViewModelValidationMessages.Count);
            Assert.AreEqual("", target.ViewModelValidationMessages[0].Details);
            Assert.IsFalse(target.ViewModelValidationMessages[0].IsWarning);
            Assert.AreEqual(expectedMessage, target.ViewModelValidationMessages[0].Message);
            Assert.AreEqual(expectedPropertyName, target.ViewModelValidationMessages[0].PropertyName);

            expected = 10;
            target.DefaultSpeechRate = expected;
            actual = target.DefaultSpeechRate;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(0, target.ViewModelValidationMessages.Count);
        }

        [TestMethod]
        public void DefaultSpeechVolumeTestMethod()
        {
            AppSettingsVM target = new AppSettingsVM();
            int actual;
            for (int e = -0; e < 101; e++)
            {
                target.DefaultSpeechVolume = e;
                actual = target.DefaultSpeechVolume;
                Assert.AreEqual(e, actual);
                Assert.AreEqual(ViewModelValidateState.Valid, target.ViewModelValidateState);
                Assert.AreEqual(0, target.ViewModelValidationMessages.Count);
            }

            int expected = -1;
            string expectedMessage = "Value cannot be less than 0 or greater than 100.";
            string expectedPropertyName = "DefaultSpeechVolume";
            target.DefaultSpeechVolume = expected;
            actual = target.DefaultSpeechVolume;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(1, target.ViewModelValidationMessages.Count);
            Assert.AreEqual("", target.ViewModelValidationMessages[0].Details);
            Assert.IsFalse(target.ViewModelValidationMessages[0].IsWarning);
            Assert.AreEqual(expectedMessage, target.ViewModelValidationMessages[0].Message);
            Assert.AreEqual(expectedPropertyName, target.ViewModelValidationMessages[0].PropertyName);

            expected = -0;
            target.DefaultSpeechVolume = expected;
            actual = target.DefaultSpeechVolume;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(0, target.ViewModelValidationMessages.Count);

            expected = 101;

            target.DefaultSpeechVolume = expected;
            actual = target.DefaultSpeechVolume;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(1, target.ViewModelValidationMessages.Count);
            Assert.AreEqual("", target.ViewModelValidationMessages[0].Details);
            Assert.IsFalse(target.ViewModelValidationMessages[0].IsWarning);
            Assert.AreEqual(expectedMessage, target.ViewModelValidationMessages[0].Message);
            Assert.AreEqual(expectedPropertyName, target.ViewModelValidationMessages[0].PropertyName);

            expected = 100;
            target.DefaultSpeechVolume = expected;
            actual = target.DefaultSpeechVolume;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(0, target.ViewModelValidationMessages.Count);
        }
    }
}
