using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Erwine.Leonard.T.SsmlNotePad.Model.Workers;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class LineNumberInfo
    {
        private int _lineNumber;
        private int _characterIndex;
        private double _marginTop;

        public int LineNumber { get { return _lineNumber; } }

        public int CharacterIndex { get { return _characterIndex; } }

        public double MarginTop { get { return _marginTop; } }

        public LineNumberInfo(int lineNumber, int characterIndex, double marginTop)
        {
            _lineNumber = lineNumber;
            _characterIndex = characterIndex;
            _marginTop = marginTop;
        }

//        internal static Task<LineNumberInfo[]> GetLineNumbersAsync(Task<TextLine[]> parseLinesTask, LayoutUpdateArgs arguments)
//        {
//#if DEBUG
//            try
//            {
//#endif
//                return parseLinesTask.ContinueWith((t, o) => GetLines(t, o as LayoutUpdateArgs).ToArray(), arguments, arguments.Token);
//#if DEBUG
//            }
//            catch
//            {
//                if (System.Diagnostics.Debugger.IsAttached)
//                    System.Diagnostics.Debugger.Break();
//                throw;
//            }
//#endif
//        }

//        internal static IEnumerable<LineNumberInfo> GetLines(Task<TextLine[]> parseLinesTask, LayoutUpdateArgs arguments)
//        {
//#if DEBUG
//            System.Diagnostics.Debug.WriteLine("{0} GetLines: arguments.Token.IsCancellationRequested = {1}; parseLinesTask.IsCanceled = {2}; parseLinesTask.IsFaulted = {3}", Task.CurrentId, arguments.Token.IsCancellationRequested, parseLinesTask.IsCanceled, parseLinesTask.IsFaulted);
//#endif
//            if (arguments.Token.IsCancellationRequested || parseLinesTask.IsCanceled || parseLinesTask.IsFaulted)
//                yield break;

//            TextLine[] lines = parseLinesTask.Result;
//            if (lines != null)
//            {
//#if DEBUG
//                System.Diagnostics.Debug.WriteLine("{0}: parseLinesTask returned {1} items", Task.CurrentId, lines.Length);
//#endif
//                foreach (TextLine line in lines)
//                {
//                    if (arguments.Token.IsCancellationRequested)
//                        break;
//                    if (line == null)
//                        continue;
//                    int index = arguments.VisibleLineStartIndexes.IndexOf(line.Index);
//                    if (index > -1 || (index = arguments.VisibleLineStartIndexes.TakeWhile(i => i < line.Index).Count()) < arguments.VisibleLineStartIndexes.Count)
//                        yield return new LineNumberInfo(line.LineNumber, line.Index, arguments.VisibleLineStartRects[index].Top);
//                }
//            }
//#if DEBUG
//            else
//                System.Diagnostics.Debug.WriteLine("{0}: parseLinesTask returned null", Task.CurrentId);
//#endif
//        }
    }
}