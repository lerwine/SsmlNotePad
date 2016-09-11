using System.ComponentModel;
using System.IO;
using System.Speech.AudioFormat;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.View
{
    /// <summary>
    /// Interaction logic for SpeechProgressWindow.xaml
    /// </summary>
    public partial class SpeechProgressWindow : Window
    {
        public SpeechProgressWindow()
        {
            InitializeComponent();
        }

        public static void Start(string ssml, Stream audioDestination, SpeechAudioFormatInfo formatInfo, Window owner = null)
        {
            SpeechProgressWindow window = new SpeechProgressWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            (window.DataContext as ViewModel.SpeechProgressVM).Start(ssml, audioDestination, formatInfo);
            window.ShowDialog();
        }

        public static void Start(string ssml, Stream audioDestination, Window owner = null)
        {
            SpeechProgressWindow window = new SpeechProgressWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            (window.DataContext as ViewModel.SpeechProgressVM).Start(ssml, audioDestination);
            window.ShowDialog();
        }

        public static void Start(string ssml, string path, SpeechAudioFormatInfo formatInfo, Window owner = null)
        {
            SpeechProgressWindow window = new SpeechProgressWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            (window.DataContext as ViewModel.SpeechProgressVM).Start(ssml, path, formatInfo);
            window.ShowDialog();
        }

        public static void Start(string ssml, string path, Window owner = null)
        {
            SpeechProgressWindow window = new SpeechProgressWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            (window.DataContext as ViewModel.SpeechProgressVM).Start(ssml, path);
            window.ShowDialog();
        }

        public static void Start(string ssml, Window owner = null)
        {
            SpeechProgressWindow window = new SpeechProgressWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            ViewModel.SpeechProgressVM vm = window.DataContext as ViewModel.SpeechProgressVM;
            vm.Start(ssml);
            window.ShowDialog();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ViewModel.SpeechProgressVM vm = DataContext as ViewModel.SpeechProgressVM;
            if (vm.CancelSpeechCommand.CanExecute(e))
                vm.CancelSpeechCommand.Execute(e);
            base.OnClosing(e);
        }
    }
}
