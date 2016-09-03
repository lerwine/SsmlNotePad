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

        public SpeechMessageVM() { Details = new ReadOnlyObservableCollection<string>(_details); }

        internal static SpeechMessageVM Create(Exception exception, bool isCritical)
        {
            if (exception == null)
                return Create((isCritical) ? "Critical Error" : "Error", (isCritical) ? "An critical error has occurred." : "An error has occurred.");

            string[] details;
            try { details = exception.ToString().SplitLines().ToArray(); } catch { details = new string[0]; }
            string message;
            if (exception is AggregateException)
            {
                AggregateException aggregateException = exception as AggregateException;
                IEnumerable<Exception> allExceptions = (aggregateException.InnerExceptions == null) ? new Exception[0] : aggregateException.InnerExceptions.Where(e => e != null);
                if (aggregateException.InnerException != null && !allExceptions.Any(e => ReferenceEquals(e, aggregateException.InnerException)))
                    allExceptions = allExceptions.Concat(new Exception[] { aggregateException.InnerException });
                message = (String.IsNullOrWhiteSpace(exception.Message)) ? ((isCritical) ? "Critical errors have occurred." : "Error have occurred.") : exception.Message;
                return Create((isCritical) ? "Critical Error" : "Error", message, details, allExceptions.Select(e => Create(e, false)).ToArray());
            }

            message = (String.IsNullOrWhiteSpace(exception.Message)) ? ((isCritical) ? "An critical error has occurred." : "An error has occurred.") : exception.Message;

            if (exception.InnerException == null)
                return Create((isCritical) ? "Critical Error" : "Error", message, details);

            return Create((isCritical) ? "Critical Error" : "Error", message, details, Create(exception.InnerException, false));
        }

        internal static SpeechMessageVM Create(string eventName, string message, params SpeechMessageVM[] innerMessages)
        {
            return Create(eventName, message, new string[0], innerMessages);
        }

        internal static SpeechMessageVM Create(string eventName, string message, IEnumerable<string> eventDetail, params SpeechMessageVM[] innerMessages)
        {
            SpeechMessageVM result = new ViewModel.SpeechMessageVM();
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
