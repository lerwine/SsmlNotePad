using System;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class LinesParsedEventArgs : EventArgs
    {
        private TextLine[] _result;

        public TextLine[] Result { get { return _result; } }

        public LinesParsedEventArgs(TextLine[] result)
        {
            _result = result ?? new TextLine[0];
        }
    }
}