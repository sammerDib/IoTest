using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.UI.ViewModel.AdvancedGridView
{
    public abstract class DataTableSourceListFacade<T> : ObservableObject, IList<T>
    {
        protected abstract List<T> SourceList { get; }

        public bool AutoUpdate { get; set; } = true;

        public void Reset(IEnumerable<T> collection)
        {
            SourceList.Clear();
            SourceList.AddRange(collection);
            UpdateCollection();
        }

        private void AutoUpdateCollection()
        {
            if (AutoUpdate) UpdateCollection();
        }

        public virtual void UpdateCollection()
        {

        }

        #region List Facade

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            SourceList.Add(item);
            AutoUpdateCollection();
        }

        public void AddRange(IEnumerable<T> collection)
        {
            SourceList.AddRange(collection);
            AutoUpdateCollection();
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return SourceList.AsReadOnly();
        }

        public void Clear()
        {
            SourceList.Clear();
            AutoUpdateCollection();
        }

        public bool Contains(T item)
        {
            return SourceList.Contains(item);
        }

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return SourceList.ConvertAll(converter);
        }

        public void CopyTo(T[] array)
        {
            SourceList.CopyTo(array);
            AutoUpdateCollection();
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            SourceList.CopyTo(index, array, arrayIndex, count);
            AutoUpdateCollection();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            SourceList.CopyTo(array, arrayIndex);
            AutoUpdateCollection();
        }

        public bool Exists(Predicate<T> match)
        {
            return SourceList.Exists(match);
        }

        public T Find(Predicate<T> match)
        {
            return SourceList.Find(match);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return SourceList.FindAll(match);
        }

        public int FindIndex(Predicate<T> match)
        {
            return SourceList.FindIndex(match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return SourceList.FindIndex(startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return SourceList.FindIndex(startIndex, count, match);
        }

        public T FindLast(Predicate<T> match)
        {
            return SourceList.FindLast(match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return SourceList.FindLastIndex(match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return SourceList.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return SourceList.FindLastIndex(startIndex, count, match);
        }

        public void ForEach(Action<T> action)
        {
            SourceList.ForEach(action);
        }

        public List<T> GetRange(int index, int count)
        {
            return SourceList.GetRange(index, count);
        }

        public int IndexOf(T item)
        {
            return SourceList.IndexOf(item);
        }

        public int IndexOf(T item, int index)
        {
            return SourceList.IndexOf(item, index);
        }

        public int IndexOf(T item, int index, int count)
        {
            return SourceList.IndexOf(item, index, count);
        }

        public void Insert(int index, T item)
        {
            SourceList.Insert(index, item);
            AutoUpdateCollection();
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            SourceList.InsertRange(index, collection);
            AutoUpdateCollection();
        }

        public int LastIndexOf(T item)
        {
            return SourceList.LastIndexOf(item);
        }

        public int LastIndexOf(T item, int index)
        {
            return SourceList.LastIndexOf(item, index);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            return SourceList.LastIndexOf(item, index, count);
        }

        public bool Remove(T item)
        {
            bool remove = SourceList.Remove(item);
            AutoUpdateCollection();
            return remove;
        }

        public int RemoveAll(Predicate<T> match)
        {
            int removeAll = SourceList.RemoveAll(match);
            AutoUpdateCollection();
            return removeAll;
        }

        public void RemoveAt(int index)
        {
            SourceList.RemoveAt(index);
            AutoUpdateCollection();
        }

        public void RemoveRange(int index, int count)
        {
            SourceList.RemoveRange(index, count);
            AutoUpdateCollection();
        }

        public bool TrueForAll(Predicate<T> match)
        {
            return SourceList.TrueForAll(match);
        }

        public int Count => SourceList.Count;

        public T this[int index]
        {
            get { return SourceList[index]; }
            set
            {
                SourceList[index] = value;
                AutoUpdateCollection();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return SourceList.GetEnumerator();
        }

        #endregion
    }
}
