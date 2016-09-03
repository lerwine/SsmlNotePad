using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class LineNumberGenerator : BackgroundJobWorker<LineNumberGenerator, int>
    {
        private TextBox _contentsTextBox;
        private ObservableCollection<ViewModel.LineNumberVM> _lineNumbersCollection;
        private int _result;

        public LineNumberGenerator(TextBox contentsTextBox, ObservableCollection<ViewModel.LineNumberVM> lineNumbersCollection)
        {
            _contentsTextBox = contentsTextBox;
            _lineNumbersCollection = lineNumbersCollection;
        }

        protected override int Run(LineNumberGenerator previousWorker)
        {
#if DEBUG
            int? currentTaskId = Task.CurrentId;
#endif
            if (Token.IsCancellationRequested)
            {
                Debug.WriteLine("Task {0} Exiting, IsCancellationRequested = true", currentTaskId);
                return 0;
            }

            _result = CheckGet(_contentsTextBox, () => _lineNumbersCollection.Count);
            Debug.WriteLine("Task {0}: _lineNumbersCollection.Count = {1}", currentTaskId, _result);
            Tuple<int, double?> lineAndMargin = _contentsTextBox.Dispatcher.Invoke(() =>
            {
                if (Token.IsCancellationRequested)
                {
                    Debug.WriteLine("Task {0}: lineAndMargin Exiting, IsCancellationRequested = true", currentTaskId);
                    return new Tuple<int, double?>(0, null);
                }
                int l = _contentsTextBox.GetFirstVisibleLineIndex();
                int c = _contentsTextBox.GetCharacterIndexFromLineIndex(l);
                if (l < 0 || c < 0)
                {
                    Debug.WriteLine("Task {0}: FirstVisibleLineIndex = {1}, CharacterIndexFromLineIndex({1}) = {2}", currentTaskId, l, c);
                    return new Tuple<int, double?>(0, null);
                }
                double top = _contentsTextBox.GetRectFromCharacterIndex(c).Top;
                Debug.WriteLine("Task {0}: FirstVisibleLineIndex = {1}, CharacterIndexFromLineIndex({1}) = {2}, RectFromCharacterIndex({2}).Top = {3}", currentTaskId, l, c, top);
                return new Tuple<int, double?>(l, top);
            });
            int collectionIndex = 0;
            for (int l = lineAndMargin.Item1 + 1; lineAndMargin.Item2.HasValue; l++)
            {
                Debug.WriteLine("Task {0}: LineIndex = {1}, CollectionIndex = {2}", currentTaskId, l, collectionIndex);
                Debug.Indent();
                try
                {
                    if (Token.IsCancellationRequested)
                    {
                        Debug.WriteLine("Task {0}: Exiting, IsCancellationRequested = true", currentTaskId);
                        return collectionIndex;
                    }
                    int lineNumber = lineAndMargin.Item1 + 1;
                    lineAndMargin = CheckGet(_contentsTextBox, () =>
                    {
                        if (Token.IsCancellationRequested)
                        {
                            Debug.WriteLine("Task {0}: CheckGet Exiting, IsCancellationRequested = true", currentTaskId);
                            return new Tuple<int, double?>(0, null);
                        }
                        Debug.WriteLine("Task {0}: CollectionIndex = {1}, _lineNumbersCollection.Count = {2}", currentTaskId, collectionIndex, _lineNumbersCollection.Count);
                        if (collectionIndex < _lineNumbersCollection.Count)
                        {
                            if (_lineNumbersCollection[collectionIndex].Number != lineNumber)
                                _lineNumbersCollection[collectionIndex].Number = lineNumber;
                            if (_lineNumbersCollection[collectionIndex].Margin.Top != lineAndMargin.Item2.Value)
                                _lineNumbersCollection[collectionIndex].Margin = new System.Windows.Thickness(0.0, lineAndMargin.Item2.Value, 0.0, 0.0);
                        }
                        else
                            _lineNumbersCollection.Add(new ViewModel.LineNumberVM(lineNumber, lineAndMargin.Item2.Value));
                        int c = _contentsTextBox.GetCharacterIndexFromLineIndex(l);
                        Debug.WriteLine("Task {0}: _contentsTextBox.GetCharacterIndexFromLineIndex({1}) = {2}", currentTaskId, l, c);
                        if (l >= _contentsTextBox.LineCount || c < 0)
                            return new Tuple<int, double?>(0, null);
                        return new Tuple<int, double?>(l, _contentsTextBox.GetRectFromCharacterIndex(c).Top);
                    });
                    collectionIndex++;
                }
                catch { throw; }
                finally { Debug.Unindent(); }
            }

            if (Token.IsCancellationRequested)
            {
                Debug.WriteLine("Task {0}: Exiting, IsCancellationRequested = true", currentTaskId);
                return collectionIndex;
            }

            CheckInvoke(_contentsTextBox, () =>
            {
                while (_lineNumbersCollection.Count > collectionIndex)
                {
                    if (Token.IsCancellationRequested)
                    {
                        Debug.WriteLine("Task {0}: CheckInvoke Exiting, IsCancellationRequested = true", currentTaskId);
                        return;
                    }

                    Debug.WriteLine("Task {0}: _lineNumbersCollection.RemoveAt({1})", currentTaskId, collectionIndex);
                    _lineNumbersCollection.RemoveAt(collectionIndex);
                }
            });

            return collectionIndex;
        }
    }
}
