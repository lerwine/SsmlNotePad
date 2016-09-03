using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public static class XmlHelper
    {
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
