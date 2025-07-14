using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

// An event is generated when when an item property is modified
// Here subscription example
//
// var collectionItems = new TrulyObservableCollection<ItemViewModel>();
// collectionItems.ItemPropertyChanged += _collectionItems_CollectionItemChanged;

//   private void _collectionItems_CollectionItemChanged(IList Collection, object sender, System.ComponentModel.PropertyChangedEventArgs e)
//  {
//    if (e.PropertyName == nameof(ItemViewModelPropertyName))
//    {
//       // Do work here
//    }
//}

namespace UnitySC.Shared.UI.Extensions
{
    public delegate void ItemPropertyChangedEventHandler(IList sourceList, object item, PropertyChangedEventArgs e);

    public class TrulyObservableCollection<T> : ObservableCollection<T>
    {
        #region Constructors

        public TrulyObservableCollection() : base()
        {
            CollectionChanged += ObservableCollection_CollectionChanged;
        }

        public TrulyObservableCollection(IEnumerable<T> c) : base(c)
        {
            CollectionChanged += ObservableCollection_CollectionChanged;
        }

        public TrulyObservableCollection(List<T> l) : base(l)
        {
            CollectionChanged += ObservableCollection_CollectionChanged;
        }

        #endregion Constructors

        public new void Clear()
        {
            foreach (var item in this)
                if (item is INotifyPropertyChanged i)
                    i.PropertyChanged -= Element_PropertyChanged;
            base.Clear();
        }

        private void ObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    if (item != null && item is INotifyPropertyChanged i)
                        i.PropertyChanged -= Element_PropertyChanged;

            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    if (item != null && item is INotifyPropertyChanged i)
                    {
                        i.PropertyChanged -= Element_PropertyChanged;
                        i.PropertyChanged += Element_PropertyChanged;
                    }
        }

        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Contains((T)sender))
                ItemPropertyChanged?.Invoke(this, sender, e);
        }


        public event ItemPropertyChangedEventHandler ItemPropertyChanged;
    }
}
