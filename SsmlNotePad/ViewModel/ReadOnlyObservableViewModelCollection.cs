using System.Collections.ObjectModel;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ReadOnlyObservableViewModelCollection<T> : ReadOnlyObservableCollection<T>
        where T : class
    {
        public new ObservableViewModelCollection<T> Items { get { return base.Items as ObservableViewModelCollection<T>; } }

        public ReadOnlyObservableViewModelCollection(ObservableViewModelCollection<T> list) : base(list) { }
    }
}
