//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Erwine.Leonard.T.SsmlNotePad.Common;

//namespace Erwine.Leonard.T.SsmlNotePad.Process.XmlTextParsing
//{
//    public sealed class ElementToken : LinkedToken, ITokenContainer
//    {
//        private int _lastLineIndex;
//        private int _length;
//        private AttributeToken[] _attributes;
//        private LinkedToken[] _content;

//        public override int LastLineIndex { get { return _lastLineIndex; } }
//        public override int Length { get { return _length; } }
//        public string Name { get; private set; }
//        public ReadOnlyCollection<AttributeToken> Attributes { get; private set; }
//        public ReadOnlyCollection<LinkedToken> Content { get; private set; }
//        public string ExtraWhitespace { get; private set; }
//        public string TagClose { get; private set; }

//        private ElementToken(ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken, string name) 
//            : base(parent, previousSibling, lastToken)
//        {
//            Name = name ?? "";
//        }

//        public static LinkedToken TokenizeElement(BufferedTextReader reader, ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken, CancellationToken cancellationToken)
//        {
//            char c;
//            if (!reader.TryRead(out c))
//                return previousSibling;
//            if (c != '<')
//            {
//                reader.UnRead(c);
//                return previousSibling;
//            }

//            if (!reader.TryRead(out c))
//                return new InvalidToken(parent, previousSibling, lastToken, "<");

//            if (c == '!' || c == '?')
//            {
//                reader.UnRead('<', c);
//                return previousSibling;
//            }

//            if (!Char.IsLetter(c))
//                return new InvalidToken(parent, previousSibling, lastToken, new String(new char[] { '<', c }));

//            StringBuilder sb = new StringBuilder();
//            while (Char.IsLetterOrDigit(c))
//            {
//                sb.Append(c);
//                if (!reader.TryRead(out c))
//                    return new ElementToken(parent, previousSibling, lastToken, sb.ToString());
//            }
//            if (c == ':')
//            {
//                sb.Append(c);
//                if (!reader.TryRead(out c))
//                    return new ElementToken(parent, previousSibling, lastToken, sb.ToString());
//                if (!Char.IsLetter(c))
//                {
//                    sb.Insert(0, '<');
//                    return new InvalidToken(parent, previousSibling, lastToken, sb.ToString());
//                }

//                while (Char.IsLetterOrDigit(c))
//                {
//                    sb.Append(c);
//                    if (!reader.TryRead(out c))
//                        return new ElementToken(parent, previousSibling, lastToken, sb.ToString());
//                }
//            }

//            string name = sb.ToString();

//            ElementToken et;
//            if (!reader.TryRead(out c))
//                return new ElementToken(parent, previousSibling, lastToken, name);
//            sb.Clear();
//            do
//            {
//                while (Char.IsWhiteSpace(c))
//                {
//                    sb.Append(c);
//                    if (!reader.TryRead(out c))
//                    {
//                        et = new ElementToken(parent, previousSibling, lastToken, name);
//                        et.ExtraWhitespace = sb.ToString();
//                        return et;
//                    }
//                }

//                throw new NotImplementedException();
//            } while (Char.IsWhiteSpace(c));
            
//            if (c == '/')
//            {
//                if (!reader.TryRead(out c))
//                {
//                    et = new ElementToken(parent, previousSibling, lastToken, name);
//                    et.ExtraWhitespace = sb.ToString();
//                    et.TagClose = "/";
//                    return et;
//                }
//                if (c != '>')
//                {
//                    sb.Insert(0, name);
//                    sb.Insert(0, '<');
//                    return new InvalidToken(parent, previousSibling, lastToken, sb.ToString());
//                }
//                et = new ElementToken(parent, previousSibling, lastToken, name);
//                et.ExtraWhitespace = sb.ToString();
//                et.TagClose = "/>";
//                return et;
//            }

//            throw new NotImplementedException();
//        }
//    }
//}
