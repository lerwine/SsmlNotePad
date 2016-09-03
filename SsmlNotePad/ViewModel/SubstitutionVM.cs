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
    public class SubstitutionVM : ValidatingViewModel
    {
        public SubstitutionVM()
        {
            SetValidation(DependencyPropertyName_DisplayedText, DisplayedText_Empty);
            SetValidation(DependencyPropertyName_SpokenText, SpokenText_Empty);
        }

        public static bool TryGetSubstitution(string displayedText, string spokenText, Window owner, out string result, out int selectionOffset)
        {
            View.SubstitutionWindow window = new View.SubstitutionWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            SubstitutionVM vm = window.DataContext as SubstitutionVM;
            if (vm == null)
            {
                vm = new SubstitutionVM();
                window.DataContext = vm;
            }
            window.Closing += vm.Window_Closing;
            vm.DisplayedText = Common.XmlHelper.XmlDecode(displayedText ?? "");
            vm.SpokenText = (String.IsNullOrWhiteSpace(spokenText)) ? vm.DisplayedText : Common.XmlHelper.XmlDecode(spokenText);
            bool? closeStatus = window.ShowDialog();
            spokenText = Common.XmlHelper.XmlEncode(vm.SpokenText, Common.XmlEncodeOption.DoubleQuotedAttribute);
            displayedText = Common.XmlHelper.XmlEncode(vm.DisplayedText);
            result = String.Format("<sub alias=\"{0}\">{1}</sub>", spokenText, displayedText);
            selectionOffset = (spokenText.Length == 0) ? 12 : 14 + spokenText.Length;
            return closeStatus.HasValue && closeStatus.Value;
        }

        public static bool TryGetSubstitution(string displayedText, string spokenText, out string result, out int selectionOffset) { return TryGetSubstitution(displayedText, spokenText, null, out result, out selectionOffset); }

        public static bool TryGetSubstitution(string displayedText, Window owner, out string result, out int selectionOffset) { return TryGetSubstitution(displayedText, null, owner, out result, out selectionOffset); }

        public static bool TryGetSubstitution(string displayedText, out string result, out int selectionOffset) { return TryGetSubstitution(displayedText, null, null, out result, out selectionOffset); }
        
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => Window_Closing(sender, e));
                return;
            }

            View.SubstitutionWindow window = App.GetWindowByDataContext<View.SubstitutionWindow, SubstitutionVM>(this);
            bool? dialogResult = (window == null) ? null : window.DialogResult;
            if (!dialogResult.HasValue || !dialogResult.Value)
                return;

            if (ViewModelValidateState == ViewModelValidateState.Error)
                e.Cancel = true;
        }

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
            View.SubstitutionWindow window = App.GetWindowByDataContext<View.SubstitutionWindow, SubstitutionVM>(this);
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
            View.SubstitutionWindow window = App.GetWindowByDataContext<View.SubstitutionWindow, SubstitutionVM>(this);
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        #endregion

        #region DisplayedText Property Members

        public const string DependencyPropertyName_DisplayedText = "DisplayedText";
        public const string DisplayedText_Empty = "Displayed text cannot be empty.";
        /// <summary>
        /// Identifies the <seealso cref="DisplayedText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayedTextProperty = DependencyProperty.Register(DependencyPropertyName_DisplayedText, typeof(string), typeof(SubstitutionVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SubstitutionVM).DisplayedText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SubstitutionVM).DisplayedText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SubstitutionVM).DisplayedText_CoerceValue(baseValue)));

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
            SetValidation(DependencyPropertyName_DisplayedText, (String.IsNullOrWhiteSpace(newValue)) ? DisplayedText_Empty : null);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="DisplayedText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string DisplayedText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region SpokenText Property Members

        public const string DependencyPropertyName_SpokenText = "SpokenText";
        public const string SpokenText_Empty = "Spoken text cannot be empty.";

        /// <summary>
        /// Identifies the <seealso cref="SpokenText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpokenTextProperty = DependencyProperty.Register(DependencyPropertyName_SpokenText, typeof(string), typeof(SubstitutionVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SubstitutionVM).SpokenText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SubstitutionVM).SpokenText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SubstitutionVM).SpokenText_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string SpokenText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(SpokenTextProperty));
                return Dispatcher.Invoke(() => SpokenText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SpokenTextProperty, value);
                else
                    Dispatcher.Invoke(() => SpokenText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SpokenText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="SpokenText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="SpokenText"/> property was changed.</param>
        protected virtual void SpokenText_PropertyChanged(string oldValue, string newValue)
        {
            SetValidation(DependencyPropertyName_SpokenText, (String.IsNullOrWhiteSpace(newValue)) ? SpokenText_Empty : null);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SpokenText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string SpokenText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        protected override void OnErrorsChanged(DataErrorsChangedEventArgs args)
        {
            base.OnErrorsChanged(args);
            OKCommand.IsEnabled = ViewModelValidateState != ViewModelValidateState.Error;
        }
    }
}
