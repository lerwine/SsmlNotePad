using System;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class ValidationCompletedEventArgs : EventArgs
    {
        private ValidationError[] _result;
        private bool _isCanceled;
        private Exception _fault;

        public ValidationError[] Result { get { return _result; } }
        public bool IsCanceled { get { return _isCanceled; } }
        public Exception Fault { get { return _fault; } }

        public ValidationCompletedEventArgs()
        {
            _isCanceled = true;
            _fault = null;
            _result = new ValidationError[0];
        }

        public ValidationCompletedEventArgs(Exception fault)
        {
            _isCanceled = false;
            _fault = fault;
            _result = new ValidationError[0];
        }

        public ValidationCompletedEventArgs(ValidationError[] result)
        {
            _isCanceled = false;
            _fault = null;
            _result = result ?? new ValidationError[0];
        }
    }
}