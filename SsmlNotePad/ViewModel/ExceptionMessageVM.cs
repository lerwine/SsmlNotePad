using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ExceptionMessageVM : ErrorMessageVM
    {
        #region ExceptionType Property Members

        /// <summary>
        /// Defines the name for the <see cref="ExceptionType"/> dependency property.
        /// </summary>
        public const string PropertyName_ExceptionType = "ExceptionType";
        
        private static readonly DependencyPropertyKey ExceptionTypePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ExceptionType, typeof(string), typeof(ExceptionMessageVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ExceptionType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExceptionTypeProperty = ExceptionTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Status ExceptionType for last file operation.
        /// </summary>
        public string ExceptionType
        {
            get { return GetValue(ExceptionTypeProperty) as string; }
            private set { SetValue(ExceptionTypePropertyKey, value); }
        }

        #endregion

        #region Source Property Members

        /// <summary>
        /// Defines the name for the <see cref="Source"/> dependency property.
        /// </summary>
        public const string PropertyName_Source = "Source";

        private static readonly DependencyPropertyKey SourcePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Source, typeof(string), typeof(ExceptionMessageVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = SourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Status Source for last file operation.
        /// </summary>
        public string Source
        {
            get { return GetValue(SourceProperty) as string; }
            private set { SetValue(SourcePropertyKey, value); }
        }

        #endregion

        #region StackTrace Property Members

        /// <summary>
        /// Defines the name for the <see cref="StackTrace"/> dependency property.
        /// </summary>
        public const string PropertyName_StackTrace = "StackTrace";

        private static readonly DependencyPropertyKey StackTracePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_StackTrace, typeof(string), typeof(ExceptionMessageVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="StackTrace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StackTraceProperty = StackTracePropertyKey.DependencyProperty;

        /// <summary>
        /// Status StackTrace for last file operation.
        /// </summary>
        public string StackTrace
        {
            get { return GetValue(StackTraceProperty) as string; }
            private set { SetValue(StackTracePropertyKey, value); }
        }

        #endregion

        #region TargetSite Property Members

        /// <summary>
        /// Defines the name for the <see cref="TargetSite"/> dependency property.
        /// </summary>
        public const string PropertyName_TargetSite = "TargetSite";

        private static readonly DependencyPropertyKey TargetSitePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_TargetSite, typeof(string), typeof(ExceptionMessageVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="TargetSite"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetSiteProperty = TargetSitePropertyKey.DependencyProperty;

        /// <summary>
        /// Status TargetSite for last file operation.
        /// </summary>
        public string TargetSite
        {
            get { return GetValue(TargetSiteProperty) as string; }
            private set { SetValue(TargetSitePropertyKey, value); }
        }

        #endregion

        public ExceptionMessageVM(AggregateException exception) : this(null, exception) { }

        public ExceptionMessageVM(Exception exception) : this(null, exception) { }

        public ExceptionMessageVM(string message, AggregateException exception) : this(message, exception, false) { }

        public ExceptionMessageVM(AggregateException exception, bool isWarning) : this(null, exception, isWarning) { }

        public ExceptionMessageVM(string message, AggregateException exception, bool isWarning)
            : base((String.IsNullOrEmpty(message)) ? ((exception == null) ? null : exception.Message) : message, isWarning)
        {
            if (exception == null)
                return;

            IEnumerable<Exception> innerExceptions = (exception.InnerExceptions == null) ? new Exception[0] : exception.InnerExceptions.Where(i => i != null);

            if (exception.InnerException != null && !innerExceptions.Any(i => ReferenceEquals(i, exception.InnerException)))
                innerExceptions = (new Exception[] { exception.InnerException }).Concat(innerExceptions);

            Initialize(exception, innerExceptions, isWarning);
        }

        public ExceptionMessageVM(string message, Exception exception) : this(message, exception, false) { }

        public ExceptionMessageVM(Exception exception, bool isWarning) : this(null, exception, isWarning) { }

        public ExceptionMessageVM(string message, Exception exception, bool isWarning)
            : base((String.IsNullOrEmpty(message)) ? ((exception == null) ? null : exception.Message) : message, isWarning)
        {
            if (exception != null)
                Initialize(exception, (exception.InnerException == null) ? new Exception[0] : new Exception[] { exception.InnerException }, isWarning);
        }

        public void UpdateFrom(string message, Exception exception, bool isWarning)
        {
            UpdateFrom((String.IsNullOrEmpty(message)) ? ((exception == null) ? null : exception.Message) : message, isWarning);
            InnerInnerErrors.Clear();
            if (exception != null)
                Initialize(exception, (exception.InnerException == null) ? new Exception[0] : new Exception[] { exception.InnerException }, isWarning);
        }

        private void Initialize(Exception exception, IEnumerable<Exception> innerExceptions, bool isWarning)
        {
            ExceptionType = exception.GetType().FullName;
            try { Source = exception.Source; } catch { }
            try { StackTrace = exception.StackTrace; } catch { }
            try { TargetSite = exception.TargetSite.ToString(); } catch { }

            foreach (Exception exc in innerExceptions)
            {
                if (exc is AggregateException)
                    InnerInnerErrors.Add(new ExceptionMessageVM(exc as AggregateException, isWarning));
                else if (exc is XmlException)
                    InnerInnerErrors.Add(new XmlValidationMessageVM(exc as XmlException, isWarning));
                else if (exc is XmlSchemaException)
                    InnerInnerErrors.Add(new XmlValidationMessageVM(exc as XmlSchemaException, isWarning));
                else
                    InnerInnerErrors.Add(new ExceptionMessageVM(exc, isWarning));
            }
        }
    }
}