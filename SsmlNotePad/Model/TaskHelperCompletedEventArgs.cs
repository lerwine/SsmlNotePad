using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class TaskHelperCompletedEventArgs
    {
        public Task Task { get; set; }

        public TaskHelperCompletedEventArgs(Task task) { this.Task = task; }

        public TaskHelperCompletedEventArgs() { }
    }

    public class TaskHelperCompletedEventArgs<TResult>
    {
        public Task<TResult> Task { get; set; }

        public TaskHelperCompletedEventArgs(Task<TResult> task) { Task = task; }

        public TaskHelperCompletedEventArgs() { }
    }
}