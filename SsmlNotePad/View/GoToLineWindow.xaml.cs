using System;
using System.Windows;
using System.Windows.Input;

namespace Erwine.Leonard.T.SsmlNotePad.View
{
    /// <summary>
    /// Interaction logic for GoToLineWindow.xaml
    /// </summary>
    public partial class GoToLineWindow : Window
    {
        public static int? GetGotoLine(int? defaultValue, Window owner = null)
        {
            GoToLineWindow window = new GoToLineWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            if (defaultValue.HasValue)
                window.lineNumberTextBox.Text = defaultValue.Value.ToString();
            bool? closeStatus = window.ShowDialog();
            int result;
            if (closeStatus.HasValue && closeStatus.Value && Int32.TryParse(window.lineNumberTextBox.Text, out result) && result > 0)
                return result;

            return null;
        }

        public GoToLineWindow() { InitializeComponent(); }

        private void OKButton_Click(object sender, RoutedEventArgs e) { DialogResult = lineNumberTextBox.Text.Length > 0; }

        private void CancelButton_Click(object sender, RoutedEventArgs e) { DialogResult = false; }

        private void lineNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
                e.Handled = true;
        }
    }
}
