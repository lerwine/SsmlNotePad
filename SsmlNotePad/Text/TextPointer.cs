using System;

namespace Erwine.Leonard.T.SsmlNotePad.Text
{
    public struct TextPointer : IEquatable<TextPointer>, IComparable<TextPointer>
    {
        internal const string ParameterName_source = "source";
        internal const string ParameterName_charIndex = "charIndex";
        internal const string ParameterName_lineNumber = "lineNumber";
        internal const string ParameterName_linePosition = "linePosition";
        private int _charIndex;
        private TextLineInfo _currentLine;

        public int CharIndex { get { return _charIndex; } }

        public int LineNumber { get { return (_currentLine == null) ? 1 : _currentLine.Number; } }

        public int LinePosition { get { return (_currentLine == null) ? 1 : _charIndex - _currentLine.CharIndex + 1; } }

        public TextLineInfo CurrentLine { get { return _currentLine; } }
        
        public bool IsEmpty { get { return _currentLine == null; } }

        public TextPointer(TextLineInfo source, int characterOffset)
        {
            if (source == null)
                throw new ArgumentNullException(ParameterName_source);
            
            if (characterOffset < 0)
            {
                while (characterOffset < 0)
                {
                    if (source.Previous == null)
                    {
                        if ((source.CharIndex + characterOffset) < 0)
                            throw new ArgumentOutOfRangeException("characterOffset");
                        break;
                    }
                    source = source.Previous;
                    characterOffset += source.AllText.Length;
                }
            }
            else
            {
                while (characterOffset >= source.AllText.Length)
                {
                    characterOffset -= source.AllText.Length;
                    if (source.Next == null)
                    {
                        if (characterOffset > source.AllText.Length)
                            throw new ArgumentOutOfRangeException("characterOffset");
                        characterOffset = source.AllText.Length;
                        break;
                    }
                    source = source.Next;
                }
            }

            _currentLine = source;
            _charIndex = source.CharIndex + characterOffset;
        }

        public TextPointer(TextLineInfo source, int lineOffset, int characterOffset)
        {
            if (source == null)
                throw new ArgumentNullException(ParameterName_source);

            if (lineOffset < 0)
            {
                while (lineOffset < 0)
                {
                    if ((source = source.Previous) == null)
                        throw new ArgumentOutOfRangeException("lineNumber");
                    lineOffset++;
                }
            }
            else
            {
                while (lineOffset > 0)
                {
                    if ((source = source.Next) == null)
                        throw new ArgumentOutOfRangeException("lineNumber");
                    lineOffset--;
                }
            }

            if (characterOffset < 0)
            {
                while (characterOffset < 0)
                {
                    if (source.Previous == null)
                    {
                        if ((source.CharIndex + characterOffset) < 0)
                            throw new ArgumentOutOfRangeException("characterOffset");
                        break;
                    }
                    source = source.Previous;
                    characterOffset += source.AllText.Length;
                }
            }
            else
            {
                while (characterOffset >= source.AllText.Length)
                {
                    characterOffset -= source.AllText.Length;
                    if (source.Next == null)
                    {
                        if (characterOffset > source.AllText.Length)
                            throw new ArgumentOutOfRangeException("characterOffset");
                        characterOffset = source.AllText.Length;
                        break;
                    }
                    source = source.Next;
                }
            }

            _currentLine = source;
            _charIndex = source.CharIndex + characterOffset;
        }

        public string GetText(int length)
        {
            if (_currentLine != null)
                return _currentLine.SubString(_charIndex - _currentLine.CharIndex, length);

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            return "";
        }

        public TextPointer Add(int charCount)
        {
            if (charCount == 0)
                return this;

            if (_currentLine == null)
                throw new ArgumentOutOfRangeException("charCount");
            
            try { return new TextPointer(_currentLine, (_charIndex - _currentLine.CharIndex) + charCount); }
            catch { throw new ArgumentOutOfRangeException("charCount"); }
        }

        public TextPointer Add(TextPointer value)
        {
            if ((value._currentLine == null) ? _currentLine != null : !value._currentLine.IsOfSameSet(_currentLine))
                throw new InvalidOperationException("Pointers do not derrive from the same source.");

            if (value._charIndex == 0)
                return this;

            if (_currentLine == null)
                throw new ArgumentOutOfRangeException("value");

            try { return new TextPointer(_currentLine, (_charIndex - _currentLine.CharIndex) + value._charIndex); }
            catch { throw new ArgumentOutOfRangeException("value"); }
        }

        public TextPointer Add(int lineCount, int charCount)
        {
            if (lineCount == 0 && charCount == 0)
                return this;

            if (_currentLine == null)
                throw new ArgumentOutOfRangeException((lineCount == 0) ? "charCount" : "lineCount");

            try { return new TextPointer(_currentLine, (_currentLine.Number - 1) + lineCount, (_charIndex - _currentLine.CharIndex) + charCount); }
            catch (ArgumentOutOfRangeException exc) { throw new ArgumentOutOfRangeException((exc.ParamName == "characterOffset") ? "charCount" : "lineCount"); }
        }

        public TextPointer Subtract(int charCount)
        {
            if (charCount == 0)
                return this;

            if (_currentLine == null)
                throw new ArgumentOutOfRangeException("charCount");

            try { return new TextPointer(_currentLine, (_charIndex - _currentLine.CharIndex) - charCount); }
            catch { throw new ArgumentOutOfRangeException("charCount"); }
        }

        public TextPointer Subtract(TextPointer value)
        {
            if ((value._currentLine == null) ? _currentLine != null : !value._currentLine.IsOfSameSet(_currentLine))
                throw new InvalidOperationException("Pointers do not derrive from the same source.");

            if (value._charIndex == 0)
                return this;

            if (_currentLine == null)
                throw new ArgumentOutOfRangeException("value");

            try { return new TextPointer(_currentLine, _charIndex - value._charIndex); }
            catch { throw new ArgumentOutOfRangeException("value"); }
        }

        public TextPointer Subtract(int lineCount, int charCount)
        {
            if (lineCount == 0 && charCount == 0)
                return this;

            if (_currentLine == null)
                throw new ArgumentOutOfRangeException((lineCount == 0) ? "charCount" : "lineCount");

            try { return new TextPointer(_currentLine, (_currentLine.Number - 1) - lineCount, (_charIndex - _currentLine.CharIndex) - charCount); }
            catch (ArgumentOutOfRangeException exc) { throw new ArgumentOutOfRangeException((exc.ParamName == "characterOffset") ? "charCount" : "lineCount"); }
        }

        public int CompareTo(TextPointer other)
        {
            if ((other._currentLine == null) ? _currentLine != null : !other._currentLine.IsOfSameSet(_currentLine))
                throw new InvalidOperationException("Pointers do not derrive from the same source.");

            if (other._currentLine == null)
                return 0;
            
            return _charIndex.CompareTo(other._charIndex);
        }

        public bool Equals(TextPointer other)
        {
            if ((other._currentLine == null) ? _currentLine != null : !other._currentLine.IsOfSameSet(_currentLine))
                throw new InvalidOperationException("Pointers do not derrive from the same source.");

            return other._currentLine != null && _charIndex.Equals(other._charIndex);
        }

        public override bool Equals(object obj) { return obj != null && obj is TextPointer && Equals((TextPointer)obj); }

        public override int GetHashCode() { unchecked { return (_currentLine == null) ? -1 : _charIndex; } }

        public override string ToString() { return String.Format("Line {0}, Position {1} (Index {2})", _currentLine.Number, _charIndex - _currentLine.CharIndex + 1, _charIndex); }

        public static bool operator <(TextPointer x, TextPointer y) { return (x == null) ? y != null : x.CompareTo(y) < 0; }

        public static bool operator >(TextPointer x, TextPointer y) { return (y == null) ? x != null : y.CompareTo(x) < 0; }

        public static bool operator <=(TextPointer x, TextPointer y) { return (x == null) ? true : x.CompareTo(y) <= 0; }

        public static bool operator >=(TextPointer x, TextPointer y) { return (y == null) ? true : y.CompareTo(x) <= 0; }

        public static bool operator ==(TextPointer x, TextPointer y) { return (x == null) ? y == null : x.Equals(y); }

        public static bool operator !=(TextPointer x, TextPointer y) { return (y == null) ? x == null : y.Equals(x); }
    }
}