using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class ObservableViewModelCollection<T> : ObservableCollection<T>
        where T : class
    {
        public Dispatcher Dispatcher { get; private set; }

        public ObservableViewModelCollection(Dispatcher dispatcher, IList<T> list) : this(dispatcher, (list == null) ? null : list.AsEnumerable()) { }

        public ObservableViewModelCollection(Dispatcher dispatcher, IEnumerable<T> collection)
            : base((collection == null) ? new T[0] : collection.Where(i => i != null).Distinct(ObjectReferenceComparer.Default))
        {
            Dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        public ObservableViewModelCollection(DispatcherObject obj, IList<T> list) : this(obj, (list == null) ? null : list.AsEnumerable()) { }

        public ObservableViewModelCollection(DispatcherObject obj, IEnumerable<T> collection) : this((obj == null) ? null : obj.Dispatcher, collection) { }

        public ObservableViewModelCollection(Dispatcher dispatcher) : base()
        {
            Dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        public ObservableViewModelCollection(DispatcherObject obj) : this((obj == null) ? null : obj.Dispatcher) { }

        public ObservableViewModelCollection(IList<T> list) : this(null as Dispatcher, (list == null) ? null : list.AsEnumerable()) { }

        public ObservableViewModelCollection(IEnumerable<T> collection) : this(null as Dispatcher, collection) { }

        public ObservableViewModelCollection() : this(null as Dispatcher) { }

        protected override void ClearItems()
        {
            if (Dispatcher.CheckAccess())
                base.ClearItems();
            else
                Dispatcher.Invoke(() => base.ClearItems());
        }

        protected override void InsertItem(int index, T item)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => InsertItem(index, item));
                return;
            }

            if (item == null)
                throw new ArgumentNullException("item");

            if (this.Any(o => ReferenceEquals(o, item)))
                throw new InvalidOperationException("Item already exists in this collection.");

            base.InsertItem(index, item);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (Dispatcher.CheckAccess())
                base.MoveItem(oldIndex, newIndex);
            else
                Dispatcher.Invoke(() => base.MoveItem(oldIndex, newIndex));
        }

        protected override void RemoveItem(int index)
        {
            if (Dispatcher.CheckAccess())
                base.RemoveItem(index);
            else
                Dispatcher.Invoke(() => base.RemoveItem(index));
        }

        protected override void SetItem(int index, T item)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => SetItem(index, item));
                return;
            }

            if (item == null)
                throw new ArgumentNullException("item");

            if ((index > 0 && this.Take(index).Any(o => ReferenceEquals(o, item))) ||
                    (index < Count - 1 && this.Skip(index + 1).Any(o => ReferenceEquals(o, item))))
                throw new InvalidOperationException("Item already exists in this collection.");

            base.SetItem(index, item);
        }

        public class ObjectReferenceComparer : IEqualityComparer<T>
        {
            public static readonly ObjectReferenceComparer Default = new ObjectReferenceComparer();

            public bool Equals(T x, T y) { return (x == null) ? y == null : y != null && ReferenceEquals(x, y); }

            public int GetHashCode(T obj) { return (obj == null) ? 0 : obj.GetHashCode(); }
        }

    }
}
