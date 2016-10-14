using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.ViewModel.Xml.Ssml;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for SsmlMarkupViewModelTest
    /// </summary>
    [TestClass]
    public class SsmlEditViewModelTest
    {
        public SsmlEditViewModelTest()
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
        public void ConstructorTestMethod()
        {
            EditVM target = new EditVM();
            Assert.IsNotNull(target.TextContentControl);
            Assert.IsNotNull(target.MarkupInfo);
            Assert.IsTrue(target.TextContentControl.AcceptsReturn);
            Assert.IsTrue(target.TextContentControl.AcceptsTab);
        }

        [TestMethod]
        public void ValidationTestMethod()
        {
            EditVM target = new EditVM();
            target.TextContentControl.Text = "JustEmpty";
            target.WaitForTasks();
            Assert.AreNotEqual(0, target.MarkupInfo.Count);
        }
    }
}
