using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.ViewModel;
using System.ComponentModel;
using System.Diagnostics;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for OtherViewModels
    /// </summary>
    [TestClass]
    public class OtherViewModelsUnitTest
    {
        public OtherViewModelsUnitTest() { }

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

        public static StackFrame[] GetCalledStackFrames1()
        {
            StackTrace stackTrace = new StackTrace(1);
            return stackTrace.GetFrames();
        }

        public static StackFrame[] GetCalledStackFrames2()
        {
            return GetCalledStackFrames1();
        }

        [TestMethod]
        public void ZipTestMethod()
        {
            string[] arr1 = { "0", "1", "2", "3", "4" };
            string[] arr2 = { "a", "b", "c" };
            string[] zipped = arr1.Zip(arr2, (i1, i2) => String.Format("i1 = {0}, i2 = {1}", i1, i2)).ToArray();
            Assert.AreEqual(arr2.Length, zipped.Length);

            arr1 = new string[] { "0", "1", "2" };
            arr2 = new string[] { "a", "b", "c", "d" };
            zipped = arr1.Zip(arr2, (i1, i2) => String.Format("i1 = {0}, i2 = {1}", i1, i2)).ToArray();
            Assert.AreEqual(arr1.Length, zipped.Length);

            StackFrame[] stackFrames = GetCalledStackFrames2();
            Assert.AreEqual("GetCalledStackFrames2", stackFrames[0].GetMethod().Name);
            Assert.AreEqual("ZipTestMethod", stackFrames[1].GetMethod().Name);
        }

        [TestMethod]
        public void DefaultSpeechSettingsTestMethod()
        {
            DefaultSpeechSettingsVM vm = new DefaultSpeechSettingsVM();
        }
    }
}
