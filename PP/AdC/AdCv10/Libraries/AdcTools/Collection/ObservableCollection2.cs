using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace AdcTools.Collection
{
    /// <summary>
    /// Une ObservableCollection avec un AddRange() qui ne fait qu'une notification
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableCollection2<T> : ObservableCollection<T>
    {
        public ObservableCollection2() { }
        public ObservableCollection2(List<T> list) : base(list) { }
        public ObservableCollection2(IEnumerable<T> collection) : base(collection) { }

        public void AddRange(IList<T> list)
        {
            int index = Items.Count();

            foreach (T t in list)
                Items.Add(t);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index);
            OnCollectionChanged(args);
        }

        public void AddRange(IEnumerable<T> list)
        {
            int index = Items.Count();

            foreach (T t in list)
                Items.Add(t);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list.ToList(), index);
            OnCollectionChanged(args);
        }
    }
}
