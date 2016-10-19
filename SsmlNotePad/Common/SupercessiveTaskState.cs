using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public class SupercessiveTaskState<TSource, TResult> : SynchronizedState<SupercedableTaskState<TSource, TResult>>
    {
        private SynchronizedState<int> _currentTaskId;
        private TResult _currentResult;
        private Exception _currentError = null;

        public const string PropertyName_CurrentResult = "CurrentResult";
        public const string PropertyName_CurrentError = "CurrentError";

        public TResult CurrentResult
        {
            get { return _currentResult; }
            set
            {
                if (EqualityComparer<TResult>.Default.Equals(_currentResult, value))
                    return;

                try { RaisePropertyChanging(PropertyName_CurrentResult); }
                finally
                {
                    _currentResult = value;
                    RaisePropertyChanged(PropertyName_CurrentResult);
                }
            }
        }

        public Exception CurrentError
        {
            get { return _currentError; }
            set
            {
                if ((_currentError == null) ? value == null: value != null && ReferenceEquals(_currentError, value))
                    return;

                try { RaisePropertyChanging(PropertyName_CurrentError); }
                finally
                {
                    _currentError = value;
                    RaisePropertyChanged(PropertyName_CurrentError);
                }
            }
        }

        public SupercessiveTaskState(TSource source, TResult initialValue) : base(new SupercedableTaskState<TSource, TResult>(source, initialValue))
        {
            _currentResult = initialValue;
            _currentTaskId = new Common.SynchronizedState<int>(CurrentState.TaskId);
        }
        
        public void StartNew(TSource source, TResult value)
        {
            using (SynchronizedStateChange<SupercedableTaskState<TSource, TResult>> stateChange = ChangeState())
                stateChange.NewState = SupercedableTaskState<TSource, TResult>.CheckSupercede(stateChange.CurrentState, source, value);
        }

        public void StartNew(TSource source, Func<TSource, CancellationToken, TResult> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            using (SynchronizedStateChange<SupercedableTaskState<TSource, TResult>> stateChange = ChangeState())
                stateChange.NewState = SupercedableTaskState<TSource, TResult>.CheckSupercede(stateChange.CurrentState, source, function);
        }

        protected override void State_Changed(SynchronizedStateEventArgs<SupercedableTaskState<TSource, TResult>> args)
        {
            using (SynchronizedStateChange<int> idChange = _currentTaskId.ChangeState())
                idChange.NewState = args.CurrentState.TaskId;

            args.CurrentState.ContinueWith(StateChangeContinuation);
        }

        private void StateChangeContinuation(Task<TResult> task)
        {
            using (SynchronizedStateChange<int> idChange = _currentTaskId.ChangeState())
            {
                if (task.Id == _currentTaskId.CurrentState)
                {
                    try { CurrentResult = task.Result; }
                    catch (Exception exception) { CurrentError = exception; }
                }
            }
        }
    }
}
