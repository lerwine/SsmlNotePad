using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Text
{
    public class TextLineInfo : IEquatable<TextLineInfo>, IEquatable<string>
    {
        internal const string ParameterName_startIndex = "startIndex";
        internal const string ParameterName_length = "length";

        public int Number { get; private set; }

        public int CharIndex { get; private set; }

        public int Length { get; private set; }

        public string AllText { get; private set; }

        public TextLineInfo Previous { get; private set; }

        public TextLineInfo Next { get; private set; }

        public string Text { get { return (Length == 0) ? "" : ((Length == AllText.Length) ? AllText : AllText.Substring(0, Length)); } }

        public string LineEnding { get { return (Length == AllText.Length) ? "" : AllText.Substring(Length); } }

        private TextLineInfo(int index, TextLineInfo previous) { CharIndex = index; Previous = previous; Number = previous.Number + 1; }

        public TextLineInfo(string text)
        {
            Number = 1;

            if (String.IsNullOrEmpty(text))
            {
                AllText = "";
                return;
            }

            TextLineInfo current = this;
            int charIndex = 0;
            for (int index = 0; index< text.Length; index++)
            {
                if (text[index] == '\r')
                {
                    current.Length = index - charIndex;
                    if (index < text.Length - 1 && text[index + 1] == '\n')
                        index++;
                }
                else if (text[index] == '\n')
                    current.Length = index - charIndex;
                else
                    continue;

                current.AllText = text.Substring(charIndex, (index - charIndex) + 1);
                charIndex = index + 1;
                current.Next = new TextLineInfo(charIndex, current);
                current = current.Next;
            }
            current.AllText = (charIndex == text.Length) ? "" : text.Substring(charIndex, text.Length - charIndex);
            current.Length = current.AllText.Length;
        }

        public bool IsOfSameSet(TextLineInfo item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (ReferenceEquals(item, this))
                return true;

            if (item.Number < Number)
                return item.IsOfSameSet(this);

            while (item.Number > Number)
                item = item.Previous;

            return ReferenceEquals(item, this);
        }

        public TextPointer CreateTextPointer(int characterOffset) { return new TextPointer(this, characterOffset); }
        
        public TextPointer CreateTextPointer(int lineOffset, int characterOffset) { return new TextPointer(this, lineOffset, characterOffset); }
        
        public static IEnumerable<TextLineInfo> Load(string text)
        {
            if (text == null)
                return new TextLineInfo[0];

            return (new TextLineInfo(text)).AsEnumerable();
        }

        private IEnumerable<TextLineInfo> AsEnumerable()
        {
            TextLineInfo textLineInfo = this;
            do
            {
                yield return textLineInfo;
                textLineInfo = textLineInfo.Next;
            } while (textLineInfo != null);
        }

        public bool Equals(TextLineInfo other) { return other != null && (ReferenceEquals(other, this) || (CharIndex == other.CharIndex && Text == other.Text)); }

        public bool Equals(string other) { return other != null && (other == Text || other == AllText); }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is string)
                return Equals(obj as string);

            return Equals(obj as TextLineInfo);
        }

        public override int GetHashCode() { return CharIndex; }

        public override string ToString() { return AllText; }

        public string SubString(int startIndex, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            return SubString(this, startIndex, length);
        }

        private static string SubString(TextLineInfo line, int startIndex, int length)
        {
            while (startIndex < 0 && line.Previous != null)
            {
                startIndex += line.Previous.AllText.Length;
                line = line.Previous;
            }

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");

            if (length == 0)
                return "";

            int relativeStartIndex = startIndex - line.CharIndex;

            if (length <= relativeStartIndex + line.AllText.Length)
                return line.AllText.Substring(relativeStartIndex, line.AllText.Length - relativeStartIndex);

            if (line.Next == null)
                return line.AllText.Substring(relativeStartIndex);

            return line.AllText.Substring(relativeStartIndex) + SubString(line.Next, 0, length - (line.AllText.Length - relativeStartIndex));
        }
    }
}
