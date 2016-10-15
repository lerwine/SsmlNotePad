using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ViewModelValidationMessageVM : DependencyObject
    {
        #region IsWarning Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="IsWarning"/> has changed.
        // /// </summary>
        // public event EventHandler IsWarningPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="IsWarning"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsWarning = "IsWarning";

        /// <summary>
        /// Identifies the <see cref="IsWarning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsWarningProperty = DependencyProperty.Register(DependencyPropertyName_IsWarning, typeof(bool), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata(false,
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ViewModelValidationMessageVM).IsWarning_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))/*,
                    (DependencyObject d, object baseValue) => (d as ViewModelValidationMessageVM).IsWarning_CoerceValue(baseValue)*/));

        /// <summary>
        /// Indicates whether the message is a warning.
        /// </summary>
        public bool IsWarning
        {
            get { return (bool)(GetValue(IsWarningProperty)); }
            set { SetValue(IsWarningProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="IsWarning"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <see cref="IsWarning"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <see cref="IsWarning"/> property was changed.</param>
        protected virtual void IsWarning_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement ViewModelValidationMessageVM.IsWarning_PropertyChanged(bool, bool)
            // IsWarningPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        // /// <summary>
        // /// This gets called whenever <see cref="IsWarning"/> is being re-evaluated, or coercion is specifically requested.
        // /// </summary>
        // /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        // /// <returns>The coerced value.</returns>
        // public virtual bool IsWarning_CoerceValue(object baseValue)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        #region PropertyName Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="PropertyName"/> has changed.
        // /// </summary>
        // public event EventHandler PropertyNamePropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_PropertyName = "PropertyName";

        /// <summary>
        /// Identifies the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(DependencyPropertyName_PropertyName, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ViewModelValidationMessageVM).PropertyName_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as ViewModelValidationMessageVM).PropertyName_CoerceValue(baseValue)));

        /// <summary>
        /// Name of property being validated.
        /// </summary>
        public string PropertyName
        {
            get { return GetValue(PropertyNameProperty) as string; }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="PropertyName"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="PropertyName"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="PropertyName"/> property was changed.</param>
        protected virtual void PropertyName_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement ViewModelValidationMessageVM.PropertyName_PropertyChanged(string, string)
            // PropertyNamePropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This gets called whenever <see cref="PropertyName"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string PropertyName_CoerceValue(object baseValue)
        {
            // TODO: Implement ViewModelValidationMessageVM.PropertyName_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Message Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="Message"/> has changed.
        // /// </summary>
        // public event EventHandler MessagePropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="Message"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Message = "Message";

        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(DependencyPropertyName_Message, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ViewModelValidationMessageVM).Message_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as ViewModelValidationMessageVM).Message_CoerceValue(baseValue)));

        /// <summary>
        /// Validation message
        /// </summary>
        public string Message
        {
            get { return GetValue(MessageProperty) as string; }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="Message"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="Message"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="Message"/> property was changed.</param>
        protected virtual void Message_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement ViewModelValidationMessageVM.Message_PropertyChanged(string, string)
            // MessagePropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This gets called whenever <see cref="Message"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Message_CoerceValue(object baseValue)
        {
            // TODO: Implement ViewModelValidationMessageVM.Message_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Details Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="Details"/> has changed.
        // /// </summary>
        // public event EventHandler DetailsPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="Details"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Details = "Details";

        /// <summary>
        /// Identifies the <see cref="Details"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsProperty = DependencyProperty.Register(DependencyPropertyName_Details, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ViewModelValidationMessageVM).Details_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as ViewModelValidationMessageVM).Details_CoerceValue(baseValue)));

        /// <summary>
        /// Validation message details.
        /// </summary>
        public string Details
        {
            get { return GetValue(DetailsProperty) as string; }
            set { SetValue(DetailsProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="Details"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="Details"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="Details"/> property was changed.</param>
        protected virtual void Details_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement ViewModelValidationMessageVM.Details_PropertyChanged(string, string)
            // DetailsPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This gets called whenever <see cref="Details"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Details_CoerceValue(object baseValue)
        {
            // TODO: Implement ViewModelValidationMessageVM.Details_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        public ViewModelValidationMessageVM(string propertyName, string message, string details, bool isWarning)
            : this(propertyName, message, isWarning)
        {
            Details = details;
        }

        public ViewModelValidationMessageVM(string propertyName, string message, bool isWarning)
        {
            PropertyName = propertyName;
            Message = message;
            IsWarning = IsWarning;
        }

        public ViewModelValidationMessageVM() { }

    }
}