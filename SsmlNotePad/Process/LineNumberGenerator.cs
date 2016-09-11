using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            _result = 0;

            if (Token.IsCancellationRequested)
            {
                Logger.WriteLine("Exiting, IsCancellationRequested = true");
                return _result;
            }

            Tuple<int, int> firstAndLastLineIndexes = CheckGet(_contentsTextBox, () => new Tuple<int, int>(_contentsTextBox.GetFirstVisibleLineIndex(), _contentsTextBox.GetLastVisibleLineIndex()));
            if (firstAndLastLineIndexes.Item1 < 0 || Token.IsCancellationRequested)
                return _result;

            for (int lineIndex=firstAndLastLineIndexes.Item1; lineIndex <= firstAndLastLineIndexes.Item2; lineIndex++)
            {
                double? marginTop = CheckGet<double?>(_contentsTextBox, () =>
                {
                    if (Token.IsCancellationRequested)
                        return null;

                    int characterIndex = _contentsTextBox.GetCharacterIndexFromLineIndex(lineIndex);

                    if (characterIndex < 0)
                        return null;

                    return _contentsTextBox.GetRectFromCharacterIndex(characterIndex).Top;
                });
                if (!marginTop.HasValue)
                    return _result;
                
                CheckInvoke(_contentsTextBox, () =>
                {
                    if (Token.IsCancellationRequested)
                        return;
                    
                    if (_result < _lineNumbersCollection.Count)
                    {
                        _lineNumbersCollection[_result].Number = lineIndex + 1;
                        if (_lineNumbersCollection[_result].Margin.Top != marginTop.Value)
                            _lineNumbersCollection[_result].Margin = new System.Windows.Thickness(0.0, marginTop.Value, 0.0, 0.0);
                    }
                    else
                        _lineNumbersCollection.Add(new ViewModel.LineNumberVM(lineIndex + 1, marginTop.Value));
                });

                if (Token.IsCancellationRequested)
                {
                    Logger.WriteLine("lineAndMargin Exiting, IsCancellationRequested = true");
                    return _result;
                }

                _result++;
            }

            if (Token.IsCancellationRequested)
            {
                Logger.WriteLine("Exiting, IsCancellationRequested = true");
                return _result;
            }

            CheckInvoke(_contentsTextBox, () =>
            {
                while (_lineNumbersCollection.Count > _result)
                {
                    if (Token.IsCancellationRequested)
                    {
                        Logger.WriteLine("CheckInvoke Exiting, IsCancellationRequested = true");
                        return;
                    }

                    Logger.WriteLine("_lineNumbersCollection.RemoveAt({0})", _result);
                    _lineNumbersCollection.RemoveAt(_result);
                }
            });

            return _result;
        }
    }
}
