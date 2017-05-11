using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ErrorMessageVM : DependencyObject
    {
        #region Message Property Members

        /// <summary>
        /// Defines the name for the <see cref="Message"/> dependency property.
        /// </summary>
        public const string PropertyName_Message = "Message";
        
        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Message, typeof(string), typeof(ErrorMessageVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        /// <summary>
        /// Status message for last file operation.
        /// </summary>
        public string Message
        {
            get { return GetValue(MessageProperty) as string; }
            private set { SetValue(MessagePropertyKey, value); }
        }

        #endregion

        #region InnerErrors Property Members

        /// <summary>
        /// Defines the name for the <see cref="InnerErrors"/> dependency property.
        /// </summary>
        public const string PropertyName_InnerErrors = "InnerErrors";

        private static readonly DependencyPropertyKey InnerErrorsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_InnerErrors,
            typeof(ReadOnlyObservableCollection<ErrorMessageVM>), typeof(ErrorMessageVM), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="InnerErrors"/> read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerErrorsProperty = InnerErrorsPropertyKey.DependencyProperty;

        private ObservableCollection<ErrorMessageVM> _innerInnerErrors = new ObservableCollection<ErrorMessageVM>();

        /// <summary>
        /// Inner collection for <see cref="InnerErrors"/>.
        /// </summary>
        protected ObservableCollection<ErrorMessageVM> InnerInnerErrors { get { return _innerInnerErrors; } }

        /// <summary>
        /// XML validation messages.
        /// </summary>
        public ReadOnlyObservableCollection<ErrorMessageVM> InnerErrors
        {
            get { return (ReadOnlyObservableCollection<ErrorMessageVM>)(GetValue(InnerErrorsProperty)); }
            private set { SetValue(InnerErrorsPropertyKey, value); }
        }

        #endregion

        #region IsWarning Property Members

        /// <summary>
        /// Defines the name for the <see cref="IsWarning"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_IsWarning = "IsWarning";

        /// <summary>
        /// Identifies the <see cref="IsWarning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsWarningProperty = DependencyProperty.Register(DependencyPropertyName_IsWarning, typeof(bool), typeof(ErrorMessageVM),
                new PropertyMetadata(true));

        /// <summary>
        /// Indicates whether the content should be selected after it is inserted.
        /// </summary>
        public bool IsWarning
        {
            get { return (bool)(GetValue(IsWarningProperty)); }
            set { SetValue(IsWarningProperty, value); }
        }

        #endregion

        public ErrorMessageVM(string message, bool isWarning)
        {
            Message = message;
            IsWarning = isWarning;
        }

        public void UpdateFrom(string message, bool isWarning)
        {
            Message = message;
            IsWarning = isWarning;
        }
    }
}