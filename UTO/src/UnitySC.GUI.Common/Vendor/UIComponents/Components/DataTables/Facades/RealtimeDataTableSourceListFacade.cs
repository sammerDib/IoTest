using System.Collections;
using System.Collections.Generic;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Facades
{
    public abstract class RealtimeDataTableSourceListFacade<T> : Notifier, ICollection<T>
    {
        protected readonly object LockEnumerator = new object();

        protected LinkedList<T> SourceList { get; } = new LinkedList<T>();

        private LinkedList<T> BufferList { get; } = new LinkedList<T>();

        public int MaxItemNumber { get; set; }

        private bool _isPaused;

        public bool IsPaused
        {
            get { return _isPaused; }
            private set { SetAndRaiseIfChanged(ref _isPaused, value); }
        }

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator() => SourceList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => SourceList.GetEnumerator();

        #endregion

        #region Implementation of ICollection<T>

        public void Add(T item) => Add(item, true);

        private void Add(T item, bool raiseCollectionChanged)
        {
            lock (LockEnumerator)
            {
                if (IsPaused)
                {
                    BufferList.AddLast(item);

                    while (BufferList.Count > MaxItemNumber)
                    {
                        BufferList.RemoveFirst();
                    }

                    return;
                }

                SourceList.AddLast(item);

                while (Count > MaxItemNumber)
                {
                    RemoveFirst(raiseCollectionChanged);
                }

                if (raiseCollectionChanged)
                {
                    OnItemAdded(item);
                }
            }
        }

        public void Clear()
        {
            lock (LockEnumerator)
            {
                BufferList.Clear();
                SourceList.Clear();
                OnCleared();
            }
        }

        public bool Contains(T item)
        {
            lock (LockEnumerator)
            {
                return SourceList.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (LockEnumerator)
            {
                SourceList.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (LockEnumerator)
            {
                var remove = SourceList.Remove(item);
                OnItemRemoved(item);
                return remove;
            }
        }

        public int Count => SourceList.Count;

        public bool IsReadOnly => false;

        #endregion

        private void RemoveFirst(bool raiseCollectionChanged)
        {
            lock (LockEnumerator)
            {
                var firstItem = SourceList.First;
                SourceList.RemoveFirst();
                if (raiseCollectionChanged)
                {
                    OnItemRemoved(firstItem.Value);
                }
            }
        }

        protected abstract void OnItemAdded(T item);

        protected abstract void OnItemRemoved(T item);

        protected abstract void OnCleared();

        protected abstract void OnReset();

        #region Pause / Resume

        public void Pause()
        {
            if (IsPaused) return;
            IsPaused = true;
        }

        public void Resume()
        {
            if (!IsPaused) return;

            lock (LockEnumerator)
            {
                IsPaused = false;
                foreach (var item in BufferList)
                {
                    Add(item, false);
                }
                BufferList.Clear();

                OnReset();
            }
        }

        #endregion
    }
}
