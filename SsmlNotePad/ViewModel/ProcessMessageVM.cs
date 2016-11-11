using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Erwine.Leonard.T.SsmlNotePad.Model;
using Erwine.Leonard.T.SsmlNotePad.Reserved;
using System.Xml.Schema;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ProcessMessageVM : DependencyObject, Reserved.IProcessMessageViewModel<ProcessMessageVM, ReadOnlyObservableCollection<ProcessMessageVM>>
    {
        protected ProcessMessageVM(string message, MessageLevel level, Exception exception, DateTime created)
            : this(message, level, created)
        {
            if (exception == null)
                return;

            if (String.IsNullOrWhiteSpace(Message))
                message = exception.Message;

            IEnumerable<Exception> innerExceptions = (exception.InnerException == null) ? new Exception[0] : new Exception[] { exception.InnerException };
            if (exception is AggregateException)
                innerExceptions = innerExceptions.Concat((exception.InnerException as AggregateException).InnerExceptions.Where(i => !innerExceptions.Any(e => ReferenceEquals(i, e))));

            foreach (Exception e in innerExceptions)
                InnerMessages.Add(ProcessMessageVM.Create(exception, created));
        }

        protected ProcessMessageVM(string message, Exception exception, DateTime created) : this(message, MessageLevel.Critical, exception, created) { }

        protected ProcessMessageVM(MessageLevel level, Exception innerException, DateTime created) : this((innerException == null) ? null : innerException.Message, MessageLevel.Critical, innerException, created) { }

        protected ProcessMessageVM(Exception innerException, DateTime created) : this(MessageLevel.Critical, innerException, created) { }

        protected ProcessMessageVM(string message, MessageLevel level, Exception innerException) : this(message, level, innerException, DateTime.Now) { }

        protected ProcessMessageVM(string message, MessageLevel level, DateTime created)
        {
            Message = message;
            Level = level;
            Created = created;
        }

        protected ProcessMessageVM(string message, MessageLevel level) : this(message, level, DateTime.Now) { }
        
        protected ProcessMessageVM()
        {
            InnerMessages = new ProcessMessageCollection<ViewModel.ProcessMessageVM>();
        }

        private static ProcessMessageVM Create(Exception exception, DateTime created)
        {
            if (exception is XmlSchemaException)
                return new XmlValidationMessage(MessageLevel.Error, exception as XmlSchemaException, created);

            if (exception is XmlException)
                return new XmlValidationMessage(MessageLevel.Error, exception as XmlException, created);

            return new ProcessMessageVM(MessageLevel.Error, exception, created);
        }
        
        #region Created Property Members

        /// <summary>
        /// Defines the name for the <see cref="Created"/> dependency property.
        /// </summary>
        public const string PropertyName_Created = "Created";

        private static readonly DependencyPropertyKey CreatedPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Created, typeof(DateTime), typeof(ProcessMessageVM),
            new PropertyMetadata(default(DateTime)));

        /// <summary>
        /// Identifies the <see cref="Created"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CreatedProperty = CreatedPropertyKey.DependencyProperty;

        /// <summary>
        /// When message was created
        /// </summary>
        public DateTime Created
        {
            get { return (DateTime)(GetValue(CreatedProperty)); }
            private set { SetValue(CreatedPropertyKey, value); }
        }

        #endregion

        #region InnerMessages Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="InnerMessages"/> dependency property.
        /// </summary>
        public const string PropertyName_InnerMessages = "InnerMessages";

        private static readonly DependencyPropertyKey InnerMessagesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_InnerMessages, typeof(ProcessMessageCollection<ProcessMessageVM>), typeof(ProcessMessageVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="InnerMessages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerMessagesProperty = InnerMessagesPropertyKey.DependencyProperty;

        /// <summary>
        /// Inner messages
        /// </summary>
        public ProcessMessageCollection<ProcessMessageVM> InnerMessages
        {
            get { return (ProcessMessageCollection<ProcessMessageVM>)(GetValue(InnerMessagesProperty)); }
            private set { SetValue(InnerMessagesPropertyKey, value); }
        }

        #endregion

        #region Level Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Level"/> dependency property.
        /// </summary>
        public const string PropertyName_Level = "Level";

        private static readonly DependencyPropertyKey LevelPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Level, typeof(MessageLevel), typeof(ProcessMessageVM),
            new PropertyMetadata(MessageLevel.Information));

        /// <summary>
        /// Identifies the <see cref="Level"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LevelProperty = LevelPropertyKey.DependencyProperty;

        /// <summary>
        /// Message level
        /// </summary>
        public MessageLevel Level
        {
            get { return (MessageLevel)(GetValue(LevelProperty)); }
            protected set { SetValue(LevelPropertyKey, value); }
        }

        #endregion
        
        #region Message Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Message"/> dependency property.
        /// </summary>
        public const string PropertyName_Message = "Message";

        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Message, typeof(string), typeof(ProcessMessageVM),
            new PropertyMetadata("", null, (DependencyObject d, object baseValue) => baseValue as string ?? ""));

        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        /// <summary>
        /// Message text
        /// </summary>
        public string Message
        {
            get { return GetValue(MessageProperty) as string; }
            protected set { SetValue(MessagePropertyKey, value); }
        }
        
        #endregion

        IProcessMessageCollection<ReadOnlyObservableCollection<ProcessMessageVM>, ProcessMessageVM> IProcessMessage<ProcessMessageVM, ReadOnlyObservableCollection<ProcessMessageVM>>.InnerMessages { get { return InnerMessages; } }

        IProcessMessageViewModelCollection<ReadOnlyObservableCollection<ProcessMessageVM>, ProcessMessageVM> IProcessMessageViewModel<ProcessMessageVM, ReadOnlyObservableCollection<ProcessMessageVM>>.InnerMessages { get { return InnerMessages; } }
    }
}
