using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.Metro.View
{
    public class GeneratedColumnListViewBase : UserControl
    {
        #region Overrides of FrameworkElement

        public GeneratedColumnListViewBase()
        {
            DataContextChanged += OnDataContextChanged;
            Loaded += GeneratedColumnListViewBase_Loaded;
        }

        private void GeneratedColumnListViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            ListView.PreviewMouseWheel += ListView_PreviewMouseWheel;
            Loaded -= GeneratedColumnListViewBase_Loaded;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnGenerateColumnsRequested();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            OnGenerateColumnsRequested();
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty GenerateColumnsFlagProperty = DependencyProperty.Register(
            nameof(GenerateColumnsFlag), typeof(bool), typeof(GeneratedColumnListViewBase), new PropertyMetadata(default(bool), OnGenerateCommandFlagChanged));

        private static void OnGenerateCommandFlagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GeneratedColumnListViewBase nanotopoPointsListView)
            {
                nanotopoPointsListView.OnGenerateColumnsRequested();
            }
        }

        protected virtual void OnGenerateColumnsRequested()
        {

        }

        public bool GenerateColumnsFlag
        {
            get { return (bool)GetValue(GenerateColumnsFlagProperty); }
            set { SetValue(GenerateColumnsFlagProperty, value); }
        }

        #endregion


        private ListView _listView;

        public ListView ListView
        {
            get 
            {
                if (_listView == null)
                {
                    _listView = FindChild<ListView>(this);
                }
                return _listView; 
            }
            set { _listView = value; }
        }



        private AutoRelayCommand _ensureSelectedIsVisibleCommand;

        public AutoRelayCommand EnsureSelectedIsVisibleCommand
        {
            get
            {
                return _ensureSelectedIsVisibleCommand ?? (_ensureSelectedIsVisibleCommand = new AutoRelayCommand(
                    () =>
                    {
                        ListView.ScrollIntoView(ListView.SelectedItem);
                    },
                    () => { return !IsUserVisible((FrameworkElement)ListView.ItemContainerGenerator.ContainerFromItem(ListView.SelectedItem), ListView); }
                    
                ));
            }
        }


        /// <summary>
        /// Looks for a child control within a parent by type
        /// </summary>
        public static T FindChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            // confirm parent is valid.
            if (parent == null) return null;
            if (parent is T) return parent as T;

            DependencyObject foundChild = null;

            if (parent is FrameworkElement) (parent as FrameworkElement).ApplyTemplate();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null) break;
            }

            return foundChild as T;
        }


        private static bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (element == null) return false;
            if (!element.IsVisible)
                return false;

            Rect bounds =
                element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }

        public bool IsItemVisible(ListView listView, object item)
        {
            // Get the container (ListViewItem) for the specified item
            ListViewItem container = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(item);

            // If the container is not null (i.e., the item is realized in the ListView)
            if (container != null)
            {
                // Get the bounds of the container relative to the ListView
                Rect itemRect = new Rect(container.TranslatePoint(new Point(), listView), container.RenderSize);

                // Get the bounds of the ListView's viewport (visible area)
                FrameworkElement scrollHost = GetScrollHost(listView);
                if (scrollHost != null)
                {
                    Rect viewportRect = new Rect(scrollHost.TranslatePoint(new Point(), listView), scrollHost.RenderSize);

                    // Check if the item's bounds intersect with the ListView's viewport bounds
                    return viewportRect.IntersectsWith(itemRect);
                }
            }

            // If the container is null, the item is not visible
            return false;
        }

        private FrameworkElement GetScrollHost(DependencyObject depObj)
        {
            if (depObj == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is ScrollViewer)
                {
                    return child as FrameworkElement;
                }

                var result = GetScrollHost(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            EnsureSelectedIsVisibleCommand.NotifyCanExecuteChanged();
        }

    }
}
