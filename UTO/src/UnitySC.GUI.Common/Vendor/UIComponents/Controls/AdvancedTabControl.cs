using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using UnitySC.GUI.Common.Vendor.UIComponents.Extensions;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <summary>
    /// This control allows the display in tabulation form without the default behavior of the <see cref="TabControl"/>, which allows in certain cases to avoid the appearance of errors.
    /// Indeed, the <see cref="TabControl"/> temporarily deactivates the content that is not displayed.
    /// Conversely, this <see cref="AdvancedTabControl"/> removes the content that is not displayed which increases the display time but also increases performance.
    /// This control also allows you to hide its TabItems with the <see cref="AdvancedTabItem.IsShown"/> property while keeping the selection consistent.
    /// </summary>
    public class AdvancedTabControl : ItemsControl
    {
        private List<AdvancedTabItem> GetTabItems()
        {
            var tabItems = new List<AdvancedTabItem>();

            foreach (var item in Items)
            {
                if (item is AdvancedTabItem advancedTabItem)
                {
                    tabItems.Add(advancedTabItem);
                }
                else
                {
                    if (ItemContainerGenerator.ContainerFromItem(item) is AdvancedTabItem generatedTabItem)
                    {
                        tabItems.Add(generatedTabItem);
                    }
                }
            }

            return tabItems;
        }

        public event EventHandler SelectionChanged;

        static AdvancedTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(AdvancedTabControl),
                new FrameworkPropertyMetadata(typeof(AdvancedTabControl)));
        }

        public AdvancedTabControl()
        {
            IsVisibleChanged += OnIsVisibleChanged;
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        #region Overrides of FrameworkElement

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            var generator = sender as ItemContainerGenerator;
            if (generator == null || generator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            UpdateTabItemWidth();
            SelectFirstIfSelectedItemIsNull();
        }

        #endregion

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => UpdateTabItemWidth();

        private void SelectFirstIfSelectedItemIsNull()
        {
            var items = GetTabItems();
            if (items.Any(item => item.IsSelected))
            {
                return;
            }

            var first = items.FirstOrDefault();
            if (first != null)
            {
                Select(first);
            }
        }

        #region Overrides of FrameworkElement

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            UpdateSelectedContent();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateTabItemWidth();
        }

        private void UpdateTabItemWidth()
        {
            var count = GetTabItems().Count(item => item.IsShown);
            if (count > 0)
            {
                TabItemWidth = ActualWidth / count;
                AnyVisibleItem = true;
            }
            else
            {
                TabItemWidth = ActualWidth;
                AnyVisibleItem = false;
            }
        }

        #endregion

        public AdvancedTabItem SelectedItem => GetTabItems().SingleOrDefault(item => item.IsSelected);

        public void UpdateSelectedContent()
        {
            UpdateTabItemWidth();

            var selectedItem = SelectedItem;
            if (selectedItem != null && selectedItem.IsShown)
            {
                return;
            }

            var newSelectedItemFounded = false;
            foreach (var tabItem in GetTabItems())
            {
                if (!newSelectedItemFounded && tabItem.IsShown)
                {
                    tabItem.SetValue(AdvancedTabItem.IsSelectedPropertyKey, true);
                    newSelectedItemFounded = true;

                    if (tabItem.IsGenerated)
                    {
                        SelectedContent = tabItem.DataContext;
                    }
                    else
                    {
                        SelectedContent = tabItem.Content;
                    }
                }
                else
                {
                    tabItem.SetValue(AdvancedTabItem.IsSelectedPropertyKey, false);
                }
            }

            if (!newSelectedItemFounded)
            {
                SelectedContent = null;
            }

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public static readonly DependencyProperty TabItemWidthProperty = DependencyProperty.Register(
            nameof(TabItemWidth),
            typeof(double),
            typeof(AdvancedTabControl),
            new PropertyMetadata(default(double)));

        public double TabItemWidth
        {
            get => (double)GetValue(TabItemWidthProperty);
            set => SetValue(TabItemWidthProperty, value);
        }

        public void Select(AdvancedTabItem advancedTabItem)
        {
            var items = GetTabItems();
            if (!items.Contains(advancedTabItem))
            {
                return;
            }

            foreach (var item in items)
            {
                item.SetValue(AdvancedTabItem.IsSelectedPropertyKey, false);
            }

            advancedTabItem.SetValue(AdvancedTabItem.IsSelectedPropertyKey, true);

            if (advancedTabItem.IsGenerated)
            {
                SelectedContent = advancedTabItem.DataContext;
            }
            else
            {
                SelectedContent = advancedTabItem.Content;
            }

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(SelectedContent),
            typeof(object),
            typeof(AdvancedTabControl),
            new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty SelectedContentProperty =
            SelectedContentPropertyKey.DependencyProperty;

        public object SelectedContent
        {
            get => GetValue(SelectedContentProperty);
            protected set => SetValue(SelectedContentPropertyKey, value);
        }

        public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register(
            nameof(TabStripPlacement),
            typeof(Dock),
            typeof(AdvancedTabControl),
            new PropertyMetadata(default(Dock)));

        public Dock TabStripPlacement
        {
            get => (Dock)GetValue(TabStripPlacementProperty);
            set => SetValue(TabStripPlacementProperty, value);
        }

        public static readonly DependencyPropertyKey AnyVisibleItemPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(AnyVisibleItem),
            typeof(bool),
            typeof(AdvancedTabControl),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty AnyVisibleItemProperty = AnyVisibleItemPropertyKey.DependencyProperty;

        public bool AnyVisibleItem
        {
            get => (bool)GetValue(AnyVisibleItemProperty);
            protected set => SetValue(AnyVisibleItemPropertyKey, value);
        }

        /// <summary>Creates or identifies the element used to display the specified item.</summary>
        /// <returns>The element used to display the specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride() => new AdvancedTabItem
        {
            IsGenerated = true
        };

        public static readonly DependencyProperty HeaderItemTemplateProperty = DependencyProperty.Register(
            nameof(HeaderItemTemplate),
            typeof(DataTemplate),
            typeof(AdvancedTabControl),
            new PropertyMetadata(default(DataTemplate)));

        public DataTemplate HeaderItemTemplate
        {
            get => (DataTemplate)GetValue(HeaderItemTemplateProperty);
            set => SetValue(HeaderItemTemplateProperty, value);
        }
    }

    /// <summary>
    /// This TabItem is to be used inside a <see cref="AdvancedTabControl"/>.
    /// It is possible to hide it dynamically with the <see cref="IsShown"/> property while keeping the consistency of the selected item.
    /// </summary>
    public class AdvancedTabItem : ContentControl
    {
        public event EventHandler IsSelectedChanged;

        static AdvancedTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(AdvancedTabItem),
                new FrameworkPropertyMetadata(typeof(AdvancedTabItem)));
        }

        public AdvancedTabItem()
        {
            MouseDown += MvvmTabItem_MouseDown;
        }

        private void MvvmTabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mvvmTabControl = this.GetAncestor<AdvancedTabControl>();
            mvvmTabControl?.Select(this);
        }

        public static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.Register(
            nameof(IsGenerated),
            typeof(bool),
            typeof(AdvancedTabItem),
            new PropertyMetadata(default(bool)));

        public bool IsGenerated
        {
            get { return (bool)GetValue(IsGeneratedProperty); }
            set { SetValue(IsGeneratedProperty, value); }
        }

        public static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsSelected),
            typeof(bool),
            typeof(AdvancedTabItem),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.None,
                IsSelectedPropertyChanged));

        private static void IsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AdvancedTabItem self)
            {
                self.IsSelectedChanged?.Invoke(self, EventArgs.Empty);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            protected set => SetValue(IsSelectedPropertyKey, value);
        }

        public static readonly DependencyProperty IsShownProperty = DependencyProperty.Register(
            nameof(IsShown),
            typeof(bool),
            typeof(AdvancedTabItem),
            new PropertyMetadata(true, OnIsShownChanged));

        private static void OnIsShownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AdvancedTabItem self)
            {
                self.RaiseCollapsedChanged();
            }
        }

        public bool IsShown
        {
            get => (bool)GetValue(IsShownProperty);
            set => SetValue(IsShownProperty, value);
        }

        private void RaiseCollapsedChanged()
        {
            var mvvmTabControl = this.GetAncestor<AdvancedTabControl>();
            mvvmTabControl?.UpdateSelectedContent();
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(AdvancedTabItem),
            new PropertyMetadata(default(object)));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
    }
}
