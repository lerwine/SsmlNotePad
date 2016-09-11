//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Erwine.Leonard.T.SsmlNotePad.Process.XmlTextParsing
//{
//    public abstract class LinkedToken : XmlToken
//    {
//        public abstract override int Length { get; }
//        public abstract override int LastLineIndex { get; }
//        public LinkedToken PreviousNode { get; private set; }
//        public LinkedToken NextNode { get; private set; }
//        public ITokenContainer ParentNode { get; private set; }

//        protected LinkedToken(ITokenContainer parent, LinkedToken previousSibling, XmlToken lastToken) 
//            : base((lastToken == null) ? 0 : lastToken.StartCharacterIndex + lastToken.Length, (lastToken == null) ? 0 : lastToken.LastLineIndex)
//        {
//            ParentNode = parent;
//            if (previousSibling != null)
//            {
//                previousSibling.NextNode = this;
//                PreviousNode = previousSibling;
//            }
//        }
//    }
//}
