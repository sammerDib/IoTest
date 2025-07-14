using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public class ListViewMultiSelectionBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (SelectedItems != null)
            {
                AssociatedObject.SelectedItems.Clear();
                foreach (var item in SelectedItems)
                {
                    AssociatedObject.SelectedItems.Add(item);
                }
            }
        }

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(ListViewMultiSelectionBehavior),
                new UIPropertyMetadata(null, SelectedItemsChanged));


        private static void SelectedItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is not ListViewMultiSelectionBehavior behavior)
                return;

            var newValue = e.NewValue as INotifyCollectionChanged;

            if (e.OldValue is INotifyCollectionChanged oldValue)
            {
                oldValue.CollectionChanged -= behavior.SourceCollectionChanged;
                if (behavior.AssociatedObject != null)
                {
                    behavior.AssociatedObject.SelectionChanged -= behavior.ListViewSelectionChanged;
                }
            }
            if (newValue != null)
            {
                if (behavior.AssociatedObject != null)
                {
                    behavior.AssociatedObject.SelectedItems.Clear();
                    foreach (var item in (IEnumerable)newValue)
                    {
                        behavior.AssociatedObject.SelectedItems.Add(item);
                    }

                    behavior.AssociatedObject.SelectionChanged += behavior.ListViewSelectionChanged;
                }

                newValue.CollectionChanged += behavior.SourceCollectionChanged;
            }



        }

        private bool _isUpdatingTarget;
        private bool _isUpdatingSource;

        void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isUpdatingSource)
                return;

            try
            {
                _isUpdatingTarget = true;

                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        AssociatedObject.SelectedItems.Remove(item);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        AssociatedObject.SelectedItems.Add(item);
                    }
                }

                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    AssociatedObject.SelectedItems.Clear();
                }
            }
            finally
            {
                _isUpdatingTarget = false;
            }
        }

        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingTarget)
                return;

            if (e.OriginalSource is ListView { IsEnabled: false })
                return;

            var selectedItems = SelectedItems;
            if (selectedItems == null)
                return;

            try
            {
                _isUpdatingSource = true;

                foreach (var item in e.RemovedItems)
                {
                    SelectedItems.Remove(item);
                }

                foreach (var item in e.AddedItems)
                {
                    SelectedItems.Add(item);
                }
            }
            finally
            {
                _isUpdatingSource = false;
            }
        }

    }
}
