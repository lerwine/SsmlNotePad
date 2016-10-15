using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class TaskHelper : IDisposable
    {
        private object _syncRoot = new object();

        public event EventHandler<TaskHelperCompletedEventArgs> TaskCompleted;

        public Task Task { get; private set; }

        public CancellationTokenSource TokenSource { get; private set; }

        public void StartNew(Action<object> action, object state)
        {
            CancellationTokenSource tokenSource;
            Task task;
            lock (_syncRoot)
            {
                tokenSource = TokenSource;
                task = Task;
                TokenSource = new CancellationTokenSource();
                Task = Task.Factory.StartNew(action, state, TokenSource.Token);
            }

            if (TokenSource == null)
                return;

            try
            {
                if (!task.IsCompleted && !tokenSource.IsCancellationRequested)
                    tokenSource.Cancel();
            }
            finally { tokenSource.Dispose(); }
        }

        private void OnTaskCompleted(Task task)
        {
            if (!task.IsCanceled)
                TaskCompleted?.Invoke(this, new TaskHelperCompletedEventArgs(task));
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            CancellationTokenSource tokenSource;

            lock (_syncRoot)
            {
                if (TokenSource == null)
                    return;

                tokenSource = TokenSource;
                TokenSource = null;
            }

            if (!tokenSource.IsCancellationRequested)
                tokenSource.Cancel();

            tokenSource.Dispose();
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() { Dispose(true); }

        #endregion
    }

    public class TaskHelper<TResult> : IDisposable
    {
        private object _syncRoot = new object();

        public event EventHandler<TaskHelperCompletedEventArgs<TResult>> TaskCompleted;

        public Task<TResult> Task { get; private set; }

        public CancellationTokenSource TokenSource { get; private set; }

        public void StartNew(Func<object, TResult> function, object state)
        {
            CancellationTokenSource tokenSource;
            Task<TResult> task;
            lock (_syncRoot)
            {
                tokenSource = TokenSource;
                task = Task;
                TokenSource = new CancellationTokenSource();
                Task = Task<TResult>.Factory.StartNew(function, state, TokenSource.Token);
            }

            if (TokenSource == null)
                return;

            try
            {
                if (!task.IsCompleted && !tokenSource.IsCancellationRequested)
                    tokenSource.Cancel();
            }
            finally { tokenSource.Dispose(); }
        }

        private void OnTaskCompleted(Task<TResult> task)
        {
            if (!task.IsCanceled)
                TaskCompleted?.Invoke(this, new TaskHelperCompletedEventArgs<TResult>(task));
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            CancellationTokenSource tokenSource;

            lock (_syncRoot)
            {
                if (TokenSource == null)
                    return;

                tokenSource = TokenSource;
                TokenSource = null;
            }

            if (!tokenSource.IsCancellationRequested)
                tokenSource.Cancel();

            tokenSource.Dispose();
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() { Dispose(true); }

        #endregion
    }
}
