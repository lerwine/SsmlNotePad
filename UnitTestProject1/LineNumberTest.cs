﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.ViewModel;
using System.Windows;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for LineNumberTest
    /// </summary>
    [TestClass]
    public class LineNumberTest
    {
        public LineNumberTest() { }

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
        public void LineNumberConstructorTestMethod()
        {
            LineNumberVM target = new LineNumberVM();
            int expectedNumber = 1;
            Assert.AreEqual(0.0, target.Margin.Left);
            Assert.AreEqual(0.0, target.Margin.Top);
            Assert.AreEqual(0.0, target.Margin.Right);
            Assert.AreEqual(0.0, target.Margin.Bottom);
            Assert.AreEqual(expectedNumber, target.Number);

            double expectedMarginTop = -0.12;
            target.Margin = new Thickness(0.0, expectedMarginTop, 0.0, 0.0);
            Assert.AreEqual(0.0, target.Margin.Left);
            Assert.AreEqual(expectedMarginTop, target.Margin.Top);
            Assert.AreEqual(0.0, target.Margin.Right);
            Assert.AreEqual(0.0, target.Margin.Bottom);
            Assert.AreEqual(expectedNumber, target.Number);

            expectedNumber = 7;
            target.Number = expectedNumber;
            Assert.AreEqual(0.0, target.Margin.Left);
            Assert.AreEqual(expectedMarginTop, target.Margin.Top);
            Assert.AreEqual(0.0, target.Margin.Right);
            Assert.AreEqual(0.0, target.Margin.Bottom);
            Assert.AreEqual(expectedNumber, target.Number);

            expectedMarginTop = 5.5;
            target.Margin = new Thickness(0.0, expectedMarginTop, 0.0, 0.0);
            Assert.AreEqual(0.0, target.Margin.Left);
            Assert.AreEqual(expectedMarginTop, target.Margin.Top);
            Assert.AreEqual(0.0, target.Margin.Right);
            Assert.AreEqual(0.0, target.Margin.Bottom);
            Assert.AreEqual(expectedNumber, target.Number);
        }
    }
}
