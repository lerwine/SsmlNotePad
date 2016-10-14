using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad
{
    public static class StringExtensions
    {
        public static readonly Regex OuterWhitespaceRegex = new Regex(@"^(?<l>\s+)?(?<c>\S+(\s+\S+)*)(?<t>\s+)?$", RegexOptions.Compiled);
        public static string ExtractOuterWhitespace(this string s, out string leadingWs, out string trailingWs)
        {
            if (s == null)
            {
                leadingWs = null;
                trailingWs = null;
                return null;
            }

            Match m = OuterWhitespaceRegex.Match(s);
            if (m.Success)
            {
                leadingWs = (m.Groups["l"].Success) ? m.Groups["l"].Value : "";
                trailingWs = (m.Groups["t"].Success) ? m.Groups["t"].Value : "";
                return m.Groups["c"].Value;
            }
            leadingWs = s;
            trailingWs = "";
            return "";
        }
        public static string AsNormalizedWhitespace(this string s, bool nullAsEmpty = false, bool noTrim = false)
        {
            if (s == null)
                return (nullAsEmpty) ? "" : null;

            if ((noTrim) ? s.Length == 0 : (s = s.Trim()).Length == 0)
                return s;

            return String.Join(" ", SplitWhiteSpace(s));
        }

        public static string AsNormalizedLineEndings(this string s, bool unix = false, bool includeExtended = false)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            return String.Join((unix) ? "\n" : "\r\n", SplitLines(s, includeExtended));
        }

        public static int LineSeparatorCount(this string s, bool includeExtended = false)
        {
            if (String.IsNullOrEmpty(s))
                return 0;

            int count = 0;
            bool cr = false;
            foreach (char c in s)
            {
                if (c == '\r')
                {
                    if (cr)
                        count++;
                    else
                        cr = true;
                    continue;
                }
                if (c == '\n')
                {
                    count++;
                    cr = false;
                    continue;
                }
                if (cr)
                {
                    count++;
                    cr = false;
                }
                if (includeExtended)
                {
                    UnicodeCategory uc = Char.GetUnicodeCategory(c);
                    if (uc == UnicodeCategory.LineSeparator || uc == UnicodeCategory.ParagraphSeparator)
                        count++;
                }
            }
            if (cr)
                count++;
            return count;
        }

        public static IEnumerable<string> SplitWhiteSpace(this string s)
        {
            if (s == null)
                yield break;

            int start = 0;
            for (int i = 0; i < s.Length; i++)
            {
                UnicodeCategory c = Char.GetUnicodeCategory(s[i]);
                if (c != UnicodeCategory.SpaceSeparator && c != UnicodeCategory.Control && c != UnicodeCategory.LineSeparator && c != UnicodeCategory.ParagraphSeparator)
                    continue;

                if (start < i)
                    yield return s.Substring(start, i - start);
                else
                    yield return "";
                for (int n = i + 1; n < s.Length; n++)
                {
                    c = Char.GetUnicodeCategory(s[n]);
                    if (c != UnicodeCategory.SpaceSeparator && c != UnicodeCategory.Control && c != UnicodeCategory.LineSeparator && c != UnicodeCategory.ParagraphSeparator)
                    {
                        i = n - 1;
                        break;
                    }
                }
                start = i + 1;
            }
            if (start == 0)
                yield return s;
            else if (start < s.Length)
                yield return s.Substring(start);
            else
                yield return "";
        }

        public static List<string> SplitWhiteSpace(this string s, out List<string> whiteSpace)
        {
            List<string> nonWs = new List<string>();
            whiteSpace = new List<string>();
            if (s == null)
                return nonWs;

            int start = 0;
            for (int i = 0; i < s.Length; i++)
            {
                UnicodeCategory c = Char.GetUnicodeCategory(s[i]);
                if (c != UnicodeCategory.SpaceSeparator && c != UnicodeCategory.Control && c != UnicodeCategory.LineSeparator && c != UnicodeCategory.ParagraphSeparator)
                    continue;

                if (start < i)
                    nonWs.Add(s.Substring(start, i - start));
                else
                    nonWs.Add("");
                int si = i;
                for (int n = i + 1; n < s.Length; n++)
                {
                    c = Char.GetUnicodeCategory(s[n]);
                    if (c != UnicodeCategory.SpaceSeparator && c != UnicodeCategory.Control && c != UnicodeCategory.LineSeparator && c != UnicodeCategory.ParagraphSeparator)
                    {
                        i = n - 1;
                        start = n;
                        break;
                    }
                }
                if (i == si)
                    start = i + 1;

                whiteSpace.Add(s.Substring(start, start - si));
            }
            if (start == 0)
                nonWs.Add(s);
            else if (start < s.Length)
                nonWs.Add(s.Substring(start));
            else
                nonWs.Add("");
            return nonWs;
        }

        public static IEnumerable<string> SplitLines(this string s, bool includeExtended = false)
        {
            if (s == null)
                yield break;

            int start = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                bool jump;
                if (c == '\r')
                    jump = i < s.Length - 1 && s[i + 1] == '\n';
                else if (c == '\n' || (includeExtended && (Char.GetUnicodeCategory(c) == UnicodeCategory.LineSeparator || Char.GetUnicodeCategory(c) == UnicodeCategory.ParagraphSeparator)))
                    jump = false;
                else
                    continue;
                if (start < i)
                    yield return s.Substring(start, i - start);
                else
                    yield return "";
                if (jump)
                    i++;
                start = i + 1;
            }
            if (start == 0)
                yield return s;
            else if (start < s.Length)
                yield return s.Substring(start);
            else
                yield return "";
        }

        public static List<string> SplitLines(this string s, out List<string> lineEndings)
        {
            return s.SplitLines(false, out lineEndings);
        }

        public static List<string> SplitLines(this string s, bool includeExtended, out List<string> lineEndings)
        {
            List<string> lines = new List<string>();
            lineEndings = new List<string>();
            if (s == null)
                return lines;

            int start = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                string nl;
                if (c == '\r')
                {
                    if (i < s.Length - 1 && s[i + 1] == '\n')
                        nl = "\r\n";
                    else
                        nl = "\r";
                }
                else if (c == '\n' || (includeExtended && (Char.GetUnicodeCategory(c) == UnicodeCategory.LineSeparator || Char.GetUnicodeCategory(c) == UnicodeCategory.ParagraphSeparator)))
                    nl = c.ToString();
                else
                    continue;
                lineEndings.Add(nl);
                if (start < i)
                    lines.Add(s.Substring(start, i - start));
                else
                    lines.Add("");
                if (nl.Length == 2)
                    i++;
            }
            if (start == 0)
                lines.Add(s);
            else if (start < s.Length)
                lines.Add(s.Substring(start));
            else
                lines.Add("");
            return lines;
        }
        
        public static string AsTruncated(this string s, int maxLen, bool normalizeOnOversize = false, bool nullAsEmpty = false)
        {
            if (s == null)
                return s;

            if (s.Length <= maxLen || (normalizeOnOversize && (s = s.AsNormalizedWhitespace(nullAsEmpty)).Length <= maxLen))
                return s;

            return s.Substring(0, maxLen);
        }

        public static string DefaultIfNullOrEmpty(this string s, string defaultValue) { return (String.IsNullOrEmpty(s)) ? defaultValue : s; }

        public static string DefaultIfNullOrWhiteSpace(this string s, string defaultValue) { return (String.IsNullOrWhiteSpace(s)) ? defaultValue : s; }
    }

    public class StringLines : IList<string>, IEquatable<StringLines>, IEquatable<string>, IComparable<StringLines>, IComparable<string>
    {
        private object _syncRoot = new object();
        private string _text = null;
        private List<string> _lines = new List<string>();
        private List<string> _endings = new List<string>();
        private string _defaultNewLine = "\r\n";

        public string DefaultNewLine
        {
            get { return _defaultNewLine; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _defaultNewLine = "\r\n";
                else
                {
                    if ((value.Length == 2) ? value != "\r\n" : value.Length != 1 || !IsLineSeparatorChar(value[0]))
                        throw new ArgumentException("Invalid newline string", "value");
                    _defaultNewLine = value;
                }
            }
        }

        public static bool IsLineSeparatorChar(char c, bool includeExtended = false)
        {
            if (c == '\r' || c == '\n')
                return true;

            if (!includeExtended)
                return false;

            UnicodeCategory u = Char.GetUnicodeCategory(c);
            return u == UnicodeCategory.LineSeparator || u == UnicodeCategory.ParagraphSeparator;
        }

        public static bool IsLineSeparator(string s, int index, out int length)
        {
            return IsLineSeparator(s, index, false, out length);
        }
        public static bool IsLineSeparator(string s, int index, bool includeExtended, out int length)
        {
            char c = s[index];
            if (c == '\r')
            {
                length = (index < s.Length - 1 && s[index + 1] == '\n') ? 2 : 1;
                return true;
            }
            else if (c == '\n')
            {
                length = 1;
                return true;
            }

            if (includeExtended)
            {
                UnicodeCategory u = Char.GetUnicodeCategory(c);
                if (u == UnicodeCategory.LineSeparator || u == UnicodeCategory.ParagraphSeparator)
                {
                    length = 1;
                    return true;
                }
            }

            length = 0;
            return false;
        }

        public static Task<string> ReplaceLineSeparatorsAsync(string s, bool includeExtended = false)
        {
            return Task.Factory.StartNew(() =>
            {
                if (String.IsNullOrEmpty(s) || !s.Any(c => IsLineSeparatorChar(c, includeExtended)))
                    return s;

                StringBuilder sb = new StringBuilder();

                using (CharEnumerator e = s.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        if (!IsLineSeparatorChar(e.Current, includeExtended))
                        {
                            sb.Append(e.Current);
                            break;
                        }
                    }

                    if (sb.Length > 0)
                    {
                        bool isStartOfLine = false;

                        while (e.MoveNext())
                        {
                            if (IsLineSeparatorChar(e.Current, includeExtended))
                                isStartOfLine = true;
                            else
                            {
                                if (isStartOfLine)
                                {
                                    sb.Append(' ');
                                    isStartOfLine = false;
                                }
                            }
                        }
                    }
                }

                return sb.ToString();
            });
        }

        public static string ReplaceLineSeparators(string s, bool includeExtended = false) { return ReplaceLineSeparatorsAsync(s, includeExtended).Result; }

        public StringLines()
        {
            _lines = new List<string>();
            _endings = new List<string>();
            _text = null;
        }

        public StringLines(string text, string defaultNewLine = null, bool leaveEndingsAsIs = false, bool includeExtended = false)
        {
            if (!String.IsNullOrEmpty(defaultNewLine))
                DefaultNewLine = defaultNewLine;

            if (leaveEndingsAsIs && !String.IsNullOrEmpty(text) && text.Any(c => IsLineSeparatorChar(c, includeExtended)))
            {
                Task.Factory.StartNew(() => _lines = text.SplitLines(includeExtended, out _endings)).Wait();
                return;
            }

            _text = text;

            if (text == null)
            {
                _lines = new List<string>();
                _endings = new List<string>();
            }
            else
            {
                _lines = null;
                _endings = null;
            }
        }

        public StringLines(params string[] lines) : this((lines == null) ? null : lines.AsEnumerable()) { }

        public StringLines(IEnumerable<string> collection, string defaultNewLine = null, bool includeExtended = false)
        {
            Task.Factory.StartNew(() =>
            {
                if (!String.IsNullOrEmpty(defaultNewLine))
                    DefaultNewLine = defaultNewLine;

                string[] lines;
                if (collection == null || (lines = collection.Select(l => (String.IsNullOrEmpty(l)) ? "" : ReplaceLineSeparators(l, includeExtended))
                    .ToArray()).Length == 0)
                {
                    _lines = new List<string>();
                    _endings = new List<string>();
                }
                else
                {
                    _lines = new List<string>(lines);
                    if (lines.Length > 1)
                        _endings = new List<string>(Enumerable.Repeat(DefaultNewLine, lines.Length - 1));
                    else
                        _endings = new List<string>();
                }
                _text = null;
            }).Wait();
        }

        public string Text
        {
            get { return WithText(() => _text); }
            set
            {
                lock (_syncRoot)
                {
                    if (value == null)
                    {
                        if (_lines != null)
                        {
                            if (_lines.Count == 0)
                                return;
                            _lines.Clear();
                            _endings.Clear();
                        }
                        else
                        {
                            _lines = new List<string>();
                            _endings = new List<string>();
                        }
                    }
                    else
                    {
                        if (_text != null && _text == value)
                            return;
                        _lines = null;
                        _endings = null;
                    }

                    _text = value;
                }
            }
        }

        private void _EnsureLines(bool includeExtended)
        {
            if (_lines == null)
                Task.Factory.StartNew(() => _lines = _text.SplitLines(includeExtended, out _endings)).Wait();
        }

        private void _EnsureText()
        {
            if (_text != null || _lines.Count == 0)
                return;

            Task.Factory.StartNew(() =>
            {
                StringBuilder sb = new StringBuilder(_lines[0]);
                for (int i = 0; i < _endings.Count; i++)
                {
                    sb.Append(_endings[i]);
                    sb.Append(_lines[i + 1]);
                }
                _text = sb.ToString();
            });
        }

        private void WithText(Action action)
        {
            lock (_syncRoot)
            {
                _EnsureText();
                action();
            }
        }

        private T WithText<T>(Func<T> func)
        {
            lock (_syncRoot)
            {
                _EnsureText();
                return func();
            }
        }

        private void WithLines(Action action, bool includeExtended = false)
        {
            lock (_syncRoot)
            {
                _EnsureLines(includeExtended);
                action();
            }
        }

        private T WithLines<T>(Func<T> func, bool includeExtended = false)
        {
            lock (_syncRoot)
            {
                _EnsureLines(includeExtended);
                return func();
            }
        }

        public int Length { get { return WithText(() => (_text == null) ? 0 : _text.Length); } }

        public int Count { get { return WithLines(() => _lines.Count); } }

        bool ICollection<string>.IsReadOnly { get { return false; } }
        
        public string this[int index]
        {
            get { return WithLines(() => _lines[index]); }
            set
            {
                WithLines(() =>
                {
                    string s = (String.IsNullOrEmpty(value)) ? "" : ReplaceLineSeparators(value);
                    if (s == _lines[index])
                        return;
                    _lines[index] = s;
                    _text = null;
                });
            }
        }

        public int IndexOf(string item)
        {
            return WithLines(() => _lines.IndexOf((String.IsNullOrEmpty(item)) ? "" : ReplaceLineSeparators(item)));
        }
        
        public void Insert(int index, string item)
        {
            WithLines(() =>
            {
                _lines.Insert(index, (String.IsNullOrEmpty(item)) ? "" : ReplaceLineSeparators(item));
                _text = null;
                if (_lines.Count == 1)
                    return;
                if (index == _endings.Count)
                    _endings.Add(DefaultNewLine);
                else
                    _endings.Insert((index > 0) ? index - 1 : 0, DefaultNewLine);
            });
        }

        public void RemoveAt(int index)
        {
            WithLines(() =>
            {
                _lines.RemoveAt(index);
                _text = null;
                if (_lines.Count == 0)
                    return;

                _endings.RemoveAt((index > 0) ? index - 1 : 0);
            });
        }

        public void Add(string item)
        {
            WithLines(() =>
            {
                if (_lines.Count > 0)
                    _endings.Add(DefaultNewLine);
                _lines.Add((String.IsNullOrEmpty(item)) ? "" : ReplaceLineSeparators(item));
                _text = null;
            });
        }

        public void AddRange(params string[] list) { AddRange((list == null) ? null : list.AsEnumerable()); }

        public void AddRange(IEnumerable<string> collection)
        {
            string[] lines;
            if (collection == null || (lines = collection.Select(s => (String.IsNullOrEmpty(s)) ? "" : ReplaceLineSeparators(s))
                    .ToArray()).Length == 0)
                return;

            WithLines(() =>
            {
                _lines.AddRange(lines);
                _text = null;
                if (_lines.Count == 1)
                    return;
                _endings.AddRange(Enumerable.Repeat(DefaultNewLine, lines.Length - 1));
            });
        }

        public void Import(string text, bool leaveEndingsAsIs = false)
        {
            WithLines(() => _Import(text, leaveEndingsAsIs));
        }

        public void _Import(string text, bool leaveEndingsAsIs)
        {
            List<string> lines;

            if (leaveEndingsAsIs)
            {
                lines = Task.Factory.StartNew(() =>
                {
                    List<string> endings;
                    List<string> l= text.SplitLines(out endings);
                    if (l.Count > 0)
                        _endings.AddRange(endings);
                    return l;
                }).Result;
            }
            else
            {
                lines = Task.Factory.StartNew(() =>
                {
                    List<string> l = new List<string>(text.SplitLines());
                    if (l.Count > 0)
                        _endings.AddRange(Enumerable.Repeat(DefaultNewLine, (_lines.Count == 0) ? l.Count - 1 : l.Count));
                    return l;
                }).Result;
            }

            if (lines.Count == 0)
                return;

            _text = null;
            _lines.AddRange(lines);
        }

        public void Import(string text, int index, bool leaveEndingsAsIs = false)
        {
            WithLines(() =>
            {
                if (_lines.Count == 0)
                {
                    if (index != 0)
                        throw new ArgumentOutOfRangeException("index");
                    _Import(text, leaveEndingsAsIs);
                    return;
                }

                Task.Factory.StartNew(() =>
                {
                    List<string> lines;
                    List<string> endings;

                    if (leaveEndingsAsIs)
                    {
                        lines = text.SplitLines(out endings);
                        if (lines.Count == 0)
                            return;

                        endings.Insert(0, DefaultNewLine);
                    }
                    else
                    {
                        lines = new List<string>(text.SplitLines());
                        if (lines.Count == 0)
                            return;
                        endings = new List<string>(Enumerable.Repeat(DefaultNewLine, lines.Count));
                    }

                    _text = null;
                    if (index == _endings.Count)
                    {
                        for (int i = lines.Count - 1; i > -1; i++)
                            _lines.Insert(index, lines[i]);
                        for (int i = 0; i < endings.Count; i++)
                            _endings.Add(endings[i]);
                    }
                    else
                    {
                        for (int i = lines.Count - 1; i > -1; i++)
                        {
                            _lines.Insert(index, lines[i]);
                            _endings.Insert(index, endings[i]);
                        }
                    }
                }).Wait();
            });
        }
        
        public void Clear()
        {
            WithLines(() =>
            {
                _lines = new List<string>();
                _endings = new List<string>();
                _text = null;
            });
        }

        public bool Contains(string item)
        {
            return WithLines(() => _lines.Contains((String.IsNullOrEmpty(item)) ? "" : ReplaceLineSeparators(item)));
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            WithLines(() => _lines.CopyTo(array, arrayIndex));
        }

        public bool Remove(string item)
        {
            return WithLines(() =>
            {
                int index = _lines.IndexOf((String.IsNullOrEmpty(item)) ? "" : ReplaceLineSeparators(item));
                if (index < 0)
                    return false;

                _lines.RemoveAt(index);
                _text = null;
                if (_lines.Count == 0)
                    return true;

                _endings.RemoveAt((index > 0) ? index - 1 : 0);
                return true;
            });
        }

        public IEnumerator<string> GetEnumerator()
        {
            return WithLines(() => _lines.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return WithLines(() => (_lines as IEnumerable).GetEnumerator());
        }

        public bool Equals(StringLines other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;
            
            return WithLines(() => other.WithLines(() =>
            {
                if (other == null)
                    return _lines.Count == 0;
                if (other._lines.Count != _lines.Count)
                    return false;
                return other._lines.Zip(_lines, (a, b) => new { X = a, Y = b }).TakeWhile(a => a.X == a.Y).Count() == _lines.Count;
            }));
        }

        public bool Equals(string other)
        {
            return WithLines(() =>
            {
                if (other == null)
                    return _lines.Count == 0;
                string[] o = other.SplitLines().ToArray();
                if (o.Length != _lines.Count)
                    return false;
                return o.Zip(_lines, (a, b) => new { X = a, Y = b }).TakeWhile(a => a.X == a.Y).Count() == _lines.Count;
            });
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is string)
                return Equals(obj as string);

            return Equals(obj as StringLines);
        }

        public override int GetHashCode()
        {
            return WithLines(() => String.Join("", _lines).GetHashCode());
        }

        public override string ToString() { return Text ?? ""; }

        public int CompareTo(StringLines other)
        {
            if (other == null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return WithLines(() => other.WithLines(() =>
            {
                return _lines.Zip(other._lines, 
                    (a, b) => (a == null) ? ((b == null) ? 0 : 1) : ((b == null) ? -1 : a.CompareTo(b)))
                    .SkipWhile(i => i == 0).DefaultIfEmpty(_lines.Count.CompareTo(other._lines.Count)).First();
            }));
        }

        public int CompareTo(string other)
        {
            return WithLines(() =>
            {
                if (other == null)
                    return (_lines.Count == 0) ? 0 : 1;
                string[] o = other.SplitLines().ToArray();
                return _lines.Zip(o, (a, b) => (a == null) ? ((b == null) ? 0 : 1) : ((b == null) ? -1 : a.CompareTo(b)))
                    .SkipWhile(i => i == 0).DefaultIfEmpty(_lines.Count.CompareTo(o.Length)).First();
            });
        }
    }
}
