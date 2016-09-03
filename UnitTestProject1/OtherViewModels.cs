using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.ViewModel;
using System.ComponentModel;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for OtherViewModels
    /// </summary>
    [TestClass]
    public class OtherViewModels
    {
        public OtherViewModels()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
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
        public void SubstitutionViewModelTestMethod()
        {
            SubstitutionVM vm = new SubstitutionVM();
            string expected = "Displayed text cannot be empty.";
            string actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);

            expected = "Spoken text cannot be empty.";
            actual = (vm as IDataErrorInfo)["SpokenText"];
            Assert.AreEqual(expected, actual);

            expected = "Test";
            vm.DisplayedText = expected;
            actual = vm.DisplayedText;
            Assert.AreEqual(expected, actual);

            expected = "";
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);

            expected = "Spoken text cannot be empty.";
            actual = (vm as IDataErrorInfo)["SpokenText"];
            Assert.AreEqual(expected, actual);

            expected = "Text";
            vm.SpokenText = expected;
            actual = expected;
            Assert.AreEqual(expected, actual);

            expected = "";
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);

            actual = (vm as IDataErrorInfo)["SpokenText"];
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AudioFileViewModelTestMethod()
        {
            AudioFileVM vm = new AudioFileVM();
            string expected = "Audio file path / URI must be provided.";
            string actual = (vm as IDataErrorInfo)["AudioUri"];
            Assert.AreEqual(expected, actual);
            Assert.IsFalse(vm.ShowDescriptionControls);

            expected = "";
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);
            Assert.IsFalse(vm.ShowDescriptionControls);

            expected = "Test";
            vm.DisplayedText = expected;
            actual = vm.DisplayedText;
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(vm.ShowDescriptionControls);

            expected = "";
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);

            expected = "Audio file path / URI must be provided.";
            actual = (vm as IDataErrorInfo)["AudioUri"];
            Assert.AreEqual(expected, actual);
            
            expected = "Text";
            vm.Description = expected;
            actual = expected;
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(vm.ShowDescriptionControls);

            expected = "Audio file path / URI must be provided.";
            actual = (vm as IDataErrorInfo)["AudioUri"];
            Assert.AreEqual(expected, actual);

            expected = "";
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);

            expected = "Values";
            vm.AudioUri = expected;
            actual = vm.AudioUri;
            Assert.AreEqual(expected, actual);
            expected = "Audio file path / URI is not valid.";
            actual = (vm as IDataErrorInfo)["AudioUri"];
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(vm.ShowDescriptionControls);

            expected = "";
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);

            expected = "C:\\test.wav";
            vm.AudioUri = expected;
            actual = vm.AudioUri;
            Assert.AreEqual(expected, actual);
            expected = "";
            actual = (vm as IDataErrorInfo)["AudioUri"];
            Assert.AreEqual(expected, actual);
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(vm.ShowDescriptionControls);

            vm.DisplayedText = expected;
            actual = vm.DisplayedText;
            Assert.AreEqual(expected, actual);
            actual = (vm as IDataErrorInfo)["AudioUri"];
            Assert.AreEqual(expected, actual);
            expected = "Display text cannot be empty if description is provided.";
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(vm.ShowDescriptionControls);

            expected = "";
            vm.Description = expected;
            actual = vm.Description;
            Assert.AreEqual(expected, actual);
            expected = "different";
            vm.DisplayedText = expected;
            actual = vm.DisplayedText;
            Assert.AreEqual(expected, actual);
            expected = "";
            actual = (vm as IDataErrorInfo)["AudioUri"];
            Assert.AreEqual(expected, actual);
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(vm.ShowDescriptionControls);
            
            vm.DisplayedText = expected;
            actual = vm.DisplayedText;
            Assert.AreEqual(expected, actual);
            actual = (vm as IDataErrorInfo)["AudioUri"];
            Assert.AreEqual(expected, actual);
            actual = (vm as IDataErrorInfo)["DisplayedText"];
            Assert.AreEqual(expected, actual);
            Assert.IsFalse(vm.ShowDescriptionControls);
        }
    }
}
