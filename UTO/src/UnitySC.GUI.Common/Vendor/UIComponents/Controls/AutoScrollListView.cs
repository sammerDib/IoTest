using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using UnitySC.GUI.Common.Vendor.UIComponents.Extensions;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class AutoScrollListView : ListView
    {
        public AutoScrollListView()
        {
            ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private ScrollViewer _scrollViewer;

        #region Overrides of FrameworkElement

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = this.GetChildren<ScrollViewer>().First();

            _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        #endregion

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (!CheckAccessibility())
            {
                return;
            }

            if (SelectedItem != null)
            {
                _currentScrollItem = Items.CurrentItem;

                EnableAutoScrollToEnd = false;
            }
        }

        #region ItemContainerGenerator

        private bool _needResetContainer = true;

        private void ItemContainerGenerator_ItemsChanged(object sender = null, ItemsChangedEventArgs e = null)
        {
            if (!CheckAccessibility())
            {
                return;
            }

            if (e?.Action == NotifyCollectionChangedAction.Reset)
            {
                _needResetContainer = true;
                return;
            }

            Scroll();
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (!CheckAccessibility() || !_needResetContainer)
            {
                return;
            }

            _needResetContainer = false;

            Scroll();
        }

        #endregion

        private void Scroll()
        {
            if (EnableAutoScrollToEnd)
            {
                ScrollToEnd();
            }
            else if (EnableAutoScrollToSelectedItem && SelectedItem != null)
            {
                var index = Items.IndexOf(SelectedItem);
                if (index < 0)
                {
                    //The collection changed event is reported to the AutoScrollListView and an access to the index of the selected item fails because the SelectedItem is updated after the collection changed event.
                    return;
                }
                ScrollIntoView(Items[index]);
            }
            
        }

        private void ScrollToCurrentItem()
        {
            try
            {
                ScrollIntoView(Items.CurrentItem);
            }
            catch
            {
                // ignored
            }
        }

        private void ScrollToEnd()
        {
            _scrollViewer?.ScrollToBottom();

            if (AutoSelectLastItemOnAutoScroll && SelectedItem != Items.CurrentItem)
            {
                SelectedItem = Items.CurrentItem;
            }
        }

        private void ScrollToItem()
        {
            if (_currentScrollItem != null && Items.Contains(_currentScrollItem))
            {
                Items.MoveCurrentTo(_currentScrollItem);
                ScrollToCurrentItem();
            }
        }

        public static readonly DependencyProperty EnableAutoScrollToEndProperty = DependencyProperty.Register(
            nameof(EnableAutoScrollToEnd), typeof(bool), typeof(AutoScrollListView), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AutoScrollChanged));


        public bool EnableAutoScrollToEnd
        {
            get => (bool)GetValue(EnableAutoScrollToEndProperty);
            set => SetValue(EnableAutoScrollToEndProperty, value);
        }

        public static readonly DependencyProperty EnableAutoScrollToSelectedItemProperty = DependencyProperty.Register(
            nameof(EnableAutoScrollToSelectedItem), typeof(bool), typeof(AutoScrollListView), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public bool EnableAutoScrollToSelectedItem
        {
            get => (bool)GetValue(EnableAutoScrollToSelectedItemProperty);
            set => SetValue(EnableAutoScrollToSelectedItemProperty, value);
        }

        private object _currentScrollItem;


        private static void AutoScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoScrollListView autoScrollListView)
            {
                autoScrollListView.UpdateScroll();
            }
        }

        private void UpdateScroll()
        {
            if (!CheckAccessibility()) return;

            if (!EnableAutoScrollToEnd)
            {
                _currentScrollItem = null;
            }
            else
            {
                _currentScrollItem = Items.CurrentItem;
                if (EnableAutoScrollToEnd)
                {
                    ScrollToEnd();
                }
            }
        }


        private bool CheckAccessibility()
        {
            if (IsInitialized)
            {
                return Items.Count > 0 && ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated;
            }

            return false;
        }

        public static readonly DependencyProperty AutoSelectLastItemOnAutoScrollProperty = DependencyProperty.Register(
            nameof(AutoSelectLastItemOnAutoScroll), typeof(bool), typeof(AutoScrollListView), new PropertyMetadata(default(bool)));

        public bool AutoSelectLastItemOnAutoScroll
        {
            get { return (bool)GetValue(AutoSelectLastItemOnAutoScrollProperty); }
            set { SetValue(AutoSelectLastItemOnAutoScrollProperty, value); }
        }

        #region event

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_scrollViewer.ContentVerticalOffset == 0)
            {
                EnableAutoScrollToEnd = false;
                return;
            }
            EnableAutoScrollToEnd = Math.Abs(_scrollViewer.VerticalOffset - _scrollViewer.ScrollableHeight) < 0.1 && Math.Abs(_scrollViewer.VerticalOffset - _scrollViewer.ScrollableHeight) > -0.1;
        }

        #endregion event

    }
}
