using System;

namespace Erwine.Leonard.T.SsmlNotePad.Text
{
    public struct TextRange
    {
        internal const string ParameterName_source = "source";
        internal const string ParameterName_startIndex = "startIndex";
        internal const string ParameterName_endIndex = "endIndex";
        internal const string ParameterName_startLineNumber = "startLineNumber";
        internal const string ParameterName_startLinePosition = "startLinePosition";
        internal const string ParameterName_endLineNumber = "endLineNumber";
        internal const string ParameterName_endLinePosition = "endLinePosition";
        private TextPointer _start;
        private TextPointer _end;

        public TextPointer Start { get { return _start; } }

        public TextPointer End { get { return _end; } }

        public int Length { get { return End.CharIndex - Start.CharIndex; } }

        public TextRange(TextLineInfo source, int offset, int length)
        {
            if (source == null)
                throw new ArgumentNullException(ParameterName_source);

            TextPointer start;
            try { start = new TextPointer(source, offset); }

            catch { throw new ArgumentOutOfRangeException("offset"); }
            TextPointer end;
            try { end = start.Add(length); }
            catch { throw new ArgumentOutOfRangeException("length"); }

            if (end < start)
            {
                _start = end;
                _end = start;
            }
            else
            {
                _start = start;
                _end = end;
            }
        }

        public TextRange(TextPointer start, int length)
        {
            if (length == 0)
            {
                _start = start;
                _end = start;
                return;
            }
            TextPointer end;
            try { end = start.Add(length); }
            catch { throw new ArgumentOutOfRangeException("length"); }

            if (end < start)
            {
                _start = end;
                _end = start;
            }
            else
            {
                _start = start;
                _end = end;
            }
        }

        public TextRange(TextPointer start, TextPointer end)
        {
            if ((start.CurrentLine == null) ? end.CurrentLine != null : !start.CurrentLine.IsOfSameSet(end.CurrentLine))
                throw new InvalidOperationException("Start and end are not from the same source.");

            if (end < start)
            {
                _start = end;
                _end = start;
            }
            else
            {
                _start = start;
                _end = end;
            }
        }

        public TextRange SetStart(int charIndex)
        {
            if (charIndex < 0)
                throw new ArgumentOutOfRangeException("charIndex");

            if (charIndex == _start.CharIndex)
                return this;

            if (_start.IsEmpty)
                throw new ArgumentOutOfRangeException("charIndex");

            TextLineInfo source = _start.CurrentLine;
            if (charIndex < source.CharIndex)
            {
                while (charIndex < source.CharIndex)
                    source = source.Previous;
            }
            else
            {
                while (charIndex > source.CharIndex + source.Length)
                {
                    if ((source = source.Next) == null)
                        throw new ArgumentOutOfRangeException("charIndex");
                }
            }

            return new TextRange(source, charIndex - source.CharIndex, _end.CharIndex);
        }

        public TextRange SetStart(TextPointer start)
        {
            if (start == _start)
                return this;

            try { return new TextRange(start, _end); }
            catch (ArgumentOutOfRangeException) { throw new ArgumentOutOfRangeException("start"); }
        }

        public TextRange SetStart(int lineNumber, int linePosition)
        {
            if (lineNumber < 1)
                throw new ArgumentOutOfRangeException("lineNumber");

            if (linePosition < 1)
                throw new ArgumentOutOfRangeException("linePosition");

            if (lineNumber == _start.LineNumber && linePosition == _start.LinePosition)
                return this;

            if (_start.IsEmpty)
                throw new ArgumentOutOfRangeException((lineNumber == 1) ? "linePosition" : "lineNumber");

            TextLineInfo source = _start.CurrentLine;
            if (lineNumber < source.Number)
            {
                while (lineNumber < source.Number)
                    source = source.Previous;
            }
            else
            {
                while (lineNumber > source.Number)
                {
                    if ((source = source.Next) == null)
                        throw new ArgumentOutOfRangeException("lineNumber");
                }
            }
            int charIndex = linePosition - 1;
            if (charIndex < source.CharIndex)
            {
                while (charIndex < source.CharIndex)
                    source = source.Previous;
            }
            else
            {
                while (charIndex > source.CharIndex + source.Length)
                {
                    if ((source = source.Next) == null)
                        throw new ArgumentOutOfRangeException("charIndex");
                }
            }

            return new TextRange(source, charIndex - source.CharIndex, _end.CharIndex);
        }

        public TextRange AddStart(int charCount) { return (charCount == 0) ? this : new TextRange(_start.Add(charCount), _end); }

        public TextRange AddStart(int lineCount, int charCount) { return (lineCount == 0 && charCount == 0) ? this : new TextRange(_start.Add(lineCount, charCount), _end); }

        public TextRange SubtractStart(int charCount) { return (charCount == 0) ? this : new TextRange(_start.Subtract(charCount), _end); }

        public TextRange SubtractStart(int lineCount, int charCount) { return (lineCount == 0 && charCount == 0) ? this : new TextRange(_start.Subtract(lineCount, charCount), _end); }

        public TextRange SetEnd(int charIndex)
        {
            if (charIndex < 0)
                throw new ArgumentOutOfRangeException("charIndex");

            if (charIndex == _end.CharIndex)
                return this;

            if (_end.IsEmpty)
                throw new ArgumentOutOfRangeException("charIndex");

            TextLineInfo source = _end.CurrentLine;
            if (charIndex < source.CharIndex)
            {
                while (charIndex < source.CharIndex)
                    source = source.Previous;
            }
            else
            {
                while (charIndex > source.CharIndex + source.Length)
                {
                    if ((source = source.Next) == null)
                        throw new ArgumentOutOfRangeException("charIndex");
                }
            }

            return new TextRange(source, _start.CharIndex, charIndex - source.CharIndex);
        }

        public TextRange SetEnd(TextPointer end)
        {
            if (end == _end)
                return this;

            try { return new TextRange(_start, end); }
            catch (ArgumentOutOfRangeException) { throw new ArgumentOutOfRangeException("end"); }
        }

        public TextRange SetEnd(int lineNumber, int linePosition)
        {
            if (lineNumber < 1)
                throw new ArgumentOutOfRangeException("lineNumber");

            if (linePosition < 1)
                throw new ArgumentOutOfRangeException("linePosition");

            if (lineNumber == _end.LineNumber && linePosition == _end.LinePosition)
                return this;

            if (_end.IsEmpty)
                throw new ArgumentOutOfRangeException((lineNumber == 1) ? "linePosition" : "lineNumber");

            TextLineInfo source = _end.CurrentLine;
            if (lineNumber < source.Number)
            {
                while (lineNumber < source.Number)
                    source = source.Previous;
            }
            else
            {
                while (lineNumber > source.Number)
                {
                    if ((source = source.Next) == null)
                        throw new ArgumentOutOfRangeException("lineNumber");
                }
            }
            int charIndex = linePosition - 1;
            if (charIndex < source.CharIndex)
            {
                while (charIndex < source.CharIndex)
                    source = source.Previous;
            }
            else
            {
                while (charIndex > source.CharIndex + source.Length)
                {
                    if ((source = source.Next) == null)
                        throw new ArgumentOutOfRangeException("charIndex");
                }
            }

            return new TextRange(source, _start.CharIndex, charIndex - source.CharIndex);
        }

        public TextRange AddEnd(int charCount) { return (charCount == 0) ? this : new TextRange(_start, _end.Add(charCount)); }

        public TextRange AddEnd(int lineCount, int charCount) { return (lineCount == 0 && charCount == 0) ? this : new TextRange(_start, _end.Add(lineCount, charCount)); }

        public TextRange SubtractEnd(int charCount) { return (charCount == 0) ? this : new TextRange(_start, _end.Subtract(charCount)); }

        public TextRange SubtractEnd(int lineCount, int charCount) { return (lineCount == 0 && charCount == 0) ? this : new TextRange(_start, _end.Subtract(lineCount, charCount)); }

        public string GetText()
        {
            if (_start.CharIndex == _end.CharIndex)
                return "";

            return _start.GetText(_end.CharIndex - _start.CharIndex);
        }
    }
}