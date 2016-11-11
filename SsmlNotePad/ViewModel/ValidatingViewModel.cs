using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ValidatingViewModel : DependencyObject, INotifyDataErrorInfo, IDataErrorInfo
    {
        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire view model.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        
        #region Messages Property Members

        /// <summary>
        /// Defines the name for the <see cref="Messages"/> dependency property.
        /// </summary>
        public const string PropertyName_Messages = "Messages";

        private static readonly DependencyPropertyKey MessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Messages, typeof(NotificationMessageCollection), typeof(ValidatingViewModel),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Messages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessagesProperty = MessagesPropertyKey.DependencyProperty;

        /// <summary>
        /// Error and validation messages.
        /// </summary>
        public NotificationMessageCollection Messages
        {
            get { return (NotificationMessageCollection)(GetValue(MessagesProperty)); }
            private set { SetValue(MessagesPropertyKey, value); }
        }

        NotificationMessageCollection Model.INotificationMessageHost<NotificationMessageCollection, NotificationMessageVM>.Messages { get { return Messages; } }

        Reserved.INotificationMessageViewModelCollection<Reserved.INotificationMessageViewModel> Reserved.INotificationMessageHostViewModel.Messages { get { return Messages; } }

        Model.INotificationMessageCollection<Model.INotificationMessage> Model.INotificationMessageHost.Messages { get { return Messages; } }

        #endregion

        #region AlertLevel Property Members

        /// <summary>
        /// Defines the name for the <see cref="AlertLevel"/> dependency property.
        /// </summary>
        public const string PropertyName_AlertLevel = "AlertLevel";

        private static readonly DependencyPropertyKey AlertLevelPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_AlertLevel, typeof(Model.AlertLevel), typeof(ValidatingViewModel),
            new PropertyMetadata(Model.AlertLevel.None));

        /// <summary>
        /// Identifies the <see cref="AlertLevel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlertLevelProperty = AlertLevelPropertyKey.DependencyProperty;

        /// <summary>
        /// Alert level for messages.
        /// </summary>
        public Model.AlertLevel AlertLevel
        {
            get { return (Model.AlertLevel)(GetValue(AlertLevelProperty)); }
            private set { SetValue(AlertLevelPropertyKey, value); }
        }

        #endregion

        #region TooltipMessage Property Members

        /// <summary>
        /// Defines the name for the <see cref="TooltipMessage"/> dependency property.
        /// </summary>
        public const string PropertyName_TooltipMessage = "TooltipMessage";

        private static readonly DependencyPropertyKey TooltipMessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_TooltipMessage, typeof(string), typeof(ValidatingViewModel),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="TooltipMessage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TooltipMessageProperty = TooltipMessagePropertyKey.DependencyProperty;

        /// <summary>
        /// Tooltip message relating to alert level.
        /// </summary>
        public string TooltipMessage
        {
            get { return GetValue(TooltipMessageProperty) as string; }
            private set { SetValue(TooltipMessagePropertyKey, value); }
        }

        #endregion

        string IDataErrorInfo.Error { get { return GetErrors(null).FirstOrDefault(); } }

        bool INotifyDataErrorInfo.HasErrors { get { return AlertLevel == Model.AlertLevel.Error || AlertLevel == Model.AlertLevel.Critical; } }

        string IDataErrorInfo.this[string columnName] { get { return GetErrors(columnName).FirstOrDefault(); } }

        public ValidatingViewModel()
        {
            Messages = new NotificationMessageCollection();
            Messages.CollectionChanged += Messages_CollectionChanged;
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Action action = () =>
            {
                if (Messages.Count == 0)
                {
                    TooltipMessage = "";
                    AlertLevel = Model.AlertLevel.None;
                    return;
                }

                var r = Messages.Cast<NotificationMessageVM>().Select(m => new { Message = m.Message, Level = m.Level }).Aggregate((a, m) => (a.Level > m.Level) ? a : m);
                TooltipMessage = r.Message;
                switch (r.Level)
                {
                    case Model.MessageLevel.Diagnostic:
                        AlertLevel = Model.AlertLevel.Diagnostic;
                        break;
                    case Model.MessageLevel.Verbose:
                        AlertLevel = Model.AlertLevel.Verbose;
                        break;
                    case Model.MessageLevel.Information:
                        AlertLevel = Model.AlertLevel.Information;
                        break;
                    case Model.MessageLevel.Alert:
                        AlertLevel = Model.AlertLevel.Alert;
                        break;
                    case Model.MessageLevel.Warning:
                        AlertLevel = Model.AlertLevel.Warning;
                        break;
                    case Model.MessageLevel.Error:
                        AlertLevel = Model.AlertLevel.Error;
                        break;
                    default:
                        AlertLevel = Model.AlertLevel.Critical;
                        break;
                }
            };

            if (Dispatcher.CheckAccess())
                action();
            else
                Dispatcher.Invoke(action);
        }

        protected void RaiseErrorsChanged(string propertyName)
        {
            DataErrorsChangedEventArgs args = new DataErrorsChangedEventArgs(propertyName);
            try { OnErrorsChanged(args); }
            finally { ErrorsChanged?.Invoke(this, args); }
        }
        
        /// <summary>
        /// This gets invoked when the validation errors have changed for a property or for the entire view model.
        /// </summary>
        /// <param name="args">Contains data for the <see cref="ErrorsChanged"/> event</param>
        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs args) { }

        public IEnumerable<string> GetErrors(string propertyName)
        {
            IEnumerable<PropertyMessageVM> result = Messages.OfType<PropertyMessageVM>().Where(m => m.Level >= Model.MessageLevel.Error);
            if (propertyName == null)
                result = result.Where(m => m.PropertyName == propertyName);
            return result.Select(m => m.Message);
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) { return GetErrors(propertyName); }
    }
}