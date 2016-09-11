//using System;

//namespace Erwine.Leonard.T.SsmlNotePad.Process.XmlTextParsing
//{
//    public class InvalidToken : LinkedToken
//    {
//        private int _lastLineIndex;
//        public string Text { get; private set; }
//        public string Message { get; private set; }
//        public override int Length { get { return Text.Length; } }

//        public override int LastLineIndex { get { return _lastLineIndex; } }

//        public InvalidToken(ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken, string text, string message = null)
//            : base(parent, previousSibling, lastToken)
//        {
//            Text = text ?? "";
//            _lastLineIndex = (lastToken == null) ? Text.LineSeparatorCount() : lastToken.LastLineIndex + Text.LineSeparatorCount();
//            Message = message ?? "Invalid Token";   
//        }
//    }
//}