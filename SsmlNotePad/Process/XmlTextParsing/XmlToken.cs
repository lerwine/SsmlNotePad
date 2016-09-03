using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Process.XmlTextParsing
{
    public abstract class XmlToken
    {
        public int StartCharacterIndex { get; private set; }
        public int StartLineIndex { get; private set; }
        public abstract int Length { get; }
        public abstract int LastLineIndex { get; }
        protected XmlToken(int characterIndex, int lineIndex)
        {
            StartCharacterIndex = characterIndex;
            StartLineIndex = lineIndex;
        }
    }
}
