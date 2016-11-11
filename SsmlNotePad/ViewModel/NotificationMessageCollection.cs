using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class NotificationMessageCollection : IList<NotificationMessageVM>, INotifyCollectionChanged, INotifyPropertyChanged, IRaiseItemChangedEvents, IBindingList
    {
        private object _innerSyncRoot = new object();
        private static Dispatcher _dispatcher;
        private LevelMessageCollection<NotificationMessageVM> _allItems;
        private LevelMessageCollection<PropertyMessageVM> _propertyMessages;
        private LevelMessageCollection<LineMessageVM> _lineMessages;
        private LevelMessageCollection<NotificationMessageVM> _otherMessages;
        private ListSortDescriptionCollection _currentSort = new ListSortDescriptionCollection();

        public class LevelMessageCollection<T> : ReadOnlyObservableCollection<T>
            where T : NotificationMessageVM
        {
            private ObservableCollection<T> _allItems;
            private ObservableCollection<T> _verboseItems;
            private ObservableCollection<T> _informationalItems;
            private ObservableCollection<T> _alertItems;
            private ObservableCollection<T> _warningItems;
            private ObservableCollection<T> errorItems;

            private LevelMessageCollection(ObservableCollection<T> allItems) : base(allItems) { _allItems = allItems; }

            public LevelMessageCollection() : this(new ObservableCollection<T>()) { }

            public LevelMessageCollection(IEnumerable<T> messages) : this(new ObservableCollection<T>(messages)) { }
        }

        private static ReadOnlyDictionary<string, PropertyDescriptor> _supportedProperties;

        /// <summary>
        /// Dictionary of property supported by <see cref="ApplySort(PropertyDescriptor, ListSortDirection)"/>.
        /// </summary>
        public static ReadOnlyDictionary<string, PropertyDescriptor> SupportedProperties { get { return _supportedProperties; } }

        /// <summary>
        /// Dispatcher object for the thread which created the current <see cref="NotificationMessageCollection"/>.
        /// </summary>
        public Dispatcher Dispatcher { get { return _dispatcher; } }

        static NotificationMessageCollection()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _supportedProperties = new ReadOnlyDictionary<string, PropertyDescriptor>(TypeDescriptor.GetProperties(typeof(NotificationMessageVM)).OfType<PropertyDescriptor>()
                .Concat(TypeDescriptor.GetProperties(typeof(PropertyMessageVM)).OfType<PropertyDescriptor>())
                .Concat(TypeDescriptor.GetProperties(typeof(LineMessageVM)).OfType<PropertyDescriptor>())
                .GroupBy(p => p.Name).Select(g => g.First()).ToDictionary(p => p.Name, p => p));
        }

        public NotificationMessageCollection() : this(new NotificationMessageVM[0]) { }

        public NotificationMessageCollection(params NotificationMessageVM[] messages) : this((messages ?? new NotificationMessageVM[0]).AsEnumerable()) { }

        public NotificationMessageCollection(IEnumerable<NotificationMessageVM> messages)
        {
            LevelMessageCollection<NotificationMessageVM> allItems = (messages == null) ? new LevelMessageCollection<NotificationMessageVM>() : 
                new LevelMessageCollection<NotificationMessageVM>(messages.Where(m => m != null).Distinct());
            _allItems = allItems;
            _propertyMessages = new LevelMessageCollection<PropertyMessageVM>(allItems.OfType<PropertyMessageVM>());
            _lineMessages = new LevelMessageCollection<LineMessageVM>(allItems.OfType<LineMessageVM>());
            _otherMessages = new LevelMessageCollection<NotificationMessageVM>(allItems.Where(i => !(i is PropertyMessageVM || i is LineMessageVM)));
        }

        protected bool AllowEdit { get { return false; } }

        bool IBindingList.AllowEdit { get { return AllowEdit; } }

        /// <summary>
        /// Override this property as well as <see cref="AddNew"/> to support <seealso cref="IBindingList.AddNew"/>.
        /// </summary>
        protected virtual bool AllowNew { get { return false; } }

        bool IBindingList.AllowNew { get { return AllowEdit; } }

        /// <summary>
        /// Override this property to prevent items from being removed.
        /// </summary>
        protected virtual bool AllowRemove { get { return true; } }

        bool IBindingList.AllowRemove { get { return AllowRemove; } }

        #region InvokeLocked overloads

        #region 6 args

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg5">Type of fifth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg6">Type of sixth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="function"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="function"/>.</param>
        /// <param name="arg5"><typeparamref name="TArg5"/> value to pass as fifth parameter to <paramref name="function"/>.</param>
        /// <param name="arg6"><typeparamref name="TArg6"/> value to pass as sixth parameter to <paramref name="function"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> function, 
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, DispatcherPriority priority)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { return function(arg1, arg2, arg3, arg4, arg5, arg6); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                return (TResult)(Dispatcher.Invoke(function, priority, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }));
        }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2, T3, T4, T5, T6}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg5">Type of fifth argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg6">Type of sixth argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2, T3, T4, T5, T6}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="action"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="action"/>.</param>
        /// <param name="arg5"><typeparamref name="TArg5"/> value to pass as fifth parameter to <paramref name="action"/>.</param>
        /// <param name="arg6"><typeparamref name="TArg6"/> value to pass as sixth parameter to <paramref name="action"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> action, 
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, DispatcherPriority priority)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { action(arg1, arg2, arg3, arg4, arg5, arg6); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                Dispatcher.Invoke(action, priority, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg5">Type of fifth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg6">Type of sixth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="function"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="function"/>.</param>
        /// <param name="arg5"><typeparamref name="TArg5"/> value to pass as fifth parameter to <paramref name="function"/>.</param>
        /// <param name="arg6"><typeparamref name="TArg6"/> value to pass as sixth parameter to <paramref name="function"/>.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> function,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        { return InvokeLocked(function, arg1, arg2, arg3, arg4, arg5, arg6, DispatcherPriority.Normal); }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2, T3, T4, T5, T6}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg5">Type of fifth argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg6">Type of sixth argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2, T3, T4, T5, T6}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="action"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="action"/>.</param>
        /// <param name="arg5"><typeparamref name="TArg5"/> value to pass as fifth parameter to <paramref name="action"/>.</param>
        /// <param name="arg6"><typeparamref name="TArg6"/> value to pass as sixth parameter to <paramref name="action"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> action,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        { InvokeLocked(action, arg1, arg2, arg3, arg4, arg5, arg6, DispatcherPriority.Normal); }

        #endregion

        #region 5 args

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, T3, T4, T5, TResult}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg5">Type of fifth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, T3, T4, T5, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="function"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="function"/>.</param>
        /// <param name="arg5"><typeparamref name="TArg5"/> value to pass as fifth parameter to <paramref name="function"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> function,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, DispatcherPriority priority)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { return function(arg1, arg2, arg3, arg4, arg5); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                return (TResult)(Dispatcher.Invoke(function, priority, new object[] { arg1, arg2, arg3, arg4, arg5 }));
        }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2, T3, T4, T5}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg5">Type of fifth argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2, T3, T4, T5}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="action"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="action"/>.</param>
        /// <param name="arg5"><typeparamref name="TArg5"/> value to pass as fifth parameter to <paramref name="action"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2, TArg3, TArg4, TArg5>(Action<TArg1, TArg2, TArg3, TArg4, TArg5> action,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, DispatcherPriority priority)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { action(arg1, arg2, arg3, arg4, arg5); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                Dispatcher.Invoke(action, priority, new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, T3, T4, T5, TResult}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg5">Type of fifth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, T3, T4, T5, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="function"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="function"/>.</param>
        /// <param name="arg5"><typeparamref name="TArg5"/> value to pass as fifth parameter to <paramref name="function"/>.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> function,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        { return InvokeLocked(function, arg1, arg2, arg3, arg4, arg5, DispatcherPriority.Normal); }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2, T3, T4, T5}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg5">Type of fifth argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2, T3, T4, T5}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="action"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="action"/>.</param>
        /// <param name="arg5"><typeparamref name="TArg5"/> value to pass as fifth parameter to <paramref name="action"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2, TArg3, TArg4, TArg5>(Action<TArg1, TArg2, TArg3, TArg4, TArg5> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        { InvokeLocked(action, arg1, arg2, arg3, arg4, arg5, DispatcherPriority.Normal); }

        #endregion

        #region 4 args

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, T3, T4, TResult}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, T3, T4, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="function"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="function"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TArg3, TArg4, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TResult> function,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, DispatcherPriority priority)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { return function(arg1, arg2, arg3, arg4); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                return (TResult)(Dispatcher.Invoke(function, priority, new object[] { arg1, arg2, arg3, arg4 }));
        }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2, T3, T4}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2, T3, T4}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="action"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="action"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, DispatcherPriority priority)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { action(arg1, arg2, arg3, arg4); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                Dispatcher.Invoke(action, priority, new object[] { arg1, arg2, arg3, arg4 });
        }

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, T3, T4, TResult}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, T3, T4, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="function"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="function"/>.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TArg3, TArg4, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TResult> function, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        { return InvokeLocked(function, arg1, arg2, arg3, arg4, DispatcherPriority.Normal); }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2, T3, T4}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg4">Type of fourth argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2, T3, T4}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="action"/>.</param>
        /// <param name="arg4"><typeparamref name="TArg4"/> value to pass as fourth parameter to <paramref name="action"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        { InvokeLocked(action, arg1, arg2, arg3, arg4, DispatcherPriority.Normal); }

        #endregion

        #region 3 args

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, T3, TResult}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, T3, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="function"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, TResult> function, TArg1 arg1, TArg2 arg2, TArg3 arg3, DispatcherPriority priority)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { return function(arg1, arg2, arg3); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                return (TResult)(Dispatcher.Invoke(function, priority, new object[] { arg1, arg2, arg3 }));
        }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2, T3}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2, T3}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="action"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, DispatcherPriority priority)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { action(arg1, arg2, arg3); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                Dispatcher.Invoke(action, priority, new object[] { arg1, arg2, arg3 });
        }

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, T3, TResult}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, T3, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="function"/>.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, TResult> function, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        { return InvokeLocked(function, arg1, arg2, arg3, DispatcherPriority.Normal); }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2, T3}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg3">Type of third argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2, T3}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="arg3"><typeparamref name="TArg3"/> value to pass as third parameter to <paramref name="action"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        { InvokeLocked(action, arg1, arg2, arg3, DispatcherPriority.Normal); }

        #endregion

        #region 2 args

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, TResult}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TResult>(Func<TArg1, TArg2, TResult> function, TArg1 arg1, TArg2 arg2, DispatcherPriority priority)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { return function(arg1, arg2); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                return (TResult)(Dispatcher.Invoke(function, priority, new object[] { arg1, arg2 }));
        }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2>(Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2, DispatcherPriority priority)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { action(arg1, arg2); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                Dispatcher.Invoke(action, priority, new object[] { arg1, arg2 });
        }

        /// <summary>
        /// Invoke <seealso cref="Func{T1, T2, TResult}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T1, T2, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="function"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="function"/>.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg1, TArg2, TResult>(Func<TArg1, TArg2, TResult> function, TArg1 arg1, TArg2 arg2)
        { return InvokeLocked(function, arg1, arg2, DispatcherPriority.Normal); }

        /// <summary>
        /// Invoke <seealso cref="Action{T1, T2}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg1">Type of first argument to pass to <paramref name="action"/>.</typeparam>
        /// <typeparam name="TArg2">Type of second argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T1, T2}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg1"><typeparamref name="TArg1"/> value to pass as first parameter to <paramref name="action"/>.</param>
        /// <param name="arg2"><typeparamref name="TArg2"/> value to pass as second parameter to <paramref name="action"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg1, TArg2>(Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2) { InvokeLocked(action, arg1, arg2, DispatcherPriority.Normal); }

        #endregion

        #region 1 arg

        /// <summary>
        /// Invoke <seealso cref="Func{T, TResult}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg">Type of argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg"><typeparamref name="TArg"/> value to pass as the parameter to <paramref name="function"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg, TResult>(Func<TArg, TResult> function, TArg arg, DispatcherPriority priority)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { return function(arg); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                return (TResult)(Dispatcher.Invoke(function, priority, new object[] { arg }));
        }

        /// <summary>
        /// Invoke <seealso cref="Action{T}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg">Type of argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg"><typeparamref name="TArg"/> value to pass as the parameter to <paramref name="action"/>.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg>(Action<TArg> action, TArg arg, DispatcherPriority priority)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { action(arg); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                Dispatcher.Invoke(action, priority, new object[] { arg });
        }

        /// <summary>
        /// Invoke <seealso cref="Func{T, TResult}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TArg">Type of argument to pass to <paramref name="function"/>.</typeparam>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{T, TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg"><typeparamref name="TArg"/> value to pass as the parameter to <paramref name="function"/>.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TArg, TResult>(Func<TArg, TResult> function, TArg arg) { return InvokeLocked(function, arg, DispatcherPriority.Normal); }

        /// <summary>
        /// Invoke <seealso cref="Action{T}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <typeparam name="TArg">Type of argument to pass to <paramref name="action"/>.</typeparam>
        /// <param name="action"><seealso cref="Action{T}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="arg"><typeparamref name="TArg"/> value to pass as the parameter to <paramref name="action"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked<TArg>(Action<TArg> action, TArg arg) { InvokeLocked(action, arg, DispatcherPriority.Normal); }

        #endregion

        #region No args

        /// <summary>
        /// Invoke <seealso cref="Func{TResult}"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TResult>(Func<TResult> function, DispatcherPriority priority)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { return function(); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                return Dispatcher.Invoke(function, priority);
        }

        /// <summary>
        /// Invoke <seealso cref="Action"/> on current <see cref="Dispatcher"/> thread, blocking access from other threads to this 
        /// <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <param name="action"><seealso cref="Action"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <param name="priority">The priority, relative to the other pending operations in the <seealso cref="System.Windows.Threading.Dispatcher"/> event queue,
        /// that <paramref name="function"/> is invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked(Action action, DispatcherPriority priority)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (Dispatcher.CheckAccess())
            {
                Monitor.Enter(_innerSyncRoot);
                try { action(); }
                finally { Monitor.Exit(_innerSyncRoot); }
            }
            else
                Dispatcher.Invoke(action, priority);
        }

        /// <summary>
        /// Invoke <seealso cref="Func{TResult}"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution, and return a <typeparamref name="TResult"/> value.
        /// </summary>
        /// <typeparam name="TResult">Value obtained by executing <paramref name="function"/>.</typeparam>
        /// <param name="function"><seealso cref="Func{TResult}"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <returns><typeparamref name="TResult"/> that was returned by <paramref name="function"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
        protected TResult InvokeLocked<TResult>(Func<TResult> function) { return InvokeLocked(function, DispatcherPriority.Normal); }

        /// <summary>
        /// Invoke <seealso cref="Action"/> on current <see cref="Dispatcher"/> thread with normal <seealso cref="DispatcherPriority"/>,
        /// blocking access from other threads to this <seealso cref="NotificationMessageCollection"/> during execution.
        /// </summary>
        /// <param name="action"><seealso cref="Action"/> to invoke on current <see cref="Dispatcher"/> thread with thread-exclusive lock.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        protected void InvokeLocked(Action action) { InvokeLocked(action, DispatcherPriority.Normal); }

        #endregion

        #endregion

        public bool IsSorted { get { return InvokeLocked(() => _currentSort.Count == 0); } }

        public ListSortDirection SortDirection
        {
            get
            {
                return InvokeLocked(() => _currentSort.OfType<ListSortDescription>().Select(d => d.SortDirection).DefaultIfEmpty(ListSortDirection.Ascending).Last());
            }
        }

        public PropertyDescriptor SortProperty
        {
            get
            {
                return InvokeLocked(() => _currentSort.OfType<ListSortDescription>().Select(d => d.PropertyDescriptor).LastOrDefault());
            }
        }

        bool IBindingList.SupportsChangeNotification { get { return true; } }

        bool IBindingList.SupportsSearching { get { return true; } }

        bool IBindingList.SupportsSorting { get { return true; } }

        bool IRaiseItemChangedEvents.RaisesItemChangedEvents { get { return true; } }

        /// <summary>
        /// Occurs when the <see cref="NotificationMessageCollection"/> changes or a <see cref="NotificationMessageVM"/> item in the list changes.
        /// </summary>
        public event ListChangedEventHandler ListChanged;

        /// <summary>
        /// Raises the <see cref="ListChanged"/> event for a <see cref="NotificationMessageVM"/> that was moved.
        /// </summary>
        /// <param name="newIndex">The new index of the <see cref="NotificationMessageVM"/> that was moved.</param>
        /// <param name="oldIndex">The old index of the <see cref="NotificationMessageVM"/> that was moved.</param>
        protected void RaiseListChanged(int newIndex, int oldIndex) { RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, newIndex, oldIndex)); }

        /// <summary>
        /// Raises the <see cref="ListChanged"/> event for a <see cref="NotificationMessageVM"/> whose property value has changed.
        /// </summary>
        /// <param name="index">The index of the <see cref="NotificationMessageVM"/> that was changed.</param>
        /// <param name="propDesc">The <seealso cref="PropertyDescriptor"/> describing the property.</param>
        protected void RaiseListChanged(int index, PropertyDescriptor propDesc) { RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index, propDesc)); }

        /// <summary>
        /// Raises the <see cref="ListChanged"/> event for a <see cref="NotificationMessageVM"/> that added or deleted.
        /// </summary>
        /// <param name="index">The index of the <see cref="NotificationMessageVM"/> that was added or removed.</param>
        /// <param name="isDelete">true if the item <see cref="NotificationMessageVM"/> deleted; otherwise false if the <see cref="NotificationMessageVM"/> was added.</param>
        protected void RaiseListChanged(int index, bool wasRemoved)
        {
            RaiseListChanged(new ListChangedEventArgs((wasRemoved) ? ListChangedType.ItemDeleted : ListChangedType.ItemAdded, index));
        }

        /// <summary>
        /// Raises the <see cref="ListChanged"/> event to inidicate that the <see cref="NotificationMessageCollection"/> has been reset.
        /// </summary>
        protected void RaiseListChanged() { RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0)); }

        private void RaiseListChanged(ListChangedEventArgs args)
        {
            try { OnListChanged(args); }
            finally { ListChanged?.Invoke(this, args); }
        }

        /// <summary>
        /// This gets invoked when the <see cref="NotificationMessageCollection"/> changes or a <see cref="NotificationMessageVM"/> item in the list changes.
        /// </summary>
        protected virtual void OnListChanged(ListChangedEventArgs args) { }

        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Override this method as well as <seealso cref="AllowNew"/> to support <seealso cref="IBindingList.AddNew"/>.
        /// </summary>
        /// <returns>New <see cref="NotificationMessageVM"/> item that has been added to the current <see cref="NotificationMessageCollection"/>.</returns>
        protected NotificationMessageVM AddNew() { throw new NotSupportedException(); }

        object IBindingList.AddNew() { return AddNew(); }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }
        
        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }
    }
}