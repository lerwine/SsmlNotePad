using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    /// <summary>
    /// Synchronizes state update across threads.
    /// </summary>
    /// <typeparam name="TState">Type of state value to synchronize.</typeparam>
    /// <remarks>The purpose of this class is to prevent any concurrent modifications to the <see cref="CurrentState"/> property, which represents the state value.</remarks>
    public class SynchronizedState<TState> : IDisposable, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private object _syncRoot = new object();
        private Dispatcher _dispatcher;
        private AutoResetEvent _stateChangedEvent = new AutoResetEvent(false);
        private bool _stateChanging = false;
        private ManualResetEvent _stateChangeActiveEvent = new ManualResetEvent(false);
        private ManualResetEvent _stateChangeInactiveEvent = new ManualResetEvent(true);
        private ManualResetEvent _stateNotChangingEvent = new ManualResetEvent(true);
        private TState _currentState;
        private IEqualityComparer<TState> _comparer;

        public const string PropertyName_CurrentState = "CurrentState";
        public const string PropertyName_StateChanging = "StateChanging";

        public bool StateChanging
        {
            get { return _stateChanging; }
            private set
            {
                if (_stateChanging == value)
                    return;

                try { RaisePropertyChanging(PropertyName_StateChanging); }
                finally
                {
                    _stateChanging = value;
                    RaisePropertyChanged(PropertyName_StateChanging);
                }
            }
        }

        /// <summary>
        /// Current state value.
        /// </summary>
        public TState CurrentState { get { return _currentState; } }

        /// <summary>
        /// Occurs when <see cref="CurrentState"/> is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Occurs when <see cref="CurrentState"/> has changed.
        /// </summary>
        public event EventHandler<SynchronizedStateEventArgs<TState>> OnStateChanged;

        /// <summary>
        /// Occurs when <see cref="CurrentState"/> has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> is invoked.
        /// </summary>
        public event EventHandler<SynchronizedStateEventArgs<TState>> OnStartStateChange;

        /// <summary>
        /// Occurs when the <seealso cref="SynchronizedStateChange{TState}"/> object created by <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> 
        /// is being disposed.
        /// </summary>
        public event EventHandler<SynchronizedStateEventArgs<TState>> OnEndStateChange;

        /// <summary>
        /// Blocks the current thread until <see cref="CurrentState"/> has changed.
        /// </summary>
        public void WaitStateChanged()
        {
            AutoResetEvent e = _stateChangedEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

            e.WaitOne();
        }

        /// <summary>
        /// Blocks the current thread for a specified duration or until <see cref="CurrentState"/> has changed, whichever comes first.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <seealso cref="System.Threading.Timeout.Infinite"/> to wait indefinitely.</param>
        /// <returns>true if <see cref="CurrentState"/> has changed; otherwise, false.</returns>
        public bool WaitStateChanged(int millisecondsTimeout)
        {
            AutoResetEvent e = _stateChangedEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

            return e.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Blocks the current thread for a specified duration or until <see cref="CurrentState"/> has changed, whichever comes first.
        /// </summary>
        /// <param name="timeout">A <seealso cref="TimeSpan"/> that represents the number of milliseconds to wait, or a <seealso cref="TimeSpan"/> that represents -1
        /// milliseconds to wait indefinitely.</param>
        /// <returns>true if <see cref="CurrentState"/> has changed; otherwise, false.</returns>
        public bool WaitStateChanged(TimeSpan timeout)
        {
            AutoResetEvent e = _stateChangedEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);
            
            return e.WaitOne(timeout);
        }

        /// <summary>
        /// Blocks the current thread until a state change is in progress.
        /// </summary>
        /// <remarks>A state change is considered to be in progress while there are any <seealso cref="SynchronizedStateChange{TState}"/> objects created by
        /// <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> which remain indisposed.</remarks>
        public void WaitStateChangeActive()
        {
            ManualResetEvent e = _stateChangeActiveEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

            e.WaitOne();
        }

        /// <summary>
        /// Blocks the current thread for a specified duration or until a state change is in progress, whichever comes first.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <seealso cref="System.Threading.Timeout.Infinite"/> to wait indefinitely.</param>
        /// <remarks>A state change is considered to be in progress while there are any <seealso cref="SynchronizedStateChange{TState}"/> objects created by
        /// <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> which remain indisposed.</remarks>
        public bool WaitStateChangeActive(int millisecondsTimeout)
        {
            ManualResetEvent e = _stateChangeActiveEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

            return e.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Blocks the current thread for a specified duration or until a state change is in progress, whichever comes first.
        /// </summary>
        /// <param name="timeout">A <seealso cref="TimeSpan"/> that represents the number of milliseconds to wait, or a <seealso cref="TimeSpan"/> that represents -1 
        /// milliseconds to wait indefinitely.</param>
        /// <returns>true if a state change is in progress; otherwise, false.</returns>
        /// <remarks>A state change is considered to be in progress while there are any <seealso cref="SynchronizedStateChange{TState}"/> objects created by
        /// <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> which remain indisposed.</remarks>
        public bool WaitStateChangeActive(TimeSpan timeout)
        {
            ManualResetEvent e = _stateChangeActiveEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

            return e.WaitOne(timeout);
        }

        /// <summary>
        /// Blocks the current thread while a state change is in progress.
        /// </summary>
        /// <remarks>A state change is considered to be no longer in progress when no existing <seealso cref="SynchronizedStateChange{TState}"/> objects created by
        /// <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> remain indisposed.</remarks>
        public void WaitStateChangeInactive()
        {
            ManualResetEvent e = _stateChangeInactiveEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

            e.WaitOne();
        }

        /// <summary>
        /// Blocks the current thread for a specified duration or until a state change is no longer in progress, whichever comes first.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <seealso cref="System.Threading.Timeout.Infinite"/> to wait indefinitely.</param>
        /// <remarks>A state change is considered to be no longer in progress when no existing <seealso cref="SynchronizedStateChange{TState}"/> objects created by
        /// <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> remain indisposed.</remarks>
        public bool WaitStateChangeInactive(int millisecondsTimeout)
        {
            ManualResetEvent e = _stateChangeInactiveEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

            return e.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Blocks the current thread for a specified duration or until a state change is no longer in progress, whichever comes first.
        /// </summary>
        /// <param name="timeout">A <seealso cref="TimeSpan"/> that represents the number of milliseconds to wait, or a <seealso cref="TimeSpan"/> that represents -1 
        /// milliseconds to wait indefinitely.</param>
        /// <remarks>A state change is considered to be no longer in progress when no existing <seealso cref="SynchronizedStateChange{TState}"/> objects created by
        /// <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> remain indisposed.</remarks>
        public bool WaitStateChangeInactive(TimeSpan timeout)
        {
            ManualResetEvent e = _stateChangeInactiveEvent;
            if (e == null)
                throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

            return e.WaitOne(timeout);
        }

        /// <summary>
        /// Intialize new <see cref="SynchronizedState{TState}"/> object with an initial state value and optional custom <seealso cref="IEqualityComparer{TState}"/>.
        /// </summary>
        /// <param name="initialState"><typeparamref name="TState"/> value to use as the initial state.</param>
        /// <param name="comparer">Optional custom comparer to use when detecting a change to the <see cref="CurrentState"/> value.
        /// If null, then <seealso cref="EqualityComparer{TState}"/> will be used.</param>
        public SynchronizedState(TState initialState, IEqualityComparer<TState> comparer)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _currentState = initialState;
            _comparer = comparer ?? EqualityComparer<TState>.Default;
        }

        /// <summary>
        /// Intialize new <see cref="SynchronizedState{TState}"/> object with an initial state value and default <seealso cref="IEqualityComparer{TState}"/>.
        /// </summary>
        /// <param name="initialState"><typeparamref name="TState"/> value to use as the initial state.</param>
        public SynchronizedState(TState initialState) : this(initialState, null) { }

        /// <summary>
        /// Intialize new <see cref="SynchronizedState{TState}"/> object with the default state value and <seealso cref="IEqualityComparer{TState}"/>.
        /// </summary>
        public SynchronizedState() : this(default(TState)) { }

        public Task ChangeStateAsync(object userState, Action<SynchronizedStateChange<TState>> action)
        {
            return Task.Factory.StartNew(o =>
            {
                object[] args = o as object[];
                Action<SynchronizedStateChange<TState>> a = args[0] as Action<SynchronizedStateChange<TState>>;
                using (SynchronizedStateChange<TState> changeState = ChangeState(args[1]))
                    a(changeState);
            }, new object[] { action, userState });
        }

        public Task ChangeStateAsync(Action<SynchronizedStateChange<TState>> action) { return ChangeStateAsync(null, action); }
        
        /// <summary>
        /// Create new <seealso cref="SynchronizedStateChange{TState}"/> object to for changing the value of <see cref="CurrentState"/>.
        /// </summary>
        /// <param name="userState">User state object to associate with this state change.</param>
        /// <returns>A <seealso cref="SynchronizedStateChange{TState}"/> object which is can change the <see cref="CurrentState"/> value.</returns>
        /// <remarks>Warning! Do not invoke this concurrently on the same thread, or it will cause a perpetual lock condition.</remarks>
        public SynchronizedStateChange<TState> ChangeState(object userState)
        {
            if (_dispatcher.CheckAccess())
                throw new InvalidOperationException("State change is already in progress on the same thread.");

            ManualResetEvent prevStateNotChangingEvent, currentStateNotChangingEvent;
            bool startStateChange;
            lock (_syncRoot)
            {
                if (_stateNotChangingEvent == null)
                    throw new ObjectDisposedException(typeof(SynchronizedState<TState>).FullName);

                startStateChange = !StateChanging;
                if (startStateChange)
                {
                    _stateChangeActiveEvent.Set();
                    _stateChangeInactiveEvent.Reset();
                    StateChanging = true;
                }
                prevStateNotChangingEvent = _stateNotChangingEvent;
                currentStateNotChangingEvent = new ManualResetEvent(false);
                _stateNotChangingEvent = currentStateNotChangingEvent;
                Task.Factory.StartNew(ChangeWaitAction, currentStateNotChangingEvent);
            }
            
            try
            {
                if (startStateChange)
                    State_ChangeStart(new SynchronizedStateEventArgs<TState>(_currentState, userState));
            }
            catch (Exception exception)
            {
                prevStateNotChangingEvent.WaitOne();
                prevStateNotChangingEvent.Dispose();
                currentStateNotChangingEvent.Set();
                State_ChangeEnd(new SynchronizedStateEventArgs<TState>(_currentState, exception, userState));
                throw;
            }

            return new SynchronizedStateChange<TState>(prevStateNotChangingEvent, currentStateNotChangingEvent, this, userState, ApplyState);
        }

        /// <summary>
        /// Create new <seealso cref="SynchronizedStateChange{TState}"/> object to for changing the value of <see cref="CurrentState"/>.
        /// </summary>
        /// <returns>A <seealso cref="SynchronizedStateChange{TState}"/> object which is can change the <see cref="CurrentState"/> value.</returns>
        public SynchronizedStateChange<TState> ChangeState() { return ChangeState(null); }

        protected virtual void RaisePropertyChanging(string propertyName)
        {
            if (_dispatcher.CheckAccess())
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            else
                _dispatcher.BeginInvoke(new Action<string>(n => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(n))), propertyName);
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (_dispatcher.CheckAccess())
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            else
                _dispatcher.BeginInvoke(new Action<string>(n => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n))), propertyName);
        }

        private void ApplyState(object userState, TState state)
        {
            bool stateChanged;
            TState previousState;
            lock (_syncRoot)
            {
                previousState = _currentState;
                stateChanged = !_comparer.Equals(state, previousState);
            }
            
            try
            {
                if (stateChanged && _stateChangedEvent != null)
                    RaisePropertyChanging(PropertyName_CurrentState);
            }
            finally
            {
                if (stateChanged)
                    _currentState = state;
                try
                {
                    try
                    {
                        if (stateChanged && _stateChangedEvent != null)
                            _stateChangedEvent.Set();
                    }
                    finally
                    {
                        if (_stateChangedEvent != null)
                            State_ChangeEnd(new SynchronizedStateEventArgs<TState>(previousState, state, userState));
                    }
                }
                finally
                {
                    if (stateChanged && _stateChangedEvent != null)
                    {
                        try { State_Changed(new SynchronizedStateEventArgs<TState>(previousState, state, userState)); }
                        finally { RaisePropertyChanged(PropertyName_CurrentState); }
                    }
                }
            }
        }

        /// <summary>
        ///  Occurs when <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> is invoked.
        /// </summary>
        /// <param name="args">Containing the value of <see cref="CurrentState"/> when the change was started.</param>
        protected virtual void State_ChangeStart(SynchronizedStateEventArgs<TState> args)
        {
            OnStartStateChange?.Invoke(this, args);
        }

        /// <summary>
        /// Occurs when the <seealso cref="SynchronizedStateChange{TState}"/> object created by <see cref="ChangeState"/> or <see cref="ChangeState(object)"/> 
        /// is being disposed.
        /// </summary>
        /// <param name="args">Containing the values of <see cref="CurrentState"/> when the change was started as well as the value when the current change was ended.</param>
        protected virtual void State_ChangeEnd(SynchronizedStateEventArgs<TState> args)
        {
            OnEndStateChange?.Invoke(this, args);
        }

        /// <summary>
        /// Occurs when <see cref="CurrentState"/> has changed.
        /// </summary>
        /// <param name="args">Containing the previous and current values of <see cref="CurrentState"/>.</param>
        protected virtual void State_Changed(SynchronizedStateEventArgs<TState> args)
        {
            OnStateChanged?.Invoke(this, args);
        }
        
        /// <summary>
        /// Waits for the state to finish changing and update public events
        /// </summary>
        /// <param name="currentStateNotChangingEvent">State change event to wait for</param>
        private void ChangeWaitAction(object currentStateNotChangingEvent)
        {
            (currentStateNotChangingEvent as ManualResetEvent).WaitOne();
            lock (_syncRoot)
            {
                if (ReferenceEquals(currentStateNotChangingEvent, _stateNotChangingEvent))
                {
                    _stateChangeActiveEvent.Reset();
                    _stateChangeInactiveEvent.Set();
                    StateChanging = false;
                }
            }
        }

        #region IDisposable Support
        
        /// <summary>
        /// Disposes internal event wait handles.
        /// </summary>
        /// <param name="disposing">true if this was invoked through the <see cref="IDisposable.Dispose"/> method, otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            EventWaitHandle stateNotChangingEvent, stateChangeActiveEvent, stateChangeInactiveEvent, stateChangedEvent;
            lock (_syncRoot)
            {
                if (_stateNotChangingEvent == null)
                    return;

                stateNotChangingEvent = (StateChanging) ? _stateNotChangingEvent : null;
                stateChangeActiveEvent = _stateChangeActiveEvent;
                stateChangeInactiveEvent = _stateChangeInactiveEvent;
                stateChangedEvent = _stateChangedEvent;
                _stateNotChangingEvent = null;
                _stateChangeActiveEvent = null;
                _stateChangeInactiveEvent = null;
                _stateChangedEvent = null;
                StateChanging = false;
            }

            if (stateNotChangingEvent == null)
                return;

            try
            {
                try
                {
                    try { stateChangedEvent.Set(); }
                    finally
                    {
                        if (!stateChangeInactiveEvent.WaitOne(10000))
                        {
                            stateChangeInactiveEvent.Set();
                            stateChangeInactiveEvent.Dispose();
                        }
                    }
                }
                finally
                {
                    if (stateChangeActiveEvent.WaitOne(100))
                    {
                        stateChangeActiveEvent.Reset();
                        stateChangeActiveEvent.Dispose();
                    }
                }
            }
            finally
            {
                if (!stateNotChangingEvent.WaitOne(10000))
                {
                    stateNotChangingEvent.Set();
                    stateNotChangingEvent.Dispose();
                }
            }
        }

        void IDisposable.Dispose() { Dispose(true); }

        #endregion

        public override int GetHashCode() { return _comparer.GetHashCode(_currentState); }

        public override bool Equals(object obj)
        {
            SynchronizedState<TState> s = obj as SynchronizedState<TState>;
            if (s == null)
                return false;

            if (ReferenceEquals(s, this))
                return true;

            lock (_syncRoot)
            {
                lock (s._syncRoot)
                    return _comparer.Equals(CurrentState, s._currentState);
            }
            
        }
    }
}
