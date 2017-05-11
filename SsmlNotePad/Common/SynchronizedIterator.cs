using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class SynchronizedIterator<TSource, TTarget>
    {
        IList<TSource> _source;
        IList<TTarget> _target;
        public SynchronizedIterator(IList<TSource> source, IList<TTarget> target)
        {
            _source = source;
            _target = target;
        }
        
    }

    //public class CancellableJob : IAsyncResult, IDisposable
    //{
    //    private object _syncRoot = new object();
    //    private ManualResetEvent _completedWaitHandle = new ManualResetEvent(false);
    //    private Task _task;
    //    private object _asyncState;
    //    private bool _isCompleted;
    //    private bool _isCanceled;
    //    private bool _isFaulted;
    //    private TaskStatus _status;
    //    private Exception _exception;
    //    private int _taskId;

    //    public object AsyncState { get { return _asyncState; } }

    //    WaitHandle IAsyncResult.AsyncWaitHandle { get { return _completedWaitHandle; } }

    //    bool IAsyncResult.CompletedSynchronously
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public bool IsCompleted
    //    {
    //        get
    //        {
    //            lock (_syncRoot)
    //            {
    //                if (_isCompleted)
    //                    return true;
    //                if (!_task.IsCompleted)
    //                    return false;

    //                _UpdateFromTask();
    //                return true;
    //            }
    //        }
    //    }

    //    private void _UpdateFromTask()
    //    {
    //        _isCompleted = true;
    //        _isCanceled = _task.IsCompleted;
    //        if (!_isCanceled)
    //        {
    //            _isFaulted = _task.IsFaulted;
    //            if (_isFaulted)
    //                _exception = _task.Exception;
    //        }
    //        _status = _task.Status;
    //        _taskId = _task.Id;
    //    }

    //    public bool IsCanceled
    //    {
    //        get
    //        {
    //            lock (_syncRoot)
    //            {
    //                if (_isCompleted)
    //                    return _isCanceled;

    //                return _task.IsCanceled;
    //            }
    //        }
    //    }

    //    public bool IsFaulted
    //    {
    //        get
    //        {
    //            lock (_syncRoot)
    //            {
    //                if (_isCompleted)
    //                    return _isFaulted;

    //                return _task.IsFaulted;
    //            }
    //        }
    //    }

    //    public TaskStatus Status
    //    {
    //        get
    //        {
    //            lock (_syncRoot)
    //            {
    //                if (_isCompleted)
    //                    return _status;

    //                return _task.Status;
    //            }
    //        }
    //    }

    //    public Exception Exception
    //    {
    //        get
    //        {
    //            lock (_syncRoot)
    //            {
    //                if (_isCompleted)
    //                    return _exception;

    //                return _task.Exception;
    //            }
    //        }
    //    }

    //    public int TaskId
    //    {
    //        get
    //        {
    //            lock (_syncRoot)
    //            {
    //                if (_isCompleted)
    //                    return _taskId;

    //                return _task.Id;
    //            }
    //        }
    //    }

    //    #region IDisposable Support

    //    private bool disposedValue = false; // To detect redundant calls

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!disposedValue)
    //        {
    //            if (disposing)
    //            {
    //                // TODO: dispose managed state (managed objects).
    //            }

    //            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
    //            // TODO: set large fields to null.

    //            disposedValue = true;
    //        }
    //    }

    //    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    //    // ~CancellableJob() {
    //    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //    //   Dispose(false);
    //    // }

    //    // This code added to correctly implement the disposable pattern.
    //    public void Dispose()
    //    {
    //        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //        Dispose(true);
    //        // TODO: uncomment the following line if the finalizer is overridden above.
    //        // GC.SuppressFinalize(this);
    //    }
    //    #endregion
    //}
}
