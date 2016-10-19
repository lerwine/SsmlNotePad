using System;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class SynchronizedStateEventArgs<TState>
    {
        public TState PrevioiusState { get; private set; }

        public TState CurrentState { get; private set; }

        public Exception Error { get; private set; }

        public object UserState { get; private set; }

        public SynchronizedStateEventArgs(TState previousState, TState currentState, Exception error, object userState)
        {
            PrevioiusState = previousState;
            CurrentState = currentState;
            Error = error;
            UserState = userState;
        }

        public SynchronizedStateEventArgs(TState previousState, TState currentState, object userState) : this(previousState, currentState, null, userState) { }

        public SynchronizedStateEventArgs(TState state, Exception error, object userState) : this(state, state, error, userState) { }

        public SynchronizedStateEventArgs(TState state, object userState) : this(state, state, userState) { }
    }
}