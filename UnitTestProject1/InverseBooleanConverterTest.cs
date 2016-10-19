using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.ViewModel;
using Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter;
using System.Windows.Data;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for InverseBooleanConverter
    /// </summary>
    [TestClass]
    public class InverseBooleanConverterTest
    {
        public InverseBooleanConverterTest() { }

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
        public void ViewModelValidationMessageConstructorTestMethod()
        {
            InverseBooleanConverter target = new InverseBooleanConverter();
            object value = null;
            object actual = (target as IValueConverter).Convert(value, typeof(bool), null, null);
            Assert.IsNull(value);

            target.NullSource = true;
            actual = (target as IValueConverter).Convert(value, typeof(bool), null, null);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(bool));
            Assert.IsTrue((bool)actual);

            value = false;
            actual = (target as IValueConverter).Convert(value, typeof(bool), null, null);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(bool));
            Assert.IsTrue((bool)actual);

            target.NullSource = false;
            actual = (target as IValueConverter).Convert(value, typeof(bool), null, null);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(bool));
            Assert.IsTrue((bool)actual);

            value = false;
            actual = (target as IValueConverter).Convert(value, typeof(bool), null, null);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(bool));
            Assert.IsTrue((bool)actual);

            value = true;
            target.NullSource = true;
            actual = (target as IValueConverter).Convert(value, typeof(bool), null, null);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(bool));
            Assert.IsFalse((bool)actual);
        }
    }
}