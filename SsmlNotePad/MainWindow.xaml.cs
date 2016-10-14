using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Erwine.Leonard.T.SsmlNotePad.ViewModel.Command;

namespace Erwine.Leonard.T.SsmlNotePad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _previousFirstVisibleLineIndex = -1,
            _previousLastVisibleLineIndex = -1;
        
        public MainWindow()
        {
            InitializeComponent();
            //MainViewModel.TextChanged += MainViewModel_TextChanged;
        }

        //~MainWindow() { MainViewModel.TextChanged -= MainViewModel_TextChanged; }

         private bool _canExecuteClose = false;

         private void contentsTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            ViewModel.MainWindowVM viewModel = DataContext as ViewModel.MainWindowVM;
            int firstVisibleLineIndex = contentsTextBox.GetFirstVisibleLineIndex();
             if (_previousFirstVisibleLineIndex != firstVisibleLineIndex)
             {
                 _previousFirstVisibleLineIndex = firstVisibleLineIndex;
                 _previousLastVisibleLineIndex = contentsTextBox.GetLastVisibleLineIndex();
                viewModel.InvalidateLineNumbers();
             }
             else
             {
                 int lastVisibleLineIndex = contentsTextBox.GetLastVisibleLineIndex();
                 if (_previousLastVisibleLineIndex != lastVisibleLineIndex)
                 {
                     _previousFirstVisibleLineIndex = firstVisibleLineIndex;
                     _previousLastVisibleLineIndex = lastVisibleLineIndex;
                    viewModel.InvalidateLineNumbers();
                 }
             }

            viewModel.LayoutUpdated(contentsTextBox, e);
         }

         //private void MainViewModel_TextChanged(object sender, EventArgs e)
         //{
         //    if (contentsTextBox.Text != MainViewModel.Text)
         //        contentsTextBox.Text = MainViewModel.Text;
         //}

         private void ContentsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.MainWindowVM viewModel = DataContext as ViewModel.MainWindowVM;
            viewModel.ValidateDocument(contentsTextBox.Text);
            viewModel.InvalidateLineNumbers();
        }

         private void contentsTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.MainWindowVM viewModel = DataContext as ViewModel.MainWindowVM;
            viewModel.SelectionStart = contentsTextBox.SelectionStart;
            viewModel.SelectionLength = contentsTextBox.SelectionLength;
             string text = contentsTextBox.Text;
             int start = contentsTextBox.SelectionStart;
             if (contentsTextBox.SelectionStart == 0)
             {
                viewModel.CurrentColNumber = 1;
                viewModel.CurrentLineNumber = 1;
                 return;
             }
             Task.Factory.StartNew(() =>
             {
                 string[] lines = text.Substring(0, start).SplitLines().ToArray();
                 viewModel.Dispatcher.Invoke(() =>
                 {
                     viewModel.CurrentColNumber = lines[lines.Length - 1].Length + 1;
                     viewModel.CurrentLineNumber = lines.Length;
                 });
             });
         }

         protected override void OnActivated(EventArgs e)
         {
             _canExecuteClose = true;
             base.OnActivated(e);
         }

         private void CommandBinding_CloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
         {
             if (e.Handled)
                 return;
             e.Handled = true;
             e.CanExecute = _canExecuteClose;
         }

         private void CommandBindingClose_Executed(object sender, ExecutedRoutedEventArgs e)
         {
             if (e.Handled)
                 return;
             e.Handled = true;
             if (!_canExecuteClose)
                 return;

             _canExecuteClose = false;
             Close();
         }

         protected override void OnClosing(CancelEventArgs e)
         {
             (DataContext as ViewModel.MainWindowVM).OnClosing(this, e);
             base.OnClosing(e);
             if (e.Cancel)
                 _canExecuteClose = true;
         }

         protected override void OnClosed(EventArgs e)
         {
             (DataContext as ViewModel.MainWindowVM).OnClosed(e);
             base.OnClosed(e);
         }
    }
}
