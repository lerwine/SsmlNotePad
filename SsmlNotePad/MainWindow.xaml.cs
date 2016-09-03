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
        public MainWindow() { InitializeComponent(); }

        private bool _canExecuteClose = false;

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

        private void contentsTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            (DataContext as ViewModel.MainWindowVM).LayoutUpdated(contentsTextBox, e);
        }

        private void InvokeCanExecute(CanExecuteRoutedEventArgs e, ICommand command)
        {
            if (e.Handled)
                return;
            e.Handled = true;
            e.CanExecute = command.CanExecute(new object[] { e.Parameter, this });
        }

        private void InvokeExecute(ExecutedRoutedEventArgs e, ICommand command)
        {
            if (e.Handled)
                return;
            e.Handled = true;
            command.CanExecute(new object[] { e.Parameter, this });
        }

        private void CommandBindingNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            InvokeCanExecute(e, (DataContext as ViewModel.MainWindowVM).NewDocumentCommand);
        }

        private void CommandBindingOpen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            InvokeCanExecute(e, (DataContext as ViewModel.MainWindowVM).OpenDocumentCommand);
        }

        private void CommandBindingSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            InvokeCanExecute(e, (DataContext as ViewModel.MainWindowVM).SaveDocumentCommand);
        }

        private void CommandBindingSaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            InvokeCanExecute(e, (DataContext as ViewModel.MainWindowVM).SaveAsCommand);
        }

        private void CommandBindingFind_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            InvokeCanExecute(e, (DataContext as ViewModel.MainWindowVM).FindTextCommand);
        }

        private void CommandBindingReplace_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            InvokeCanExecute(e, (DataContext as ViewModel.MainWindowVM).ReplaceTextCommand);
        }

        private void CommandBindingProperties_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            InvokeCanExecute(e, (DataContext as ViewModel.MainWindowVM).ContextPropertiesCommand);
        }

        private void CommandBindingHelp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            InvokeCanExecute(e, (DataContext as ViewModel.MainWindowVM).HelpCommand);
        }

        private void CommandBindingNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InvokeExecute(e, (DataContext as ViewModel.MainWindowVM).NewDocumentCommand);
        }

        private void CommandBindingOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InvokeExecute(e, (DataContext as ViewModel.MainWindowVM).OpenDocumentCommand);
        }

        private void CommandBindingSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InvokeExecute(e, (DataContext as ViewModel.MainWindowVM).SaveDocumentCommand);
        }

        private void CommandBindingSaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InvokeExecute(e, (DataContext as ViewModel.MainWindowVM).SaveAsCommand);
        }

        private void CommandBindingFind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InvokeExecute(e, (DataContext as ViewModel.MainWindowVM).FindTextCommand);
        }

        private void CommandBindingReplace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InvokeExecute(e, (DataContext as ViewModel.MainWindowVM).ReplaceTextCommand);
        }

        private void CommandBindingProperties_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InvokeExecute(e, (DataContext as ViewModel.MainWindowVM).ContextPropertiesCommand);
        }

        private void CommandBindingHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InvokeExecute(e, (DataContext as ViewModel.MainWindowVM).HelpCommand);
        }
    }
}
