using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.Text;
using System.Text.RegularExpressions;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for TextUnitTests
    /// </summary>
    [TestClass]
    public class TextUnitTests
    {
        public TextUnitTests() { }

        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get { return _testContextInstance; } set { _testContextInstance = value; } }

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
        public void TextLineInfoTest()
        {
            string[] testLines = new string[] { "", "     ", "Test", "  Test   " };
            testLines = testLines.Concat(testLines.SelectMany(s => (new string[] { "\r\n", "\n", "\r" })
                .SelectMany(nl => (new string[] { "", "   ", "   Line\t2  " }).Select(t => s + nl + t)))).ToArray();
            testLines = testLines.Concat(testLines.SelectMany(s => (new string[] { "\r\n", "\r" })
                            .SelectMany(nl => (new string[] { "", " ", "." }).Select(t => s + nl + t)))).ToArray();
            testLines = testLines.Concat(testLines.Where(s => !s.EndsWith("\r")).SelectMany(s => (new string[] { "", " ", "." }).Select(t => s + "\r" + t))).ToArray();
            testLines = testLines.Concat(testLines.SelectMany(s => (new string[] { "\r\n", "\r" })
                            .SelectMany(nl => (new string[] { "", " ", " Last Line  " }).Select(t => s + nl + t)))).ToArray();
            testLines = testLines.Concat(testLines.Where(s => !s.EndsWith("\r")).SelectMany(s => (new string[] { "", " ", " Last Line  " }).Select(t => s + "\r" + t))).ToArray();

            IEnumerable<TextLineInfo> result = TextLineInfo.Load(null);
            Assert.IsNotNull(result);
            TextLineInfo[] arr = result.ToArray();
            Assert.AreEqual(0, arr.Length);

            TextLineInfo target = new TextLineInfo(null);
            Assert.AreEqual(1, target.Number);
            Assert.AreEqual(0, target.CharIndex);
            Assert.AreEqual(0, target.Length);
            Assert.IsNotNull(target.Text);
            Assert.IsNotNull(target.LineEnding);
            Assert.IsNotNull(target.AllText);
            Assert.IsNull(target.Previous);
            Assert.IsNull(target.Next);

            Regex regex = new Regex(@"(?<t>[^\r\n]*)(?<e>\r\n?|\n)");
            foreach (string source in testLines)
            {
                MatchCollection mc = regex.Matches(source);
                result = TextLineInfo.Load(source);
                Assert.IsNotNull(result);
                arr = result.ToArray();
                Assert.AreEqual(mc.Count + 1, arr.Length);
                for (int lineIndex = 0; lineIndex < mc.Count; lineIndex++)
                {
                    Assert.AreEqual(lineIndex + 1, arr[lineIndex].Number);
                    Assert.AreEqual(mc[lineIndex].Index, arr[lineIndex].CharIndex);
                    Assert.AreEqual(mc[lineIndex].Groups["t"].Length, arr[lineIndex].Length);
                    Assert.IsNotNull(arr[lineIndex].Text);
                    Assert.IsNotNull(arr[lineIndex].LineEnding);
                    Assert.IsNotNull(arr[lineIndex].AllText);
                    Assert.AreEqual(mc[lineIndex].Value, arr[lineIndex].AllText);
                    Assert.AreEqual(mc[lineIndex].Groups["t"].Value, arr[lineIndex].Text);
                    Assert.AreEqual(mc[lineIndex].Groups["e"].Value, arr[lineIndex].LineEnding);
                    if (lineIndex == 0)
                        Assert.IsNull(arr[lineIndex].Previous);
                    else
                    {
                        Assert.IsNotNull(arr[lineIndex].Previous);
                        Assert.AreSame(arr[lineIndex - 1], arr[lineIndex].Previous);
                    }
                    Assert.IsNotNull(arr[lineIndex].Next);
                }

                target = arr[arr.Length - 1];
                Assert.AreEqual(arr.Length, target.Number);
                int charIndex = (mc.Count == 0) ? 0 : mc[mc.Count - 1].Index + mc[mc.Count - 1].Length;
                Assert.AreEqual(charIndex, target.CharIndex);
                Assert.AreEqual(source.Length - charIndex, target.Length);
                Assert.IsNotNull(target.Text);
                Assert.IsNotNull(target.LineEnding);
                Assert.IsNotNull(target.AllText);
                Assert.AreEqual(source.Substring(charIndex), target.AllText);
                Assert.AreEqual(source.Substring(charIndex), target.Text);
                Assert.AreEqual("", target.LineEnding);
                if (mc.Count == 0)
                    Assert.IsNull(target.Previous);
                else
                {
                    Assert.IsNotNull(target.Previous);
                    Assert.AreSame(arr[arr.Length - 2], target.Previous);
                }
                Assert.IsNull(target.Next);

                target = new TextLineInfo(source);
                TextLineInfo previous = null;
                for (int i = 0; i < mc.Count; i++)
                {
                    Assert.IsNotNull(target);
                    Assert.AreEqual(i + 1, target.Number);
                    Assert.AreEqual(mc[i].Index, target.CharIndex);
                    Assert.AreEqual(mc[i].Groups["t"].Length, target.Length);
                    Assert.IsNotNull(target.Text);
                    Assert.IsNotNull(target.LineEnding);
                    Assert.IsNotNull(target.AllText);
                    Assert.AreEqual(mc[i].Groups["t"].Value, target.Text);
                    Assert.AreEqual(mc[i].Groups["e"].Value, target.LineEnding);
                    Assert.AreEqual(mc[i].Value, target.AllText);
                    if (i < 1)
                        Assert.IsNull(target.Previous);
                    else
                    {
                        Assert.IsNotNull(target.Previous);
                        Assert.AreSame(previous, target.Previous);
                    }
                    Assert.IsNotNull(target.Next);
                    previous = target;
                    target = target.Next;
                }
                Assert.AreEqual(mc.Count + 1, target.Number);
                charIndex = (mc.Count == 0) ? 0 : mc[mc.Count - 1].Index + mc[mc.Count - 1].Length;
                Assert.AreEqual(charIndex, target.CharIndex);
                Assert.AreEqual(source.Length - charIndex, target.Length);
                Assert.IsNotNull(target.Text);
                Assert.IsNotNull(target.LineEnding);
                Assert.IsNotNull(target.AllText);
                Assert.AreEqual(source.Substring(charIndex), target.Text);
                Assert.AreEqual("", target.LineEnding);
                Assert.AreEqual(source.Substring(charIndex), target.AllText);
                if (mc.Count == 0)
                    Assert.IsNull(target.Previous);
                else
                {
                    Assert.IsNotNull(target.Previous);
                    Assert.AreSame(previous, target.Previous);
                }
                Assert.IsNull(target.Next);
            }
        }

        [TestMethod]
        public void TextPointerConstructorTest1()
        {
            TextLineInfo source = new TextLineInfo("First Line\r\nSecond Line\rLast");
            var testData = new[]
            {
                new { Source = source, CharacterOffset = 0, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
                new { Source = source, CharacterOffset = 2, CharIndex = 2, LineNumber = 1, LinePosition = 3, CurrentLine = source },
                new { Source = source, CharacterOffset = 10, CharIndex = 10, LineNumber = 1, LinePosition = 11, CurrentLine = source },
                new { Source = source, CharacterOffset = 11, CharIndex = 11, LineNumber = 1, LinePosition = 12, CurrentLine = source },
                new { Source = source, CharacterOffset = 12, CharIndex = 12, LineNumber = 2, LinePosition = 1, CurrentLine = source.Next },
                new { Source = source, CharacterOffset = 23, CharIndex = 23, LineNumber = 2, LinePosition = 12, CurrentLine = source.Next },
                new { Source = source, CharacterOffset = 24, CharIndex = 24, LineNumber = 3, LinePosition = 1, CurrentLine = source.Next.Next },
                new { Source = source, CharacterOffset = 28, CharIndex = 28, LineNumber = 3, LinePosition = 5, CurrentLine = source.Next.Next },
                new { Source = source.Next, CharacterOffset = 0, CharIndex = 12, LineNumber = 2, LinePosition = 1, CurrentLine = source.Next },
                new { Source = source.Next, CharacterOffset = 11, CharIndex = 23, LineNumber = 2, LinePosition = 12, CurrentLine = source.Next },
                new { Source = source.Next, CharacterOffset = -4, CharIndex = 8, LineNumber = 1, LinePosition = 9, CurrentLine = source },
                new { Source = source.Next, CharacterOffset = -12, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
                new { Source = source.Next.Next, CharacterOffset = 0, CharIndex = 24, LineNumber = 3, LinePosition = 1, CurrentLine = source.Next.Next },
                new { Source = source.Next.Next, CharacterOffset = 3, CharIndex = 27, LineNumber = 3, LinePosition = 4, CurrentLine = source.Next.Next },
                new { Source = source.Next.Next, CharacterOffset = -10, CharIndex = 14, LineNumber = 2, LinePosition = 3, CurrentLine = source.Next },
                new { Source = source.Next.Next, CharacterOffset = -18, CharIndex = 6, LineNumber = 1, LinePosition = 7, CurrentLine = source },
                new { Source = source.Next.Next, CharacterOffset = -24, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source }
            };

            foreach (var item in testData)
            {
                TextPointer target = new TextPointer(item.Source, item.CharacterOffset);
                Assert.AreEqual(item.CharIndex, target.CharIndex);
                Assert.AreEqual(item.LineNumber, target.LineNumber);
                Assert.AreEqual(item.LinePosition, target.LinePosition);
                Assert.IsNotNull(target.CurrentLine);
                Assert.AreSame(item.CurrentLine, target.CurrentLine);
                Assert.IsFalse(target.IsEmpty);
            }
        }
        
        [TestMethod]
        public void TextPointerConstructorTest2()
        {
            TextLineInfo source = new TextLineInfo("First Line\r\nSecond Line\rLast");
            var testData = new[]
            {
                new { Source = source, LineOffset = 0, CharacterOffset = 0, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
                new { Source = source, LineOffset = 0, CharacterOffset = 2, CharIndex = 2, LineNumber = 1, LinePosition = 3, CurrentLine = source },
                new { Source = source, LineOffset = 0, CharacterOffset = 10, CharIndex = 10, LineNumber = 1, LinePosition = 11, CurrentLine = source },
                new { Source = source, LineOffset = 0, CharacterOffset = 11, CharIndex = 11, LineNumber = 1, LinePosition = 12, CurrentLine = source },
                new { Source = source, LineOffset = 0, CharacterOffset = 12, CharIndex = 12, LineNumber = 2, LinePosition = 1, CurrentLine = source.Next },
                new { Source = source, LineOffset = 0, CharacterOffset = 23, CharIndex = 23, LineNumber = 2, LinePosition = 12, CurrentLine = source.Next },
                new { Source = source, LineOffset = 0, CharacterOffset = 24, CharIndex = 24, LineNumber = 3, LinePosition = 1, CurrentLine = source.Next.Next },
                new { Source = source, LineOffset = 0, CharacterOffset = 28, CharIndex = 28, LineNumber = 3, LinePosition = 5, CurrentLine = source.Next.Next },
                new { Source = source.Next, LineOffset = 0, CharacterOffset = 0, CharIndex = 12, LineNumber = 2, LinePosition = 1, CurrentLine = source.Next },
                new { Source = source.Next, LineOffset = 0, CharacterOffset = 11, CharIndex = 23, LineNumber = 2, LinePosition = 12, CurrentLine = source.Next },
                new { Source = source.Next, LineOffset = 0, CharacterOffset = -4, CharIndex = 8, LineNumber = 1, LinePosition = 9, CurrentLine = source },
                new { Source = source.Next, LineOffset = 0, CharacterOffset = -12, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
                new { Source = source.Next.Next, LineOffset = 0, CharacterOffset = 0, CharIndex = 24, LineNumber = 3, LinePosition = 1, CurrentLine = source.Next.Next },
                new { Source = source.Next.Next, LineOffset = 0, CharacterOffset = 3, CharIndex = 27, LineNumber = 3, LinePosition = 4, CurrentLine = source.Next.Next },
                new { Source = source.Next.Next, LineOffset = 0, CharacterOffset = -10, CharIndex = 14, LineNumber = 2, LinePosition = 3, CurrentLine = source.Next },
                new { Source = source.Next.Next, LineOffset = 0, CharacterOffset = -18, CharIndex = 6, LineNumber = 1, LinePosition = 7, CurrentLine = source },
                new { Source = source.Next.Next, LineOffset = 0, CharacterOffset = -24, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
                new { Source = source, LineOffset = 1, CharacterOffset = 0, CharIndex = 12, LineNumber = 2, LinePosition = 1, CurrentLine = source.Next },
                new { Source = source, LineOffset = 1, CharacterOffset = 11, CharIndex = 23, LineNumber = 2, LinePosition = 12, CurrentLine = source.Next },
                new { Source = source, LineOffset = 1, CharacterOffset = -4, CharIndex = 8, LineNumber = 1, LinePosition = 9, CurrentLine = source },
                new { Source = source, LineOffset = 1, CharacterOffset = -12, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
                new { Source = source.Next, LineOffset = -1, CharacterOffset = 0, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
                new { Source = source.Next, LineOffset = -1, CharacterOffset = 2, CharIndex = 2, LineNumber = 1, LinePosition = 3, CurrentLine = source },
                new { Source = source.Next, LineOffset = -1, CharacterOffset = 10, CharIndex = 10, LineNumber = 1, LinePosition = 11, CurrentLine = source },
                new { Source = source.Next, LineOffset = -1, CharacterOffset = 11, CharIndex = 11, LineNumber = 1, LinePosition = 12, CurrentLine = source },
                new { Source = source.Next, LineOffset = -1, CharacterOffset = 12, CharIndex = 12, LineNumber = 2, LinePosition = 1, CurrentLine = source.Next },
                new { Source = source.Next, LineOffset = -1, CharacterOffset = 23, CharIndex = 23, LineNumber = 2, LinePosition = 12, CurrentLine = source.Next },
                new { Source = source.Next, LineOffset = -1, CharacterOffset = 24, CharIndex = 24, LineNumber = 3, LinePosition = 1, CurrentLine = source.Next.Next },
                new { Source = source.Next, LineOffset = -1, CharacterOffset = 28, CharIndex = 28, LineNumber = 3, LinePosition = 5, CurrentLine = source.Next.Next },
                new { Source = source.Next, LineOffset = 1, CharacterOffset = 0, CharIndex = 24, LineNumber = 3, LinePosition = 1, CurrentLine = source.Next.Next },
                new { Source = source.Next, LineOffset = 1, CharacterOffset = 3, CharIndex = 27, LineNumber = 3, LinePosition = 4, CurrentLine = source.Next.Next },
                new { Source = source.Next, LineOffset = 1, CharacterOffset = -10, CharIndex = 14, LineNumber = 2, LinePosition = 3, CurrentLine = source.Next },
                new { Source = source.Next, LineOffset = 1, CharacterOffset = -18, CharIndex = 6, LineNumber = 1, LinePosition = 7, CurrentLine = source },
                new { Source = source.Next, LineOffset = 1, CharacterOffset = -24, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
                new { Source = source, LineOffset = 2, CharacterOffset = 0, CharIndex = 24, LineNumber = 3, LinePosition = 1, CurrentLine = source.Next.Next },
                new { Source = source, LineOffset = 2, CharacterOffset = 3, CharIndex = 27, LineNumber = 3, LinePosition = 4, CurrentLine = source.Next.Next },
                new { Source = source, LineOffset = 2, CharacterOffset = -10, CharIndex = 14, LineNumber = 2, LinePosition = 3, CurrentLine = source.Next },
                new { Source = source, LineOffset = 2, CharacterOffset = -18, CharIndex = 6, LineNumber = 1, LinePosition = 7, CurrentLine = source },
                new { Source = source, LineOffset = 2, CharacterOffset = -24, CharIndex = 0, LineNumber = 1, LinePosition = 1, CurrentLine = source },
            };

            foreach (var item in testData)
            {
                TextPointer target = new TextPointer(item.Source, item.LineOffset, item.CharacterOffset);
                Assert.AreEqual(item.CharIndex, target.CharIndex);
                Assert.AreEqual(item.LineNumber, target.LineNumber);
                Assert.AreEqual(item.LinePosition, target.LinePosition);
                Assert.IsNotNull(target.CurrentLine);
                Assert.AreSame(item.CurrentLine, target.CurrentLine);
                Assert.IsFalse(target.IsEmpty);
            }
        }

        [TestMethod]
        public void TextPointerAddTest1()
        {
            string s = "First Line\r\nSecond Line\rLast";
            TextLineInfo source = new TextLineInfo(s);
            
            for (int p = 0; p <= source.Length; p++)
            {
                TextPointer target = new TextPointer(source, p);
                for (int charIndex = 0; charIndex <= source.Length; charIndex++)
                {
                    TextPointer actual = target.Add(charIndex - p);
                    Assert.AreEqual(charIndex, actual.CharIndex);
                    Assert.AreEqual((charIndex < 12) ? 1 : ((charIndex < 24) ? 2 : 3), actual.LineNumber);
                    Assert.AreEqual((charIndex < 12) ? charIndex + 1 : ((charIndex < 24) ? charIndex - 11 : charIndex - 23), actual.LinePosition);
                    Assert.IsNotNull(actual.CurrentLine);
                    Assert.AreEqual((charIndex < 12) ? source : ((charIndex < 24) ? source.Next : source.Next.Next), actual.CurrentLine);
                    Assert.IsFalse(actual.IsEmpty);
                }
            }
        }

        [TestMethod]
        public void TextPointerSubtractTest1()
        {
            string s = "First Line\r\nSecond Line\rLast";
            TextLineInfo source = new TextLineInfo(s);

            for (int p = 0; p <= source.Length; p++)
            {
                TextPointer target = new TextPointer(source, p);
                for (int charIndex = 0; charIndex <= source.Length; charIndex++)
                {
                    TextPointer actual = target.Subtract(p - charIndex);
                    Assert.AreEqual(charIndex, actual.CharIndex);
                    Assert.AreEqual((charIndex < 12) ? 1 : ((charIndex < 24) ? 2 : 3), actual.LineNumber);
                    Assert.AreEqual((charIndex < 12) ? charIndex + 1 : ((charIndex < 24) ? charIndex - 11 : charIndex - 23), actual.LinePosition);
                    Assert.IsNotNull(actual.CurrentLine);
                    Assert.AreEqual((charIndex < 12) ? source : ((charIndex < 24) ? source.Next : source.Next.Next), actual.CurrentLine);
                    Assert.IsFalse(actual.IsEmpty);
                }
            }
        }

        [TestMethod]
        public void TextPointerAddTest2()
        {
            string s = "First Line\r\nSecond Line\rLast";
            TextLineInfo source = new TextLineInfo(s);

            for (int p = 0; p <= source.Length; p++)
            {
                int l1 = (p < 12) ? 1 : ((p < 24) ? 2 : 3);
                int i1 = (p < 12) ? p + 1 : ((p < 24) ? p - 11 : p - 23);
                TextPointer target = new TextPointer(source, p);
                for (int charIndex = 0; charIndex <= source.Length; charIndex++)
                {
                    int l2 = (charIndex < 12) ? 1 : ((charIndex < 24) ? 2 : 3);
                    int i2 = (charIndex < 12) ? charIndex + 1 : ((charIndex < 24) ? charIndex - 11 : charIndex - 23);
                    TextPointer actual = target.Add(l2 - l1, i2 - i1);
                    Assert.AreEqual(charIndex, actual.CharIndex);
                    Assert.AreEqual(l2, actual.LineNumber);
                    Assert.AreEqual(i2, actual.LinePosition);
                    Assert.IsNotNull(actual.CurrentLine);
                    Assert.AreEqual((charIndex < 12) ? source : ((charIndex < 24) ? source.Next : source.Next.Next), actual.CurrentLine);
                    Assert.IsFalse(actual.IsEmpty);
                }
            }
        }

        [TestMethod]
        public void TextPointerSubtractTest2()
        {
            string s = "First Line\r\nSecond Line\rLast";
            TextLineInfo source = new TextLineInfo(s);

            for (int p = 0; p <= source.Length; p++)
            {
                int l1 = (p < 12) ? 1 : ((p < 24) ? 2 : 3);
                int i1 = (p < 12) ? p + 1 : ((p < 24) ? p - 11 : p - 23);
                TextPointer target = new TextPointer(source, p);
                for (int charIndex = 0; charIndex <= source.Length; charIndex++)
                {
                    int l2 = (charIndex < 12) ? 1 : ((charIndex < 24) ? 2 : 3);
                    int i2 = (charIndex < 12) ? charIndex + 1 : ((charIndex < 24) ? charIndex - 11 : charIndex - 23);
                    TextPointer actual = target.Subtract(l1 - l2, i1 - i2);
                    Assert.AreEqual(charIndex, actual.CharIndex);
                    Assert.AreEqual(l2, actual.LineNumber);
                    Assert.AreEqual(i2, actual.LinePosition);
                    Assert.IsNotNull(actual.CurrentLine);
                    Assert.AreEqual((charIndex < 12) ? source : ((charIndex < 24) ? source.Next : source.Next.Next), actual.CurrentLine);
                    Assert.IsFalse(actual.IsEmpty);
                }
            }
        }

        [TestMethod]
        public void TextPointerEqualsTest()
        {
            string s = "First Line\r\nSecond Line\rLast";
            TextLineInfo source = new TextLineInfo(s);
            TextPointer x = new TextPointer(source, 0);
            TextPointer y = new TextPointer(source, 0);
            Assert.IsTrue(x.Equals(y));
            Assert.IsTrue(y.Equals(x));
            y = new TextPointer(source, 12);
            Assert.IsFalse(x.Equals(y));
            Assert.IsFalse(y.Equals(x));
            x = new TextPointer(source, 12);
            Assert.IsTrue(x.Equals(y));
            Assert.IsTrue(y.Equals(x));
        }

        [TestMethod]
        public void TextRangeConstructorTest1()
        {
            TextLineInfo textLineInfo = new TextLineInfo(null);
            TextRange target = new TextRange(textLineInfo, 0, 0);
            Assert.AreEqual(new TextPointer(textLineInfo, 0), target.Start);
            Assert.AreEqual(new TextPointer(textLineInfo, 0), target.End);
            Assert.AreEqual(0, target.Length);
            Assert.AreEqual("", target.GetText());

            textLineInfo = new TextLineInfo("");
            target = new TextRange(textLineInfo, 0, 0);
            Assert.AreEqual(new TextPointer(textLineInfo, 0), target.Start);
            Assert.AreEqual(new TextPointer(textLineInfo, 0), target.End);
            Assert.AreEqual(0, target.Length);
            Assert.AreEqual("", target.GetText());

            target = new TextRange(textLineInfo, 1, 0);
            Assert.AreEqual(new TextPointer(textLineInfo, 1), target.Start);
            Assert.AreEqual(new TextPointer(textLineInfo, 1), target.End);
            Assert.AreEqual(0, target.Length);
            Assert.AreEqual("", target.GetText());

            textLineInfo = new TextLineInfo("Test Line 1\r\nLine 2");
            target = new TextRange(textLineInfo, 0, 0);
            Assert.AreEqual(new TextPointer(textLineInfo, 0), target.Start);
            Assert.AreEqual(new TextPointer(textLineInfo, 0), target.End);
            Assert.AreEqual(0, target.Length);
            Assert.AreEqual("", target.GetText());

            textLineInfo = new TextLineInfo("Test Line 1\r\nLine 2");
            target = new TextRange(textLineInfo, 1, 0);
            Assert.AreEqual(new TextPointer(textLineInfo, 1), target.Start);
            Assert.AreEqual(new TextPointer(textLineInfo, 1), target.End);
            Assert.AreEqual(0, target.Length);
            Assert.AreEqual("", target.GetText());

            target = new TextRange(textLineInfo, 0, 4);
            Assert.AreEqual(new TextPointer(textLineInfo, 0), target.Start);
            Assert.AreEqual(new TextPointer(textLineInfo, 4), target.End);
            Assert.AreEqual(4, target.Length);
            Assert.AreEqual("Test", target.GetText());

            target = new TextRange(textLineInfo, 13, 4);
            Assert.AreEqual(new TextPointer(textLineInfo, 13), target.Start);
            Assert.AreEqual(new TextPointer(textLineInfo, 17), target.End);
            Assert.AreEqual(4, target.Length);
            Assert.AreEqual("Line", target.GetText());

            target = new TextRange(textLineInfo, 18, 1);
            Assert.AreEqual(new TextPointer(textLineInfo, 18), target.Start);
            Assert.AreEqual(new TextPointer(textLineInfo, 19), target.End);
            Assert.AreEqual(1, target.Length);
            Assert.AreEqual("2", target.GetText());
        }
    }
}
