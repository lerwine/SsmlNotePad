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

        public MainWindow() { InitializeComponent(); }

        private bool _canExecuteClose = false;

        private void contentsTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            ViewModel.MainWindowVM vm = DataContext as ViewModel.MainWindowVM;
            int firstVisibleLineIndex = contentsTextBox.GetFirstVisibleLineIndex();
            if (_previousFirstVisibleLineIndex != firstVisibleLineIndex)
            {
                _previousFirstVisibleLineIndex = firstVisibleLineIndex;
                _previousLastVisibleLineIndex = contentsTextBox.GetLastVisibleLineIndex();
                vm.InvalidateLineNumbers();
            }
            else
            {
                int lastVisibleLineIndex = contentsTextBox.GetLastVisibleLineIndex();
                if (_previousLastVisibleLineIndex != lastVisibleLineIndex)
                {
                    _previousFirstVisibleLineIndex = firstVisibleLineIndex;
                    _previousLastVisibleLineIndex = lastVisibleLineIndex;
                    vm.InvalidateLineNumbers();
                }
            }

            vm.LayoutUpdated(contentsTextBox, e);
        }

        private void ContentsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.MainWindowVM vm = DataContext as ViewModel.MainWindowVM;
            vm.ValidateDocument(contentsTextBox.Text);
            vm.InvalidateLineNumbers();
        }

        private void contentsTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.MainWindowVM vm = DataContext as ViewModel.MainWindowVM;
            vm.SelectedText = contentsTextBox.SelectedText;
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
