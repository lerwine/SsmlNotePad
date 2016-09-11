using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class SpeechMessageVM : DependencyObject
    {
        #region EventName Property Members

        public const string PropertyName_EventName = "EventName";

        private static readonly DependencyPropertyKey EventNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_EventName, typeof(string), typeof(SpeechMessageVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="EventName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = EventNamePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string EventName
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(EventNameProperty));
                return Dispatcher.Invoke(() => EventName);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(EventNamePropertyKey, value);
                else
                    Dispatcher.Invoke(() => EventName = value);
            }
        }

        #endregion

        #region Message Property Members

        public const string PropertyName_Message = "Message";

        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Message, typeof(string), typeof(SpeechMessageVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(MessageProperty));
                return Dispatcher.Invoke(() => Message);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(MessagePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Message = value);
            }
        }

        #endregion

        #region Details Property Members

        private ObservableCollection<string> _details = new ObservableCollection<string>();

        public const string PropertyName_Details = "Details";

        private static readonly DependencyPropertyKey DetailsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Details, typeof(ReadOnlyObservableCollection<string>), typeof(SpeechMessageVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Details"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsProperty = DetailsPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<string> Details
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<string>)(GetValue(DetailsProperty));
                return Dispatcher.Invoke(() => Details);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DetailsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Details = value);
            }
        }

        #endregion

        #region InnerMessages Property Members

        private ObservableCollection<SpeechMessageVM> _innerMessages = new ObservableCollection<SpeechMessageVM>();

        public const string PropertyName_InnerMessages = "InnerMessages";

        private static readonly DependencyPropertyKey InnerMessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_InnerMessages, typeof(ReadOnlyObservableCollection<SpeechMessageVM>), typeof(SpeechMessageVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="InnerMessages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerMessagesProperty = InnerMessagesPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<SpeechMessageVM> InnerMessages
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<SpeechMessageVM>)(GetValue(InnerMessagesProperty));
                return Dispatcher.Invoke(() => InnerMessages);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(InnerMessagesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => InnerMessages = value);
            }
        }

        #endregion

        #region Severity Property Members

        public const string DependencyPropertyName_Severity = "Severity";

        /// <summary>
        /// Identifies the <seealso cref="Severity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(DependencyPropertyName_Severity, typeof(MessageSeverity), typeof(SpeechMessageVM),
                new PropertyMetadata(MessageSeverity.Information,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as SpeechMessageVM).Severity_PropertyChanged((MessageSeverity)(e.OldValue), (MessageSeverity)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as SpeechMessageVM).Severity_PropertyChanged((MessageSeverity)(e.OldValue), (MessageSeverity)(e.NewValue)));
                }));

        /// <summary>
        /// 
        /// </summary>
        public MessageSeverity Severity
        {
            get
            {
                if (CheckAccess())
                    return (MessageSeverity)(GetValue(SeverityProperty));
                return Dispatcher.Invoke(() => Severity);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SeverityProperty, value);
                else
                    Dispatcher.Invoke(() => Severity = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Severity"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="MessageSeverity"/> value before the <seealso cref="Severity"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="MessageSeverity"/> value after the <seealso cref="Severity"/> property was changed.</param>
        protected virtual void Severity_PropertyChanged(MessageSeverity oldValue, MessageSeverity newValue) { }
        
        #endregion

        public SpeechMessageVM() { Details = new ReadOnlyObservableCollection<string>(_details); }

        internal static SpeechMessageVM Create(Exception exception, MessageSeverity severity = MessageSeverity.Error)
        {
            if (exception == null)
            {
                switch (severity)
                {
                    case MessageSeverity.Critical:
                        return Create("Critical Error", "A critical error has occurred.", MessageSeverity.Critical);
                    case MessageSeverity.Error:
                        return Create("Error", "An error has occurred.", MessageSeverity.Error);
                    case MessageSeverity.Warning:
                        return Create("Warning", "A warning error has occurred.", MessageSeverity.Warning);
                    default:
                        return Create("Unknown", "", severity);
                }
            }

            string[] details;
            try { details = exception.ToString().SplitLines().ToArray(); } catch { details = new string[0]; }
            string message;
            string eventName = (severity == MessageSeverity.Critical) ? "Critical Error" : severity.ToString("F");
            if (exception is AggregateException)
            {
                AggregateException aggregateException = exception as AggregateException;
                IEnumerable<Exception> allExceptions = (aggregateException.InnerExceptions == null) ? new Exception[0] : aggregateException.InnerExceptions.Where(e => e != null);
                if (aggregateException.InnerException != null && !allExceptions.Any(e => ReferenceEquals(e, aggregateException.InnerException)))
                    allExceptions = allExceptions.Concat(new Exception[] { aggregateException.InnerException });
                if (String.IsNullOrWhiteSpace(exception.Message))
                {
                    switch (severity)
                    {
                        case MessageSeverity.Critical:
                            message = "Critical errors have occurred.";
                            break;
                        case MessageSeverity.Warning:
                            message = "Warnings have occurred.";
                            break;
                        default:
                            message = "Errors have occurred.";
                            break;
                    }
                }
                else
                    message = exception.Message;
                return Create(eventName, message, severity, details, allExceptions.Select(e => Create(e)).ToArray());
            }
            

            if (exception.InnerException == null)
                return Create(eventName, exception.Message, severity, details);

            return Create(eventName, exception.Message, severity, details, Create(exception.InnerException));
        }

        internal static SpeechMessageVM Create(string eventName, string message, MessageSeverity severity = MessageSeverity.Information, params SpeechMessageVM[] innerMessages)
        {
            return Create(eventName, message, severity, new string[0], innerMessages);
        }

        internal static SpeechMessageVM Create(string eventName, string message, MessageSeverity severity, IEnumerable<string> eventDetail, params SpeechMessageVM[] innerMessages)
        {
            SpeechMessageVM result = new SpeechMessageVM();
            result.EventName = eventName;
            result.Message = message;
            if (eventDetail != null)
            {
                foreach (string s in eventDetail.Where(s => !String.IsNullOrWhiteSpace(s)))
                    result._details.Add(s);
            }

            if (innerMessages == null)
                return result;

            foreach (SpeechMessageVM i in innerMessages.Where(m => m != null))
                result._innerMessages.Add(i);

            return result;
        }
    }
}
