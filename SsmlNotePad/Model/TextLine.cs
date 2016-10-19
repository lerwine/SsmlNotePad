using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class TextLine : IEquatable<TextLine>
    {
        public static readonly Regex LineRegex = new Regex(@"(?<l>[^\r\n]*)(?<e>\r\n?|\n)", RegexOptions.Compiled);

        public TextLine(int lineNumber, int index, string lineContent, string lineEnding)
        {
            LineNumber = lineNumber;
            Index = index;
            LineContent = lineContent;
            LineEnding = lineEnding;
        }

        public int Index { get; private set; }

        public string LineContent { get; private set; }

        public string LineEnding { get; private set; }

        public int LineNumber { get; private set; }

        public static Task<TextLine[]> SplitAsync(string text, CancellationToken token)
        {
            return Task<TextLine[]>.Factory.StartNew(o => Split(o as string).ToArray(), text, token);
        }

        internal static IEnumerable<TextLine> Split(string text, CancellationToken cancellationToken)
        {
            if (text == null)
                yield break;

            if (text.Length == 0)
            {
                yield return new TextLine(1, 0, "", "");
                yield break;
            }

            int lineNumber = 1;
            foreach (Match m in LineRegex.Matches(text))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                yield return new TextLine(lineNumber, m.Index, m.Groups["l"].Value, m.Groups["e"].Value);
                lineNumber++;
            }
            if (!cancellationToken.IsCancellationRequested)
            {
                int charIndex = text.Length - 1;
                while (charIndex > -1 && text[charIndex] != '\r' && text[charIndex] != '\n')
                    charIndex--;
                yield return new TextLine(lineNumber, charIndex + 1, (charIndex == text.Length) ? "" : text.Substring(charIndex), "");
            }
        }

        public static IEnumerable<TextLine> Split(string text)
        {
            if (text == null)
                yield break;

            if (text.Length == 0)
            {
                yield return new TextLine(1, 0, "", "");
                yield break;
            }

            int lineNumber = 1;
            foreach (Match m in LineRegex.Matches(text))
            {
                yield return new TextLine(lineNumber, m.Index, m.Groups["l"].Value, m.Groups["e"].Value);
                lineNumber++;
            }
            int charIndex = text.Length - 1;
            while (charIndex > -1 && text[charIndex] != '\r' && text[charIndex] != '\n')
                charIndex--;
            yield return new TextLine(lineNumber, charIndex + 1, (charIndex == text.Length) ? "" : text.Substring(charIndex), "");
        }

        public bool Equals(TextLine other)
        {
            return other != null && (ReferenceEquals(this, other) && LineNumber == other.LineNumber && Index == other.Index && 
                LineContent == other.LineContent && LineEnding == other.LineEnding);
        }

        public override bool Equals(object obj) { return Equals(obj as TextLine); }

        public override int GetHashCode() { return Index; }

        public override string ToString() { return LineContent + LineEnding; }
    }
}
