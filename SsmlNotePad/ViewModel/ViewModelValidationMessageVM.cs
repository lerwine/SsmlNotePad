using System;
using System.Windows;
using Erwine.Leonard.T.SsmlNotePad.Model;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ViewModelValidationMessageVM : DependencyObject
    {
        public const string ValidationMessage_NoXmlData = "No SSML markup defined.";
        
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
                new PropertyMetadata("", null,
                    (DependencyObject d, object baseValue) => (d as ViewModelValidationMessageVM).PropertyName_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string PropertyName
        {
            get { return GetValue(PropertyNameProperty) as string; }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// This gets called whenever <see cref="PropertyName"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string PropertyName_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

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
                new PropertyMetadata("", null,
                    (DependencyObject d, object baseValue) => (d as ViewModelValidationMessageVM).Message_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get { return GetValue(MessageProperty) as string; }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// This gets called whenever <see cref="Message"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Message_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

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
                new PropertyMetadata("", null,
                    (DependencyObject d, object baseValue) => (d as ViewModelValidationMessageVM).Details_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string Details
        {
            get { return GetValue(DetailsProperty) as string; }
            set { SetValue(DetailsProperty, value); }
        }

        /// <summary>
        /// This gets called whenever <see cref="Details"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Details_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        public ViewModelValidationMessageVM(string propertyName, string message, string details, bool isWarning)
            : this(propertyName, message, isWarning)
        {
            Details = details;
        }

        public ViewModelValidationMessageVM(string propertyName, string message, bool isWarning)
        {
            PropertyName = propertyName ?? "";
            Message = message;
            IsWarning = IsWarning;
        }

        public ViewModelValidationMessageVM() { }

        public ViewModelValidationMessageVM(string propertyName, string message, Exception exception, int lineNumber, int linePosition, bool isWarning)
        {
            PropertyName = propertyName;
            if (lineNumber < 1)
                Message = message;
            else if (linePosition < 1)
                Message = String.Format("Line {0}: {1}", lineNumber, message);
            else
                Message = String.Format("Line {0}, Column {1}: {2}", lineNumber, linePosition, message);
            Details = (exception == null) ? "" : exception.ToString();
            IsWarning = isWarning;
        }
    }
}