//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Globalization;
//using System.IO;
//using System.Text;
//using System.Threading;
//using System.Xml.Linq;

//namespace Erwine.Leonard.T.SsmlNotePad.Process.XmlTextParsing
//{
//    public class XmlTokenSet : ITokenContainer
//    {
//        public Encoding Encoding { get; private set; }

//        private List<LinkedToken> _innerList = new List<LinkedToken>();

//        public ReadOnlyCollection<LinkedToken> Content { get; private set; }
//        public LinkedToken this[int index] { get { return _innerList[index]; } }
        
//        public int Count { get { return _innerList.Count; } }

//        public static XmlTokenSet Tokenize(StreamReader streamReader, CancellationToken token)
//        {
//            if (streamReader == null)
//                throw new ArgumentNullException("streamReader");

//            using (Common.BufferedTextReader bufferedReader = new Common.BufferedTextReader(streamReader))
//            {
//                XmlTokenSet result = _Tokenize(bufferedReader, token);
//                if (result.Encoding == null)
//                    result.Encoding = streamReader.CurrentEncoding;
//                return result;
//            }
//        }

//        public static XmlTokenSet Tokenize(TextReader textReader, CancellationToken token, Encoding defaultEncoding = null)
//        {
//            if (textReader == null)
//                throw new ArgumentNullException("textReader");

//            using (Common.BufferedTextReader bufferedReader = new Common.BufferedTextReader(textReader))
//            {
//                XmlTokenSet result = _Tokenize(bufferedReader, token);
//                if (result.Encoding == null)
//                    result.Encoding = defaultEncoding ?? Encoding.UTF8;
//                return result;
//            }
//        }

//        private XmlTokenSet() { Content = new ReadOnlyCollection<XmlTextParsing.LinkedToken>(_innerList); }

//        private static XmlTokenSet _Tokenize(Common.BufferedTextReader reader, CancellationToken cancellationToken)
//        {
//            XmlTokenSet result = new XmlTokenSet();
//            string s;
//            XmlTokenType tokenType;
//            LinkedToken previousSibling = null;
//            if (cancellationToken.IsCancellationRequested)
//                return result;
            
//            while ((tokenType = ReadNextNodeType(reader)) != XmlTokenType.None)
//            {
//                switch (tokenType)
//                {
//                    case XmlTokenType.CDataSection:
//                        previousSibling = CharacterDataToken.TokenizeCDataSection(reader, result, previousSibling, previousSibling, cancellationToken);
//                        break;
//                    case XmlTokenType.CharacterData:
//                        previousSibling = CharacterDataToken.TokenizeCharacterData(reader, result, previousSibling, previousSibling, cancellationToken);
//                        break;
//                    case XmlTokenType.CharacterEntity:
//                        previousSibling = CharacterDataToken.TokenizeCharacterEntity(reader, result, previousSibling, previousSibling, cancellationToken);
//                        break;
//                    case XmlTokenType.Comment:
//                        previousSibling = CharacterDataToken.TokenizeComment(reader, result, previousSibling, previousSibling, cancellationToken);
//                        break;
//                    case XmlTokenType.DocType:
//                        previousSibling = DocumentTypeToken.TokenizeDocType(reader, result, previousSibling, previousSibling, cancellationToken);
//                        break;
//                    case XmlTokenType.Element:
//                        previousSibling = ElementToken.TokenizeElement(reader, result, previousSibling, previousSibling, cancellationToken);
//                        break;
//                    case XmlTokenType.NewLine:
//                        previousSibling = CharacterDataToken.TokenizeNewLine(reader, result, previousSibling, previousSibling, cancellationToken);
//                        break;
//                    case XmlTokenType.ProcessingInstruction:
//                        previousSibling = ProcessingInstructionToken.TokenizeProcessingInstruction(reader, result, previousSibling, previousSibling, cancellationToken);
//                        break;
//                    default:
//                        reader.Read();
//                        char c;
//                        if (reader.TryRead(out c))
//                        {
//                            if (c == '!')
//                            {
//                                if (reader.TryRead(out c))
//                                    previousSibling = new InvalidToken(result, previousSibling, previousSibling, new String(new char[] { '<', '!', c }));
//                                else
//                                    previousSibling = new InvalidToken(result, previousSibling, previousSibling, new String(new char[] { '<', '!' }));
//                            }
//                            else
//                            {
//                                previousSibling = new InvalidToken(result, previousSibling, previousSibling, new String(new char[] { '<', c }));
//                            }
//                        }
//                        else
//                            previousSibling = new InvalidToken(result, previousSibling, previousSibling, "<");
//                        result._innerList.Add(previousSibling);
//                        break;
//                }

//                if (cancellationToken.IsCancellationRequested)
//                    return result;
//            }

//            return result;
//        }

//        private static XmlTokenType ReadNextNodeType(Common.BufferedTextReader reader)
//        {
//            char c;
//            if (!reader.TryPeek(out c))
//                return XmlTokenType.None;

//            if (c == '>')
//                return XmlTokenType.InvalidToken;

//            if (c == '&')
//                return XmlTokenType.CharacterEntity;

//            if (c == '<')
//            {
//                reader.Read();
//                if (!reader.TryPeek(out c))
//                {
//                    reader.UnRead('<');
//                    return XmlTokenType.InvalidToken;
//                }
//                if (c == '?')
//                {
//                    reader.UnRead('<');
//                    return XmlTokenType.ProcessingInstruction;
//                }

//                if (c == '!')
//                {
//                    reader.Read();
//                    if (!reader.TryPeek(out c))
//                    {
//                        reader.UnRead('<', '!');
//                        return XmlTokenType.InvalidToken;
//                    }
//                    reader.UnRead('<', '!');
//                    if (c == '-')
//                        return XmlTokenType.Comment;
//                    if (c == 'D')
//                        return XmlTokenType.DocType;
//                    if (c == '[')
//                        return XmlTokenType.CDataSection;

//                    return XmlTokenType.InvalidToken;
//                }
//                if (Char.IsLetter(c))
//                {
//                    reader.UnRead('<');
//                    return XmlTokenType.Element;
//                }
//                reader.UnRead('<');
//                return XmlTokenType.InvalidToken;
//            }

//            if (c == '\r' || c == '\n')
//                return XmlTokenType.NewLine;

//            return XmlTokenType.CharacterData;
//        }
//    }
//}