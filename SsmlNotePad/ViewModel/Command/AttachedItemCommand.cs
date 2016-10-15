using System;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Command
{
    public class AttachedItemCommand<T> : RelayCommand
    {
        #region Item Property Members

        public const string PropertyName_Item = "Item";

        private static readonly DependencyPropertyKey ItemPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Item, typeof(T), typeof(AttachedItemCommand<T>),
                new PropertyMetadata(default(T)));

        /// <summary>
        /// Identifies the <see cref="Items"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemProperty = ItemPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public T Item
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(ItemProperty));
                return (T)(GetValue(ItemProperty));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ItemPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Item = value);
            }
        }

        #endregion

        public AttachedItemCommand(T item, Action<T> execute) : base(() => execute(item)) { Item = item; }

        public AttachedItemCommand(T item, Action<T, object> execute) : base((object o) => execute(item, o)) { Item = item; }

        public AttachedItemCommand(T item, Action<T> execute, bool allowSimultaneousExecute, bool isDisabled = false) : base(() => execute(item)) { Item = item; }
    }
}
