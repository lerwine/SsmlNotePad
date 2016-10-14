using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Text
{
    public class MultiLine : IList<string>
    {
        private LineCollection _lines = new LineCollection();
        IndexList _lineIndexes;

        public IReadOnlyList<int> LineIndexes { get { return _lineIndexes; } }

        public MultiLine() { _lineIndexes = new IndexList(_lines); }

        public MultiLine(string text) : this() { _lines.Append(text); }

        public string this[int index]
        {
            get
            {
                lock (_lines)
                    return _lines.Item(index).LineText;
            }
            set
            {
                lock (_lines)
                    _lines.Set(index, value);
            }
        }

        public int Count
        {
            get
            {
                lock (_lines)
                    return _lines.Count;
            }
        }

        bool ICollection<string>.IsReadOnly { get { return false; } }

        public void Add(string item)
        {
            lock (_lines)
                _lines.Append(item);
        }

        public void Clear()
        {
            lock (_lines)
                _lines.Clear();
        }

        public bool Contains(string item)
        {
            lock (_lines)
                return _lines.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            lock (_lines)
                _lines.GetValues().ToArray().CopyTo(array, arrayIndex);
        }

        public int IndexOf(string item)
        {
            lock (_lines)
                return _lines.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            lock (_lines)
                _lines.Insert(index, item);
        }

        public bool Remove(string item)
        {
            lock (_lines)
                return _lines.Remove(item);
        }

        public void RemoveAt(int index)
        {
            lock (_lines)
                _lines.RemoveAt(index);
        }

        public IEnumerator<string> GetEnumerator() { return new LineTextEnumerator(_lines); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public override int GetHashCode()
        {
            lock (_lines)
                return _lines.GetHashCode();
        }

        public override string ToString()
        {
            lock (_lines)
                return _lines.ToString();
        }

        class LineInfo
        {
            private string _allText = "";
            public int Index { get; private set; }
            public int Length { get; private set; }
            public LineInfo Previous { get; private set; }
            public LineInfo Next { get; private set; }
            public string AllText { get { return _allText; } }
            public string LineText { get { return (Index == _allText.Length) ? _allText : _allText.Substring(0, Index); } }
            public LineInfo() { }
            public LineInfo(string text)
            {
                if (String.IsNullOrEmpty(text))
                    return;

                int nextIndex = text.Length;
                Length = nextIndex;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\r')
                    {
                        Length = i;
                        nextIndex = i + ((i < text.Length - 1 && text[i + 1] == '\n') ? 2 : 1);
                        break;
                    }
                    if (text[i] == '\n')
                    {
                        Length = i;
                        nextIndex = i + 1;
                        break;
                    }
                }

                if (nextIndex == text.Length)
                {
                    _allText = text;
                    if (Length != nextIndex)
                        Next = new LineInfo { Previous = this, Index = nextIndex };
                    return;
                }

                _allText = text.Substring(0, nextIndex);

                Append(this, new LineInfo(text.Substring(nextIndex)));
            }

            public static LineInfo Append(LineInfo previous, LineInfo next)
            {
                if (next == null || next._allText.Length == 0)
                {
                    if (previous != null && previous.Next == null && previous.Length == 0 && previous._allText.Length > 0)
                        previous.Next = new LineInfo { Previous = previous, Index = previous.Index + previous._allText.Length };
                    return previous;
                }

                if (next.Previous != null)
                    next.Previous.Next = null;

                if (previous == null)
                {
                    if (next.Index == 0)
                        return next;

                    next.Index = 0;
                    next.Previous = null;
                    if (next.Next != null)
                        return Append(next, next.Next);

                    if (next.Length == 0 && next._allText.Length > 0)
                        next.Next = new LineInfo { Previous = next, Index = next._allText.Length };
                    return next;
                }

                if (previous.Next != null)
                {
                    LineInfo following = next;
                    while (following.Next != null)
                        following = following.Next;
                    Append(following, previous.Next);
                    previous.Next = null;
                }

                if (previous._allText.Length == 0)
                {
                    previous._allText = next._allText;
                    previous.Length = next.Length;
                    return (next.Next == null) ? previous : Append(previous, next.Next);
                }

                if (previous.Length == previous._allText.Length)
                    previous._allText += "\r\n";

                int offset = (previous.Index + previous._allText.Length) - next.Index;
                if (offset == 0)
                    return previous;

                while (next != null)
                {
                    next.Index += offset;
                    next = next.Next;
                }

                return previous;
            }

            internal bool IsEqualTo(LineInfo lineInfo)
            {
                if (lineInfo.LineText != LineText)
                    return false;

                if (Next == null)
                    return lineInfo.Next == null;

                return lineInfo.Next != null && Next.IsEqualTo(lineInfo.Next);
            }

            internal static LineInfo Remove(LineInfo item, int count)
            {
                LineInfo previous = item.Previous;
                int offset = 0;
                for (int i = 0; i < count; i++)
                {
                    offset += item._allText.Length;
                    item = item.Next;
                }
                item.Previous = previous;
                if (previous != null)
                    previous.Next = item;
                previous = item;
                while (item != null)
                {
                    item.Index -= offset;
                    item = item.Next;
                }
                return previous;
            }

            internal void Set(string value)
            {
                int offset;
                if (String.IsNullOrEmpty(value))
                {
                    if (Length == 0)
                        return;
                    offset = Length;
                    Length = 0;
                    _allText = (offset == _allText.Length) ? "" : _allText.Substring(offset);
                    offset = 0 - offset;
                }
                else
                {
                    LineInfo lineInfo = new LineInfo(value);
                    if (lineInfo.Length == lineInfo._allText.Length)
                        _allText = (Length == _allText.Length) ? lineInfo._allText : lineInfo._allText + _allText.Substring(Length);
                    else
                        _allText = lineInfo._allText;
                    offset = lineInfo.Length - Length;
                    Length = lineInfo.Length;
                    if (lineInfo.Next != null)
                    {
                        LineInfo.Append(this, lineInfo.Next);
                        return;
                    }
                }

                for (LineInfo l = Next; l != null; l = l.Next)
                    l.Index += offset;
            }
        }

        class LineCollection
        {
            public LineInfo First { get; set; }

            public int Count { get { return GetCount(First); } }

            public static int GetCount(LineInfo lineInfo)
            {
                int index = 0;
                while (lineInfo != null)
                {
                    index++;
                    lineInfo = lineInfo.Next;
                }
                return index;
            }

            public LineInfo Item(int index)
            {
                if (index < 0)
                    throw new IndexOutOfRangeException();
                for (LineInfo lineInfo = First; lineInfo != null; lineInfo = lineInfo.Next)
                {
                    if (index == 0)
                        return lineInfo;
                    index--;
                }
                throw new IndexOutOfRangeException();
            }

            internal void Append(string text)
            {
                if (text == null)
                    return;

                if (First == null)
                    First = new LineInfo(text);
                else
                {
                    if (text.Length == 0)
                        return;
                    LineInfo previous = First;
                    while (previous.Next != null)
                        previous = previous.Next;
                    LineInfo.Append(previous, new LineInfo(text));
                }
            }

            internal bool Contains(string text)
            {
                if (text == null || First == null)
                    return false;

                LineInfo lineInfo = new LineInfo(text);
                for (LineInfo item = First; item != null; item = item.Next)
                {
                    if (item.IsEqualTo(lineInfo))
                        return true;
                }

                return false;
            }

            internal IEnumerable<string> GetValues()
            {
                for (LineInfo lineInfo = First; lineInfo != null; lineInfo = lineInfo.Next)
                    yield return lineInfo.LineText;
            }

            internal void Set(int index, string value)
            {
                if (value == null)
                    RemoveAt(index);
                else
                    Item(index).Set(value);
            }

            internal void Clear() { First = null; }

            internal int IndexOf(string item)
            {
                if (item == null || First == null)
                    return -1;

                LineInfo lineInfo = new LineInfo(item);
                int index = 0;
                for (LineInfo l = First; l != null; l = l.Next)
                {
                    if (l.IsEqualTo(lineInfo))
                        return index;
                    index++;
                }

                return -1;
            }

            internal void Insert(int index, string item)
            {
                if (item == null)
                    return;

                if (index < 0)
                    throw new IndexOutOfRangeException();

                if (First == null)
                {
                    if (index > 0)
                        throw new IndexOutOfRangeException();
                    First = new LineInfo(item);
                }

                LineInfo lineInfo = First;

                if (index == 0)
                {
                    First = new LineInfo(item);
                    LineInfo.Append(First, lineInfo);
                    return;
                }

                while (lineInfo != null)
                {
                    if (index == 1)
                    {
                        LineInfo.Append(lineInfo, new LineInfo(item));
                        return;
                    }
                    lineInfo = lineInfo.Next;
                    index--;
                }
                throw new IndexOutOfRangeException();
            }

            internal bool Remove(string text)
            {
                if (text == null || First == null)
                    return false;

                LineInfo lineInfo = new LineInfo(text);
                for (LineInfo item = First; item != null; item = item.Next)
                {
                    if (item.IsEqualTo(lineInfo))
                    {
                        if (item.Previous == null)
                            First = LineInfo.Remove(item, GetCount(lineInfo));
                        else
                            LineInfo.Remove(item, GetCount(lineInfo));
                        return true;
                    }
                }

                return false;
            }

            internal void RemoveAt(int index) { LineInfo.Remove(Item(index), 1); }

            public override string ToString()
            {
                if (First == null)
                    return "";

                if (First.Next == null)
                    return First.AllText;

                StringBuilder sb = new StringBuilder(First.AllText);
                for (LineInfo lineInfo = First.Next; lineInfo != null; lineInfo = lineInfo.Next)
                    sb.Append(lineInfo.AllText);
                return sb.ToString();
            }

            public override int GetHashCode()
            {
                if (First == null)
                    return 0;

                if (First.Next == null)
                    return First.LineText.GetHashCode();

                StringBuilder sb = new StringBuilder(First.LineText);
                for (LineInfo lineInfo = First.Next; lineInfo != null; lineInfo = lineInfo.Next)
                    sb.Append(lineInfo.LineText);
                return sb.ToString().GetHashCode();
            }
        }

        class BaseLineEnumerator : IDisposable
        {
            LineCollection _lines;
            LineInfo _current = null;
            bool _enumerated = false;
            protected BaseLineEnumerator(LineCollection lines)
            {
                _lines = lines;
            }

            protected LineInfo CurrentLine
            {
                get
                {
                    if (_lines == null)
                        throw new ObjectDisposedException(typeof(MultiLine).FullName);

                    if (_enumerated && _current != null)
                        return _current;

                    throw new InvalidOperationException();
                }
            }

            public bool MoveNext()
            {
                if (_lines == null)
                    throw new ObjectDisposedException(typeof(MultiLine).FullName);

                if (_current == null)
                {
                    if (_enumerated)
                        return false;
                    _enumerated = true;
                    _current = _lines.First;
                }
                else
                    _current = _current.Next;

                return _current != null;
            }

            public void Reset()
            {
                if (_lines == null)
                    throw new ObjectDisposedException(typeof(MultiLine).FullName);

                _enumerated = false;
                _current = null;
            }

            #region IDisposable Support

            protected virtual void Dispose(bool disposing)
            {
                if (_lines != null && disposing)
                    _lines = null;
            }

            public void Dispose() { Dispose(true); }

            #endregion

        }

        class LineIndexEnumerator : BaseLineEnumerator, IEnumerator<int>
        {
            public LineIndexEnumerator(LineCollection lines) : base(lines) { }

            public int Current { get { return CurrentLine.Index; } }

            object IEnumerator.Current { get { return Current; } }
        }

        class LineTextEnumerator : BaseLineEnumerator, IEnumerator<string>
        {
            public LineTextEnumerator(LineCollection lines) : base(lines) { }

            public string Current { get { return CurrentLine.LineText; } }

            object IEnumerator.Current { get { return Current; } }
        }

        class IndexList : IReadOnlyList<int>
        {
            private LineCollection _lines;

            public IndexList(LineCollection lines) { _lines = lines; }

            public int this[int index] { get { return _lines.Item(index).Index; } }

            public int Count { get { return _lines.Count; } }

            public IEnumerator<int> GetEnumerator() { return new LineIndexEnumerator(_lines); }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }
    }
}
