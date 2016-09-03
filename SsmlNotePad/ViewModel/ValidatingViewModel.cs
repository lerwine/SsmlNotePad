using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ValidatingViewModel : DependencyObject, INotifyDataErrorInfo, IDataErrorInfo
    {
        #region ViewModelValidateState Property Members

        public const string PropertyName_ViewModelValidateState = "ViewModelValidateState";

        private static readonly DependencyPropertyKey ViewModelValidateStatePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ViewModelValidateState, typeof(ViewModelValidateState), typeof(ValidatingViewModel),
                new PropertyMetadata(ViewModelValidateState.Valid));

        /// <summary>
        /// Identifies the <seealso cref="ViewModelValidateState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelValidateStateProperty = ViewModelValidateStatePropertyKey.DependencyProperty;

        /// <summary>
        /// Validation state for viewmodel object
        /// </summary>
        public ViewModelValidateState ViewModelValidateState
        {
            get
            {
                if (CheckAccess())
                    return (ViewModelValidateState)(GetValue(ViewModelValidateStateProperty));
                return Dispatcher.Invoke(() => ViewModelValidateState);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ViewModelValidateStatePropertyKey, value);
                else
                    Dispatcher.Invoke(() => ViewModelValidateState = value);
            }
        }

        bool INotifyDataErrorInfo.HasErrors { get { return ViewModelValidateState == ViewModelValidateState.Error; } }

        #endregion

        #region ViewModelValidationMessages Property Members

        private ViewModelValidationMessageVM[] _lastMessages = new ViewModelValidationMessageVM[0];

        /// <summary>
        /// Read/Write collection to manage Validation errors.
        /// </summary>
        protected ObservableViewModelCollection<ViewModelValidationMessageVM> InnerViewModelValidationMessages { get; private set; }

        public const string PropertyName_ViewModelValidationMessages = "ViewModelValidationMessages";

        private static readonly DependencyPropertyKey ViewModelValidationMessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ViewModelValidationMessages, typeof(ReadOnlyObservableViewModelCollection<ViewModelValidationMessageVM>), typeof(ValidatingViewModel),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="ViewModelValidationMessages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelValidationMessagesProperty = ViewModelValidationMessagesPropertyKey.DependencyProperty;

        /// <summary>
        /// Contains Validation errors for a specified property or for the entire view model.
        /// </summary>
        public ReadOnlyObservableViewModelCollection<ViewModelValidationMessageVM> ViewModelValidationMessages
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableViewModelCollection<ViewModelValidationMessageVM>)(GetValue(ViewModelValidationMessagesProperty));
                return Dispatcher.Invoke(() => ViewModelValidationMessages);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ViewModelValidationMessagesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ViewModelValidationMessages = value);
            }
        }

        #endregion

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire view model.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// This gets invoked when the validation errors have changed for a property or for the entire view model.
        /// </summary>
        /// <param name="args">Contains data for the <see cref="ValidatingViewModel.ErrorsChanged"/> event</param>
        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs args)
        {
            if (args != null)
                ErrorsChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire view model.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="System.String.Empty"/> 
        /// to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or view model.</returns>
        public IEnumerable<ViewModelValidationMessageVM> GetErrorItems(string propertyName)
        {
            return (String.IsNullOrEmpty(propertyName)) ? ViewModelValidationMessages.Where(i => !i.IsWarning) :
                ViewModelValidationMessages.Where(i => !i.IsWarning && i.PropertyName == propertyName);
        }


        /// <summary>
        /// Gets the validation errors for a specified property or for the entire view model.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="System.String.Empty"/> 
        /// to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or view model.</returns>
        public IEnumerable<string> GetErrors(string propertyName)
        {
            return ((String.IsNullOrEmpty(propertyName)) ? ViewModelValidationMessages.Where(i => i != null && !i.IsWarning) :
                ViewModelValidationMessages.Where(i => i != null && !i.IsWarning && i.PropertyName == propertyName)).Select(m => m.Message);
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) { return GetErrors(propertyName); }

        string IDataErrorInfo.Error
        {
            get
            {
                Func<string> getErrors = () =>
                {
                    if (ViewModelValidateState != ViewModelValidateState.Error)
                        return "";
                    string[] messages = GetErrorItems(null).Select(e => (e.PropertyName.Length == 0) ? e.Message : String.Format("{0}: {1}", e.PropertyName, e.Message)).ToArray();
                    return (messages.Length == 0) ? "" : ((messages.Length == 1) ? messages[0] : String.Join(Environment.NewLine, messages));
                };

                if (CheckAccess())
                    return getErrors();
                
                return Dispatcher.Invoke(getErrors);
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                Func<string> getErrors = () =>
                {
                    if (ViewModelValidateState != ViewModelValidateState.Error)
                        return "";
                    string[] messages = ((String.IsNullOrEmpty(columnName)) ? ViewModelValidationMessages.Where(i => !i.IsWarning && i.PropertyName.Length == 0) :
                        GetErrorItems(columnName)).Select(m => m.Message).Where(m => !String.IsNullOrWhiteSpace(m)).ToArray();
                    return (messages.Length == 0) ? "" : ((messages.Length == 1) ? messages[0] : String.Join(Environment.NewLine, messages));
                };

                if (CheckAccess())
                    return getErrors();

                return Dispatcher.Invoke(getErrors);
            }
        }

        public ValidatingViewModel()
        {
            InnerViewModelValidationMessages = new ObservableViewModelCollection<ViewModel.ViewModelValidationMessageVM>();
            InnerViewModelValidationMessages.CollectionChanged += ViewModelValidationMessages_CollectionChanged;
            ViewModelValidationMessages = new ReadOnlyObservableViewModelCollection<ViewModelValidationMessageVM>(InnerViewModelValidationMessages);
        }

        private void ViewModelValidationMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Dispatcher.CheckAccess())
                OnViewModelValidationMessagesChanged(e.Action, e.OldStartingIndex,
                    e.OldItems == null ? new ViewModelValidationMessageVM[0] : e.OldItems.OfType<ViewModelValidationMessageVM>(),
                    e.NewStartingIndex, e.NewItems == null ? new ViewModelValidationMessageVM[0] : e.NewItems.OfType<ViewModelValidationMessageVM>());
        }

        protected void SetValidation(string propertyName, string message, bool isWarning = false)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => SetValidation(propertyName, message, isWarning));
                return;
            }

            IEnumerable<ViewModelValidationMessageVM> current = InnerViewModelValidationMessages.Where(m => m != null && m.PropertyName == propertyName).ToArray();
            if (!String.IsNullOrWhiteSpace(message))
            {
                ViewModelValidationMessageVM keep = current.FirstOrDefault(m => m.Message == message && m.Details.Length == 0 && m.IsWarning == isWarning);
                if (keep == null)
                {
                    keep = new ViewModelValidationMessageVM(propertyName, message, isWarning);
                    InnerViewModelValidationMessages.Add(keep);
                }
                current = current.Where(c => !ReferenceEquals(c, keep));
            }

            foreach (ViewModelValidationMessageVM vm in current.ToArray())
                InnerViewModelValidationMessages.Remove(vm);
        }

        protected void AddValidation(string propertyName, string message, bool isWarning = false)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Trim().Length == 0)
                throw new ArgumentException("Message cannot be empty.");

            if (!CheckAccess())
                Dispatcher.Invoke(() => AddValidation(propertyName, message, isWarning));
            else if (!InnerViewModelValidationMessages.Any(m => m != null && m.PropertyName == propertyName && m.Message == message && m.Details.Length == 0 && m.IsWarning == isWarning))
                InnerViewModelValidationMessages.Add(new ViewModelValidationMessageVM(propertyName, message, isWarning));
        }

        protected void SetValidation(string propertyName, string message, string details, bool isWarning = false)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Trim().Length == 0)
                throw new ArgumentException("Message cannot be empty.");

            if (details == null)
                details = "";

            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => SetValidation(propertyName, message, details, isWarning));
                return;
            }

            IEnumerable<ViewModelValidationMessageVM> current = InnerViewModelValidationMessages.Where(m => m != null && m.PropertyName == propertyName).ToArray();
            if (!String.IsNullOrWhiteSpace(message))
            {
                ViewModelValidationMessageVM keep = current.FirstOrDefault(m => m.Message == message && m.Details == details && m.IsWarning == isWarning);
                if (keep == null)
                {
                    keep = new ViewModelValidationMessageVM(propertyName, message, isWarning);
                    InnerViewModelValidationMessages.Add(keep);
                }
                current = current.Where(c => !ReferenceEquals(c, keep));
            }

            foreach (ViewModelValidationMessageVM vm in current.ToArray())
                InnerViewModelValidationMessages.Remove(vm);
        }

        protected void AddValidation(string propertyName, string message, string details, bool isWarning = false)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Trim().Length == 0)
                throw new ArgumentException("Message cannot be empty.");

            if (details == null)
                details = "";

            if (!CheckAccess())
                Dispatcher.Invoke(() => AddValidation(propertyName, message, details, isWarning));
            else if (!InnerViewModelValidationMessages.Any(m => m != null && m.PropertyName == propertyName && m.Message == message && m.Details == details && m.IsWarning == isWarning))
                InnerViewModelValidationMessages.Add(new ViewModelValidationMessageVM(propertyName, message, details, isWarning));
        }

        protected virtual void OnViewModelValidationMessagesChanged(NotifyCollectionChangedAction action, int oldStartingIndex,
            IEnumerable<ViewModelValidationMessageVM> oldItems, int newStartingIndex, IEnumerable<ViewModelValidationMessageVM> newItems)
        {
            ViewModelValidateState state;
            string[] errorsChanged;
            lock (_lastMessages)
            {
                bool hadErrors = ViewModelValidateState == ViewModelValidateState.Error;
                ViewModelValidationMessageVM[] currentMessages = ViewModelValidationMessages.Where(i => !i.IsWarning).ToArray();
                state = (currentMessages.Length > 0) ? ViewModelValidateState.Error :
                    ((ViewModelValidationMessages.Count > 0) ? ViewModelValidateState.Warning : ViewModelValidateState.Valid);
                errorsChanged = _lastMessages.Where(n => !currentMessages.Any(i => i.Equals(n)))
                    .Concat(currentMessages.Where(i => !_lastMessages.Any(n => n.Equals(i)))).Select(i => i.PropertyName).Distinct().ToArray();
                if ((state == ViewModelValidateState.Error) != hadErrors && !errorsChanged.Any(i => i.Length == 0))
                    errorsChanged = errorsChanged.Concat(new string[] { "" }).ToArray();
                _lastMessages = currentMessages;
            }

            ViewModelValidateState = state;
            foreach (string propertyName in errorsChanged)
                OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
