using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.Shared.UI.Graph.Utils;

namespace UnitySC.Shared.ResultUI.Common.Controls
{
    /// <summary>
    /// This control allows the display in tabulation form without the default behavior of the <see cref="TabControl"/>, which allows in certain cases (for example with the use of the LightningChart) to avoid the appearance of errors.
    /// Indeed, the <see cref="TabControl"/> temporarily deactivates the content that is not displayed and lightningChart is then no longer able to update the graphs.
    /// Conversely, this <see cref="AdvancedTabControl"/> removes the content that is not displayed which increases the display time but also increases performance.
    /// This control also allows you to hide its TabItems with the <see cref="AdvancedTabItem.IsShown"/> property while keeping the selection consistent.
    /// </summary>
    public class AdvancedTabControl : ItemsControl
    {
        public event EventHandler SelectionChanged;

        static AdvancedTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvancedTabControl), new FrameworkPropertyMetadata(typeof(AdvancedTabControl)));
        }

        public AdvancedTabControl()
        {
            IsVisibleChanged += OnIsVisibleChanged;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateTabItemWidth();
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
            int count = Items.OfType<AdvancedTabItem>().Count(item => item.IsShown);
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

        public AdvancedTabItem SelectedItem => Items.OfType<AdvancedTabItem>().SingleOrDefault(item => item.IsSelected);

        public void UpdateSelectedContent()
        {
            UpdateTabItemWidth();

            var selectedItem = SelectedItem;
            if (selectedItem != null && selectedItem.IsShown) return;

            bool newSelectedItemFounded = false;
            foreach (object item in Items)
            {
                if (item is AdvancedTabItem tabItem)
                {
                    if (!newSelectedItemFounded && tabItem.IsShown)
                    {
                        tabItem.SetValue(AdvancedTabItem.IsSelectedPropertyKey, true);
                        newSelectedItemFounded = true;

                        SelectedContent = tabItem.Content;
                        SelectedContentTemplate = tabItem.ContentTemplate;
                    }
                    else
                    {
                        tabItem.SetValue(AdvancedTabItem.IsSelectedPropertyKey, false);
                    }
                }
            }

            if (!newSelectedItemFounded)
            {
                SelectedContentTemplate = null;
                SelectedContent = null;
            }

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public static readonly DependencyProperty TabItemWidthProperty = DependencyProperty.Register(
            nameof(TabItemWidth), typeof(double), typeof(AdvancedTabControl), new PropertyMetadata(default(double)));

        public double TabItemWidth
        {
            get { return (double)GetValue(TabItemWidthProperty); }
            set { SetValue(TabItemWidthProperty, value); }
        }
        
        public void Select(AdvancedTabItem advancedTabItem)
        {
            if (!Items.Contains(advancedTabItem)) return;

            foreach (object item in Items)
            {
                if (item is AdvancedTabItem tabItem)
                {
                    tabItem.SetValue(AdvancedTabItem.IsSelectedPropertyKey, false);
                }
            }

            advancedTabItem.SetValue(AdvancedTabItem.IsSelectedPropertyKey, true);
            SelectedContent = advancedTabItem.Content;
            SelectedContentTemplate = advancedTabItem.ContentTemplate;

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(SelectedContent), typeof(object), typeof(AdvancedTabControl), new FrameworkPropertyMetadata(default(object),
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;

        public object SelectedContent
        {
            get => GetValue(SelectedContentProperty);
            protected set => SetValue(SelectedContentPropertyKey, value);
        }

        public static readonly DependencyPropertyKey SelectedContentTemplatePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(SelectedContentTemplate), typeof(DataTemplate), typeof(AdvancedTabControl), new FrameworkPropertyMetadata(default(DataTemplate),
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty SelectedContentTemplateProperty = SelectedContentTemplatePropertyKey.DependencyProperty;

        public DataTemplate SelectedContentTemplate
        {
            get => (DataTemplate)GetValue(SelectedContentTemplateProperty);
            protected set => SetValue(SelectedContentTemplatePropertyKey, value);
        }

        public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register(
            nameof(TabStripPlacement), typeof(Dock), typeof(AdvancedTabControl), new PropertyMetadata(default(Dock)));

        public Dock TabStripPlacement
        {
            get => (Dock)GetValue(TabStripPlacementProperty);
            set => SetValue(TabStripPlacementProperty, value);
        }

        public static readonly DependencyPropertyKey AnyVisibleItemPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(AnyVisibleItem), typeof(bool), typeof(AdvancedTabControl), new FrameworkPropertyMetadata(default(bool),
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty AnyVisibleItemProperty = AnyVisibleItemPropertyKey.DependencyProperty;

        public bool AnyVisibleItem
        {
            get { return (bool)GetValue(AnyVisibleItemProperty); }
            protected set { SetValue(AnyVisibleItemPropertyKey, value); }
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
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvancedTabItem), new FrameworkPropertyMetadata(typeof(AdvancedTabItem)));
        }
        
        public AdvancedTabItem()
        {
            MouseDown += MvvmTabItem_MouseDown;
        }

        private void MvvmTabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mvvmTabControl = WpfUtils.FindAncestor<AdvancedTabControl>(this);
            mvvmTabControl?.Select(this);
        }
        
        public static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsSelected), typeof(bool), typeof(AdvancedTabItem), new FrameworkPropertyMetadata(default(bool),
                FrameworkPropertyMetadataOptions.None, IsSelectedPropertyChanged));

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
            nameof(IsShown), typeof(bool), typeof(AdvancedTabItem), new PropertyMetadata(true, OnIsShownChanged));

        private static void OnIsShownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AdvancedTabItem self)
            {
                self.RaiseCollapsedChanged();
            }
        }

        public bool IsShown
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        private void RaiseCollapsedChanged()
        {
            var mvvmTabControl = WpfUtils.FindAncestor<AdvancedTabControl>(this);
            mvvmTabControl?.UpdateSelectedContent();
        }
        
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header), typeof(object), typeof(AdvancedTabItem), new PropertyMetadata(default(object)));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
    }
}
