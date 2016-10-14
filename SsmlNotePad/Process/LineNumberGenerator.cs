using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Erwine.Leonard.T.SsmlNotePad.ViewModel;
using System.Windows;

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

        public class LineNumberProcessState
        {
            public string Text { get; private set; }
            public int CurrentCharIndex { get; private set; }
            public int CurrentLineIndex { get; private set; }
            public int FirstVisibleLineIndex { get; private set; }
            public int CharIndexOnLastVisibleLine { get; private set; }
            public LineNumberProcessState(TextBox contentsTextBox, ObservableCollection<ViewModel.LineNumberVM> lineNumbersCollection, CancellationToken token)
            {
                Action action = () =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    Text = contentsTextBox.Text;
                    FirstVisibleLineIndex = contentsTextBox.GetFirstVisibleLineIndex();
                    if (FirstVisibleLineIndex < 0)
                    {
                        CharIndexOnLastVisibleLine = -1;
                        CurrentLineIndex = -1;
                        CurrentCharIndex = -1;
                        return;
                    }
                    CurrentCharIndex = contentsTextBox.GetCharacterIndexFromLineIndex(FirstVisibleLineIndex);
                    _SetLineNumber(contentsTextBox, lineNumbersCollection);
                    CharIndexOnLastVisibleLine = contentsTextBox.GetCharacterIndexFromLineIndex(contentsTextBox.GetLastVisibleLineIndex());
                };

                if (contentsTextBox.CheckAccess())
                    action();
                else
                    contentsTextBox.Dispatcher.Invoke(action);

                if (token.IsCancellationRequested)
                {
                    Text = "";
                    FirstVisibleLineIndex = -1;
                    CharIndexOnLastVisibleLine = -1;
                    CurrentLineIndex = -1;
                    CurrentCharIndex = -1;
                    return;
                }

                CurrentLineIndex = 0;
                int endIndex = (CurrentCharIndex < Text.Length) ? CurrentCharIndex : Text.Length;
                for (int charIndex = 0; charIndex < endIndex; charIndex++)
                {
                    char c = Text[charIndex];
                    if (c == '\r')
                    {
                        if (charIndex < Text.Length - 1 && Text[charIndex + 1] == '\n')
                            charIndex++;
                        CurrentLineIndex++;
                    }
                    else if (c == '\n')
                        CurrentLineIndex++;
                }
            }

            public bool MoveToNextLine()
            {
                while (CurrentCharIndex < Text.Length && CurrentCharIndex < CharIndexOnLastVisibleLine)
                {
                    char c = Text[CurrentCharIndex];
                    CurrentCharIndex++;
                    if (c == '\r')
                    {
                        CurrentLineIndex++;
                        if (CurrentCharIndex < Text.Length && CurrentCharIndex < CharIndexOnLastVisibleLine && Text[CurrentCharIndex] == '\n')
                            CurrentCharIndex++;
                        return true;
                    }
                    if (c == '\n')
                    {
                        CurrentLineIndex++;
                        return true;
                    }
                }

                return false;
            }

            private void _SetLineNumber(TextBox contentsTextBox, ObservableCollection<LineNumberVM> lineNumbersCollection)
            {
                double marginTop = contentsTextBox.GetRectFromCharacterIndex(CurrentCharIndex).Top;
                int index = CurrentLineIndex - FirstVisibleLineIndex;
                if (index < 0)
                    return;
                int lineNumber = CurrentLineIndex + 1;
                if (lineNumbersCollection.Count > index)
                {
                    if (lineNumbersCollection[index].Margin.Top != marginTop)
                        lineNumbersCollection[index].Margin = new Thickness(0.0, marginTop, 0.0, 0.0);
                    if (lineNumbersCollection[index].Number != lineNumber)
                        lineNumbersCollection[index].Number = lineNumber;
                }
                else
                    lineNumbersCollection.Add(new LineNumberVM(lineNumber, marginTop));
            }

            public bool TrySetLineNumber(TextBox contentsTextBox, ObservableCollection<LineNumberVM> lineNumbersCollection, CancellationToken token)
            {
                Func<bool> func = () =>
                {
                    try
                    {
                        if (token.IsCancellationRequested || contentsTextBox.Text != Text)
                            return false;
                        double marginTop = contentsTextBox.GetRectFromCharacterIndex(CurrentCharIndex).Top;
                        int index = CurrentLineIndex - FirstVisibleLineIndex;
                        int lineNumber = CurrentLineIndex + 1;
                        if (lineNumbersCollection.Count > index)
                        {
                            if (lineNumbersCollection[index].Margin.Top != marginTop)
                                lineNumbersCollection[index].Margin = new Thickness(0.0, marginTop, 0.0, 0.0);
                            if (lineNumbersCollection[index].Number != lineNumber)
                                lineNumbersCollection[index].Number = lineNumber;
                        }
                        else
                            lineNumbersCollection.Add(new LineNumberVM(lineNumber, marginTop));
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                };

                if (contentsTextBox.CheckAccess())
                    return func();

                return contentsTextBox.Dispatcher.Invoke(func);
            }
        }

        protected override int Run(LineNumberGenerator previousWorker)
        {
            _result = 0;
            
            if (Token.IsCancellationRequested)
                return _result;

            LineNumberProcessState state = new LineNumberProcessState(_contentsTextBox, _lineNumbersCollection, Token);
            if (state.FirstVisibleLineIndex < 0 || Token.IsCancellationRequested)
                return _result;

            while (state.MoveToNextLine())
            {
                if (!state.TrySetLineNumber(_contentsTextBox, _lineNumbersCollection, Token) || Token.IsCancellationRequested)
                    break;
                _result = state.CurrentLineIndex;
            }

            if (Token.IsCancellationRequested)
                return _result;

            Action action = () =>
            {
                while (_lineNumbersCollection.Count > _result && !Token.IsCancellationRequested)
                    _lineNumbersCollection.RemoveAt(_result);
            };

            if (_contentsTextBox.CheckAccess())
                action();
            else
                _contentsTextBox.Dispatcher.Invoke(action);

            return _result;
        }
    }
}
