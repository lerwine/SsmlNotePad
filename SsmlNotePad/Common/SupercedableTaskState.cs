using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class SupercedableTaskState<TSource, TResult>
    {
        private readonly object _syncRoot = new object();
        private readonly Task<TResult> _task;
        private readonly CancellationTokenSource _tokenSource;

        public bool IsCancellationRequested { get { return Token.IsCancellationRequested; } }

        public bool IsCanceled { get { return _task.IsCanceled; } }

        public bool IsCompleted { get { return _task.IsCompleted; } }

        public bool IsFaulted { get { return _task.IsFaulted; } }

        public Exception Fault { get { return _task.Exception; } }

        public TSource Source { get; private set; }

        public TResult Result { get { return _task.Result; } }

        public TaskStatus Status { get { return _task.Status; } }
        
        public int TaskId { get { return _task.Id; } }

        public CancellationToken Token { get; private set; }
        
        public SupercedableTaskState(TSource source, Func<TSource, CancellationToken, TResult> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            Source = source;
            _tokenSource = new CancellationTokenSource();
            Token = _tokenSource.Token;
            _task = Task.Factory.StartNew<TResult>(o =>
            {
                object[] args = o as object[];
                Func<TSource, CancellationToken, TResult> f = args[2] as Func<TSource, CancellationToken, TResult>;
                return function((TSource)(args[0]), (CancellationToken)(args[1]));
            }, new object[] { source, _tokenSource.Token, function }, Token);
        }
        
        public SupercedableTaskState(TSource source, TResult value)
        {
            Source = source;
            _tokenSource = null;
            Token = CancellationToken.None;
            _task = Task.FromResult<TResult>(value);
        }

        public void ContinueWith(Action<CancellationToken, Task<TResult>> action) { _task.ContinueWith(t => action(Token, t), Token); }

        public void ContinueWith(Action<Task<TResult>> action) { _task.ContinueWith(action); }

        public static SupercedableTaskState<TSource, TResult> CheckSupercede(SupercedableTaskState<TSource, TResult> current, TSource source, Func<TSource, CancellationToken, TResult> function)
        {
            if (current == null || current.ShouldBeSuperceded(source))
                return ForceSupercede(current, source, function);

            return current;
        }

        public static SupercedableTaskState<TSource, TResult> CheckSupercede(SupercedableTaskState<TSource, TResult> current, TSource source, TResult value)
        {
            if (current == null || current.ShouldBeSuperceded(source, value))
                return ForceSupercede(current, source, value);

            return current;
        }

        public void Cancel()
        {
            lock (_syncRoot)
            {
                if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
            }
        }

        protected virtual bool ShouldBeSuperceded(TSource source)
        {
            return !EqualityComparer<TSource>.Default.Equals(Source, source);
        }

        protected virtual bool ShouldBeSuperceded(TSource source, TResult value)
        {
            if (_task.IsCompleted && !_task.IsFaulted && !_task.IsCanceled && !EqualityComparer<TResult>.Default.Equals(_task.Result, value))
                return true;

            return !EqualityComparer<TSource>.Default.Equals(Source, source);
        }

        protected virtual void OnPrepareSupercede()
        {
        }

        protected virtual void OnBeforeSupercededBy(SupercedableTaskState<TSource, TResult> following)
        {
        }

        protected virtual void OnBeforeSuperceding(SupercedableTaskState<TSource, TResult> current)
        {
        }

        protected virtual void OnAfterSupercededBy(SupercedableTaskState<TSource, TResult> following)
        {
        }

        protected virtual void OnAfterSuperceding(SupercedableTaskState<TSource, TResult> previous)
        {
        }

        private static SupercedableTaskState<TSource, TResult> ForceSupercede(SupercedableTaskState<TSource, TResult> current, Func<SupercedableTaskState<TSource, TResult>> createFollowing)
        {
            SupercedableTaskState<TSource, TResult> following;
            try
            {
                if (current != null && !current.IsCompleted && !current.IsCancellationRequested)
                    current.OnPrepareSupercede();
            }
            finally
            {
                following = createFollowing();
                try
                {
                    if (current != null)
                    {
                        try { current.OnBeforeSupercededBy(following); }
                        finally { following.OnBeforeSuperceding(current); }
                    }
                }
                finally
                {
                    if (current != null && !current.IsCompleted && !current.IsCancellationRequested)
                    {
                        try
                        {
                            try
                            {
                                if (!current.IsCompleted && !current.IsCancellationRequested)
                                    current.Cancel();
                            }
                            finally { current.OnAfterSupercededBy(following); }
                        }
                        finally { following.OnAfterSuperceding(current); }
                    }
                }
            }

            return following;
        }

        public static SupercedableTaskState<TSource, TResult> ForceSupercede(SupercedableTaskState<TSource, TResult> current, TSource source, Func<TSource, CancellationToken, TResult> function)
        {
            return ForceSupercede(current, () => new SupercedableTaskState<TSource, TResult>(source, function));
        }

        public static SupercedableTaskState<TSource, TResult> ForceSupercede(SupercedableTaskState<TSource, TResult> current, TSource source, TResult value)
        {
            return ForceSupercede(current, () => new SupercedableTaskState<TSource, TResult>(source, value));
        }
    }
}