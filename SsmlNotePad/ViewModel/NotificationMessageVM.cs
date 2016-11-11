using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Erwine.Leonard.T.SsmlNotePad.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    /// <summary>
    /// Represents a notification message.
    /// </summary>
    [DataObject]
    public class NotificationMessageVM : DependencyObject
    {
        #region ID Property Members

        /// <summary>
        /// Defines the name for the <see cref="ID"/> dependency property.
        /// </summary>
        public const string PropertyName_ID = "ID";

        private static readonly DependencyPropertyKey IDPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ID, typeof(Guid), typeof(NotificationMessageVM),
            new PropertyMetadata(Guid.Empty));

        /// <summary>
        /// Identifies the <see cref="ID"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IDProperty = IDPropertyKey.DependencyProperty;

        /// <summary>
        /// Unique identifier for notification message.
        /// </summary>
        [DataObjectField(true, true, false)]
        public Guid ID
        {
            get { return (Guid)(GetValue(IDProperty)); }
            private set { SetValue(IDPropertyKey, value); }
        }

        #endregion

        #region InnerMessages Property Members

        /// <summary>
        /// Defines the name for the <see cref="InnerMessages"/> dependency property.
        /// </summary>
        public const string PropertyName_InnerMessages = "InnerMessages";

        private static readonly DependencyPropertyKey InnerMessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_InnerMessages, typeof(NotificationMessageCollection), typeof(NotificationMessageVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="InnerMessages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerMessagesProperty = InnerMessagesPropertyKey.DependencyProperty;

        /// <summary>
        /// Inner messages.
        /// </summary>
        public NotificationMessageCollection InnerMessages
        {
            get { return (NotificationMessageCollection)(GetValue(InnerMessagesProperty)); }
            private set { SetValue(InnerMessagesPropertyKey, value); }
        }

        #endregion

        #region Level Property Members

        /// <summary>
        /// Defines the name for the <see cref="Level"/> dependency property.
        /// </summary>
        public const string PropertyName_Level = "Level";

        private static readonly DependencyPropertyKey LevelPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Level, typeof(Model.MessageLevel), typeof(NotificationMessageVM),
            new PropertyMetadata(Model.MessageLevel.Verbose));

        /// <summary>
        /// Identifies the <see cref="Level"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LevelProperty = LevelPropertyKey.DependencyProperty;

        /// <summary>
        /// Message severity
        /// </summary>
        [DataObjectField(false, false, false)]
        public MessageLevel Level
        {
            get { return (Model.MessageLevel)(GetValue(LevelProperty)); }
            private set { SetValue(LevelPropertyKey, value); }
        }

        #endregion

        #region Message Property Members

        /// <summary>
        /// Defines the name for the <see cref="Message"/> dependency property.
        /// </summary>
        public const string PropertyName_Message = "Message";

        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Message, typeof(string), typeof(NotificationMessageVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        /// <summary>
        /// Message text.
        /// </summary>
        [DataObjectField(false, false, false)]
        public string Message
        {
            get { return GetValue(MessageProperty) as string; }
            private set { SetValue(MessagePropertyKey, value); }
        }

        #endregion

        #region Time Property Members

        /// <summary>
        /// Defines the name for the <see cref="Time"/> dependency property.
        /// </summary>
        public const string PropertyName_Time = "Time";

        private static readonly DependencyPropertyKey TimePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Time, typeof(DateTime), typeof(NotificationMessageVM),
            new PropertyMetadata(default(DateTime)));

        /// <summary>
        /// Identifies the <see cref="Time"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeProperty = TimePropertyKey.DependencyProperty;

        /// <summary>
        /// Time when message was generated.
        /// </summary>
        [DataObjectField(false, false, false)]
        public DateTime Time
        {
            get { return (DateTime)(GetValue(TimeProperty)); }
            private set { SetValue(TimePropertyKey, value); }
        }

        #endregion

        #region Data Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Data"/> dependency property.
        /// </summary>
        public const string PropertyName_Data = "Data";

        private static readonly DependencyPropertyKey DataPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Data,
            typeof(ReadOnlyObservableCollection<PropertyValueVM>), typeof(NotificationMessageVM), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Data"/> read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

        private ObservableCollection<PropertyValueVM> _innerData = new ObservableCollection<PropertyValueVM>();

        /// <summary>
        /// Inner collection for <see cref="Data"/>.
        /// </summary>
        protected ObservableCollection<PropertyValueVM> InnerData { get { return _innerData; } }

        /// <summary>
        /// Properties associated with message.
        /// </summary>
        public ReadOnlyObservableCollection<PropertyValueVM> Data
        {
            get { return (ReadOnlyObservableCollection<PropertyValueVM>)(GetValue(DataProperty)); }
            private set { SetValue(DataPropertyKey, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="data">Properties of message.</param>
        /// <param name="innerMessages">Nested messages.</param>
        /// <param name="time">Time when associated event occurred.</param>
        /// <param name="id">Unique identifier for data binding.</param>
        public NotificationMessageVM(string message, MessageLevel level, IEnumerable<PropertyValueVM> data, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, Guid id)
        {
            Message = (String.IsNullOrWhiteSpace(message)) ? level.ToString("F") : message;
            Level = level;
            Data = new ReadOnlyObservableCollection<PropertyValueVM>(new ObservableCollection<PropertyValueVM>((data ?? new PropertyValueVM[0]).Where(i => i != null)));
            InnerMessages = new NotificationMessageCollection(innerMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="data">Properties of message.</param>
        /// <param name="innerMessages">Nested messages.</param>
        /// <param name="time">Time when associated event occurred.</param>
        private NotificationMessageVM(string message, MessageLevel level, IEnumerable<PropertyValueVM> data, IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
            : this(message, level, data, innerMessages, time, Guid.NewGuid()) { }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="data">Properties of message.</param>
        /// <param name="time">Time when associated event occurred.</param>
        /// <param name="innerMessage">First nested inner message.</param>
        /// <param name="additionalInnerMessages">Additional nested inner messages.</param>
        public NotificationMessageVM(string message, MessageLevel level, IEnumerable<PropertyValueVM> data, DateTime time, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalInnerMessages)
            : this(message, level, data, ((innerMessage == null) ? new NotificationMessageVM[0] : new NotificationMessageVM[] { innerMessage })
                  .Concat((additionalInnerMessages ?? new NotificationMessageVM[0]).AsEnumerable()), time) { }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="innerMessages">Nested messages.</param>
        /// <param name="time">Time when associated event occurred.</param>
        /// <param name="data">Properties of message.</param>
        public NotificationMessageVM(string message, MessageLevel level, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, params PropertyValueVM[] data)
            : this(message, level, (data ?? new PropertyValueVM[0]).AsEnumerable(), innerMessages, time) { }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="data">Properties of message.</param>
        /// <param name="innerMessage">First nested inner message.</param>
        /// <param name="additionalInnerMessages">Additional nested inner messages.</param>
        public NotificationMessageVM(string message, MessageLevel level, IEnumerable<PropertyValueVM> data, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalInnerMessages)
            : this(message, level, data, DateTime.Now, innerMessage, additionalInnerMessages) { }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="innerMessages">Nested messages.</param>
        /// <param name="data">Properties of message.</param>
        public NotificationMessageVM(string message, MessageLevel level, IEnumerable<NotificationMessageVM> innerMessages, params PropertyValueVM[] data)
            : this(message, level, innerMessages, DateTime.Now, data) { }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="time">Time when associated event occurred.</param>
        /// <param name="innerMessage">First nested inner message.</param>
        /// <param name="additionalInnerMessages">Additional nested inner messages.</param>
        public NotificationMessageVM(string message, MessageLevel level, DateTime time, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalInnerMessages)
            : this(message, level, null, time, innerMessage, additionalInnerMessages) { }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="time">Time when associated event occurred.</param>
        /// <param name="data">Properties of message.</param>
        public NotificationMessageVM(string message, MessageLevel level, DateTime time, params PropertyValueVM[] data)
            : this(message, level, null, time, data) { }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="innerMessage">First nested inner message.</param>
        /// <param name="additionalInnerMessages">Additional nested inner messages.</param>
        public NotificationMessageVM(string message, MessageLevel level, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalInnerMessages)
            : this(message, level, DateTime.Now, innerMessage, additionalInnerMessages) { }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="level">Level representing severity of message.</param>
        /// <param name="data">Properties of message.</param>
        public NotificationMessageVM(string message, MessageLevel level, params PropertyValueVM[] data) : this(message, level, DateTime.Now, data) { }

        /// <summary>
        /// Create empty <see cref="NotificationMessageVM"/> object.
        /// </summary>
        public NotificationMessageVM() : this("", MessageLevel.Diagnostic) { }

        #endregion

        #region FromException overrides

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, PropertyDescriptorCollection properties, 
            IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, properties, CoerceInnerMessages(exception, time, innerMessages), time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, PropertyDescriptorCollection properties,
            IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromException(message, level, exception, properties, innerMessages, DateTime.Now);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, PropertyDescriptorCollection properties, DateTime time, 
            params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, properties, CoerceInnerMessages(exception, time, innerMessage), time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, PropertyDescriptorCollection properties,
            params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, level, exception, properties, DateTime.Now, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<PropertyDescriptor> properties, 
            IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, properties, CoerceInnerMessages(exception, time, innerMessages), time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<PropertyDescriptor> properties,
            IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromException(message, level, exception, properties, innerMessages, DateTime.Now);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<PropertyDescriptor> properties, DateTime time,
            params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, properties, CoerceInnerMessages(exception, time, innerMessage), time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<PropertyDescriptor> properties, 
            params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, level, exception, properties, DateTime.Now, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<string> propertyNames,
            IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, propertyNames, CoerceInnerMessages(exception, time, innerMessages), time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<string> propertyNames, DateTime time,
            params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, propertyNames, CoerceInnerMessages(exception, time, innerMessage), time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<string> propertyNames,
            params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, level, exception, propertyNames, DateTime.Now, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, DateTime time,
            PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, CoerceInnerMessages(exception, time, innerMessages), time, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, 
            params string[] propertyName)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, CoerceInnerMessages(exception, time, innerMessages), time, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<NotificationMessageVM> innerMessages,
            PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(message, level, exception, innerMessages, DateTime.Now, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, IEnumerable<NotificationMessageVM> innerMessages,
            params string[] propertyName)
        {
            return FromComponent(message, level, exception, innerMessages, DateTime.Now, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, DateTime time, NotificationMessageVM innerMessage,
            params NotificationMessageVM[] additionalMessages)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, CoerceInnerMessages(exception, time, innerMessage, additionalMessages), time);
        }

        /// <summary>
        /// 
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, DateTime time, PropertyDescriptor property,
            params PropertyDescriptor[] additionalProperties)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, CoerceInnerMessages(exception, time, null), property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, DateTime time, params string[] propertyName)
        {
            return FromComponent(CoerceMessage(message, exception), level, exception, CoerceInnerMessages(exception, time, null), propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, NotificationMessageVM innerMessage,
            params NotificationMessageVM[] additionalMessages)
        {
            return FromException(message, level, exception, DateTime.Now, innerMessage, additionalMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, PropertyDescriptor property,
            params PropertyDescriptor[] additionalProperties)
        {
            return FromException(message, level, exception, DateTime.Now, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, MessageLevel level, Exception exception, params string[] propertyName)
        {
            return FromException(message, level, exception, DateTime.Now, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, PropertyDescriptorCollection properties, IEnumerable<NotificationMessageVM> innerMessages, 
            DateTime time)
        {
            return FromException(message, CoerceLevel(exception), exception, properties, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, PropertyDescriptorCollection properties, IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromException(message, CoerceLevel(exception), exception, properties, innerMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, PropertyDescriptorCollection properties, DateTime time,
            params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, CoerceLevel(exception), exception, properties, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, PropertyDescriptorCollection properties, params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, CoerceLevel(exception), exception, properties, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<PropertyDescriptor> properties, IEnumerable<NotificationMessageVM> innerMessages,
            DateTime time)
        {
            return FromException(message, CoerceLevel(exception), exception, properties, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<PropertyDescriptor> properties, IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromException(message, CoerceLevel(exception), exception, properties, innerMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <param name="innerMessage">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<PropertyDescriptor> properties, DateTime time, 
            params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, CoerceLevel(exception), exception, properties, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<PropertyDescriptor> properties, params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, CoerceLevel(exception), exception, properties, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<string> propertyNames, IEnumerable<NotificationMessageVM> innerMessages,
            DateTime time)
        {
            return FromException(message, CoerceLevel(exception), exception, propertyNames, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<string> propertyNames, DateTime time, params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, CoerceLevel(exception), exception, propertyNames, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<string> propertyNames, params NotificationMessageVM[] innerMessage)
        {
            return FromException(message, CoerceLevel(exception), exception, propertyNames, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, 
            PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(message, CoerceLevel(exception), exception, innerMessages, time, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, params string[] propertyName)
        {
            return FromException(message, CoerceLevel(exception), exception, innerMessages, time, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, PropertyDescriptor property, 
            params PropertyDescriptor[] additionalProperties)
        {
            return FromException(message, CoerceLevel(exception), exception, innerMessages, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, params string[] propertyName)
        {
            return FromException(message, CoerceLevel(exception), exception, innerMessages, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, DateTime time, NotificationMessageVM innerMessage,
            params NotificationMessageVM[] additionalMessages)
        {
            return FromException(message, CoerceLevel(exception), exception, time, innerMessage, additionalMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, DateTime time, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(message, CoerceLevel(exception), exception, time, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, DateTime time, params string[] propertyName)
        {
            return FromException(message, CoerceLevel(exception), exception, time, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalMessages)
        {
            return FromException(message, CoerceLevel(exception), exception, innerMessage, additionalMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(message, CoerceLevel(exception), exception, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="message">Message to associate with exception event</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(string message, Exception exception, params string[] propertyName)
        {
            return FromException(message, CoerceLevel(exception), exception, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, PropertyDescriptorCollection properties, IEnumerable<NotificationMessageVM> innerMessages, 
            DateTime time)
        {
            return FromException(null, level, exception, properties, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, PropertyDescriptorCollection properties, IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromException(null, level, exception, properties, innerMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, PropertyDescriptorCollection properties, DateTime time, 
            params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, level, exception, properties, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, PropertyDescriptorCollection properties, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, level, exception, properties, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<PropertyDescriptor> properties, 
            IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromException(null, level, exception, properties, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<PropertyDescriptor> properties,
            IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromException(null, level, exception, properties, innerMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<PropertyDescriptor> properties, DateTime time, 
            params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, level, exception, properties, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<PropertyDescriptor> properties, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, level, exception, properties, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<string> propertyNames, IEnumerable<NotificationMessageVM> innerMessages,
            DateTime time)
        {
            return FromException(null, level, exception, propertyNames, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<string> propertyNames, DateTime time,
            params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, level, exception, propertyNames, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<string> propertyNames, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, level, exception, propertyNames, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, DateTime time,
            PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(null, level, exception, innerMessages, time, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, DateTime time,
            params string[] propertyName)
        {
            return FromException(null, level, exception, innerMessages, time, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, 
            PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(null, level, exception, innerMessages, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, IEnumerable<NotificationMessageVM> innerMessages, params string[] propertyName)
        {
            return FromException(null, level, exception, innerMessages, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, DateTime time, NotificationMessageVM innerMessage,
            params NotificationMessageVM[] additionalMessages)
        {
            return FromException(null, level, exception, time, innerMessage, additionalMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, DateTime time, PropertyDescriptor property,
            params PropertyDescriptor[] additionalProperties)
        {
            return FromException(null, level, exception, time, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, DateTime time, params string[] propertyName)
        {
            return FromException(null, level, exception, time, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalMessages)
        {
            return FromException(null, level, exception, innerMessage, additionalMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(null, level, exception, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="level">Severity of exception event.</param>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(MessageLevel level, Exception exception, params string[] propertyName)
        {
            return FromException(null, level, exception, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, PropertyDescriptorCollection properties, IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromException(null, exception, properties, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, PropertyDescriptorCollection properties, IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromException(null, exception, properties, innerMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, PropertyDescriptorCollection properties, DateTime time, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, exception, properties, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, PropertyDescriptorCollection properties, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, exception, properties, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<PropertyDescriptor> properties, IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromException(null, exception, properties, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<PropertyDescriptor> properties, IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromException(null, exception, properties, innerMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<PropertyDescriptor> properties, DateTime time, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, exception, properties, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<PropertyDescriptor> properties, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, exception, properties, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <param name="time">Time when event occurred.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<string> propertyNames, IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromException(null, exception, propertyNames, innerMessages, time);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<string> propertyNames, DateTime time, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, exception, propertyNames, time, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyNames">Names of property values to include.</param>
        /// <param name="innerMessage">Nested inner message.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<string> propertyNames, params NotificationMessageVM[] innerMessage)
        {
            return FromException(null, exception, propertyNames, innerMessage);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, PropertyDescriptor property, 
            params PropertyDescriptor[] additionalProperties)
        {
            return FromException(null, exception, innerMessages, time, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, params string[] propertyName)
        {
            return FromException(null, exception, innerMessages, time, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<NotificationMessageVM> innerMessages, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(null, exception, innerMessages, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages"></param>
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, IEnumerable<NotificationMessageVM> innerMessages, params string[] propertyName)
        {
            return FromException(null, exception, innerMessages, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, DateTime time, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalMessages)
        {
            return FromException(null, exception, time, innerMessage, additionalMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, DateTime time, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(null, exception, time, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, DateTime time, params string[] propertyName)
        {
            return FromException(null, exception, time, propertyName);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalMessages)
        {
            return FromException(null, exception, innerMessage, additionalMessages);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromException(null, exception, property, additionalProperties);
        }

        /// <summary>
        /// Initialize new <see cref="NotificationMessageVM"/> object from an <seealso cref="Exception"/> object.
        /// </summary>
        /// <param name="exception"><seealso cref="Exception"/> that caused the event.</param>
        /// <param name="propertyName">Names of property values to include.</param>
        /// <returns>A new <see cref="NotificationMessageVM"/> object representing the source <seealso cref="Exception"/> object.</returns>
        public static NotificationMessageVM FromException(Exception exception, params string[] propertyName)
        {
            return FromException(null, exception, propertyName);
        }

        #endregion

        #region FromComponent overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, PropertyDescriptorCollection properties, IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromComponent(message, level, component, (properties == null) ? null : properties.OfType<PropertyDescriptor>(), innerMessages, time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, PropertyDescriptorCollection properties, IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromComponent(message, level, component, properties, innerMessages, DateTime.Now);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, PropertyDescriptorCollection properties, DateTime time, params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(message, level, component, (properties == null) ? null : properties.OfType<PropertyDescriptor>(), time, innerMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, PropertyDescriptorCollection properties, params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(message, level, component, properties, DateTime.Now, innerMessage);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<PropertyDescriptor> properties, IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<PropertyDescriptor> properties, IEnumerable<NotificationMessageVM> innerMessages)
        {
            return FromComponent(message, level, component, properties, innerMessages, DateTime.Now);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<PropertyDescriptor> properties, DateTime time, params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(message, level, component, properties, (innerMessage == null) ? null : innerMessage.AsEnumerable(), DateTime.Now);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        //// <param name="properties">Property values to include with the message.</param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<PropertyDescriptor> properties, params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(message, level, component, properties, DateTime.Now, innerMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="propertyNames"></param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<string> propertyNames, IEnumerable<NotificationMessageVM> innerMessages, DateTime time)
        {
            return FromComponent(message, level, component, innerMessages, time, propertyNames.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="propertyNames"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<string> propertyNames, DateTime time, params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(message, level, component, propertyNames, (innerMessage == null) ? null : innerMessage.AsEnumerable(), time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="propertyNames"></param>
        /// <param name="innerMessages">Nested inner messages.</param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<string> propertyNames, params NotificationMessageVM[] innerMessage)
        {
            return FromComponent(message, level, component, propertyNames, DateTime.Now, innerMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            if (property == null)
                return FromComponent(message, level, component, additionalProperties, innerMessages, time);

            if (innerMessages == null)
                return FromComponent(message, level, component, new PropertyDescriptor[] { property }, innerMessages, time);

            return FromComponent(message, level, component, (new PropertyDescriptor[] { property }).Concat(additionalProperties), innerMessages, time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="innerMessages"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<NotificationMessageVM> innerMessages, DateTime time, params string[] propertyName)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);

            if (propertyName == null || propertyName.Length == 0)
                return FromComponent(message, level, component, properties, innerMessages, time);

            return FromComponent(message, level, component, propertyName.Where(n => !String.IsNullOrEmpty(n)).Distinct().Select(n => properties.Find(n, true)), innerMessages, time);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="innerMessages"></param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<NotificationMessageVM> innerMessages, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromComponent(message, level, component, innerMessages, DateTime.Now, property, additionalProperties);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="innerMessages"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, IEnumerable<NotificationMessageVM> innerMessages, params string[] propertyName)
        {
            return FromComponent(message, level, component, innerMessages, DateTime.Now, propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, DateTime time, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalMessages)
        {
            if (innerMessage == null)
                return FromComponent(message, level, component, null as IEnumerable<PropertyDescriptor>, time, additionalMessages);

            if (additionalMessages == null)
                return FromComponent(message, level, component, null as IEnumerable<PropertyDescriptor>, time, new NotificationMessageVM[] { innerMessage });

            return FromComponent(message, level, component, null as IEnumerable<PropertyDescriptor>, time, (new NotificationMessageVM[] { innerMessage }).Concat(additionalMessages).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, DateTime time, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromComponent(message, level, component, null, time, property, additionalProperties);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="time">Time when event occurred.</param
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, DateTime time, params string[] propertyName)
        {
            return FromComponent(message, level, component, null, time, propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="innerMessages">First nested inner message.</param>
        /// <param name="additionalMessages">Additional nested messages.</param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, NotificationMessageVM innerMessage, params NotificationMessageVM[] additionalMessages)
        {
            return FromComponent(message, level, component, DateTime.Now, innerMessage, additionalMessages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="property"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, PropertyDescriptor property, params PropertyDescriptor[] additionalProperties)
        {
            return FromComponent(message, level, component, null, property, additionalProperties);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="component"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static NotificationMessageVM FromComponent(string message, MessageLevel level, object component, params string[] propertyName)
        {
            return FromComponent(message, level, component, null, propertyName);
        }

        #endregion

        private static string CoerceMessage(string message, Exception exception)
        {
            if (!String.IsNullOrWhiteSpace(message))
                return message;

            if (exception == null)
                return "Unexpected Error";

            if (String.IsNullOrWhiteSpace(exception.Message))
                return TypeDescriptor.GetClassName(exception);

            return exception.Message;
        }

        private static MessageLevel CoerceLevel(Exception exception)
        {
            if (exception == null)
                return MessageLevel.Error;

            return (exception is WarningException) ? MessageLevel.Warning : MessageLevel.Critical;
        }

        private static IEnumerable<NotificationMessageVM> CoerceInnerMessages(Exception exception, DateTime time, IEnumerable<NotificationMessageVM> innerMessages)
        {
            if (exception == null)
                return innerMessages;

            IEnumerable<Exception> allExceptions = (exception.InnerException == null) ? new Exception[0] : new Exception[] { exception.InnerException };
            if (exception is AggregateException)
                allExceptions = allExceptions.Concat((exception as AggregateException).InnerExceptions.Where(i => i != null && !allExceptions.Any(e => ReferenceEquals(i, e))));

            var m = allExceptions.Select(e => FromException(e, time));

            if (innerMessages == null)
                return m;

            return innerMessages.Concat(m);
        }

        private static IEnumerable<NotificationMessageVM> CoerceInnerMessages(Exception exception, DateTime time, NotificationMessageVM innerMessage, NotificationMessageVM[] additionalMessages)
        {
            if (innerMessage == null)
                return CoerceInnerMessages(exception, time, additionalMessages);

            if (additionalMessages == null)
                return CoerceInnerMessages(exception, time, new NotificationMessageVM[] { innerMessage });

            return CoerceInnerMessages(exception, time, (new NotificationMessageVM[] { innerMessage }).Concat(additionalMessages));
        }
    }
}