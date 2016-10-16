using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ViewModelValidationMessageVM : DependencyObject
    {
        #region IsWarning Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="IsWarning"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsWarning = "IsWarning";

        /// <summary>
        /// Identifies the <see cref="IsWarning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsWarningProperty = DependencyProperty.Register(DependencyPropertyName_IsWarning, typeof(bool), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Indicates whether the message is a warning.
        /// </summary>
        public bool IsWarning
        {
            get { return (bool)(GetValue(IsWarningProperty)); }
            set { SetValue(IsWarningProperty, value); }
        }
        
        #endregion

        #region PropertyName Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_PropertyName = "PropertyName";

        /// <summary>
        /// Identifies the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(DependencyPropertyName_PropertyName, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Name of property being validated.
        /// </summary>
        public string PropertyName
        {
            get { return GetValue(PropertyNameProperty) as string; }
            set { SetValue(PropertyNameProperty, value); }
        }
        
        #endregion

        #region Message Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Message"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Message = "Message";

        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(DependencyPropertyName_Message, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata("", null, (DependencyObject d, object baseValue) => (baseValue as string) ?? ""));

        /// <summary>
        /// Validation message
        /// </summary>
        public string Message
        {
            get { return GetValue(MessageProperty) as string; }
            set { SetValue(MessageProperty, value); }
        }
        
        #endregion

        #region Details Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Details"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Details = "Details";

        /// <summary>
        /// Identifies the <see cref="Details"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsProperty = DependencyProperty.Register(DependencyPropertyName_Details, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata("", null, (DependencyObject d, object baseValue) => (baseValue as string) ?? ""));

        /// <summary>
        /// Validation message details.
        /// </summary>
        public string Details
        {
            get { return GetValue(DetailsProperty) as string; }
            set { SetValue(DetailsProperty, value); }
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