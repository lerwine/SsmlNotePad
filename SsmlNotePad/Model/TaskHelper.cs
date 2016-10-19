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
        private ManualResetEvent _taskInactiveEvent = new ManualResetEvent(true);
        public ManualResetEvent TaskInactiveEvent { get { return _taskInactiveEvent; } }
        public bool _taskActive = false;
        private ManualResetEvent _taskSwitchEvent = new ManualResetEvent(true);
        private Task _currentTask = null;
        private CancellationTokenSource _tokenSource = null;
        private List<Tuple<Action<Task, object>, object>> _continueWith = new List<Tuple<Action<Task, object>, object>>();

        public bool TaskActive { get { return _taskActive; } }

        public void WaitOne(int millisecondsTimeout) { _taskInactiveEvent.WaitOne(millisecondsTimeout); }

        public void WaitOne() { _taskInactiveEvent.WaitOne(); }
        
        public void ContinueWith(Action<Task, object> continuation, object state)
        {
            ManualResetEvent taskSwitchEvent;
            lock (_syncRoot)
            {
                taskSwitchEvent = _taskSwitchEvent;
                _taskSwitchEvent = new ManualResetEvent(false);
            }
            try
            {
                taskSwitchEvent.WaitOne();
                // if (task completed) { start task to execute continuation } else { add to queue }
            }
            catch { throw; }
            finally { _taskSwitchEvent.Set(); }
        }

        protected void StartNew(Action<object> action, object state)
        {
            ManualResetEvent taskSwitchEvent;
            lock (_syncRoot)
            {
                taskSwitchEvent = _taskSwitchEvent;
                _taskSwitchEvent = new ManualResetEvent(false);
            }
            try
            {
                taskSwitchEvent.WaitOne();
                if (_tokenSource != null)
                {
                    if (!(_currentTask.IsCompleted || _tokenSource.IsCancellationRequested))
                        _tokenSource.Cancel();
                    _tokenSource.Dispose();
                }
                else
                {
                    _taskActive = true;
                    _taskInactiveEvent.Reset();
                }
                _tokenSource = new CancellationTokenSource();
                _currentTask = new Task(action, state, _tokenSource.Token);
            }
            catch { throw; }
            finally { _taskSwitchEvent.Set(); }
        }

        private void TaskCompleted(Task task)
        {
            ManualResetEvent taskSwitchEvent;
            lock (_syncRoot)
            {
                taskSwitchEvent = _taskSwitchEvent;
                _taskSwitchEvent = new ManualResetEvent(false);
            }
            try
            {
                taskSwitchEvent.WaitOne();
                if (_currentTask.Id == task.Id)
                {
                    _tokenSource.Dispose();
                    _tokenSource = null;
                    _taskActive = false;
                    _taskInactiveEvent.Set();
                }
            }
            catch { throw; }
            finally { _taskSwitchEvent.Set(); }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            //CancellationTokenSource tokenSource;

            //lock (_syncRoot)
            //{
            //    if (TokenSource == null)
            //        return;

            //    tokenSource = TokenSource;
            //    TokenSource = null;
            //}

            //if (!tokenSource.IsCancellationRequested)
            //    tokenSource.Cancel();

            //tokenSource.Dispose();
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() { Dispose(true); }

        #endregion
    }
}
