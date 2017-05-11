using System;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class LineNumbersGeneratedEventArgs : EventArgs
    {
        private LineNumberInfo[] _result;
        public LineNumberInfo[] Result { get { return _result; } }

        public LineNumbersGeneratedEventArgs(LineNumberInfo[] result)
        {
            _result = result ?? new LineNumberInfo[0];
        }
    }
}