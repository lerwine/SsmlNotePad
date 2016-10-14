using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;
using Erwine.Leonard.T.SsmlNotePad.Xml;
using Erwine.Leonard.T.SsmlNotePad.Text;
using System.Text.RegularExpressions;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for XmlContextTest
    /// </summary>
    [TestClass]
    public class XmlContextTest
    {
        public XmlContextTest()
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

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
        public void XmlNodeContextTestMethod()
        {
            XmlParseContextSettings xmlParseContextSettings = new XmlParseContextSettings();
            string xml = Markup.XmlText;
            XmlContextInfo target = new XmlContextInfo(xml, xmlParseContextSettings);
            Assert.AreEqual(14, target.Count);

            Assert.AreEqual(XmlNodeType.Text, target[0].NodeType);
            Assert.AreEqual(xml.IndexOf("?>") + 2, target[0].OuterRange.Start.CharIndex);
            Assert.AreEqual(xml.IndexOf("<sgml"), target[0].OuterRange.End.CharIndex);
            string expected = xml.Substring(xml.IndexOf("?>") + 2, xml.IndexOf("<sgml") - (xml.IndexOf("?>") + 2));
            Assert.AreEqual(expected, target[0].OuterRange.GetText());
            Assert.AreEqual(expected, target[0].InnerRange.GetText());

            Assert.AreEqual(target[0].OuterRange.End.CharIndex, target[1].OuterRange.Start.CharIndex);
            
            Assert.AreEqual(XmlNodeType.Element, target[1].NodeType);
            int index = xml.IndexOf("<sgml");
            Assert.AreEqual(index, target[1].OuterRange.Start.CharIndex);
            expected = xml.Substring(index).TrimEnd();
            Assert.AreEqual(expected, target[1].OuterRange.GetText());
            int innerIndex = xml.IndexOf(">", index + 1) + 1;
            Assert.AreEqual(index, target[1].InnerRange.Start.CharIndex);
            index = xml.IndexOf("</sgml>") + 7;
            Assert.AreEqual(index, target[1].InnerRange.End.CharIndex);
            expected = xml.Substring(innerIndex, index - innerIndex);
            Assert.AreEqual(expected, target[1].InnerRange.GetText());

            Assert.AreEqual(target[1].InnerRange.Start.CharIndex, target[2].OuterRange.Start.CharIndex);
            Assert.AreEqual(XmlNodeType.Text, target[2].NodeType);
            Assert.AreEqual(target[2].InnerRange.Start.CharIndex, target[3].OuterRange.Start.CharIndex);
            Assert.AreEqual(target[2].InnerRange.End.CharIndex, target[3].OuterRange.End.CharIndex);

            Assert.AreEqual(XmlNodeType.Comment, target[3].NodeType);
            innerIndex = xml.IndexOf("<!--");
            Assert.AreEqual(innerIndex, target[3].OuterRange.Start.CharIndex);
            Assert.AreEqual(innerIndex, target[3].InnerRange.Start.CharIndex);
            index = xml.IndexOf("-->") + 3;
            Assert.AreEqual(index, target[3].OuterRange.End.CharIndex);
            Assert.AreEqual(index, target[3].InnerRange.End.CharIndex);
            expected = xml.Substring(innerIndex, index - innerIndex);
            Assert.AreEqual(expected, target[3].OuterRange.GetText());
            Assert.AreEqual(expected, target[3].InnerRange.GetText());

            Assert.AreEqual(target[3].OuterRange.End.CharIndex, target[4].OuterRange.Start.CharIndex);
            Assert.AreEqual(XmlNodeType.Text, target[4].NodeType);
            Assert.AreEqual(target[4].InnerRange.Start.CharIndex, target[5].OuterRange.Start.CharIndex);
            Assert.AreEqual(target[4].InnerRange.End.CharIndex, target[5].OuterRange.End.CharIndex);

            Assert.AreEqual(XmlNodeType.Element, target[5].NodeType);
            innerIndex = xml.IndexOf("<img");
            Assert.AreEqual(innerIndex, target[5].OuterRange.Start.CharIndex);
            index = xml.IndexOf("</img>") + 6;
            Assert.AreEqual(index, target[5].OuterRange.End.CharIndex);
            expected = xml.Substring(innerIndex, index - innerIndex);
            Assert.AreEqual(expected, target[5].OuterRange.GetText());

            innerIndex = xml.IndexOf(">", innerIndex + 1) + 1;
            index = xml.IndexOf("</img>");
            expected = xml.Substring(innerIndex, index - innerIndex);
            Assert.AreEqual(expected, target[5].InnerRange.GetText());

            Assert.AreEqual(target[5].InnerRange.End.CharIndex, target[6].OuterRange.Start.CharIndex);

            Assert.AreEqual(XmlNodeType.Text, target[6].NodeType);
            Assert.AreEqual(innerIndex, target[6].OuterRange.Start.CharIndex);
            Assert.AreEqual(index, target[6].OuterRange.End.CharIndex);
            expected = xml.Substring(innerIndex, index - innerIndex);
            Assert.AreEqual(expected, target[6].InnerRange.GetText());
            Assert.AreEqual(expected, target[6].OuterRange.GetText());

            Assert.AreEqual(target[6].OuterRange.End.CharIndex, target[7].OuterRange.Start.CharIndex);
            Assert.AreEqual(XmlNodeType.Text, target[7].NodeType);
            Assert.AreEqual(target[7].InnerRange.Start.CharIndex, target[7].OuterRange.Start.CharIndex);
            Assert.AreEqual(target[7].InnerRange.End.CharIndex, target[7].OuterRange.End.CharIndex);

            Assert.AreEqual(target[7].OuterRange.End.CharIndex, target[8].OuterRange.Start.CharIndex);

            Assert.AreEqual(XmlNodeType.Comment, target[8].NodeType);
            innerIndex = xml.IndexOf("<!--  a GIF");
            Assert.AreEqual(innerIndex, target[8].OuterRange.Start.CharIndex);
            Assert.AreEqual(innerIndex, target[8].InnerRange.Start.CharIndex);
            index = xml.IndexOf("-->", innerIndex) + 3;
            Assert.AreEqual(index, target[8].OuterRange.End.CharIndex);
            Assert.AreEqual(index, target[8].InnerRange.End.CharIndex);
            expected = xml.Substring(innerIndex, index - innerIndex);
            Assert.AreEqual(expected, target[8].OuterRange.GetText());
            Assert.AreEqual(expected, target[8].InnerRange.GetText());

            Assert.AreEqual(target[8].OuterRange.End.CharIndex, target[9].OuterRange.Start.CharIndex);

            Assert.AreEqual(XmlNodeType.Element, target[9].NodeType);
            innerIndex = xml.IndexOf("<img title=\"&g");
            Assert.AreEqual(innerIndex, target[9].OuterRange.Start.CharIndex);
            index = xml.IndexOf("GIF\" />") + 7;
            Assert.AreEqual(index, target[9].OuterRange.End.CharIndex);
            expected = xml.Substring(innerIndex, index - innerIndex);
            Assert.AreEqual(expected, target[9].OuterRange.GetText());

            Assert.AreEqual(target[9].OuterRange.End.CharIndex, target[10].OuterRange.Start.CharIndex);
            Assert.AreEqual(XmlNodeType.Text, target[10].NodeType);
            Assert.AreEqual(target[10].InnerRange.Start.CharIndex, target[10].OuterRange.Start.CharIndex);
            Assert.AreEqual(target[10].InnerRange.End.CharIndex, target[10].OuterRange.End.CharIndex);

            Assert.AreEqual(target[10].OuterRange.End.CharIndex, target[11].OuterRange.Start.CharIndex);

            Assert.AreEqual(XmlNodeType.Comment, target[11].NodeType);
            innerIndex = xml.IndexOf("<!--No");
            Assert.AreEqual(innerIndex, target[11].OuterRange.Start.CharIndex);
            Assert.AreEqual(innerIndex, target[11].InnerRange.Start.CharIndex);
            index = xml.IndexOf("-->", innerIndex) + 3;
            Assert.AreEqual(index, target[11].OuterRange.End.CharIndex);
            Assert.AreEqual(index, target[11].InnerRange.End.CharIndex);
            expected = xml.Substring(innerIndex, index - innerIndex);
            Assert.AreEqual(expected, target[11].OuterRange.GetText());
            Assert.AreEqual(expected, target[11].InnerRange.GetText());

            Assert.AreEqual(target[11].OuterRange.End.CharIndex, target[12].OuterRange.Start.CharIndex);
            Assert.AreEqual(XmlNodeType.Text, target[12].NodeType);
            Assert.AreEqual(target[12].InnerRange.Start.CharIndex, target[12].OuterRange.Start.CharIndex);
            Assert.AreEqual(target[12].InnerRange.End.CharIndex, target[12].OuterRange.End.CharIndex);

            Assert.AreEqual(target[13].OuterRange.End.CharIndex, target[6].InnerRange.End.CharIndex);
        }
    }
}