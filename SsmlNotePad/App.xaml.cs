using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string AppResourceName_MainWindowViewModel = "MainWindowViewModel";
        public const string AppResourceName_AppSettingsViewModel = "AppSettingsViewModel";
        public const string AppResourceName_SpeechGenerationProgressViewModel = "SpeechGenerationProgressViewModel";

        public static TWindow GetWindowByDataContext<TWindow, TDataContext>(TDataContext obj)
            where TWindow : Window
            where TDataContext : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            
            if (App.Current.CheckAccess())
                return App.Current.Windows.Cast<Window>().OfType<TWindow>().FirstOrDefault(w => w.DataContext != null && ReferenceEquals(w.DataContext, obj));

            return App.Current.Dispatcher.Invoke(() => GetWindowByDataContext<TWindow, TDataContext>(obj));
        }

        public static ViewModel.MainWindowVM MainWindowViewModel
        {
            get
            {
                if (App.Current.CheckAccess())
                    return App.Current.FindResource(AppResourceName_MainWindowViewModel) as ViewModel.MainWindowVM;
                return App.Current.Dispatcher.Invoke(() => App.Current.FindResource(AppResourceName_MainWindowViewModel) as ViewModel.MainWindowVM);
            }
        }

        public static ViewModel.AppSettingsVM AppSettingsViewModel
        {
            get
            {
                if (App.Current.CheckAccess())
                    return App.Current.FindResource(AppResourceName_AppSettingsViewModel) as ViewModel.AppSettingsVM;
                return App.Current.Dispatcher.Invoke(() => App.Current.FindResource(AppResourceName_AppSettingsViewModel) as ViewModel.AppSettingsVM);
            }
        }

        public static ViewModel.SpeechGenerationProgressVM SpeechGenerationProgressViewModel
        {
            get
            {
                if (App.Current.CheckAccess())
                    return App.Current.FindResource(AppResourceName_SpeechGenerationProgressViewModel) as ViewModel.SpeechGenerationProgressVM;
                return App.Current.Dispatcher.Invoke(() => App.Current.FindResource(AppResourceName_SpeechGenerationProgressViewModel) as ViewModel.SpeechGenerationProgressVM);
            }
        }
    }

}
