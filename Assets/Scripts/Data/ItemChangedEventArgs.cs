using System;

namespace Assets.Scripts.Data
{
    public sealed class ItemChangedEventArgs<T> : EventArgs
    {
        public T NewItem { get; }

        public T OldItem { get; }

        public ItemChangedEventArgs(T newItem, T oldItem)
        {
            this.NewItem = newItem;
            this.OldItem = oldItem;
        }
    }
}
