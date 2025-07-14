using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Filters.Controls
{
    [TemplatePart(Name = "PART_PopupListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_SelectedItemsControl", Type = typeof(ListBox))]
    public class FilterBox : ComboBox
    {
        private const string ElementPopupListBox = "PART_PopupListBox";
        private const string ElementSelectedItemsListBox = "PART_SelectedItemsControl";
        private const string ElementPopup = "PART_Popup";
        private const string ElementSelecteAllItem = "PART_SelectAllItem";
        private const string ElementClearButton = "PART_ClearButton";

        #region Apply Default Style

        static FilterBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterBox), new FrameworkPropertyMetadata(typeof(FilterBox)));
        }

        #endregion

        private ListBox _sourceListBox;
        private ListBox _selectedItemsControl;
        private bool _templateApplied;
        private Popup _popup;
        private ListBoxItem _selectAllItem;
        private Button _clearButton;

        // Blocks the interpretation of events when the collection is updated by the dependancyProperty
        private bool _lockCollectionChangedEvent;

        // Blocks the interpretation of events when the collection is updated by the control template's elements
        private bool _lockCollectionChangedByItemSource;

        #region Overrides

        // Move the popup at the same time as the height of the control changes
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (!_templateApplied) { return; }

            // Move the popup placement
            if (_popup != null)
            {
                var offset = _popup.HorizontalOffset;
                _popup.HorizontalOffset = offset + 1;
                _popup.HorizontalOffset = offset;
            }
        }

        // When applying the template, retrieves the elements necessary for the operation of the control in the controlTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _templateApplied = true;

            _sourceListBox = GetTemplateChild(ElementPopupListBox) as ListBox;
            if (_sourceListBox != null)
            {
                _sourceListBox.SelectionChanged += SourceListBoxSelectionChanged;
                _sourceListBox.ItemContainerGenerator.StatusChanged += SourceListBox_ItemContainerGenerator_OnStatusChanged;
            }

            _selectedItemsControl = GetTemplateChild(ElementSelectedItemsListBox) as ListBox;
            if (_selectedItemsControl != null)
            {
                _selectedItemsControl.SelectionChanged += SelectedItemsControl_SelectionChanged;
            }

            _popup = GetTemplateChild(ElementPopup) as Popup;

            _clearButton = GetTemplateChild(ElementClearButton) as Button;
            if (_clearButton != null)
            {
                _clearButton.Click += OnClearButtonClicked;
            }

            _selectAllItem = GetTemplateChild(ElementSelecteAllItem) as ListBoxItem;
            if (_selectAllItem != null)
            {
                _selectAllItem.PreviewMouseDown += SelectAllItem_PreviewMouseDown;
            }

            // Selects / deselects items based on the initial state of the collection
            SelectedItemsCollectionChanged(null, null);
        }

        #region Overrides of UIElement

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (IsDropDownOpen == false)
            {
                e.Handled = true;
                if (VisualTreeHelper.GetParent(this) is FrameworkElement parent)
                {
                    parent.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                    {
                        RoutedEvent = MouseWheelEvent,
                        Source = this,
                    });
                }
            }
        }

        #endregion

        private void SelectAllItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    SelectItem(item);
                }
            }

            IsDropDownOpen = false;
        }

        // Call on Popup is show, the generated collection must be sync with selectedItems
        // The listbox contained in the popup being virtualized, it is necessary to refresh the states of the ListboxItems generated, from SelectedItems
        private void SourceListBox_ItemContainerGenerator_OnStatusChanged(object sender, EventArgs e)
        {
            var generator = sender as ItemContainerGenerator;
            if (generator == null || generator.Status != GeneratorStatus.ContainersGenerated) return;

            _lockCollectionChangedByItemSource = true;
            foreach (var item in ItemsSource)
            {
                if (SelectedItems.Contains(item))
                {
                    SelectItem(item);
                }
                else
                {
                    UnselectItem(item);
                }
            }
            _lockCollectionChangedByItemSource = false;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            _lockCollectionChangedByItemSource = newValue == null;
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        #endregion

        #region Events

        // When the user clicks the clear all button
        private void OnClearButtonClicked(object sender, RoutedEventArgs e)
        {
            _lockCollectionChangedEvent = true;
            var selectedItems = SelectedItems.Cast<object>().ToList();
            foreach (var item in selectedItems)
            {
                UnselectItem(item);
                SelectedItems.Remove(item);
            }
            _lockCollectionChangedEvent = false;

            if (_popup != null)
            {
                _popup.IsOpen = false;
            }
        }

        // On selecting an item from the list of selected items, deselect the item from the list of all items
        private void SelectedItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _lockCollectionChangedEvent = true;
            foreach (var item in e.AddedItems)
            {
                UnselectItem(item);
                SelectedItems.Remove(item);
            }
            _lockCollectionChangedEvent = false;
        }

        // Synchronize selected items from the internal listBox control to the dependencyProperty
        // When an item is selected by the view in the source collection, update the collection of selected items.
        private void SourceListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Prevent clear the selected item collection when destroying the control
            if (_lockCollectionChangedByItemSource) return;
            
            //Only if the event comes from the view
            if (e != null)
            {
                _lockCollectionChangedEvent = true;
                base.OnSelectionChanged(e);
                foreach (var eAddedItem in e.AddedItems)
                {
                    SelectedItems.Add(eAddedItem);
                }
                foreach (var eRemovedItem in e.RemovedItems)
                {
                    SelectedItems.Remove(eRemovedItem);
                }
                _lockCollectionChangedEvent = false;
                RaiseSelectedFiltersChangedEvent();
            }
        }

        #endregion

        #region Source ListBox Helper

        private ListBoxItem GetListBoxItem(object itemDataContext)
        {
            //Need to scroll to item because when virtualization is enabled, the itemContainerGenerator returns null for items that are not displayed
            //_sourceListBox.ScrollIntoView(itemDataContext);
            //_sourceListBox.UpdateLayout();
            var generator = _sourceListBox.ItemContainerGenerator;
            return (ListBoxItem)generator.ContainerFromItem(itemDataContext);
        }

        private void UnselectItem(object item)
        {
            var lbi = GetListBoxItem(item);
            if (lbi != null) lbi.IsSelected = false;
        }

        private void SelectItem(object item)
        {
            var lbi = GetListBoxItem(item);
            if (lbi != null) lbi.IsSelected = true;
        }

        #endregion

        #region Title

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(FilterBox), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region SelectedItems

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(IList), typeof(FilterBox), new FrameworkPropertyMetadata(new ObservableCollection<object>(), OnSelectedItemsPropertyChanged));

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        #region CollectionChanged Management

        private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var filterBox = (FilterBox)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                filterBox.DetachSelectedItemsCollection(oldCollection);
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                filterBox.AttachSelectedItemsCollection(newCollection);
            }
        }

        private void DetachSelectedItemsCollection(INotifyCollectionChanged collection)
        {
            collection.CollectionChanged -= SelectedItemsCollectionChanged;
        }

        private void AttachSelectedItemsCollection(INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += SelectedItemsCollectionChanged;
        }

        #endregion

        // Synchronize selected items from the dependencyProperty to the control
        private void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Prevent synchronization of the lists when the event comes from the control
            if (_lockCollectionChangedEvent) return;

            _lockCollectionChangedByItemSource = true;

            foreach (var item in SelectedItems)
            {
                //If the item needs to be selected
                if (!_sourceListBox.SelectedItems.Contains(item))
                {
                    SelectItem(item);
                }
            }

            foreach (var item in _sourceListBox.SelectedItems.Cast<object>().ToList())
            {
                //If the item needs to be unselected
                if (!SelectedItems.Contains(item))
                {
                    UnselectItem(item);
                }
            }
            
            _lockCollectionChangedByItemSource = false;
        }

        #endregion

        #region DataTemplates dependencies

        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register(
            "SelectedItemTemplate", typeof(DataTemplate), typeof(FilterBox), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }

        #endregion

        #region Routed Event

        public static readonly RoutedEvent SelectedFiltersChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedFiltersChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FilterBox));

        // Provide CLR accessors for the event
        public event RoutedEventHandler SelectedFiltersChanged
        {
            add { AddHandler(SelectedFiltersChangedEvent, value); }
            remove { RemoveHandler(SelectedFiltersChangedEvent, value); }
        }

        protected void RaiseSelectedFiltersChangedEvent()
        {
            var newEventArgs = new RoutedEventArgs(SelectedFiltersChangedEvent);
            RaiseEvent(newEventArgs);
        }

        #endregion
    }
}
