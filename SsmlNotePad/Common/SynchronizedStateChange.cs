using System;
using System.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    /// <summary>
    /// Represents a synchronized change to the <typeparamref name="TState"/> value of a <seealso cref="SynchronizedState{TState}"/> object.
    /// </summary>
    /// <typeparam name="TState">Type of state value to be changed.</typeparam>
    /// <remarks>Until this object is disposed, the value of <seealso cref="SynchronizedState{TState}.CurrentState"/> on the associated object cannot be modified by any
    /// subsequently instantiated <see cref="SynchronizedStateChange{TState}"/> object.
    /// When this object is disposed, the value of <see cref="NewState"/> will be applied to <seealso cref="SynchronizedState{TState}.CurrentState"/>.</remarks>
    public class SynchronizedStateChange<TState> : IDisposable
    {
        private ManualResetEvent _stateNotChangingEvent;
        private SynchronizedState<TState> _stateObj;
        private Action<object, TState> _setResultState;

        /// <summary>
        /// Current state value.
        /// </summary>
        public TState CurrentState { get { return _stateObj.CurrentState; } }

        /// <summary>
        /// New state value to be applied to <seealso cref="SynchronizedState{TState}.CurrentState"/> when this object is disposed.
        /// </summary>
        public TState NewState { get; set; }

        /// <summary>
        /// User state value passed to the <seealso cref="SynchronizedState{TState}.ChangeState(object)"/> method.
        /// </summary>
        public object UserState { get; private set; }
        
        internal SynchronizedStateChange(ManualResetEvent prevStateNotChangingEvent, ManualResetEvent currentStateNotChangingEvent, SynchronizedState<TState> stateObj, object userState, Action<object, TState> setResultState)
        {
            if (prevStateNotChangingEvent == null)
                throw new ArgumentNullException("prevStateNotChangingEvent");

            if (currentStateNotChangingEvent == null)
                throw new ArgumentNullException("currentStateNotChangingEvent");

            if (setResultState == null)
                throw new ArgumentNullException("setResultState");

            _stateNotChangingEvent = currentStateNotChangingEvent;
            _setResultState = setResultState;
            _stateObj = stateObj;
            prevStateNotChangingEvent.WaitOne();
            prevStateNotChangingEvent.Dispose();
            NewState = stateObj.CurrentState;
            UserState = userState;
        }

        internal SynchronizedStateChange(ManualResetEvent prevStateNotChangingEvent, ManualResetEvent currentStateNotChangingEvent, SynchronizedState<TState> stateObj, Action<object, TState> setResultState)
            : this(prevStateNotChangingEvent, currentStateNotChangingEvent, stateObj, null, setResultState) { }

        #region IDisposable Support

        private bool _isDisposed = false; // To detect redundant calls

        /// <summary>
        /// Occurs when this object is being disposed.
        /// </summary>
        /// <param name="disposing">true if this was invoked through the <see cref="Dispose"/> method, otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            bool isDisposed = _isDisposed;
            _isDisposed = true;
            if (isDisposed || !disposing)
                return;
            try { _setResultState(UserState, NewState); }
            catch { throw; }
            finally { _stateNotChangingEvent.Set(); }
        }
        
        /// <summary>
        /// Before disposing this object, the value of <see cref="NewState"/> applied to the associated <seealso cref="SynchronizedState{TState}.CurrentState"/>
        /// property, and allows the next (if any) subsequent <see cref="SynchronizedStateChange{TState}"/> object to modify that property.
        /// </summary>
        public void Dispose() { Dispose(true); }

        #endregion
    }
}