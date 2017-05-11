using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public abstract class RestartableJobBase<TArg> : IDisposable
    {
        private object _syncRoot = new object();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Task _workerTask, _completeTask;

        protected void CancelForRestart(Func<CancellationToken, TArg, Task> createTask, TArg arg)
        {
            lock (_syncRoot)
            {
                if (_workerTask != null && !_workerTask.IsCompleted)
                {
                    try
                    {
                        _completeTask.ContinueWith((t, o) =>
                        {
                            (o as CancellationTokenSource).Dispose();
                        }, _tokenSource);
                        _tokenSource.Cancel();
                    }
                    finally { _tokenSource = new CancellationTokenSource(); }
                }
                _workerTask = createTask(_tokenSource.Token, arg);
                _completeTask = _workerTask.ContinueWith(_RaiseWorkerComplete, new object[] { _tokenSource.Token, arg });
            }
        }
        
        private void _RaiseWorkerComplete(Task workerTask, object obj)
        {
            bool isCanceled;
            Exception exception;
            object[] args = obj as object[];
            CancellationToken token = (CancellationToken)(args[0]);
            lock (_syncRoot)
            {
                isCanceled = token.IsCancellationRequested;
                exception = (workerTask.IsFaulted) ? workerTask.Exception : null;
            }
            if (isCanceled)
                RaiseWorkerCanceled((TArg)(args[1]));
            else if (exception != null)
                RaiseWorkerFault((TArg)(args[1]), exception);
            else
                RaiseWorkerComplete(workerTask, (TArg)(args[1]));
        }

        public event EventHandler<WorkerEventArgs<TArg>> WorkerCanceled;

        public event EventHandler<WorkerEventArgs<TArg, Exception>> WorkerFault;

        protected void RaiseWorkerCanceled(TArg arg)
        {
            WorkerEventArgs<TArg> eventArgs = new WorkerEventArgs<TArg>(arg);
            try { OnWorkerCanceled(eventArgs); }
            finally { WorkerCanceled?.Invoke(this, eventArgs); }
        }

        protected virtual void OnWorkerCanceled(WorkerEventArgs<TArg> args) { }

        protected void RaiseWorkerFault(TArg arg, Exception exception)
        {
            WorkerEventArgs<TArg, Exception> eventArgs = new WorkerEventArgs<TArg, Exception>(arg, exception);
            try { OnWorkerFault(eventArgs); }
            finally { WorkerFault?.Invoke(this, eventArgs); }

        }

        protected virtual void OnWorkerFault(WorkerEventArgs<TArg, Exception> args) { }

        protected abstract void RaiseWorkerComplete(Task workerTask, TArg arg);

        #region IDisposable Support

        private bool _isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (!disposing)
                return;

            lock (_syncRoot)
            {
                if (_workerTask == null)
                    return;
                if (!_workerTask.IsCompleted)
                    _tokenSource.Cancel();
            }
            if (!_workerTask.IsCompleted)
                _completeTask.Wait();
            _tokenSource.Dispose();
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RestartableJob() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class RestartableJob<TArg> : RestartableJobBase<TArg>
    {
        public event EventHandler<WorkerEventArgs<TArg>> WorkerComplete;

        public void Start(Action<TArg, CancellationToken> action, TArg arg)
        {
            CancelForRestart((token, v) =>
            {
                return Task.Factory.StartNew(o =>
                {
                    object[] args = o as object[];
                    Action<TArg, CancellationToken> a = args[0] as Action<TArg, CancellationToken>;
                    a((TArg)(args[1]), (CancellationToken)(args[2]));
                }, new object[] { action, v, token }, token);
            }, arg);
        }

        protected override void RaiseWorkerComplete(Task workerTask, TArg arg)
        {
            WorkerEventArgs<TArg> eventArgs = new WorkerEventArgs<TArg>(arg);
            try { OnWorkerComplete(eventArgs); }
            finally { WorkerComplete?.Invoke(this, eventArgs); }
        }

        protected virtual void OnWorkerComplete(WorkerEventArgs<TArg> eventArgs) { }
    }

    public class RestartableJob<TArg, TResult> : RestartableJobBase<TArg>
    {
        public event EventHandler<WorkerEventArgs<TArg, TResult>> WorkerComplete;

        public void Start(Func<TArg, CancellationToken, TResult> func, TArg arg)
        {
            CancelForRestart((token, a) =>
            {
                return Task.Factory.StartNew<TResult>(o =>
                {
                    object[] args = o as object[];
                    Func<TArg, CancellationToken, TResult> f = args[0] as Func<TArg, CancellationToken, TResult>;
                    return f((TArg)(args[1]), (CancellationToken)(args[2]));
                }, new object[] { func, a, token }, token);
            }, arg);
        }

        protected override void RaiseWorkerComplete(Task workerTask, TArg arg)
        {
            WorkerEventArgs<TArg, TResult> eventArgs = new WorkerEventArgs<TArg, TResult>(arg, ((Task<TResult>)workerTask).Result);
            try { OnWorkerComplete(eventArgs); }
            finally { WorkerComplete?.Invoke(this, eventArgs); }
        }

        protected virtual void OnWorkerComplete(WorkerEventArgs<TArg, TResult> eventArgs) { }
    }

    public class WorkerEventArgs<TArg> : EventArgs
    {
        public TArg Arg { get; private set; }

        public WorkerEventArgs(TArg arg)
        {
            Arg = arg;
        }

        public override string ToString()
        {
            if ((object)Arg == null)
                return String.Format("{0}:\r\n\tArg is null", GetType().FullName);
            return String.Format("{0}:\r\n\tArg = {1}", GetType().FullName, Arg.ToString());
        }
    }

    public class WorkerEventArgs<TArg, TResult> : WorkerEventArgs<TArg>
    {
        public TResult Result { get; private set; }

        public WorkerEventArgs(TArg arg, TResult result) : base(arg) { Result = result; }

        public override string ToString()
        {
            if ((object)Result == null)
                return String.Format("{0}\r\n\tResult is null", base.ToString());
            return String.Format("{0}\r\n\tResult = {1}", base.ToString(), Result.ToString());
        }
    }
}
