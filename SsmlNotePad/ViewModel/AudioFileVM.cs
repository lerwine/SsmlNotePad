using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Collections.ObjectModel;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class AudioFileVM : ValidatingViewModel
    {
        public static bool TryGetAudioFile(string displayedText, string audioUri, string description, Window owner, out string result, out int selectionOffset)
        {
            View.AudioFileWindow window = new View.AudioFileWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            AudioFileVM vm = window.DataContext as AudioFileVM;
            if (vm == null)
            {
                vm = new AudioFileVM();
                window.DataContext = vm;
            }
            window.Closing += vm.Window_Closing;
            vm.AudioUri = Common.XmlHelper.XmlDecode(audioUri ?? "");
            vm.DisplayedText = displayedText ?? "";
            vm.Description = description ?? "";
            bool? closeStatus = window.ShowDialog();
            audioUri = Common.XmlHelper.XmlEncode(vm.AudioUri.Trim(), Common.XmlEncodeOption.DoubleQuotedAttribute);
            displayedText = Common.XmlHelper.XmlEncode(vm.DisplayedText.Trim());
            description = Common.XmlHelper.XmlEncode(vm.Description.Trim());

            if (description.Length != 0)
                result = String.Format("<audio src=\"{0}\">{1}<description>{2}</description></audio>", audioUri, displayedText, description);
            else if (displayedText.Length != 0)
                result = String.Format("<audio src=\"{0}\">{1}</audio>", audioUri, displayedText);
            else
                result = String.Format("<audio src=\"{0}\" />", audioUri);

            selectionOffset = (displayedText.Length == 0) ? 12 : 14 + audioUri.Length;
            return closeStatus.HasValue && closeStatus.Value;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => Window_Closing(sender, e));
                return;
            }

            View.AudioFileWindow window = App.GetWindowByDataContext<View.AudioFileWindow, AudioFileVM>(this);
            bool? dialogResult = (window == null) ? null : window.DialogResult;
            if (!dialogResult.HasValue || !dialogResult.Value)
                return;

            if (ViewModelValidateState == ViewModelValidateState.Error)
                e.Cancel = true;
        }

        public static bool TryGetAudioFile(string displayedText, string audioUri, Window owner, out string result, out int selectionOffset) { return TryGetAudioFile(displayedText, audioUri, null, owner, out result, out selectionOffset); }

        public static bool TryGetAudioFile(string displayedText, string audioUri, string description, out string result, out int selectionOffset) { return TryGetAudioFile(displayedText, audioUri, description, null, out result, out selectionOffset); }

        public static bool TryGetAudioFile(string displayedText, string audioUri, out string result, out int selectionOffset) { return TryGetAudioFile(displayedText, audioUri, null, null, out result, out selectionOffset); }

        public static bool TryGetAudioFile(string displayedText, Window owner, out string result, out int selectionOffset) { return TryGetAudioFile(displayedText, null, owner, out result, out selectionOffset); }

        public static bool TryGetAudioFile(string displayedText, out string result, out int selectionOffset) { return TryGetAudioFile(displayedText, null, null, null, out result, out selectionOffset); }

        public AudioFileVM() { SetValidation(DependencyPropertyName_AudioUri, Error_EmptyUri); }

        #region Browse Command Property Members

        private Command.RelayCommand _browseCommand = null;

        public Command.RelayCommand BrowseCommand
        {
            get
            {
                if (this._browseCommand == null)
                    this._browseCommand = new Command.RelayCommand(this.OnBrowse);

                return this._browseCommand;
            }
        }

        protected virtual void OnBrowse(object parameter)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => OnBrowse(parameter));
                return;
            }

            if (!_browseCommand.IsEnabled)
                return;

            _browseCommand.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.AddExtension = true;
                openFileDialog.CheckFileExists = true;
                string localPath;
                Common.FileUtility.GetLocalPath(Dispatcher.Invoke(() => AudioUri), out localPath);
                View.AudioFileWindow window = App.GetWindowByDataContext<View.AudioFileWindow, AudioFileVM>(this);
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Select Audio File";
                if (Common.FileUtility.InvokeAudioFileDialog(openFileDialog, window ?? Dispatcher.Invoke(() => App.Current.MainWindow), localPath))
                    Dispatcher.Invoke(() => AudioUri = openFileDialog.FileName);
            }).ContinueWith(t => Dispatcher.Invoke(() => _browseCommand.IsEnabled = true));
        }

        #endregion
        
        #region OK Command Property Members

        private Command.RelayCommand _okCommand = null;

        public Command.RelayCommand OKCommand
        {
            get
            {
                if (this._okCommand == null)
                    this._okCommand = new Command.RelayCommand(this.OnOK);

                return this._okCommand;
            }
        }

        protected virtual void OnOK(object parameter)
        {
            View.AudioFileWindow window = App.GetWindowByDataContext<View.AudioFileWindow, AudioFileVM>(this);
            if (ViewModelValidateState == ViewModelValidateState.Error)
                MessageBox.Show(window ?? App.Current.MainWindow, "Fix validation errors before saving.", "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
            else if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        #endregion

        #region Cancel Command Property Members

        private Command.RelayCommand _cancelCommand = null;

        public Command.RelayCommand CancelCommand
        {
            get
            {
                if (this._cancelCommand == null)
                    this._cancelCommand = new Command.RelayCommand(this.OnCancel);

                return this._cancelCommand;
            }
        }

        protected virtual void OnCancel(object parameter)
        {
            View.AudioFileWindow window = App.GetWindowByDataContext<View.AudioFileWindow, AudioFileVM>(this);
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        #endregion

        #region AudioUri Property Members

        public const string DependencyPropertyName_AudioUri = "AudioUri";

        public const string Error_EmptyUri = "Audio file path / URI must be provided.";
        public const string Error_InvalidUri = "Audio file path / URI is not valid.";

        /// <summary>
        /// Identifies the <seealso cref="AudioUri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AudioUriProperty = DependencyProperty.Register(DependencyPropertyName_AudioUri, typeof(string), typeof(AudioFileVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AudioFileVM).AudioUri_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AudioFileVM).AudioUri_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AudioFileVM).AudioUri_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string AudioUri
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(AudioUriProperty));
                return Dispatcher.Invoke(() => AudioUri);
            }
            set
            {
                if (CheckAccess())
                    SetValue(AudioUriProperty, value);
                else
                    Dispatcher.Invoke(() => AudioUri = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="AudioUri"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="AudioUri"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="AudioUri"/> property was changed.</param>
        protected virtual void AudioUri_PropertyChanged(string oldValue, string newValue)
        {
            Uri uri;
            if (String.IsNullOrWhiteSpace(newValue))
                SetValidation(DependencyPropertyName_AudioUri, Error_EmptyUri);
            else if (!Uri.TryCreate(newValue.Trim(), UriKind.Absolute, out uri))
                SetValidation(DependencyPropertyName_AudioUri, Error_InvalidUri);
            else
                SetValidation(DependencyPropertyName_AudioUri, null);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="AudioUri"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string AudioUri_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region DisplayedText Property Members

        public const string DependencyPropertyName_DisplayedText = "DisplayedText";

        public const string Error_EmptyDisplayedText = "Display text cannot be empty if description is provided.";

        /// <summary>
        /// Identifies the <seealso cref="DisplayedText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayedTextProperty = DependencyProperty.Register(DependencyPropertyName_DisplayedText, typeof(string), typeof(AudioFileVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AudioFileVM).DisplayedText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AudioFileVM).DisplayedText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AudioFileVM).DisplayedText_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string DisplayedText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DisplayedTextProperty));
                return Dispatcher.Invoke(() => DisplayedText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DisplayedTextProperty, value);
                else
                    Dispatcher.Invoke(() => DisplayedText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="DisplayedText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="DisplayedText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="DisplayedText"/> property was changed.</param>
        protected virtual void DisplayedText_PropertyChanged(string oldValue, string newValue)
        {
            if (String.IsNullOrWhiteSpace(newValue))
            {
                if (String.IsNullOrWhiteSpace(Description))
                    ShowDescriptionControls = false;
                else
                {
                    SetValidation(DependencyPropertyName_DisplayedText, Error_EmptyDisplayedText);
                    ShowDescriptionControls = true;
                    return;
                }
            }
            else
                ShowDescriptionControls = true;

            SetValidation(DependencyPropertyName_DisplayedText, null);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="DisplayedText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string DisplayedText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region Description Property Members

        public const string DependencyPropertyName_Description = "Description";

        /// <summary>
        /// Identifies the <seealso cref="Description"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(DependencyPropertyName_Description, typeof(string), typeof(AudioFileVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AudioFileVM).Description_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AudioFileVM).Description_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AudioFileVM).Description_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DescriptionProperty));
                return Dispatcher.Invoke(() => Description);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DescriptionProperty, value);
                else
                    Dispatcher.Invoke(() => Description = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Description"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Description"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Description"/> property was changed.</param>
        protected virtual void Description_PropertyChanged(string oldValue, string newValue)
        {
            if (!String.IsNullOrWhiteSpace(DisplayedText))
                ShowDescriptionControls = true;
            else if (!String.IsNullOrWhiteSpace(newValue))
            {
                ShowDescriptionControls = true;
                SetValidation(DependencyPropertyName_DisplayedText, Error_EmptyDisplayedText);
                return;
            }

            SetValidation(DependencyPropertyName_DisplayedText, null);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Description"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Description_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region ShowDescriptionControls Property Members

        public const string PropertyName_ShowDescriptionControls = "ShowDescriptionControls";

        private static readonly DependencyPropertyKey ShowDescriptionControlsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ShowDescriptionControls, typeof(bool), typeof(AudioFileVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="ShowDescriptionControls"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowDescriptionControlsProperty = ShowDescriptionControlsPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public bool ShowDescriptionControls
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(ShowDescriptionControlsProperty));
                return Dispatcher.Invoke(() => ShowDescriptionControls);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ShowDescriptionControlsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ShowDescriptionControls = value);
            }
        }

        #endregion

        protected override void OnErrorsChanged(DataErrorsChangedEventArgs args)
        {
            base.OnErrorsChanged(args);
            OKCommand.IsEnabled = ViewModelValidateState != ViewModelValidateState.Error;
        }
    }
}
