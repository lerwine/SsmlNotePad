using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class BufferedTextReader : TextReader
    {
        private TextReader _textReader;
        private Stack<char> _buffer = new Stack<char>();

        public BufferedTextReader(TextReader textReader) : base() { _textReader = textReader; }

        public override void Close()
        {
            base.Close();
            _textReader = null;
            _buffer = null;
        }

        public override int Peek()
        {
            if (_buffer == null)
                throw new ObjectDisposedException(GetType().FullName);

            if (_buffer.Count > 0)
                return _buffer.Peek();

            if (_textReader == null)
                return -1;

            int i = _textReader.Peek();
            if (i == -1)
                _textReader = null;
            return i;
        }

        public bool TryPeek(out char c)
        {
            int i = Peek();
            if (i == -1)
            {
                c = default(char);
                return false;
            }

            c = (char)i;
            return true;
        }

        public bool TryRead(out char c)
        {
            int i = Read();
            if (i == -1)
            {
                c = default(char);
                return false;
            }

            c = (char)i;
            return true;
        }

        public void UnRead(string s)
        {
            if (_buffer == null)
                throw new ObjectDisposedException(GetType().FullName);

            if (s != null)
                UnRead(s.ToCharArray());
        }

        public void UnRead(params char[] list)
        {
            if (_buffer == null)
                throw new ObjectDisposedException(GetType().FullName);

            if (list == null)
                return;

            for (int i = list.Length - 1; i > -1; i--)
                _buffer.Push(list[i]);
        }

        public string ReadWhile(Func<char, bool> predicate)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            while (TryRead(out c))
            {
                if (!predicate(c))
                {
                    UnRead(c);
                    break;
                }
                sb.Append(c);
            }

            return sb.ToString();
        }

        public override int Read()
        {
            if (_buffer == null)
                throw new ObjectDisposedException(GetType().FullName);

            if (_buffer.Count > 0)
                return _buffer.Pop();

            if (_textReader == null)
                return -1;

            int i = _textReader.Read();
            if (i == -1)
                _textReader = null;
            return i;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (_buffer == null)
                throw new ObjectDisposedException(GetType().FullName);

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (buffer.Length - index < count)
                throw new ArgumentException("Not enough room in buffer.", "count");

            if (count == 0)
                return 0;

            int result = 0;
            while (count > 0 && _buffer.Count > 0)
            {
                buffer[index + result] = _buffer.Pop();
                result++;
                count--;
            }
            if (count == 0 || _textReader == null)
                return result;

            int c = _textReader.Read(buffer, index + result, count);
            if (c < count)
                _textReader = null;
            return result + c;
        }

        public override int ReadBlock(char[] buffer, int index, int count) { return Read(buffer, index, count); }

        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            return Task<int>.Factory.StartNew(() => Read(buffer, index, count));
        }

        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        {
            return Task<int>.Factory.StartNew(() => Read(buffer, index, count));
        }

        public override string ReadLine()
        {
            if (_buffer == null)
                throw new ObjectDisposedException(GetType().FullName);

            if (_buffer.Count == 0)
            {
                if (_textReader == null)
                    return null;
                string s = _textReader.ReadLine();
                if (s == null)
                    _textReader = null;
                return s;
            }

            StringBuilder sb = new StringBuilder();
            do
            {
                char c = _buffer.Pop();
                if (c == '\r')
                {
                    if (_buffer.Peek() == '\n')
                        _buffer.Pop();
                    return sb.ToString();
                }
                if (c == '\n')
                    return sb.ToString();
                sb.Append(c);
            } while (_buffer.Count > 0);

            if (_textReader != null)
            {
                string s = _textReader.ReadLine();
                if (s == null)
                    _textReader = null;
                else
                    sb.Append(s);
            }
            return sb.ToString();
        }

        public override Task<string> ReadLineAsync()
        {
            return Task<string>.Factory.StartNew(() => ReadLine());
        }

        public override string ReadToEnd()
        {
            if (_buffer == null)
                throw new ObjectDisposedException(GetType().FullName);

            if (_buffer.Count == 0)
            {
                if (_textReader == null)
                    return null;
                string s = _textReader.ReadToEnd();
                _textReader = null;
                return s;
            }

            StringBuilder sb = new StringBuilder();
            do
            {
                char c = _buffer.Pop();
                if (c == '\r')
                {
                    if (_buffer.Peek() == '\n')
                        _buffer.Pop();
                    return sb.ToString();
                }
                if (c == '\n')
                    return sb.ToString();
                sb.Append(c);
            } while (_buffer.Count > 0);

            if (_textReader != null)
            {
                string s = _textReader.ReadToEnd();
                if (s != null)
                    sb.Append(s);
                _textReader = null;
            }
            return sb.ToString();
        }

        public override Task<string> ReadToEndAsync()
        {
            return Task<string>.Factory.StartNew(() => ReadToEnd());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _buffer = null;
                _textReader = null;
            }

            base.Dispose(disposing);
        }
    }
}
