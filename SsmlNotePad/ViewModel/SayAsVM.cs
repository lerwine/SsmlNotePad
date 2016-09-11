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
using System.Speech.Synthesis;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class SayAsVM : ValidatingViewModel
    {
        #region DisplayText Property Members

        public const string DependencyPropertyName_DisplayText = "DisplayText";

        /// <summary>
        /// Identifies the <seealso cref="DisplayText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(DependencyPropertyName_DisplayText, typeof(string), typeof(SayAsVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SayAsVM).DisplayText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SayAsVM).DisplayText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SayAsVM).DisplayText_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DisplayTextProperty));
                return Dispatcher.Invoke(() => DisplayText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DisplayTextProperty, value);
                else
                    Dispatcher.Invoke(() => DisplayText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="DisplayText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="DisplayText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="DisplayText"/> property was changed.</param>
        protected virtual void DisplayText_PropertyChanged(string oldValue, string newValue)
        {
            if (String.IsNullOrWhiteSpace(newValue))
                SetValidation(DependencyPropertyName_DisplayText, "Display text cannot be empty.");
            else
                SetValidation(DependencyPropertyName_DisplayText, null);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="DisplayText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string DisplayText_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        #region SelectedIndex Property Members

        public const string DependencyPropertyName_SelectedIndex = "SelectedIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedIndex, typeof(int), typeof(SayAsVM),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SayAsVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SayAsVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as SayAsVM).SelectedIndex_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectedIndexProperty));
                return Dispatcher.Invoke(() => SelectedIndex);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedIndexProperty, value);
                else
                    Dispatcher.Invoke(() => SelectedIndex = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="SelectedIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="SelectedIndex"/> property was changed.</param>
        protected virtual void SelectedIndex_PropertyChanged(int oldValue, int newValue)
        {
            if (newValue < 0)
            {
                if (SelectedValue.HasValue)
                    SelectedValue = null;
            }
            else
            {
                SayAs selectedValue = Values[newValue];
                if (!SelectedValue.HasValue || SelectedValue.Value != selectedValue)
                    SelectedValue = selectedValue;
            }
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedIndex_CoerceValue(object baseValue)
        {
            int index = (int)baseValue;
            if (index < -1 || index >= Values.Count)
                return -1;

            return index;
        }

        #endregion

        #region SelectedValue Property Members

        public const string DependencyPropertyName_SelectedValue = "SelectedValue";

        /// <summary>
        /// Identifies the <seealso cref="SelectedValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(DependencyPropertyName_SelectedValue, typeof(SayAs?), typeof(SayAsVM),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SayAsVM).SelectedValue_PropertyChanged((SayAs?)(e.OldValue), (SayAs?)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SayAsVM).SelectedValue_PropertyChanged((SayAs?)(e.OldValue), (SayAs?)(e.NewValue)));
                }));

        /// <summary>
        /// 
        /// </summary>
        public SayAs? SelectedValue
        {
            get
            {
                if (CheckAccess())
                    return (SayAs?)(GetValue(SelectedValueProperty));
                return Dispatcher.Invoke(() => SelectedValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedValueProperty, value);
                else
                    Dispatcher.Invoke(() => SelectedValue = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedValue"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="SayAs?"/> value before the <seealso cref="SelectedValue"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="SayAs?"/> value after the <seealso cref="SelectedValue"/> property was changed.</param>
        protected virtual void SelectedValue_PropertyChanged(SayAs? oldValue, SayAs? newValue)
        {
            int index = (newValue.HasValue) ? Values.IndexOf(newValue.Value) : -1;
            if (SelectedIndex != index)
                SelectedIndex = index;
        }

        #endregion

        #region Values Property Members
        
        public const string PropertyName_Values = "Values";

        private static readonly DependencyPropertyKey ValuesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Values, typeof(ReadOnlyObservableCollection<SayAs>), typeof(SayAsVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Values"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValuesProperty = ValuesPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<SayAs> Values
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<SayAs>)(GetValue(ValuesProperty));
                return Dispatcher.Invoke(() => Values);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ValuesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Values = value);
            }
        }

        #endregion

        public SayAsVM()
        {
            Values = new ReadOnlyObservableCollection<SayAs>(new ObservableCollection<SayAs>(Enum.GetValues(typeof(SayAs)).OfType<SayAs>()));
        }

        private static bool _TryGetSayAs(string selectedText, SayAs? sayAs, Window owner, out string text, out int selectionOffset)
        {
            View.SayAsWindow window = new View.SayAsWindow();
            window.Owner = owner ?? App.Current.MainWindow;
            SayAsVM vm = window.DataContext as SayAsVM;
            if (vm == null)
            {
                vm = new SayAsVM();
                window.DataContext = vm;
            }
            window.Closing += vm.Window_Closing;
            vm.DisplayText = Common.XmlHelper.XmlDecode(selectedText ?? "");
            vm.SelectedValue = sayAs;
            bool? closeStatus = window.ShowDialog();
            if (!vm.SelectedValue.HasValue || vm.SelectedValue.Value == SayAs.Text)
            {
                selectionOffset = 0;
                text = selectedText;
                return false;
            }
            selectedText = Common.XmlHelper.XmlEncode(vm.DisplayText);
            string attributeText, format = null;
            switch (vm.SelectedValue.Value)
            {
                case SayAs.NumberCardinal:
                    attributeText = "cardinal";
                    break;
                case SayAs.NumberOrdinal:
                    attributeText = "ordinal";
                    break;
                case SayAs.DayMonthYear:
                    attributeText = "date";
                    format = "dmy";
                    break;
                case SayAs.MonthDayYear:
                    attributeText = "date";
                    format = "mdy";
                    break;
                case SayAs.YearMonthDay:
                    attributeText = "date";
                    format = "ymd";
                    break;
                case SayAs.YearMonth:
                    attributeText = "date";
                    format = "ym";
                    break;
                case SayAs.MonthYear:
                    attributeText = "date";
                    format = "my";
                    break;
                case SayAs.MonthDay:
                    attributeText = "date";
                    format = "md";
                    break;
                case SayAs.DayMonth:
                    attributeText = "date";
                    format = "dm";
                    break;
                case SayAs.Year:
                    attributeText = "date";
                    format = "y";
                    break;
                case SayAs.Month:
                    attributeText = "date";
                    format = "m";
                    break;
                case SayAs.Day:
                    attributeText = "date";
                    format = "d";
                    break;
                case SayAs.Time24:
                    attributeText = "time";
                    format = "hms24";
                    break;
                case SayAs.Time12:
                    attributeText = "time";
                    format = "hms12";
                    break;
                default:
                    attributeText = vm.SelectedValue.Value.ToString("F").ToLower();
                    break;
            }
            if (format != null)
                attributeText = String.Format("{0}\" format=\"{1}", attributeText, format);
            text = String.Format("<say-as interpret-as=\"{0}\">{1}</say-as>", attributeText, selectedText);
            selectionOffset = 24 + attributeText.Length;
            return closeStatus.HasValue && closeStatus.Value;
        }

        internal static bool TryGetSayAs(string selectedText, SayAs sayAs, Window owner, out string text, out int selectionOffset) { return _TryGetSayAs(selectedText, sayAs, owner, out text, out selectionOffset); }

        internal static bool TryGetSayAs(string selectedText, SayAs sayAs, out string text, out int selectionOffset) { return TryGetSayAs(selectedText, sayAs, null, out text, out selectionOffset); }

        internal static bool TryGetSayAs(string selectedText, Window owner, out string text, out int selectionOffset) { return _TryGetSayAs(selectedText, null, owner, out text, out selectionOffset); }

        internal static bool TryGetSayAs(string selectedText, out string text, out int selectionOffset) { return TryGetSayAs(selectedText, null, out text, out selectionOffset); }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => Window_Closing(sender, e));
                return;
            }

            View.SayAsWindow window = App.GetWindowByDataContext<View.SayAsWindow, SayAsVM>(this);
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
            View.SayAsWindow window = App.GetWindowByDataContext<View.SayAsWindow, SayAsVM>(this);
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
            View.SayAsWindow window = App.GetWindowByDataContext<View.SayAsWindow, SayAsVM>(this);
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        #endregion

    }
}
