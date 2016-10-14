using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class LinkedProcess : IDisposable
    {
        private object _syncRoot = new object();
        protected object SyncRoot { get { return _syncRoot; } }

        private CancellationTokenSource _tokenSource;
        protected CancellationTokenSource TokenSource { get { return _tokenSource; } }

        private CancellationToken _token;

        public bool IsCancellationRequested { get { return _token.IsCancellationRequested; } }

        public bool IsCanceled
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_tokenSource == null)
                        return Task.IsCanceled;

                    return Task.IsCompleted && Task.IsCanceled;
                }
            }
        }

        public Task Task { get; private set; }

        public void Cancel()
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
            }
        }

        #region Constructors

        protected LinkedProcess(Func<CancellationToken, Task> createTask)
        {
            _tokenSource = new CancellationTokenSource();
            try { Task = createTask(_tokenSource.Token); }
            catch
            {
                _tokenSource.Dispose();
                throw;
            }
            Task.ContinueWith(t =>
            {
                lock (_syncRoot)
                {
                    if (_tokenSource != null)
                    {
                        _tokenSource.Dispose();
                        _tokenSource = null;
                    }
                }
            });
        }

        public LinkedProcess(Action<CancellationToken, object> action, object state, TaskCreationOptions creationOptions)
            : this(t => new Task(o =>
            {
                object[] args = o as object[];
                Action<CancellationToken, object> a = args[0] as Action<CancellationToken, object>;
                a((CancellationToken)(args[1]), args[2]);
            }, new object[] { action, t, state }, t, creationOptions))
        { }

        public LinkedProcess(Action<CancellationToken> action, TaskCreationOptions creationOptions)
            : this(t => new Task(o =>
            {
                object[] args = o as object[];
                Action<CancellationToken> a = args[0] as Action<CancellationToken>;
                a((CancellationToken)(args[1]));
            }, new object[] { action, t }, t, creationOptions)) { }

        public LinkedProcess(Action<CancellationToken, object> action, object state)
            : this(t => new Task(o =>
            {
                object[] args = o as object[];
                Action<CancellationToken, object> a = args[0] as Action<CancellationToken, object>;
                a((CancellationToken)(args[1]), args[2]);
            }, new object[] { action, t, state }, t))
        { }

        public LinkedProcess(Action<CancellationToken> action)
            : this(t => new Task(o =>
            {
                object[] args = o as object[];
                Action<CancellationToken> a = args[0] as Action<CancellationToken>;
                a((CancellationToken)(args[1]));
            }, new object[] { action, t }, t)) { }

        public LinkedProcess(Action<object> action, object state, TaskCreationOptions creationOptions)
            : this(t => new Task(action, state, t, creationOptions)) { }

        public LinkedProcess(Action<object> action, object state)
            : this(t => new Task(action, state, t)) { }

        public LinkedProcess(Action action, TaskCreationOptions creationOptions)
            : this(t => new Task(action, t, creationOptions)) { }

        public LinkedProcess(Action action)
            : this(t => new Task(action, t)) { }

        #endregion

        #region StartNew overloads

        public static LinkedProcess StartNew(Action<CancellationToken, object> action, object state, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(t => Task.Factory.StartNew(o =>
            {
                object[] args = o as object[];
                Action<CancellationToken, object> a = args[0] as Action<CancellationToken, object>;
                a((CancellationToken)(args[1]), args[2]);
            }, new object[] { action, t, state }, t, creationOptions, scheduler));
        }

        public static LinkedProcess StartNew(Action<CancellationToken> action, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(t => Task.Factory.StartNew(o =>
            {
                object[] args = o as object[];
                Action<CancellationToken> a = args[0] as Action<CancellationToken>;
                a((CancellationToken)(args[1]));
            }, new object[] { action, t }, t, creationOptions, scheduler));
        }

        public static LinkedProcess StartNew(Action<CancellationToken, object> action, object state)
        {
            return new LinkedProcess(t => Task.Factory.StartNew(o =>
            {
                object[] args = o as object[];
                Action<CancellationToken, object> a = args[0] as Action<CancellationToken, object>;
                a((CancellationToken)(args[1]), args[2]);
            }, new object[] { action, t, state }, t));
        }

        public static LinkedProcess StartNew(Action<CancellationToken> action)
        {
            return new LinkedProcess(t => Task.Factory.StartNew(o =>
            {
                object[] args = o as object[];
                Action<CancellationToken> a = args[0] as Action<CancellationToken>;
                a((CancellationToken)(args[1]));
            }, new object[] { action, t }, t));
        }


        public static LinkedProcess StartNew(Action<object> action, object state, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(t => Task.Factory.StartNew(action, state, t, creationOptions, scheduler));
        }

        public static LinkedProcess StartNew(Action action, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(t => Task.Factory.StartNew(action, t, creationOptions, scheduler));
        }

        public static LinkedProcess StartNew(Action<object> action, object state)
        {
            return new LinkedProcess(t => Task.Factory.StartNew(action, state, t));
        }

        public static LinkedProcess StartNew(Action action)
        {
            return new LinkedProcess(t => Task.Factory.StartNew(action, t));
        }

        #endregion

        #region ContinueWith overloads

        public LinkedProcess ContinueWith(Action<LinkedProcess, CancellationToken, object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(c => Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess, CancellationToken, object> a = args[0] as Action<LinkedProcess, CancellationToken, object>;
                a(args[1] as LinkedProcess, (CancellationToken)(args[2]), args[3]);
            }, new object[] { action, this, c, state }, c, continuationOptions, scheduler));
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess, CancellationToken> action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(c => Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess, CancellationToken> a = args[0] as Action<LinkedProcess, CancellationToken>;
                a(args[1] as LinkedProcess, (CancellationToken)(args[2]));
            }, new object[] { action, this, c }, c, continuationOptions, scheduler));
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess, CancellationToken, object> action, object state)
        {
            return new LinkedProcess(c => Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess, CancellationToken, object> a = args[0] as Action<LinkedProcess, CancellationToken, object>;
                a(args[1] as LinkedProcess, (CancellationToken)(args[2]), args[3]);
            }, new object[] { action, this, c, state }, c));
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess, CancellationToken> action)
        {
            return new LinkedProcess(c => Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess, CancellationToken> a = args[0] as Action<LinkedProcess, CancellationToken>;
                a(args[1] as LinkedProcess, (CancellationToken)(args[2]));
            }, new object[] { action, this, c }, c));
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess, object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(c => Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess, object> a = args[0] as Action<LinkedProcess, object>;
                a(args[1] as LinkedProcess, args[2]);
            }, new object[] { action, this, state }, c, continuationOptions, scheduler));
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess> action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(c => Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess> a = args[0] as Action<LinkedProcess>;
                a(args[1] as LinkedProcess);
            }, new object[] { action, this }, c, continuationOptions, scheduler));
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess, object> action, object state)
        {
            return new LinkedProcess(c => Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess, object> a = args[0] as Action<LinkedProcess, object>;
                a(args[1] as LinkedProcess, args[2]);
            }, new object[] { action, this, state }, c));
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess> action)
        {
            return new LinkedProcess(c => Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess> a = args[0] as Action<LinkedProcess>;
                a(args[1] as LinkedProcess);
            }, new object[] { action, this }, c));
        }

        public LinkedProcess<TResult> ContinueWith<TResult>(Func<LinkedProcess, CancellationToken, TResult> func)
        {
            return LinkedProcess<TResult>.ContinueWith(this, func);
        }

        public LinkedProcess<TResult> ContinueWith<TResult>(Func<LinkedProcess, CancellationToken, object, TResult> func, object state)
        {
            return LinkedProcess<TResult>.ContinueWith(this, func, state);
        }

        public LinkedProcess<TResult> ContinueWith<TResult>(Func<LinkedProcess, CancellationToken, TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess<TResult>.ContinueWith(this, func, continuationOptions, scheduler);
        }

        public LinkedProcess<TResult> ContinueWith<TResult>(Func<LinkedProcess, CancellationToken, object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess<TResult>.ContinueWith(this, func, state, continuationOptions, scheduler);
        }

        public LinkedProcess<TResult> ContinueWith<TResult>(Func<LinkedProcess, TResult> func)
        {
            return LinkedProcess<TResult>.ContinueWith(this, func);
        }

        public LinkedProcess<TResult> ContinueWith<TResult>(Func<LinkedProcess, object, TResult> func, object state)
        {
            return LinkedProcess<TResult>.ContinueWith(this, func, state);
        }

        public LinkedProcess<TResult> ContinueWith<TResult>(Func<LinkedProcess, TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess<TResult>.ContinueWith(this, func, continuationOptions, scheduler);
        }

        public LinkedProcess<TResult> ContinueWith<TResult>(Func<LinkedProcess, object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess<TResult>.ContinueWith(this, func, state, continuationOptions, scheduler);
        }

        protected static LinkedProcess ContinueWith<TResult>(LinkedProcess<TResult> previous, Action<LinkedProcess<TResult>, CancellationToken> action)
        {
            return new LinkedProcess(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess<TResult>, CancellationToken> a = args[0] as Action<LinkedProcess<TResult>, CancellationToken>;
                a(args[1] as LinkedProcess<TResult>, (CancellationToken)(args[2]));
            }, new object[] { action, previous, c }, c));
        }

        protected static LinkedProcess ContinueWith<TResult>(LinkedProcess<TResult> previous, Action<LinkedProcess<TResult>, CancellationToken, object> action, object state)
        {
            return new LinkedProcess(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess<TResult>, CancellationToken, object> a = args[0] as Action<LinkedProcess<TResult>, CancellationToken, object>;
                a(args[1] as LinkedProcess<TResult>, (CancellationToken)(args[2]), args[3]);
            }, new object[] { action, previous, c, state }, c));
        }

        protected static LinkedProcess ContinueWith<TResult>(LinkedProcess<TResult> previous, Action<LinkedProcess<TResult>, CancellationToken> action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess<TResult>, CancellationToken> a = args[0] as Action<LinkedProcess<TResult>, CancellationToken>;
                a(args[1] as LinkedProcess<TResult>, (CancellationToken)(args[2]));
            }, new object[] { action, previous, c }, c, continuationOptions, scheduler));
        }

        protected static LinkedProcess ContinueWith<TResult>(LinkedProcess<TResult> previous, Action<LinkedProcess<TResult>, CancellationToken, object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess<TResult>, CancellationToken, object> a = args[0] as Action<LinkedProcess<TResult>, CancellationToken, object>;
                a(args[1] as LinkedProcess<TResult>, (CancellationToken)(args[2]), args[3]);
            }, new object[] { action, previous, c, state }, c, continuationOptions, scheduler));
        }

        protected static LinkedProcess ContinueWith<TResult>(LinkedProcess<TResult> previous, Action<LinkedProcess<TResult>> action)
        {
            return new LinkedProcess(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess<TResult>> a = args[0] as Action<LinkedProcess<TResult>>;
                a(args[1] as LinkedProcess<TResult>);
            }, new object[] { action, previous }, c));
        }

        protected static LinkedProcess ContinueWith<TResult>(LinkedProcess<TResult> previous, Action<LinkedProcess<TResult>, object> action, object state)
        {
            return new LinkedProcess(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess<TResult>, object> a = args[0] as Action<LinkedProcess<TResult>, object>;
                a(args[1] as LinkedProcess<TResult>, args[2]);
            }, new object[] { action, previous, state }, c));
        }

        protected static LinkedProcess ContinueWith<TResult>(LinkedProcess<TResult> previous, Action<LinkedProcess<TResult>> action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess<TResult>> a = args[0] as Action<LinkedProcess<TResult>>;
                a(args[1] as LinkedProcess<TResult>);
            }, new object[] { action, previous }, c, continuationOptions, scheduler));
        }

        protected static LinkedProcess ContinueWith<TResult>(LinkedProcess<TResult> previous, Action<LinkedProcess<TResult>, object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Action<LinkedProcess<TResult>, object> a = args[0] as Action<LinkedProcess<TResult>, object>;
                a(args[1] as LinkedProcess<TResult>, args[2]);
            }, new object[] { action, previous, state }, c, continuationOptions, scheduler));
        }

        #endregion

        #region RestartWith overloads

        public LinkedProcess RestartWith(Action<CancellationToken, object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return new LinkedProcess(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<CancellationToken, object> a = args[0] as Action<CancellationToken, object>;
                    a((CancellationToken)(args[1]), args[2]);
                }, new object[] { action, c, state }, c, continuationOptions, scheduler));
            }
        }

        public LinkedProcess RestartWith(Action<CancellationToken> action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return new LinkedProcess(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<CancellationToken> a = args[0] as Action<CancellationToken>;
                    a((CancellationToken)(args[1]));
                }, new object[] { action, c }, c, continuationOptions, scheduler));
            }
        }

        public LinkedProcess RestartWith(Action<CancellationToken, object> action, object state)
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return new LinkedProcess(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<CancellationToken, object> a = args[0] as Action<CancellationToken, object>;
                    a((CancellationToken)(args[1]), args[2]);
                }, new object[] { action, c, state }, c));
            }
        }

        public LinkedProcess RestartWith(Action<CancellationToken> action)
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return new LinkedProcess(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<CancellationToken> a = args[0] as Action<CancellationToken>;
                    a((CancellationToken)(args[1]));
                }, new object[] { action, this, c }, c));
            }
        }

        public LinkedProcess RestartWith(Action<object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return new LinkedProcess(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<object> a = args[0] as Action<object>;
                    a(args[1]);
                }, new object[] { action, this, state }, c, continuationOptions, scheduler));
            }
        }

        public LinkedProcess RestartWith(Action action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return new LinkedProcess(c => Task.ContinueWith((p, o) =>
                {
                    Action a = o as Action;
                    a();
                }, action, c, continuationOptions, scheduler));
            }
        }

        public LinkedProcess RestartWith(Action<object> action, object state)
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return new LinkedProcess(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<object> a = args[0] as Action<object>;
                    a(args[1]);
                }, new object[] { action, state }, c));
            }
        }

        public LinkedProcess RestartWith(Action action)
        {
            lock (_syncRoot)
            {
                if (!Task.IsCompleted && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return new LinkedProcess(c => Task.ContinueWith((p, o) =>
                {
                    Action a = o as Action;
                    a();
                }, action, c));
            }
        }

        public static void RestartOn(Action<CancellationToken, object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler, ref LinkedProcess linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException("linkedProcess");

            lock (linkedProcess._syncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess._tokenSource.IsCancellationRequested)
                    linkedProcess._tokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess(c => t.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<CancellationToken, object> a = args[0] as Action<CancellationToken, object>;
                    a((CancellationToken)(args[1]), args[2]);
                }, new object[] { action, c, state }, c, continuationOptions, scheduler));
            }
        }

        public static void RestartOn(Action<CancellationToken> action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler, ref LinkedProcess linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException("linkedProcess");

            lock (linkedProcess._syncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess._tokenSource.IsCancellationRequested)
                    linkedProcess._tokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess(c => t.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<CancellationToken> a = args[0] as Action<CancellationToken>;
                    a((CancellationToken)(args[1]));
                }, new object[] { action, c }, c, continuationOptions, scheduler));
            }
        }

        public static void RestartOn(Action<CancellationToken, object> action, object state, ref LinkedProcess linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException("linkedProcess");

            lock (linkedProcess._syncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess._tokenSource.IsCancellationRequested)
                    linkedProcess._tokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess(c => t.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<CancellationToken, object> a = args[0] as Action<CancellationToken, object>;
                    a((CancellationToken)(args[1]), args[2]);
                }, new object[] { action, c, state }, c));
            }
        }

        public static void RestartOn(Action<CancellationToken> action, ref LinkedProcess linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException("linkedProcess");

            lock (linkedProcess._syncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess._tokenSource.IsCancellationRequested)
                    linkedProcess._tokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess(c => t.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<CancellationToken> a = args[0] as Action<CancellationToken>;
                    a((CancellationToken)(args[1]));
                }, new object[] { action, c }, c));
            }
        }

        public static void RestartOn(Action<object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler, ref LinkedProcess linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException("linkedProcess");

            lock (linkedProcess._syncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess._tokenSource.IsCancellationRequested)
                    linkedProcess._tokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess(c => t.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<object> a = args[0] as Action<object>;
                    a(args[1]);
                }, new object[] { action, state }, c, continuationOptions, scheduler));
            }
        }

        public static void RestartOn(Action action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler, ref LinkedProcess linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException("linkedProcess");

            lock (linkedProcess._syncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess._tokenSource.IsCancellationRequested)
                    linkedProcess._tokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess(c => t.ContinueWith((p, o) =>
                {
                    Action a = o as Action;
                    a();
                }, action, c, continuationOptions, scheduler));
            }
        }

        public static void RestartOn(Action<object> action, object state, ref LinkedProcess linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException("linkedProcess");

            lock (linkedProcess._syncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess._tokenSource.IsCancellationRequested)
                    linkedProcess._tokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess(c => t.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Action<object> a = args[0] as Action<object>;
                    a(args[1]);
                }, new object[] { action, state }, c));
            }
        }

        public static void RestartOn(Action action, ref LinkedProcess linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException("linkedProcess");

            lock (linkedProcess._syncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess._tokenSource.IsCancellationRequested)
                    linkedProcess._tokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess(c => t.ContinueWith((p, o) =>
                {
                    Action a = o as Action;
                    a();
                }, action, c));
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
                return;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() { Dispose(true); }

        #endregion
    }

    public class LinkedProcess<TResult> : LinkedProcess
    {
        public new Task<TResult> Task { get { return (Task<TResult>)(base.Task); } }

        #region Constructors

        protected LinkedProcess(Func<CancellationToken, Task<TResult>> createTask) : base(t => createTask(t)) { }

        public LinkedProcess(Func<object, TResult> func, object state, TaskCreationOptions creationOptions)
            : this(t => new Task<TResult>(func, state, t, creationOptions)) { }

        public LinkedProcess(Func<TResult> func, TaskCreationOptions creationOptions)
            : this(t => new Task<TResult>(func, t, creationOptions)) { }

        public LinkedProcess(Func<object, TResult> func, object state)
            : this(t => new Task<TResult>(func, state, t)) { }

        public LinkedProcess(Func<TResult> func)
            : this(t => new Task<TResult>(func, t)) { }

        #endregion

        #region StartNew overloads

        public static LinkedProcess<TResult> StartNew(Func<TResult> func)
        {
            return new LinkedProcess<TResult>(t => Task<TResult>.Factory.StartNew(func, t));
        }

        public static LinkedProcess<TResult> StartNew(Func<object, TResult> func, object state)
        {
            return new LinkedProcess<TResult>(t => Task<TResult>.Factory.StartNew(func, state, t));
        }

        public static LinkedProcess<TResult> StartNew(Func<TResult> func, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(t => Task<TResult>.Factory.StartNew(func, t, creationOptions, scheduler));
        }

        public static LinkedProcess<TResult> StartNew(Func<object, TResult> func, object state, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(t => Task<TResult>.Factory.StartNew(func, state, t, creationOptions, scheduler));
        }

        #endregion

        #region ContinueWith overloads

        public LinkedProcess ContinueWith(Action<LinkedProcess<TResult>, CancellationToken> action)
        {
            return LinkedProcess.ContinueWith(this, action);
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess<TResult>, CancellationToken, object> action, object state)
        {
            return LinkedProcess.ContinueWith(this, action, state);
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess<TResult>, CancellationToken> action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess.ContinueWith(this, action, continuationOptions, scheduler);
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess<TResult>, CancellationToken, object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess.ContinueWith(this, action, state, continuationOptions, scheduler);
        }

        public LinkedProcess<TResult2> ContinueWith<TResult2>(Func<LinkedProcess<TResult>, TResult2> func)
        {
            return LinkedProcess<TResult2>.ContinueWith(this, func);
        }

        private static LinkedProcess<TResult> ContinueWith<TResult2>(LinkedProcess<TResult2> previous, Func<LinkedProcess<TResult2>, CancellationToken, TResult> func)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess<TResult2>, CancellationToken, TResult> f = args[0] as Func<LinkedProcess<TResult2>, CancellationToken, TResult>;
                return f(args[1] as LinkedProcess<TResult2>, (CancellationToken)(args[2]));
            }, new object[] { func, previous }, c));
        }

        public LinkedProcess<TResult2> ContinueWith<TResult2>(Func<LinkedProcess<TResult>, CancellationToken, object, TResult2> func, object state)
        {
            return LinkedProcess<TResult2>.ContinueWith(this, func, state);
        }

        private static LinkedProcess<TResult> ContinueWith<TResult2>(LinkedProcess<TResult2> previous, Func<LinkedProcess<TResult2>, CancellationToken, object, TResult> func, object state)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess<TResult2>, CancellationToken, object, TResult> f = args[0] as Func<LinkedProcess<TResult2>, CancellationToken, object, TResult>;
                return f(args[1] as LinkedProcess<TResult2>, (CancellationToken)(args[2]), args[3]);
            }, new object[] { func, previous, state }, c));
        }

        public LinkedProcess<TResult2> ContinueWith<TResult2>(Func<LinkedProcess<TResult>, CancellationToken, TResult2> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess<TResult2>.ContinueWith(this, func, continuationOptions, scheduler);
        }

        private static LinkedProcess<TResult> ContinueWith<TResult2>(LinkedProcess<TResult2> previous, Func<LinkedProcess<TResult2>, CancellationToken, TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess<TResult2>, CancellationToken, TResult> f = args[0] as Func<LinkedProcess<TResult2>, CancellationToken, TResult>;
                return f(args[1] as LinkedProcess<TResult2>, (CancellationToken)(args[2]));
            }, new object[] { func, previous }, c, continuationOptions, scheduler));
        }

        public LinkedProcess<TResult2> ContinueWith<TResult2>(Func<LinkedProcess<TResult>, CancellationToken, object, TResult2> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess<TResult2>.ContinueWith(this, func, state, continuationOptions, scheduler);
        }

        private static LinkedProcess<TResult> ContinueWith<TResult2>(LinkedProcess<TResult2> previous, Func<LinkedProcess<TResult2>, CancellationToken, object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess<TResult2>, CancellationToken, object, TResult> f = args[0] as Func<LinkedProcess<TResult2>, CancellationToken, object, TResult>;
                return f(args[1] as LinkedProcess<TResult2>, (CancellationToken)(args[2]), args[3]);
            }, new object[] { func, previous, state }, c, continuationOptions, scheduler));
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess<TResult>> action)
        {
            return LinkedProcess.ContinueWith(this, action);
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess<TResult>, object> action, object state)
        {
            return LinkedProcess.ContinueWith(this, action, state);
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess<TResult>> action, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess.ContinueWith(this, action, continuationOptions, scheduler);
        }

        public LinkedProcess ContinueWith(Action<LinkedProcess<TResult>, object> action, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess.ContinueWith(this, action, state, continuationOptions, scheduler);
        }

        //public LinkedProcess<TResult2> ContinueWith<TResult2>(Func<LinkedProcess<TResult>, TResult2> func)
        //{
        //    return LinkedProcess<TResult2>.ContinueWith(this, func);
        //}

        private static LinkedProcess<TResult> ContinueWith<TResult2>(LinkedProcess<TResult2> previous, Func<LinkedProcess<TResult2>, TResult> func)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess<TResult2>, TResult> f = args[0] as Func<LinkedProcess<TResult2>, TResult>;
                return f(args[1] as LinkedProcess<TResult2>);
            }, new object[] { func, previous }, c));
        }

        public LinkedProcess<TResult2> ContinueWith<TResult2>(Func<LinkedProcess<TResult>, object, TResult2> func, object state)
        {
            return LinkedProcess<TResult2>.ContinueWith(this, func, state);
        }

        private static LinkedProcess<TResult> ContinueWith<TResult2>(LinkedProcess<TResult2> previous, Func<LinkedProcess<TResult2>, object, TResult> func, object state)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess<TResult2>, object, TResult> f = args[0] as Func<LinkedProcess<TResult2>, object, TResult>;
                return f(args[1] as LinkedProcess<TResult2>, args[2]);
            }, new object[] { func, previous, state }, c));
        }

        public LinkedProcess<TResult2> ContinueWith<TResult2>(Func<LinkedProcess<TResult>, TResult2> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess<TResult2>.ContinueWith(this, func, continuationOptions, scheduler);
        }

        private static LinkedProcess<TResult> ContinueWith<TResult2>(LinkedProcess<TResult2> previous, Func<LinkedProcess<TResult2>, TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess<TResult2>, TResult> f = args[0] as Func<LinkedProcess<TResult2>, TResult>;
                return f(args[1] as LinkedProcess<TResult2>);
            }, new object[] { func, previous }, c, continuationOptions, scheduler));
        }

        public LinkedProcess<TResult2> ContinueWith<TResult2>(Func<LinkedProcess<TResult>, object, TResult2> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return LinkedProcess<TResult2>.ContinueWith(this, func, state, continuationOptions, scheduler);
        }

        private static LinkedProcess<TResult> ContinueWith<TResult2>(LinkedProcess<TResult2> previous, Func<LinkedProcess<TResult2>, object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess<TResult2>, object, TResult> f = args[0] as Func<LinkedProcess<TResult2>, object, TResult>;
                return f(args[1] as LinkedProcess<TResult2>, args[2]);
            }, new object[] { func, previous, state }, c, continuationOptions, scheduler));
        }
        
        internal static LinkedProcess<TResult> ContinueWith(LinkedProcess previous, Func<LinkedProcess, CancellationToken, object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess, CancellationToken, object, TResult> f = args[0] as Func<LinkedProcess, CancellationToken, object, TResult>;
                return f(args[1] as LinkedProcess, (CancellationToken)(args[2]), args[3]);
            }, new object[] { func, previous, c, state }, c, continuationOptions, scheduler));
        }

        internal static LinkedProcess<TResult> ContinueWith(LinkedProcess previous, Func<LinkedProcess, CancellationToken, TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess, CancellationToken, TResult> f = args[0] as Func<LinkedProcess, CancellationToken, TResult>;
                return f(args[1] as LinkedProcess, (CancellationToken)(args[2]));
            }, new object[] { func, previous, c }, c, continuationOptions, scheduler));
        }

        internal static LinkedProcess<TResult> ContinueWith(LinkedProcess previous, Func<LinkedProcess, CancellationToken, object, TResult> func, object state)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess, CancellationToken, object, TResult> f = args[0] as Func<LinkedProcess, CancellationToken, object, TResult>;
                return f(args[1] as LinkedProcess, (CancellationToken)(args[2]), args[3]);
            }, new object[] { func, previous, c, state }, c));
        }

        internal static LinkedProcess<TResult> ContinueWith(LinkedProcess previous, Func<LinkedProcess, CancellationToken, TResult> func)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess, CancellationToken, TResult> f = args[0] as Func<LinkedProcess, CancellationToken, TResult>;
                return f(args[1] as LinkedProcess, (CancellationToken)(args[2]));
            }, new object[] { func, previous, c }, c));
        }

        internal static LinkedProcess<TResult> ContinueWith(LinkedProcess previous, Func<LinkedProcess, object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess, object, TResult> f = args[0] as Func<LinkedProcess, object, TResult>;
                return f(args[1] as LinkedProcess, args[2]);
            }, new object[] { func, previous, state }, c, continuationOptions, scheduler));
        }

        internal static LinkedProcess<TResult> ContinueWith(LinkedProcess previous, Func<LinkedProcess, TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess, TResult> f = args[0] as Func<LinkedProcess, TResult>;
                return f(args[1] as LinkedProcess);
            }, new object[] { func, previous }, c, continuationOptions, scheduler));
        }

        internal static LinkedProcess<TResult> ContinueWith(LinkedProcess previous, Func<LinkedProcess, object, TResult> func, object state)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess, object, TResult> f = args[0] as Func<LinkedProcess, object, TResult>;
                return f(args[1] as LinkedProcess, args[2]);
            }, new object[] { func, previous, state }, c));
        }

        internal static LinkedProcess<TResult> ContinueWith(LinkedProcess previous, Func<LinkedProcess, TResult> func)
        {
            return new LinkedProcess<TResult>(c => previous.Task.ContinueWith((p, o) =>
            {
                object[] args = o as object[];
                Func<LinkedProcess, TResult> f = args[0] as Func<LinkedProcess, TResult>;
                return f(args[1] as LinkedProcess);
            }, new object[] { func, previous }, c));
        }

        #endregion

        #region RestartWith Overloads

        public LinkedProcess<TResult> RestartWith(Func<CancellationToken, TResult> func)
        {
            lock (SyncRoot)
            {
                if (!Task.IsCompleted && !TokenSource.IsCancellationRequested)
                    TokenSource.Cancel();
                return new LinkedProcess<TResult>(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Func<CancellationToken, TResult> f = args[0] as Func<CancellationToken, TResult>;
                    return f((CancellationToken)(args[1]));
                }, new object[] { func, c }, c));
            }
        }

        public LinkedProcess<TResult> RestartWith(Func<CancellationToken, object, TResult> func, object state)
        {
            lock (SyncRoot)
            {
                if (!Task.IsCompleted && !TokenSource.IsCancellationRequested)
                    TokenSource.Cancel();
                return new LinkedProcess<TResult>(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Func<CancellationToken, object, TResult> f = args[0] as Func<CancellationToken, object, TResult>;
                    return f((CancellationToken)(args[1]), args[2]);
                }, new object[] { func, c, state }, c));
            }
        }

        public LinkedProcess<TResult> RestartWith(Func<CancellationToken, TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            lock (SyncRoot)
            {
                if (!Task.IsCompleted && !TokenSource.IsCancellationRequested)
                    TokenSource.Cancel();
                return new LinkedProcess<TResult>(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Func<CancellationToken, TResult> f = args[0] as Func<CancellationToken, TResult>;
                    return f((CancellationToken)(args[1]));
                }, new object[] { func, c }, c, continuationOptions, scheduler));
            }
        }

        public LinkedProcess<TResult> RestartWith(Func<CancellationToken, object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            lock (SyncRoot)
            {
                if (!Task.IsCompleted && !TokenSource.IsCancellationRequested)
                    TokenSource.Cancel();
                return new LinkedProcess<TResult>(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Func<CancellationToken, object, TResult> f = args[0] as Func<CancellationToken, object, TResult>;
                    return f((CancellationToken)(args[1]), args[2]);
                }, new object[] { func, c, state }, c, continuationOptions, scheduler));
            }
        }

        public LinkedProcess<TResult> RestartWith(Func<TResult> func)
        {
            lock (SyncRoot)
            {
                if (!Task.IsCompleted && !TokenSource.IsCancellationRequested)
                    TokenSource.Cancel();
                return new LinkedProcess<TResult>(c => Task.ContinueWith((p, o) =>
                {
                    Func<TResult> f = o as Func<TResult>;
                    return f();
                }, func, c));
            }
        }

        public LinkedProcess<TResult> RestartWith(Func<object, TResult> func, object state)
        {
            lock (SyncRoot)
            {
                if (!Task.IsCompleted && !TokenSource.IsCancellationRequested)
                    TokenSource.Cancel();
                return new LinkedProcess<TResult>(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Func<object, TResult> f = args[0] as Func<object, TResult>;
                    return f(args[1]);
                }, new object[] { func, state }, c));
            }
        }

        public LinkedProcess<TResult> RestartWith(Func<TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            lock (SyncRoot)
            {
                if (!Task.IsCompleted && !TokenSource.IsCancellationRequested)
                    TokenSource.Cancel();
                return new LinkedProcess<TResult>(c => Task.ContinueWith((p, o) =>
                {
                    Func<TResult> f = o as Func<TResult>;
                    return f();
                }, func, c, continuationOptions, scheduler));
            }
        }

        public LinkedProcess<TResult> RestartWith(Func<object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            lock (SyncRoot)
            {
                if (!Task.IsCompleted && !TokenSource.IsCancellationRequested)
                    TokenSource.Cancel();
                return new LinkedProcess<TResult>(c => Task.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Func<object, TResult> f = args[0] as Func<object, TResult>;
                    return f(args[1]);
                }, new object[] { func, state }, c, continuationOptions, scheduler));
            }
        }

        public static void RestartOn(Func<object, TResult> func, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler, ref LinkedProcess<TResult> linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException();

            lock (linkedProcess.SyncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess.TokenSource.IsCancellationRequested)
                    linkedProcess.TokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess<TResult>(c => t.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Func<object, TResult> f = args[0] as Func<object, TResult>;
                    return f(args[1]);
                }, new object[] { func, state }, c, continuationOptions, scheduler));
            }
        }

        public static void RestartOn(Func<TResult> func, TaskContinuationOptions continuationOptions, TaskScheduler scheduler, ref LinkedProcess<TResult> linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException();

            lock (linkedProcess.SyncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess.TokenSource.IsCancellationRequested)
                    linkedProcess.TokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess<TResult>(c => t.ContinueWith((p, o) =>
                {
                    Func<TResult> f = o as Func<TResult>;
                    return f();
                }, func, c, continuationOptions, scheduler));
            }
        }

        public static void RestartOn(Func<object, TResult> func, object state, ref LinkedProcess<TResult> linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException();

            lock (linkedProcess.SyncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess.TokenSource.IsCancellationRequested)
                    linkedProcess.TokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess<TResult>(c => t.ContinueWith((p, o) =>
                {
                    object[] args = o as object[];
                    Func<object, TResult> f = args[0] as Func<object, TResult>;
                    return f(args[1]);
                }, new object[] { func, state }, c));
            }
        }

        public static void RestartOn(Func<TResult> func, ref LinkedProcess<TResult> linkedProcess)
        {
            if (linkedProcess == null)
                throw new ArgumentNullException();

            lock (linkedProcess.SyncRoot)
            {
                if (!linkedProcess.Task.IsCompleted && !linkedProcess.TokenSource.IsCancellationRequested)
                    linkedProcess.TokenSource.Cancel();
                Task t = linkedProcess.Task;
                linkedProcess = new LinkedProcess<TResult>(c => t.ContinueWith((p, o) =>
                {
                    Func<TResult> f = o as Func<TResult>;
                    return f();
                }, func, c));
            }
        }

        #endregion
    }
}
