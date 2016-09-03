using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public abstract class BackgroundJobWorker<TImplemented, TResult>
        where TImplemented : BackgroundJobWorker<TImplemented, TResult>
    {
        public CancellationToken Token { get; private set; }

        public BackgroundJobWorker()
        {
            Token = CancellationToken.None;
        }

        public Task<TResult> CreateTask(CancellationToken token, TImplemented previousWorker, bool onDemand)
        {
            Token = token;
            return (onDemand) ? new Task<TResult>(Run, previousWorker, token) : Task<TResult>.Factory.StartNew(Run, previousWorker, token);
        }

        private TResult Run(object state) { return Run(state as TImplemented); }

        /// <summary>
        /// Performs backgroiund work.
        /// </summary>
        /// <param name="previousWorker">Previous worker object.</param>
        /// <returns>Result of execution</returns>
        protected abstract TResult Run(TImplemented previousWorker);

        /// <summary>
        /// Get result value after a fault.
        /// </summary>
        /// <param name="exception">Exception which was thrown.</param>
        /// <returns>Result value representing a fault.</returns>
        public virtual TResult FromFault(AggregateException exception) { return default(TResult); }

        /// <summary>
        /// Get result value after a cancellation.
        /// </summary>
        /// <returns>Result value representing a cancellation.</returns>
        public virtual TResult FromCanceled() { return default(TResult); }

        /// <summary>
        /// Get a result value while the background process is still working
        /// </summary>
        /// <returns>Result value representing an active background job.</returns>
        public virtual TResult FromActive() { return default(TResult); }
        
        protected static T CheckGet<T>(DependencyObject obj, Func<T> func, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (obj.CheckAccess())
                return func();

            return obj.Dispatcher.Invoke(func, priority);
        }

        protected static void CheckInvoke(DependencyObject obj, Action action, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (obj.Dispatcher.CheckAccess())
                action();
            else
                obj.Dispatcher.Invoke(action, priority);
        }

        protected static void CheckInvoke<T>(DependencyObject obj, Action<T> action, T arg, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (obj.Dispatcher.CheckAccess())
                action(arg);
            else
                obj.Dispatcher.Invoke(action, priority, arg);
        }

        protected static void CheckInvoke<TArg1, TArg2>(DependencyObject obj, Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2, 
            DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (obj.Dispatcher.CheckAccess())
                action(arg1, arg2);
            else
                obj.Dispatcher.Invoke(action, priority, arg1, arg2);
        }

        protected static void CheckInvoke<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(DependencyObject obj, 
            Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, 
            TArg8 arg8, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (obj.Dispatcher.CheckAccess())
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            else
                obj.Dispatcher.Invoke(action, priority, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        protected static Task CheckInvokeAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(DependencyObject obj,
            Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7,
            TArg8 arg8, DispatcherPriority priority = DispatcherPriority.Background)
        {
            return Task.Factory.StartNew(() => obj.Dispatcher.Invoke(action, priority, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
        }
    }
}