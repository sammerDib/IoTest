using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.UIComponents.Extensions;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class BindableTreeView : TreeView
    {
        static BindableTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BindableTreeView), new FrameworkPropertyMetadata(typeof(BindableTreeView)));
        }

        public BindableTreeView()
        {
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        internal bool _lockSelection;

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);
            if (!_lockSelection)
            {
                _lockSelection = true;
                SelectedTreeElement = e.NewValue;
                _lockSelection = false;
            }
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                //Invalidate selection
                SelectedTreeElementChanged(this);
            }
        }

        public static readonly DependencyProperty SelectedTreeElementProperty = DependencyProperty.Register("SelectedTreeElement", typeof(object), typeof(BindableTreeView), new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTreeElementChanged));

        private static void SelectedTreeElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e = new DependencyPropertyChangedEventArgs())
        {
            var treeView = d as BindableTreeView;
            if (treeView != null && !treeView._lockSelection)
            {
                treeView._lockSelection = true;

                //Need to unselect all treeViewItems
                var treeViewItems = FindTreeViewItems(treeView);
                foreach (var treeViewItem in treeViewItems)
                {
                    treeViewItem.IsSelected = false;
                }

                var requiredTreeViewItem = treeView.SelectedTreeElement;

                if (requiredTreeViewItem != null)
                {
                    var treeViewItem = treeViewItems.SingleOrDefault(item => item.DataContext == requiredTreeViewItem);
                    if (treeViewItem != null)
                    {
                        treeViewItem.IsSelected = true;
                    }
                }

                treeView._lockSelection = false;
            }
        }

        public static List<TreeViewItem> FindTreeViewItems(Visual visual)
        {
            var result = new List<TreeViewItem>();

            if (visual == null)
                return result;

            var frameworkElement = visual as FrameworkElement;
            frameworkElement?.ApplyTemplate();

            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(visual); i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(visual, i) as Visual;

                var treeViewItem = child as TreeViewItem;
                if (treeViewItem != null)
                {
                    result.Add(treeViewItem);
                    if (!treeViewItem.IsExpanded)
                    {
                        treeViewItem.IsExpanded = true;
                        treeViewItem.UpdateLayout();
                    }
                }

                result.AddRange(FindTreeViewItems(child));
            }
            return result;
        }

        public object SelectedTreeElement
        {
            get { return GetValue(SelectedTreeElementProperty); }
            set { SetValue(SelectedTreeElementProperty, value); }
        }

        #region Overrides of TreeView

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BindableTreeViewItem();
        }

        #endregion

    }

    public class BindableTreeViewItem : TreeViewItem
    {
        #region Overrides of FrameworkElement

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (DataContext != null)
            {
                var treeView = this.GetAncestor<BindableTreeView>();
                if (treeView != null && treeView.SelectedTreeElement == DataContext)
                {
                    treeView._lockSelection = true;
                    IsSelected = true;
                    treeView._lockSelection = false;
                }
            }
        }

        #region Overrides of TreeViewItem

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BindableTreeViewItem();
        }

        #endregion

        #endregion
    }
}
