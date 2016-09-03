using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Process.XmlTextParsing
{
    public sealed class AttributeToken : XmlToken
    {
        private int _lastLineIndex;
        private int _length;
        private LinkedToken[] _content;

        private AttributeToken(int characterIndex, int lineIndex) : base(characterIndex, lineIndex)
        {
        }

        public override int LastLineIndex { get { return _lastLineIndex; } }
        public override int Length { get { return _length; } }
        public string LeadingWhitespace { get; private set; }
        public string Name { get; private set; }
        public string AssignmentOperator { get; private set; }
        public string OpenQuote { get; private set; }
        public ReadOnlyCollection<LinkedToken> Content { get; private set; }
        public string CloseQuote { get; private set; }
    }
}
