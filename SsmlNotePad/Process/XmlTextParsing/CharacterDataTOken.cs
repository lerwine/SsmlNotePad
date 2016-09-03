using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Process.XmlTextParsing
{
    public class CharacterDataToken : LinkedToken
    {
        private CharacterDataToken(ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken, CharacterTokenType cType, string prefix, string value, string suffix) : base(parent, previousSibling, lastToken)
        {
            throw new NotImplementedException();
        }

        public CharacterTokenType CharacterType { get; private set; }

        public override int LastLineIndex
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static LinkedToken TokenizeCDataSection(Common.BufferedTextReader reader, ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken, CancellationToken cancellationToken)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            string expected = "<![CDATA[";
            foreach (char e in expected)
            {
                if (!reader.TryRead(out c))
                    break;
                sb.Append(c);
                if (c != e)
                    return new InvalidToken(parent, previousSibling, lastToken, sb.ToString());
            }
            if (sb.Length != expected.Length)
                return new InvalidToken(parent, previousSibling, lastToken, sb.ToString());
            string prefix = sb.ToString();
            sb.Clear();
            while (reader.TryRead(out c))
            {
                if (c == ']')
                {
                    if (!reader.TryRead(out c))
                        return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.CDataSection, prefix, sb.ToString(), "]");
                    if (c == ']')
                    {
                        if (!reader.TryRead(out c))
                            return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.CDataSection, prefix, sb.ToString(), "]]");
                        if (c == '>')
                            return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.CDataSection, prefix, sb.ToString(), "]]>");
                        sb.Append(']');
                    }

                    sb.Append(']');
                }

                sb.Append(c);
            }
            return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.CDataSection, prefix, sb.ToString(), "");
        }

        internal static LinkedToken TokenizeCharacterData(Common.BufferedTextReader reader, ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken, CancellationToken cancellationToken)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            bool isWhiteSpace = true;
            while (reader.TryRead(out c))
            {
                if (c == '\r' || c == '\n' || c == '&' || c == '<' || c == '>')
                {
                    reader.UnRead(c);
                    break;
                }
                if (isWhiteSpace && !Char.IsWhiteSpace(c))
                    isWhiteSpace = false;
                sb.Append(c);
            }
            if (sb.Length == 0)
                return previousSibling;

            return new CharacterDataToken(parent, previousSibling, lastToken, (isWhiteSpace) ? CharacterTokenType.Whitespace : CharacterTokenType.Text, "", sb.ToString(), "");
        }

        internal static LinkedToken TokenizeCharacterEntity(Common.BufferedTextReader reader, ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken, CancellationToken cancellationToken)
        {
            char c;
            if (!reader.TryRead(out c))
                return previousSibling;
            if (c != '&')
            {
                reader.UnRead(c);
                return previousSibling;
            }
            if (!reader.TryRead(out c))
                return new InvalidToken(parent, previousSibling, lastToken, "&");
            StringBuilder sb = new StringBuilder();
            if (c == '#')
            {
                if (!reader.TryRead(out c))
                    return new InvalidToken(parent, previousSibling, lastToken, "&#");
                sb.Append('#');
                if (c == 'x')
                {
                    do
                    {
                        sb.Append(c);
                        if (!reader.TryRead(out c))
                            return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.CharacterEntity, "&", sb.ToString(), "");
                    } while (Char.IsNumber(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F' || c == 'a' || c == 'b' || c == 'c' || c == 'd' || c == 'e' || c == 'f');
                } else
                {
                    while (Char.IsNumber(c))
                    {
                        sb.Append(c);
                        if (!reader.TryRead(out c))
                            return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.CharacterEntity, "&", sb.ToString(), "");
                    }
                }
            } else
            {
                while (Char.IsLetter(c))
                {
                    sb.Append(c);
                    if (!reader.TryRead(out c))
                        return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.CharacterEntity, "&", sb.ToString(), "");
                }
            }
            if (c == ';')
                return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.CharacterEntity, "&", sb.ToString(), ";");

            sb.Insert(0, '&');
            return new InvalidToken(parent, previousSibling, lastToken, sb.ToString());
        }

        internal static LinkedToken TokenizeComment(Common.BufferedTextReader reader, ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken, CancellationToken cancellationToken)
        {
            char c;
            if (!reader.TryRead(out c))
                return previousSibling;
            if (c != '<')
            {
                reader.UnRead(c);
                return previousSibling;
            }
            if (!reader.TryRead(out c))
                return new InvalidToken(parent, previousSibling, lastToken, "<");
            if (c != '!')
            {
                reader.UnRead('<', c);
                return previousSibling;
            }
            if (!reader.TryRead(out c))
                return new InvalidToken(parent, previousSibling, lastToken, "<!");
            if (c != '-')
            {
                reader.UnRead('<', '!', c);
                return previousSibling;
            }
            if (!reader.TryRead(out c))
                return new InvalidToken(parent, previousSibling, lastToken, "<!-");
            if (c != '-')
                return new InvalidToken(parent, previousSibling, lastToken, new string(new char[] { '<', '!', '-', c }));
            StringBuilder sb = new StringBuilder();
            while (reader.TryRead(out c))
            {
                if (c == '-')
                {
                    if (reader.TryRead(out c))
                    {
                        if (c == '-')
                        {
                            if (reader.TryRead(out c))
                            {
                                if (c == '>')
                                    return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.Comment, "<!--", sb.ToString(), "-->");
                            }
                            else
                                return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.Comment, "<!--", sb.ToString(), "--");
                            sb.Append("-");
                        }
                    }
                    else
                        return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.Comment, "<!--", sb.ToString(), "-");

                    sb.Append("-");
                }
                else
                    sb.Append(c);
            }
            return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.Comment, "<!--", sb.ToString(), "");
        }

        internal static LinkedToken TokenizeNewLine(Common.BufferedTextReader reader, XmlTokenSet parent, LinkedToken previousSibling, XmlToken lastToken, CancellationToken cancellationToken)
        {
            char c;
            if (!reader.TryRead(out c))
                return previousSibling;

            if (c == '\r')
            {
                if (reader.TryPeek(out c) && c == '\n')
                {
                    reader.Read();
                    return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.LineSeparator, "", "\r\n", "");
                }
                return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.LineSeparator, "", "\r", "");
            }

            if (c == '\n')
                return new CharacterDataToken(parent, previousSibling, lastToken, CharacterTokenType.LineSeparator, "", "\n", "");

            reader.UnRead(c);
            return previousSibling;
        }
    }
}
