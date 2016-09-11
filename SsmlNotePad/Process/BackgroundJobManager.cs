using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public class BackgroundJobManager<TWorker, TResult>
        where TWorker : BackgroundJobWorker<TWorker, TResult>
    {
        private object _syncRoot = new object();
        private TWorker _currentWorker = null;
        private Task<TResult> _currentTask = null;
        private CancellationTokenSource _currentTokenSource = null;

        public TaskStatus Status { get { return _currentTask.Status; } }

        public bool IsCompleted { get { return _currentTask == null || _currentTask.IsCompleted; } }

        public bool Wait(int millisecondsTimeout)
        {
            Task<TResult> task;
            TWorker worker;
            lock (_syncRoot)
            {
                task = _currentTask;
                worker = _currentWorker;
                if (task.Status == TaskStatus.Created)
                    task.Start();
            }

            return task.Wait(millisecondsTimeout, worker.Token);
        }

        public void Wait()
        {
            Task<TResult> task;
            TWorker worker;
            lock (_syncRoot)
            {
                task = _currentTask;
                worker = _currentWorker;
                if (task.Status == TaskStatus.Created)
                    task.Start();
            }

            task.Wait(worker.Token);
        }

        public bool TryGetResult(int millisecondsTimeout, out TResult result, out bool isCanceled)
        {
            Task<TResult> task;
            TWorker worker;
            lock (_syncRoot)
            {
                task = _currentTask;
                worker = _currentWorker;
                if (task.Status == TaskStatus.Created)
                    task.Start();
            }

            if (!task.Wait(millisecondsTimeout, worker.Token))
            {
                result = worker.FromActive();
                isCanceled = false;
                return false;
            }

            isCanceled = task.IsCanceled;
            if (isCanceled)
                result = worker.FromCanceled();
            if (task.IsFaulted)
                result = worker.FromFault(task.Exception);
            else
                result = task.Result;
            return true;
        }

        public TResult GetResult()
        {
            Task<TResult> task;
            TWorker worker;
            lock (_syncRoot)
            {
                task = _currentTask;
                worker = _currentWorker;
                if (task.Status == TaskStatus.Created)
                    task.Start();
            }

            task.Wait(worker.Token);

            if (task.IsCanceled)
                return worker.FromCanceled();

            if (task.IsFaulted)
                return worker.FromFault(task.Exception);

            return task.Result;
        }
        public void OnCompleted(Action<Task<TResult>> action)
        {
            lock (_syncRoot)
                _currentTask.ContinueWith(Continuation, new object[] { _currentWorker, action });
        }

        private static void Continuation(Task<TResult> task, object state)
        {
            object[] args = state as object[];
            if (!(args[0] as TWorker).Token.IsCancellationRequested)
                (args[1] as Action<Task<TResult>>).Invoke(task);
        }

        public static BackgroundJobManager<TWorker, TResult> FromResult(TResult result)
        {
            BackgroundJobManager<TWorker, TResult> manager = new BackgroundJobManager<TWorker, TResult>();
            manager._currentTask = Task<TResult>.FromResult(result);
            manager._currentTokenSource = null;
            manager._currentWorker = null;
            return manager;
        }

        private BackgroundJobManager() { }

        public BackgroundJobManager(TWorker worker, bool onDemand = false)
        {
            if (worker == null)
                throw new ArgumentNullException("worker");

            _currentWorker = worker;
            _currentTokenSource = new CancellationTokenSource();
            _currentTask = worker.CreateTask(_currentTokenSource.Token, null, onDemand);
        }

        public void Replace(TWorker worker, bool onDemand = false)
        {
            if (worker == null)
                throw new ArgumentNullException("worker");

            lock (_syncRoot)
            {
                if (_currentTokenSource != null)
                {
                    if (_currentTask.Status == TaskStatus.Created)
                        _currentTokenSource.Dispose();
                    else
                    {
                        _currentTokenSource.Cancel();
                        _currentTask.ContinueWith((t, s) => (s as CancellationTokenSource).Dispose(), _currentTokenSource as object);
                    }
                }
                _currentTokenSource = new CancellationTokenSource();
                _currentTask = worker.CreateTask(_currentTokenSource.Token, _currentWorker, onDemand);
                _currentWorker = worker;
            }
        }

        public void Cancel()
        {
            if (_currentTokenSource != null)
            {
                _currentTokenSource.Cancel();
                if (_currentTask.Status == TaskStatus.Created)
                {
                    _currentTask = Task<TResult>.FromCanceled<TResult>(_currentTokenSource.Token);
                    _currentTokenSource.Dispose();
                }
                else
                    _currentTask.ContinueWith((t, s) => (s as CancellationTokenSource).Dispose(), _currentTokenSource as object);
                _currentTokenSource = null;
            }
        }

        public void SetResult(TResult result)
        {
            lock (_syncRoot)
            {
                if (_currentTokenSource != null)
                {
                    if (_currentTask.Status == TaskStatus.Created)
                        _currentTokenSource.Dispose();
                    else
                    {
                        _currentTokenSource.Cancel();
                        _currentTask.ContinueWith((t, s) => (s as CancellationTokenSource).Dispose(), _currentTokenSource as object);
                    }
                    _currentTokenSource = null;
                }
                _currentTask = Task<TResult>.FromResult(result);
            }
        }
    }
}
