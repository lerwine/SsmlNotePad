using System;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    /// <summary>
    /// Represents a view model validation error.
    /// </summary>
    public sealed class ViewModelValidationMessageVM : DependencyObject, IEquatable<ViewModelValidationMessageVM>
    {
        #region PropertyName Property Members

        public const string PropertyName_PropertyName = "PropertyName";

        private static readonly DependencyPropertyKey PropertyNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PropertyName, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Items"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = PropertyNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Name of dependency property associated with walidation message or empty if message is not specific to a single property.
        /// </summary>
        public string PropertyName
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PropertyNameProperty));
                return Dispatcher.Invoke(() => PropertyName);
            }
            private set { SetValue(PropertyNamePropertyKey, value); }
        }

        #endregion

        #region Message Property Members

        public const string PropertyName_Message = "Message";

        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Message, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        /// <summary>
        /// Brief validation message.
        /// </summary>
        public string Message
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(MessageProperty));
                return Dispatcher.Invoke(() => Message);
            }
            private set { SetValue(MessagePropertyKey, value ?? ""); }
        }

        #endregion

        #region Details Property Members

        public const string PropertyName_Details = "Details";

        private static readonly DependencyPropertyKey DetailsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Details, typeof(string), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Details"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsProperty = DetailsPropertyKey.DependencyProperty;

        /// <summary>
        /// Details about error or warning.
        /// </summary>
        public string Details
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DetailsProperty));
                return Dispatcher.Invoke(() => Details);
            }
            private set { SetValue(DetailsPropertyKey, value ?? ""); }
        }

        #endregion

        #region IsWarning Property Members

        public const string PropertyName_IsWarning = "IsWarning";

        private static readonly DependencyPropertyKey IsWarningPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsWarning, typeof(bool), typeof(ViewModelValidationMessageVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="IsWarning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsWarningProperty = IsWarningPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates whether the error is a warning.
        /// </summary>
        public bool IsWarning
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(IsWarningProperty));
                return Dispatcher.Invoke(() => IsWarning);
            }
            private set { SetValue(IsWarningPropertyKey, value); }
        }

        #endregion

        /// <summary>
        /// Initialize new <see cref="ViewModelValidationMessageVM"/> object.
        /// </summary>
        /// <param name="property"><see cref="DependencyProperty"/> associated with error or warning or null if error is not specific to a single property.</param>
        /// <param name="message">Brief validation message.</param>
        /// <param name="details">Validation error or warning details.</param>
        /// <param name="isWarning">Indicates wether the validation message is a warning.</param>
        public ViewModelValidationMessageVM(DependencyProperty property, string message, string details = null, bool isWarning = false)
            : this((property == null) ? "" : property.Name, message, details, isWarning) { }

        /// <summary>
        /// Initialize new <see cref="ViewModelValidationMessageVM"/> object.
        /// </summary>
        /// <param name="propertyName">Name of dependency property associated with walidation message or empty if message is not specific to a single property.</param>
        /// <param name="message">Brief validation message.</param>
        /// <param name="details">Validation error or warning details.</param>
        /// <param name="isWarning">Indicates wether the validation message is a warning.</param>
        public ViewModelValidationMessageVM(string propertyName, string message, string details = null, bool isWarning = false)
        {
            PropertyName = propertyName;
            if (String.IsNullOrWhiteSpace(message))
            {
                if (String.IsNullOrWhiteSpace(details))
                    Message = (isWarning) ? "An Unexpected Error (warning) has occurred." : "Unexpected Error";
                else
                    Message = details;
            }
            else
            {
                Message = message;
                Details = details;
            }
            IsWarning = isWarning;
        }

        /// <summary>
        /// Initialize new <see cref="ViewModelValidationMessageVM"/> object.
        /// </summary>
        /// <param name="property"><see cref="DependencyProperty"/> associated with error or warning or null if error is not specific to a single property.</param>
        /// <param name="message">Brief validation message.</param>
        /// <param name="isWarning">Indicates wether the validation message is a warning.</param>
        public ViewModelValidationMessageVM(DependencyProperty property, string message, bool isWarning) : this(property, message, null, isWarning) { }

        /// <summary>
        /// Initialize new <see cref="ViewModelValidationMessageVM"/> object.
        /// </summary>
        /// <param name="propertyName">Name of dependency property associated with walidation message or empty if message is not specific to a single property.</param>
        /// <param name="message">Brief validation message.</param>
        /// <param name="isWarning">Indicates wether the validation message is a warning.</param>
        public ViewModelValidationMessageVM(string propertyName, string message, bool isWarning) : this(propertyName, message, null, isWarning) { }

        /// <summary>
        /// Initialize new <see cref="ViewModelValidationMessageVM"/> object which is not specific to any single property.
        /// </summary>
        /// <param name="message">Brief validation message.</param>
        /// <param name="isWarning">Indicates wether the validation message is a warning.</param>
        public ViewModelValidationMessageVM(string message, bool isWarning = false) : this(null as string, message, null, isWarning) { }

        public bool Equals(ViewModelValidationMessageVM other)
        {
            return other != null && (ReferenceEquals(this, other) || PropertyName.Equals(other.PropertyName) && Message.Equals(other.Message) &&
                Details.Equals(other.Details) && IsWarning.Equals(other.IsWarning));
        }
    }
}
