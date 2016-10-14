using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    interface INodeIndexInfo
    {
        int OuterStartIndex { get; }
        int? InnerStartIndex { get; set; }
        int? InnerEndIndex { get; set; }
        int? OuterEndIndex { get; set; }
        string LocalName { get; }
        string NamespaceURI { get; }
        XmlNodeType NodeType { get; }
    }
    public class NodeIndexInfo : INodeIndexInfo
    {
        public NodeIndexInfo(string localName, string namespaceURI, XmlNodeType nodeType, int index)
        {
            LocalName = localName;
            NamespaceURI = namespaceURI;
            NodeType = nodeType;
            OuterStartIndex = index;
        }

        public int OuterStartIndex { get; private set; }
        public int InnerStartIndex { get { return (_innerStartIndex.HasValue) ? _innerStartIndex.Value : OuterStartIndex; } }
        private int? _innerStartIndex = null;
        int? INodeIndexInfo.InnerStartIndex
        {
            get { return _innerStartIndex; }
            set
            {
                if (_innerStartIndex.HasValue)
                    throw new InvalidOperationException("Value cannot be modified once it is set.");

                _innerStartIndex = value;
            }
        }
        private int? _innerEndIndex = null;
        public int InnerEndIndex { get { return (_innerEndIndex.HasValue) ? _innerEndIndex.Value : OuterEndIndex; } }
        int? INodeIndexInfo.InnerEndIndex
        {
            get { return _innerEndIndex; }
            set
            {
                if (_innerEndIndex.HasValue)
                    throw new InvalidOperationException("Value cannot be modified once it is set.");

                _innerEndIndex = value;
            }
        }
        private int? _outerEndIndex = null;
        public int OuterEndIndex { get { return (_outerEndIndex.HasValue) ? _outerEndIndex.Value : OuterStartIndex; } }
        int? INodeIndexInfo.OuterEndIndex
        {
            get { return _outerEndIndex; }
            set
            {
                if (_outerEndIndex.HasValue)
                    throw new InvalidOperationException("Value cannot be modified once it is set.");

                _outerEndIndex = value;
            }
        }
        public string LocalName { get; private set; }
        public string NamespaceURI { get; private set; }
        public XmlNodeType NodeType { get; private set; }
    }

    public class LineIndexInfo
    {
        public LineIndexInfo(int index, int length, string text)
        {
            Index = index;
            Length = length;
            TextAndLineEnding = text;
        }

        public int Index { get; private set; }
        public int Length { get; private set; }
        public string TextAndLineEnding { get; private set; }
        public string Text { get { return (Length == 0) ? "" : TextAndLineEnding.Substring(0, Length); } }
        public string LineEnding { get { return (Length == TextAndLineEnding.Length) ? "" : TextAndLineEnding.Substring(Length); } }
    }

    public static class XmlHelper
    {
        public static IEnumerable<NodeIndexInfo> GetNodeIndexInfo(string xml, out LineIndexInfo[] lineIndexes)
        {
            if ((lineIndexes = GetLineIndexes(xml).ToArray()).Length == 0)
                return new NodeIndexInfo[0];

            using (StringReader stringReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    return GetNodeIndexInfo(xmlReader, lineIndexes);
            }
        }

        public static IEnumerable<NodeIndexInfo> GetNodeIndexInfo(XmlReader xmlReader, LineIndexInfo[] lineIndexes)
        {
            IXmlLineInfo lineInfo = xmlReader as IXmlLineInfo;
            INodeIndexInfo lastItem = new NodeIndexInfo(null, null, XmlNodeType.None, 0);
            Stack<INodeIndexInfo> stack = new Stack<INodeIndexInfo>();
            while (xmlReader.Read())
            {
                int newCharIndex = lineIndexes[lineInfo.LineNumber - 1].Index + lineInfo.LinePosition - 1;
                INodeIndexInfo newItem;
                if (xmlReader.NodeType == XmlNodeType.EndElement)
                {
                    newItem = stack.Pop();
                    newItem.InnerEndIndex = newCharIndex - 1;
                }
                else
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        newItem = new NodeIndexInfo(xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.NodeType, newCharIndex - 1);
                        if (!xmlReader.IsEmptyElement)
                            stack.Push(newItem);
                    }
                    else
                        newItem = new NodeIndexInfo(xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.NodeType, newCharIndex);
                    if (stack.Count > 0 && !stack.Peek().InnerStartIndex.HasValue)
                        stack.Peek().InnerStartIndex = newItem.OuterStartIndex; // BUG: Stack was already pushed if it was an element
                }
                yield return lastItem as NodeIndexInfo;
                lastItem = newItem;
            }
        }

        public static IEnumerable<LineIndexInfo> GetLineIndexes(string text)
        {
            if (text == null)
                yield break;

            int lineLength;
            int startIndex = 0;
            for (int charIndex = 0; charIndex < text.Length; charIndex++)
            {
                if (text[charIndex] == '\r')
                {
                    lineLength = charIndex - startIndex;
                    if (charIndex < text.Length - 1 && text[charIndex + 1] == '\n')
                        charIndex++;
                }
                else if (text[charIndex] == '\n')
                    lineLength = charIndex - startIndex;
                else
                    continue;

                yield return new LineIndexInfo(startIndex, lineLength, text.Substring(startIndex, charIndex - startIndex));
            }

            lineLength = text.Length - startIndex;
            yield return new LineIndexInfo(startIndex, lineLength, (startIndex == text.Length) ? "" : text.Substring(startIndex));
        }

        public static XmlSchemaSet CreateSsmlSchemaSet()
        {
            string baseUri = App.AppSettingsViewModel.BaseUriPath;
            
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(Markup.SsmlSchemaNamespaceURI, App.AppSettingsViewModel.SsmlSchemaFileName);
            schemaSet.Add(Markup.SsmlSchemaNamespaceURI, App.AppSettingsViewModel.SsmlSchemaCoreFileName);
            return schemaSet;
        }

        public static bool ValidateSsml(TextReader textReader, out string errorMessage)
        {
            return ValidateSsml(textReader, false, out errorMessage);
        }

        public static bool ValidateSsml(TextReader textReader, bool includeXmlDeclaration, out string errorMessage)
        {
            if (textReader == null)
                throw new ArgumentNullException("textReader");
            
            XmlDocument document = new XmlDocument();
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, new XmlReaderSettings
                {
                    CheckCharacters = false,
                    DtdProcessing = DtdProcessing.Ignore,
                    ValidationType = ValidationType.Schema,
                    Schemas = CreateSsmlSchemaSet(),
                    CloseInput = false
                }, App.AppSettingsViewModel.BaseUriPath))
                {
                    document.Load(xmlReader);
                }
            }
            catch (Exception exc)
            {
                errorMessage = String.Format("Document validation failed:\r\n{0}", exc.Message);
                return false;
            }

            errorMessage = null;
            return true;
        }
        
        public static bool ValidateSsml(string text, bool mayBePartial, bool includeXmlDeclaration, out string errorMessage)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                if (mayBePartial)
                {
                    text = Markup.BlankSsmlDocument;
                    mayBePartial = false;
                }
                else
                {
                    errorMessage = "SSML markup is empty.";
                    return false;
                }
            }

            using (StringReader stringReader = new StringReader(text.Trim()))
            {
                if (ValidateSsml(stringReader, includeXmlDeclaration, out errorMessage))
                    return true;
            }

            if (mayBePartial && TryCreateFromPartial(text, out text))
            {
                using (StringReader stringReader = new StringReader(text))
                    return ValidateSsml(stringReader, includeXmlDeclaration, out errorMessage);
            }

            return false;
        }

        public static bool TryCreateFromPartial(string text, out string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(Markup.BlankSsmlDocument);
                xmlDocument.DocumentElement.InnerXml = text;
                xml = xmlDocument.DocumentElement.OuterXml;
                return true;
            }
            catch
            {
                xml = text;
                return false;
            }
        }

        public static bool ValidateSsml(string text, bool mayBePartial, out string errorMessage) { return ValidateSsml(text, mayBePartial, false, out errorMessage); }
        
        public static bool ReformatDocument(TextReader textReader, TextWriter textWriter, out string errorMessage)
        {
            return ReformatDocument(textReader, textWriter, false, out errorMessage);
        }

        public static bool ReformatDocument(TextReader textReader, TextWriter textWriter, bool includeXmlDeclaration, out string errorMessage)
        {
            if (textReader == null)
                throw new ArgumentNullException("textReader");

            if (textWriter == null)
                throw new ArgumentNullException("textWriter");

            if (textReader.Peek() == -1)
            {
                textWriter.Write(Markup.BlankSsmlDocument);
                errorMessage = null;
                return true;
            }

            XmlDocument document = new XmlDocument();
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, new XmlReaderSettings
                {
                    CheckCharacters = false,
                    DtdProcessing = DtdProcessing.Ignore,
                    ValidationType = ValidationType.None,
                    CloseInput = false
                }))
                {
                    document.Load(xmlReader);
                }
            }
            catch (Exception exc)
            {
                errorMessage = String.Format("Unable to reformat document due to format error:\r\n{0}", exc.Message);
                return false;
            }

            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings
            {
                CheckCharacters = false,
                Indent = true,
                OmitXmlDeclaration = !includeXmlDeclaration,
                CloseOutput = false
            }))
            {
                document.WriteTo(xmlWriter);
                xmlWriter.Flush();
            }

            errorMessage = null;
            return true;
        }

        public static bool ReformatDocument(string text, TextWriter textWriter, out string errorMessage)
        {
            return ReformatDocument(text, textWriter, false, out errorMessage);
        }

        public static bool ReformatDocument(string text, TextWriter textWriter, bool includeXmlDeclaration, out string errorMessage)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                textWriter.Write(Markup.BlankSsmlDocument);
                errorMessage = null;
                return true;
            }

            using (StringReader stringReader = new StringReader(text.Trim()))
                return ReformatDocument(stringReader, textWriter, out errorMessage);
        }

        public static string ReformatDocument(string text, out string errorMessage) { return ReformatDocument(text, false, out errorMessage); }

        public static string ReformatDocument(string text, bool includeXmlDeclaration, out string errorMessage)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                if (ReformatDocument(text, stringWriter, includeXmlDeclaration, out errorMessage))
                    return stringWriter.ToString();
            }
            return text;
        }

        public static string XmlEncode(string text, XmlEncodeOption option = XmlEncodeOption.Minimal)
        {
            if (String.IsNullOrEmpty(text))
                return text;
            return new string(XmlEncode(text as IEnumerable<char>, option).ToArray());
        }

        public static IEnumerable<char> XmlEncode(IEnumerable<char> source, XmlEncodeOption option = XmlEncodeOption.Minimal)
        {
            if (source == null)
                return null;

            if (option.HasFlag(XmlEncodeOption.WindowsLineEndings))
            {
                if (option.HasFlag(XmlEncodeOption.ApostropheEncoded))
                {
                    if (option.HasFlag(XmlEncodeOption.DoubleQuoteEncoded))
                    {
                        if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                            return _XmlEncodeDACW(source);
                        return _XmlEncodeDAW(source);
                    }

                    if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                        return _XmlEncodeACW(source);
                    return _XmlEncodeAW(source);
                }

                if (option.HasFlag(XmlEncodeOption.DoubleQuoteEncoded))
                {
                    if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                        return _XmlEncodeDCW(source);
                    return _XmlEncodeDW(source);
                }

                if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                    return _XmlEncodeCW(source);
                return _XmlEncodeW(source);
            }

            if (option.HasFlag(XmlEncodeOption.UnixLineEndings))
            {
                if (option.HasFlag(XmlEncodeOption.ApostropheEncoded))
                {
                    if (option.HasFlag(XmlEncodeOption.DoubleQuoteEncoded))
                    {
                        if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                            return _XmlEncodeDACU(source);
                        return _XmlEncodeDAU(source);
                    }

                    if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                        return _XmlEncodeACU(source);
                    return _XmlEncodeAU(source);
                }

                if (option.HasFlag(XmlEncodeOption.DoubleQuoteEncoded))
                {
                    if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                        return _XmlEncodeDCU(source);
                    return _XmlEncodeDU(source);
                }

                if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                    return _XmlEncodeCU(source);
                return _XmlEncodeU(source);
            }

            if (option.HasFlag(XmlEncodeOption.ApostropheEncoded))
            {
                if (option.HasFlag(XmlEncodeOption.DoubleQuoteEncoded))
                {
                    if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                        return _XmlEncodeDAC(source);
                    return _XmlEncodeDA(source);
                }

                if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                    return _XmlEncodeAC(source);
                return _XmlEncodeA(source);
            }

            if (option.HasFlag(XmlEncodeOption.DoubleQuoteEncoded))
            {
                if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                    return _XmlEncodeDC(source);
                return _XmlEncodeD(source);
            }

            if (option.HasFlag(XmlEncodeOption.ControlCharacterEncoded))
                return _XmlEncodeC(source);
            return _XmlEncode(source);
        }

        #region Encode Helper Methods

        private static IEnumerable<char> _XmlEncodeDACW(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'D';
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'D';
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        internal static string RemoveConsecutiveEmptyLines(string text)
        {
            List<string> lines = text.SplitLines().ToList();
            for (int i=1; i< lines.Count; i++)
            {
                if (!lines[i-1].Any(c => !Char.IsWhiteSpace(c)) && !lines[i].Any(c => !Char.IsWhiteSpace(c)))
                {
                    do
                    {
                        lines.RemoveAt(i - 1);
                    } while (i < lines.Count && !lines[i].Any(c => !Char.IsWhiteSpace(c)));
                }
            }

            return String.Join(Environment.NewLine, lines.ToArray());
        }

        internal static string RemoveEmptyLines(string text)
        {
            return String.Join(Environment.NewLine, text.SplitLines().Where(l => l.TrimEnd().Length > 0).ToArray());
        }

        internal static string JoinLines(string text)
        {
            string[] lines = text.SplitLines(true).ToArray();
            if (lines == null || lines.Length == 0)
                return "";
            if (lines.Length == 1)
                return lines[0];

            return String.Join(" ", lines.Take(1).Select(l => l.TrimEnd()).Concat(lines.Skip(1).Take(lines.Length - 2).Select(l => l.Trim()))
                .Concat(new string[] { lines[lines.Length - 1].TrimStart() }).ToArray());
        }

        internal static string CleanUpLineEndings(string text)
        {
            return String.Join(Environment.NewLine, text.SplitLines(true).Select(l => l.TrimEnd()).ToArray());
        }

        public static readonly Regex AbnormalNewlineRegex = new Regex(@"\r(?=[^\n]|$)|(?<!\r)\n", RegexOptions.Compiled);

        internal static string NormalizeNewLines(string text)
        {
            if (AbnormalNewlineRegex.IsMatch(text))
                return AbnormalNewlineRegex.Replace(text, "\r\n");

            return text;
        }

        internal static string RemoveOuterWhitespace(string text)
        {
            return String.Join(Environment.NewLine, text.SplitLines(true).Select(l => l.Trim()).ToArray());
        }

        private static IEnumerable<char> _XmlEncodeDACU(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeDAC(IEnumerable<char> source)
        {
            foreach (char c in source)
            {
                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }
        }

        private static IEnumerable<char> _XmlEncodeDAW(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'D';
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'D';
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeDAU(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeDA(IEnumerable<char> source)
        {
            foreach (char c in source)
            {
                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }
        }

        private static IEnumerable<char> _XmlEncodeDCW(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'D';
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'D';
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeDCU(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        //public static readonly Regex DigitAbbrevRegex = new Regex(@"(?<=(^|[^a-zA-Z\d])\d+(\.\d+)?\s*)(?<a>[gmkt](hz|b(ps)?)|""|'|cm|bps|hz|lbs|m[Am]|nm|oz|pb|V|W)(?=[^A-Za-z\d]|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex DigitAbbrevRegex = new Regex(Properties.Settings.Default.DigitAbbrevPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //public static readonly Regex SpellOutRegex = new Regex(@"(?<![a-zA-Z\d])(A(CL?|M[DI]|PI|RP|T[AX]?)|BEDO|D(A|[Oo]D|RIMe)|E(CC|DO|IDE|MI|SD)|eSATA|I(DE?|OS|P(X|v[46])?|rDA|R|S[OA]|TX?)|i(OS|os)|LED?|NDEF|O(HRM|SI?|LED)|P(ATA|CIe|DIF)|QIC|R(E|ARP)|S(ATA|CA|EID|LI|NEP)|UEFI)(?=s?[^a-zA-Z\d]|$)", RegexOptions.Compiled);
        public static readonly Regex SpellOutRegex = new Regex(Properties.Settings.Default.SpellOutPattern, RegexOptions.Compiled);
        public static readonly Regex SubRegex = new Regex(@"(?<![a-zA-Z\d])(ADRAM|BD-R|CMOS|DVD[+-]RW?|EE?PROM|IEEE|LRDIMM|RDIMM|S(CSI|DRAM|RAM)|UDIMM|Wi-?Fi|°(\s*[FC])?|[a-zA-Z]+\+)(?=[^a-zA-Z\d]|$)", RegexOptions.Compiled);
        public static readonly Regex MiniMicroRegex = new Regex(@"(?<![a-zA-Z\d])(?<p>mini|micro)(?<a>SD(HC|XC)?)(?=[^a-zA-Z\d]|$)", RegexOptions.Compiled);
        public static readonly Regex CurrencyRegex = new Regex(@"\$\s*(?<d>\d+(,\d+)*)(\.(?<c>\d{2}))?(?=[^\d]|$)", RegexOptions.Compiled);
        public static readonly Regex DimensionsRegex = new Regex(@"(?<=\d+(\.\d+)?\s*)[×x](?=\s*\d+(\.\d+)?)", RegexOptions.Compiled);
        public static readonly Regex SignVoltsRegex = new Regex(@"(\+|-)(?<n>\d+(\.\d+)?)\s*(?<d>V([AD]C)?)(?=[^A-Za-z\d]|$)", RegexOptions.Compiled);

        internal static string AutoReplace(string selectedText)
        {
            if (String.IsNullOrWhiteSpace(selectedText))
                return selectedText;

            if (DigitAbbrevRegex.IsMatch(selectedText))
            {
                Dictionary<string, string> mapping = Properties.Settings.Default.DigitAbbrevAlias.Split(';').Select(s => s.Trim()).Where(s => s.Length > 0)
                    .Select(s => s.Split(new char[] { '=' }, 2)).SelectMany(a => a[0].Split(',').Select(k => new { Key = k, Value = a[1] }))
                    .ToDictionary(a => a.Key, a => a.Value);
                selectedText = DigitAbbrevRegex.Replace(selectedText, m =>
                {
                    string s = m.Value.Trim();
                    string alias;
                    if (!mapping.ContainsKey(s))
                        return m.Value;

                    alias = mapping[s];
                    //switch (s)
                    //{
                    //    case "GHZ":
                    //    case "GHz":
                    //    case "Ghz":
                    //        alias = "GigaHertz";
                    //        break;
                    //    case "MHZ":
                    //    case "MHz":
                    //    case "Mhz":
                    //        alias = "MegaHertz";
                    //        break;
                    //    case "KHZ":
                    //    case "KHz":
                    //    case "Khz":
                    //        alias = "KiloHertz";
                    //        break;
                    //    case "Hz":
                    //    case "hz":
                    //        alias = "Hertz";
                    //        break;
                    //    case "Tb":
                    //        alias = "TeraBits";
                    //        break;
                    //    case "TB":
                    //        alias = "TeraBytes";
                    //        break;
                    //    case "Gb":
                    //        alias = "GigaBits";
                    //        break;
                    //    case "GB":
                    //        alias = "GigaBytes";
                    //        break;
                    //    case "PB":
                    //        alias = "PentaBytes";
                    //        break;
                    //    case "Mb":
                    //        alias = "MegaBits";
                    //        break;
                    //    case "MB":
                    //        alias = "MegaBytes";
                    //        break;
                    //    case "Kb":
                    //        alias = "KiloBits";
                    //        break;
                    //    case "KB":
                    //        alias = "KiloBytes";
                    //        break;
                    //    case "Gbps":
                    //        alias = "GigaBits per second";
                    //        break;
                    //    case "GBps":
                    //        alias = "GigaBytes per second";
                    //        break;
                    //    case "Mbps":
                    //        alias = "MegaBits per second";
                    //        break;
                    //    case "MBps":
                    //        alias = "MegaBytes per second";
                    //        break;
                    //    case "Kbps":
                    //        alias = "KiloBits per second";
                    //        break;
                    //    case "KBps":
                    //        alias = "KiloBytes per second";
                    //        break;
                    //    case "bps":
                    //        alias = "bits per second";
                    //        break;
                    //    case "nm":
                    //        alias = "nanometers";
                    //        break;
                    //    case "mm":
                    //        alias = "millimeters";
                    //        break;
                    //    case "cm":
                    //        alias = "centimeters";
                    //        break;
                    //    case "\"":
                    //        alias = "inches";
                    //        break;
                    //    case "'":
                    //        alias = "feet";
                    //        break;
                    //    case "V":
                    //        alias = "Volts";
                    //        break;
                    //    case "VDC":
                    //        alias = "Volts D C";
                    //        break;
                    //    case "VAC":
                    //        alias = "Volts A C";
                    //        break;
                    //    case "W":
                    //        alias = "Watts";
                    //        break;
                    //    case "MP":
                    //        alias = "MegaPixels";
                    //        break;
                    //    case "mA":
                    //        alias = "milliamperes";
                    //        break;
                    //    case "lbs":
                    //        alias = "pounds";
                    //        break;
                    //    case "oz":
                    //        alias = "ounces";
                    //        break;
                    //    default:
                    //        alias = null;
                    //        break;
                    //}
                    
                    return String.Format("<sub alias=\" {0}\">{1}</sub>", XmlHelper.XmlEncode(alias, XmlEncodeOption.DoubleQuotedAttribute), m.Value);
                });
            }

            if (SpellOutRegex.IsMatch(selectedText))
                selectedText = SpellOutRegex.Replace(selectedText, m => String.Format("<say-as interpret-as=\"characters\">{0}</say-as>", m.Value));

            if (DimensionsRegex.IsMatch(selectedText))
                selectedText = DimensionsRegex.Replace(selectedText, m => "<sub alias=\" by \">x</sub>");

            if (MiniMicroRegex.IsMatch(selectedText))
                selectedText = MiniMicroRegex.Replace(selectedText, m => String.Format("<sub alias=\"{0} {1}\">{2}</sub>", m.Groups["p"].Value, m.Groups["a"].Value, m.Value));

            if (SignVoltsRegex.IsMatch(selectedText))
                selectedText = SignVoltsRegex.Replace(selectedText, m =>
                {
                    string alias;
                    if (m.Groups["d"].Value == "VDC")
                        alias = "Volts D C";
                    else
                        alias = (m.Groups["d"].Value == "VAC") ? "Volts A C" : "Volts";

                    return String.Format("<sub alias=\" {0}tive {1} {2}\">{3}</sub>", (m.Value[0] == '+') ? "posi" : "nega", m.Groups["n"].Value, alias, m.Value);
                });

            if (CurrencyRegex.IsMatch(selectedText))
                selectedText = CurrencyRegex.Replace(selectedText, m =>
                {
                    if (m.Groups["c"].Success)
                        return String.Format("<sub alias=\"{0} dollars and {1} cents\">{2}</sub>", m.Groups["d"].Value, m.Groups["c"].Value, m.Value);
                    return String.Format("<sub alias=\"{0} dollars \">{1}</sub>", m.Groups["d"].Value, m.Value);
                });
            
            if (!SubRegex.IsMatch(selectedText))
                return selectedText;

            return SubRegex.Replace(selectedText, m =>
            {
                string s = m.Value.Trim();
                string alias;
                switch (s)
                {
                    case "ADRAM":
                        alias = "A-DRAM";
                        break;
                    case "BD-R":
                        alias = "BD minus R";
                        break;
                    case "CMOS":
                        alias = "C Moss";
                        break;
                    case "DVD+R":
                        alias = "DVD plus R";
                        break;
                    case "DVD-R":
                        alias = "DVD minus R";
                        break;
                    case "DVD+RW":
                        alias = "DVD plus RW";
                        break;
                    case "DVD-RW":
                        alias = "DVD minus RW";
                        break;
                    case "EPROM":
                        alias = "E PROM";
                        break;
                    case "EEPROM":
                        alias = "E E PROM";
                        break;
                    case "IEEE":
                        alias = "I triple E";
                        break;
                    case "LRDIMM":
                        alias = "L R DIMM";
                        break;
                    case "RDIMM":
                        alias = "R DIMM";
                        break;
                    case "SCSI":
                        alias = "skuzzy";
                        break;
                    case "SDRAM":
                        alias = "S DRAM";
                        break;
                    case "SRAM":
                        alias = "S RAM";
                        break;
                    case "UDIMM":
                        alias = "U DIMM";
                        break;
                    case "Wi-Fi":
                    case "WiFi":
                        alias = "why fie";
                        break;
                    case "°":
                        alias = "Degrees";
                        break;
                    default:
                        if (s.EndsWith("+"))
                            alias = String.Format("{0} plus", s.Substring(0, s.Length - 1));
                        else if (s.StartsWith("°"))
                            alias = (s.EndsWith("C")) ? "Degrees Celsius" : "Degrees Fahrenheit";
                        else
                            alias = null;
                        break;
                }

                if (alias == null)
                    return m.Value;

                return String.Format("<sub alias=\"{0}\">{1}</sub>", XmlHelper.XmlEncode(alias, XmlEncodeOption.DoubleQuotedAttribute), m.Value);
            });
        }

        private static IEnumerable<char> _XmlEncodeDC(IEnumerable<char> source)
        {
            foreach (char c in source)
            {
                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }
        }

        private static IEnumerable<char> _XmlEncodeDW(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'D';
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'D';
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeDU(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeD(IEnumerable<char> source)
        {
            foreach (char c in source)
            {
                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '"':
                        yield return '&';
                        yield return 'q';
                        yield return 'u';
                        yield return 'o';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }
        }

        private static IEnumerable<char> _XmlEncodeACW(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'D';
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'D';
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeACU(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeAC(IEnumerable<char> source)
        {
            foreach (char c in source)
            {
                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }
        }

        private static IEnumerable<char> _XmlEncodeAW(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'D';
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'D';
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeAU(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeA(IEnumerable<char> source)
        {
            foreach (char c in source)
            {
                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    case '\'':
                        yield return '&';
                        yield return 'a';
                        yield return 'p';
                        yield return 'o';
                        yield return 's';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }
        }

        private static IEnumerable<char> _XmlEncodeCW(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'D';
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'D';
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeCU(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeC(IEnumerable<char> source)
        {
            foreach (char c in source)
            {
                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c) || Char.IsControl(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }
        }

        private static IEnumerable<char> _XmlEncodeW(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'D';
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'D';
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'D';
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncodeU(IEnumerable<char> source)
        {
            bool cr = false;
            foreach (char c in source)
            {
                if (c == '\r')
                {
                    if (cr)
                    {
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        yield return 'A';
                    }
                    else
                        cr = true;
                    continue;
                }

                if (c == '\n')
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                    continue;
                }

                if (cr)
                {
                    cr = false;
                    yield return '&';
                    yield return '#';
                    yield return 'x';
                    yield return 'A';
                }

                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }

            if (cr)
            {
                yield return '&';
                yield return '#';
                yield return 'x';
                yield return 'A';
            }
        }

        private static IEnumerable<char> _XmlEncode(IEnumerable<char> source)
        {
            foreach (char c in source)
            {
                switch (c)
                {
                    case '&':
                        yield return '&';
                        yield return 'a';
                        yield return 'm';
                        yield return 'p';
                        yield return ';';
                        break;
                    case '<':
                        yield return '&';
                        yield return 'l';
                        yield return 't';
                        yield return ';';
                        break;
                    case '>':
                        yield return '&';
                        yield return 'g';
                        yield return 't';
                        yield return ';';
                        break;
                    default:
                        if (!XmlConvert.IsXmlChar(c))
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            foreach (char i in ((int)c).ToString("X").ToCharArray())
                                yield return i;
                            yield return ';';
                        }
                        else
                            yield return c;
                        break;
                }
            }
        }

        #endregion

        public static string XmlDecode(string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;
            using (CharEnumerator enumerator = text.GetEnumerator())
                return new string(XmlDecode(enumerator).ToArray());
        }

        public static IEnumerable<char> XmlDecode(IEnumerable<char> source)
        {
            if (source == null)
                return null;

            using (IEnumerator<char> enumerator = source.GetEnumerator())
                return _XmlDecode(enumerator);
        }

        public static IEnumerable<char> XmlDecode(IEnumerator<char> enumerator)
        {
            if (enumerator == null)
                return null;

            return _XmlDecode(enumerator);
        }

        public static bool IsDigitChar(char c)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
            }

            return false;
        }

        public static bool IsHexChar(char c)
        {
            switch (c)
            {
                case 'A':
                case 'a':
                case 'B':
                case 'b':
                case 'C':
                case 'c':
                case 'D':
                case 'd':
                case 'E':
                case 'e':
                case 'F':
                case 'f':
                    return true;
            }

            return IsDigitChar(c);
        }

        private static IEnumerable<char> _XmlDecode(IEnumerator<char> enumerator)
        {
            bool normalChar = true;
            while (enumerator.MoveNext())
            {
                if (normalChar)
                {
                    if (enumerator.Current != '&')
                    {
                        yield return enumerator.Current;
                        continue;
                    }
                    if (!enumerator.MoveNext())
                    {
                        yield return '&';
                        break;
                    }
                }
                else
                    normalChar = true;
                
                StringBuilder sb;
                int i;
                if (enumerator.Current == '#')
                {
                    if (!enumerator.MoveNext())
                    {
                        yield return '&';
                        yield return '#';
                        yield break;
                    }
                    if (enumerator.Current == 'x' || enumerator.Current == 'X')
                    {
                        if (!enumerator.MoveNext())
                        {
                            yield return '&';
                            yield return '#';
                            yield return 'x';
                            yield break;
                        }
                        sb = new StringBuilder();
                        while (IsHexChar(enumerator.Current))
                        {
                            sb.Append(enumerator.Current);
                            if (!enumerator.MoveNext())
                            {
                                yield return '&';
                                yield return '#';
                                yield return 'x';
                                for (i = 0; i < sb.Length; i++)
                                    yield return sb[i];
                                yield break;
                            }
                        }

                        if (enumerator.Current == ';' && Int32.TryParse(sb.ToString(), System.Globalization.NumberStyles.HexNumber, null, out i))
                        {
                            char c;
                            try
                            {
                                c = (char)i;
                                sb = null;
                            }
                            catch { c = ';'; }
                            if (sb == null)
                            {
                                yield return c;
                                continue;
                            }
                            sb.Append(c);
                        }
                        yield return '&';
                        yield return '#';
                        yield return 'x';
                        sb.Append(enumerator.Current);
                    }
                    else
                    {
                        if (!enumerator.MoveNext())
                        {
                            yield return '&';
                            yield return '#';
                            yield break;
                        }
                        sb = new StringBuilder();
                        while (IsDigitChar(enumerator.Current))
                        {
                            sb.Append(enumerator.Current);
                            if (!enumerator.MoveNext())
                            {
                                yield return '&';
                                yield return '#';
                                for (i = 0; i < sb.Length; i++)
                                    yield return sb[i];
                                yield break;
                            }
                        }

                        if (enumerator.Current == ';' && Int32.TryParse(sb.ToString(), out i))
                        {
                            char c;
                            try
                            {
                                c = (char)i;
                                sb = null;
                            }
                            catch { c = ';'; }
                            if (sb == null)
                            {
                                yield return c;
                                continue;
                            }
                            sb.Append(c);
                        }
                        yield return '&';
                        yield return '#';
                        sb.Append(enumerator.Current);
                    }
                }
                else
                {
                    sb = new StringBuilder();
                    while (Char.IsLetter(enumerator.Current))
                    {
                        sb.Append(enumerator.Current);
                        if (!enumerator.MoveNext())
                        {
                            yield return '&';
                            for (i = 0; i < sb.Length; i++)
                                yield return sb[i];
                            yield break;
                        }
                    }

                    if (enumerator.Current == ';')
                    {
                        switch (sb.ToString().ToLower())
                        {
                            case "amp":
                                yield return '&';
                                sb = null;
                                break;
                            case "lt":
                                yield return '<';
                                sb = null;
                                break;
                            case "gt":
                                yield return '>';
                                sb = null;
                                break;
                            case "quot":
                                yield return '"';
                                sb = null;
                                break;
                            case "apos":
                                yield return '\'';
                                sb = null;
                                break;
                        }
                        if (sb == null)
                            continue;
                    }
                    yield return '&';
                    sb.Append(enumerator.Current);
                }

                if (sb.Length == 0)
                    continue;

                if (sb[sb.Length - 1] == '&')
                {
                    normalChar = false;
                    sb.Remove(sb.Length - 1, 1);
                }

                for (i = 0; i < sb.Length; i++)
                    yield return sb[i];
            }

            if (!normalChar)
                yield return '&';
        }
    }
}
