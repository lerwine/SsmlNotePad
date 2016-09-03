namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Command
{
    public interface IHandlableEventArgs
    {
        bool Handled { get; set; }
    }

    public interface IHandlableEventArgs<T> : IHandlableEventArgs
    {
        T Value { get; set; }
    }
}