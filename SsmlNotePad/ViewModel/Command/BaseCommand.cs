using System;
using System.Threading;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Command
{
    /// <summary>
    /// Base class for commands
    /// </summary>
    public abstract class BaseCommand : DependencyObject, System.Windows.Input.ICommand
    {
        private readonly object _syncRoot = new object();
        private bool _allowSimultaneousExecute2;
        private bool _canExecute = true;
        private int _execCount = 0;

        public event EventHandler CanExecuteChanged;

        #region IsEnabled Property Members

        public const string DependencyPropertyName_IsEnabled = "IsEnabled";

        /// <summary>
        /// Identifies the <seealso cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(DependencyPropertyName_IsEnabled, typeof(bool), typeof(BaseCommand),
                new PropertyMetadata(true,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as BaseCommand).IsEnabled_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// True if the command is explicitly enabled; otherwise false.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(IsEnabledProperty));
                return Dispatcher.Invoke(() => IsEnabled);
            }
            set
            {
                if (CheckAccess())
                    SetValue(IsEnabledProperty, value);
                else
                    Dispatcher.Invoke(() => IsEnabled = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="IsEnabled"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="IsEnabled"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="IsEnabled"/> property was changed.</param>
        protected virtual void IsEnabled_PropertyChanged(bool oldValue, bool newValue)
        {
            if (CheckAccess())
                UpdateCanExecute();
            else
                Dispatcher.Invoke(() => IsEnabled_PropertyChanged(oldValue, newValue));
        }

        #endregion

        #region AllowSimultaneousExecute Property Members

        public const string DependencyPropertyName_AllowSimultaneousExecute = "AllowSimultaneousExecute";

        /// <summary>
        /// Identifies the <seealso cref="AllowSimultaneousExecute"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowSimultaneousExecuteProperty = DependencyProperty.Register(DependencyPropertyName_AllowSimultaneousExecute, typeof(bool), typeof(BaseCommand),
                new PropertyMetadata(false,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as BaseCommand).AllowSimultaneousExecute_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue)),
                (DependencyObject d, object baseValue) => (d as BaseCommand).AllowSimultaneousExecute_CoerceValue(baseValue)));

        /// <summary>
        /// True if multiple invocations can be executed simultaneously; otherwise false.
        /// </summary>
        public bool AllowSimultaneousExecute
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(AllowSimultaneousExecuteProperty));
                return Dispatcher.Invoke(() => AllowSimultaneousExecute);
            }
            set
            {
                if (CheckAccess())
                    SetValue(AllowSimultaneousExecuteProperty, value);
                else
                    Dispatcher.Invoke(() => AllowSimultaneousExecute = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="AllowSimultaneousExecute"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="AllowSimultaneousExecute"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="AllowSimultaneousExecute"/> property was changed.</param>
        protected virtual void AllowSimultaneousExecute_PropertyChanged(bool oldValue, bool newValue)
        {
            if (CheckAccess())
                UpdateCanExecute();
            else
                Dispatcher.Invoke(() => AllowSimultaneousExecute_PropertyChanged(oldValue, newValue));
        }

        /// <summary>
        /// This gets called whenever <seealso cref="AllowSimultaneousExecute"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual bool AllowSimultaneousExecute_CoerceValue(object baseValue)
        {
            // TODO: Implement BaseCommand.AllowSimultaneousExecute_CoerceValue(DependencyObject, object)
            return (bool)baseValue;
        }

        #endregion

        /// <summary>
        /// Initialize a new command that does not allow simultaneous execution.
        /// </summary>
        protected BaseCommand() : this(false) { }

        /// <summary>
        /// Initialize a new command.
        /// </summary>
        /// <param name="allowSimultaneousExecute">true if the command can be invoked before the previous invocation completes; otherwise false.</param>
        /// <param name="isDisabled">True if the command is initially disabled; otherwise false.</param>
        protected BaseCommand(bool allowSimultaneousExecute, bool isDisabled = false)
        {
            IsEnabled = !isDisabled;
            AllowSimultaneousExecute = allowSimultaneousExecute;
        }

        private void UpdateCanExecute()
        {
            bool couldExecute, canExecute;
            Monitor.Enter(_syncRoot);
            try
            {
                couldExecute = _canExecute;
                canExecute = _CanExecute();
                _canExecute = canExecute;
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
            if (couldExecute != canExecute)
                RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Determines whether this <see cref="BaseCommand"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                return OnCanExecute(parameter);
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Thread-safe method to Determine whether this <see cref="BaseCommand"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        protected virtual bool OnCanExecute(object parameter)
        {
            return _CanExecute();
        }

        private bool _CanExecute()
        {
            return IsEnabled && (AllowSimultaneousExecute || _execCount == 0);
        }

        /// <summary>
        /// Executes the <see cref="BaseCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public virtual void Execute(object parameter)
        {
            try
            {
                _execCount++;
                UpdateCanExecute();
                OnExecute(parameter);
            }
            catch { throw; }
            finally
            {
                _execCount--;
                UpdateCanExecute();
            }
        }

        /// <summary>
        /// This gets called when the <see cref="BaseCommand"/> is being executed on the current command target.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        protected abstract void OnExecute(object parameter);

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event to indicate that the return value of the <see cref="CanExecute"/> method has changed.
        /// </summary>
        protected void RaiseCanExecuteChanged() { CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
    }

    /// <summary>
    /// Base class for commands with explicitly typed arguments.
    /// </summary>
    /// <typeparam name="THandlableEventArgs">Type of argument that can be passed to the command.</typeparam>
    public abstract class BaseCommand<THandlableEventArgs> : BaseCommand
        where THandlableEventArgs : IHandlableEventArgs
    {
        /// <summary>
        /// Initialize a new command that does not allow simultaneous execution.
        /// </summary>
        protected BaseCommand() : base() { }

        /// <summary>
        /// Initialize a new command.
        /// </summary>
        /// <param name="allowSimultaneousExecute">true if the command can be invoked before the previous completes; otherwise false.</param>
        /// <param name="isDisabled">True if the command is initially disabled; otherwise false.</param>
        protected BaseCommand(bool allowSimultaneousExecute, bool isDisabled = false) : base(allowSimultaneousExecute, isDisabled) { }

        /// <summary>
        /// Thread-safe method to Determine whether this <see cref="BaseCommand"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        protected virtual bool OnCanExecute(THandlableEventArgs parameter) { return base.OnCanExecute(parameter); }

        protected override bool OnCanExecute(object parameter) { return OnCanExecute((THandlableEventArgs)parameter); }

        /// <summary>
        /// Executes the <see cref="BaseCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        public virtual void Execute(THandlableEventArgs parameter) { base.Execute(parameter); }

        public override void Execute(object parameter) { Execute((THandlableEventArgs)parameter); }

        /// <summary>
        /// This gets called when the <see cref="BaseCommand"/> is being executed on the current command target.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        protected abstract void OnExecute(THandlableEventArgs parameter);

        protected override void OnExecute(object parameter) { OnExecute((THandlableEventArgs)parameter); }
    }
}
