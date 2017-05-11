using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Erwine.Leonard.T.SsmlNotePad.Model.Workers
{
    public class SsmlChangedJob : Common.RestartableJob<TextChangeArgs>
    {
        ViewModel.MainWindowVM _viewModel;

        public SsmlChangedJob(ViewModel.MainWindowVM viewModel)
        {
            _viewModel = viewModel;
            viewModel.SsmlTextBox.TextChanged += SsmlTextBox_TextChanged;
            viewModel.SsmlTextBox.LayoutUpdated += SsmlTextBox_LayoutUpdated;
            if (App.Current != null)
                App.Current.Exit += App_Exit;
        }

        private void App_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            _viewModel.SsmlTextBox.TextChanged -= SsmlTextBox_TextChanged;
            _viewModel.SsmlTextBox.LayoutUpdated -= SsmlTextBox_LayoutUpdated;
            if (_currentArgs != null)
                _currentArgs.CancelTasks();
        }

        TextChangeArgs _currentArgs = null;
        private object _syncRoot = new object();

        private void SsmlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.DisableSelectionBasedCommands();
            CheckStartNew(_viewModel.SsmlTextBox, false);
        }

        private void SsmlTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            CheckStartNew(_viewModel.SsmlTextBox, true);
        }

        private void CheckStartNew(TextBox textBox, bool isLayoutUpdated)
        {
            lock (_syncRoot)
            {
                TextChangeArgs args;
                if (_currentArgs == null)
                    args = new TextChangeArgs(textBox, isLayoutUpdated);
                else if (!_currentArgs.TryCreateNewTaskArgs(textBox, isLayoutUpdated, out args))
                    return;

                _currentArgs = args;
                Start(DoWork, args);
            }
        }

        private void DoWork(TextChangeArgs args, CancellationToken token)
        {
            TextLine[] lines = args.ParseLinesTask.Result;
            if (token.IsCancellationRequested)
                return;

            if (!args.IsLayoutUpdated)
            {
                OnValidationComplete(args.ValidateXmlTask.Result, token);
                return;
            }

            args.ValidateXmlTask.ContinueWith(OnValidationComplete, token);

            TextLine currentLine = lines.TakeWhile(l => l.Index <= args.SelectionStart).LastOrDefault();
            int currentLineNumber, currentColNumber;
            if (currentLine == null)
            {
                currentLineNumber = 1;
                currentColNumber = args.SelectionStart + 1;
            }
            else
            {
                currentLineNumber = currentLine.LineNumber;
                currentColNumber = (args.SelectionStart - currentLine.Index) + 1;
            }
            _viewModel.Dispatcher.Invoke(() =>
            {
                if (token.IsCancellationRequested)
                    return;
                _viewModel.UpdateSelection(args.SelectionStart, args.SelectionLength, currentLineNumber, currentColNumber);
            });

            int highestIndex = 0;
            foreach (TextLine line in lines)
            {
                if (token.IsCancellationRequested)
                    break;
                if (line == null)
                    continue;
                int index = args.VisibleLineStartIndexes.IndexOf(line.Index);
                if (index > -1 || (index = args.VisibleLineStartIndexes.TakeWhile(i => i < line.Index).Count()) < args.VisibleLineStartIndexes.Count)
                {
                    if (index > highestIndex)
                        highestIndex = index;
                    _viewModel.Dispatcher.Invoke(() =>
                    {
                        if (token.IsCancellationRequested)
                            return;
                        if (index < _viewModel.LineNumbers.Count)
                        {
                            ViewModel.LineNumberVM vm = _viewModel.LineNumbers[index];
                            vm.Margin = new System.Windows.Thickness(0.0, args.VisibleLineStartRects[index].Top, 0.0, 0.0);
                            vm.Number = line.LineNumber;
                        }
                        else
                            _viewModel.LineNumbers.Add(new ViewModel.LineNumberVM(line.LineNumber, args.VisibleLineStartRects[index].Top));
                    });
                }
            }

            while (_viewModel.Dispatcher.Invoke(() =>
            {
                if (token.IsCancellationRequested || _viewModel.LineNumbers.Count <= highestIndex)
                    return false;
                _viewModel.LineNumbers.RemoveAt(highestIndex);
                return true;
            }))
            {
                if (token.IsCancellationRequested)
                    break;
            }
            if (!args.ValidateXmlTask.IsCompleted)
                args.ValidateXmlTask.Wait();
        }

        private void OnValidationComplete(Task<XmlValidationResult[]> validateXmlTask, object obj)
        {
            CancellationToken token = (CancellationToken)obj;
            if (!token.IsCancellationRequested)
                OnValidationComplete(validateXmlTask.Result, token);
        }

        private void OnValidationComplete(XmlValidationResult[] validationResult, CancellationToken token)
        {
            if (token.IsCancellationRequested || !ReferenceEquals(this, _currentArgs))
                return;
            
            for (int index = 0; index < validationResult.Length; index++)
            {
                _viewModel.Dispatcher.Invoke(() =>
                {
                    if (token.IsCancellationRequested)
                        return;
                    if (index < _viewModel.ValidationMessages.Count)
                        _viewModel.ValidationMessages[index].UpdateFrom(validationResult[index]);
                    else
                        _viewModel.ValidationMessages.Add(new ViewModel.XmlValidationMessageVM(validationResult[index]));
                });
            }

            while (_viewModel.Dispatcher.Invoke(() =>
            {
                if (token.IsCancellationRequested || _viewModel.ValidationMessages.Count <= validationResult.Length)
                    return false;
                _viewModel.ValidationMessages.RemoveAt(validationResult.Length);
                return true;
            }))
            {
                if (token.IsCancellationRequested)
                    break;
            }
        }

        protected override void OnWorkerCanceled(Common.WorkerEventArgs<TextChangeArgs> args)
        {
            _currentArgs.CancelTasks();
            base.OnWorkerCanceled(args);
        }
    }
}
