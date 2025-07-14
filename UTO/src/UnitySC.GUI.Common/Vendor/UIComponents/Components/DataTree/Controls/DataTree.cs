using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.DragDrop;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Interfaces;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Utilities.Extensions;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Controls
{
    public class DataTree : Control
    {
        private const string ElementTreeView = "PART_Content";
        private const string ElementDragGrid = "PART_DragGrid";
        private const string ElementDragGridTranslateTransform = "PART_DragGridTransform";

        private ItemsControl _itemsControl;
        private Grid _dragGrid;
        private TranslateTransform _dragGridTranslateTransform;

        static DataTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTree), new FrameworkPropertyMetadata(typeof(DataTree)));
        }

        public DataTree()
        {
            KeyDown += OnKeyDown;
            MouseMove += OnMouseMove;
            PreviewMouseUp += OnPreviewMouseUp;

            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _itemsControl = GetTemplateChild(ElementTreeView) as ItemsControl;
            _dragGrid = GetTemplateChild(ElementDragGrid) as Grid;
            _dragGridTranslateTransform = GetTemplateChild(ElementDragGridTranslateTransform) as TranslateTransform;
        }

        #region Dependency Properties

        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
            nameof(DataSource), typeof(IDataTreeSource), typeof(DataTree), new PropertyMetadata(default(IDataTreeSource), OnDataSourceChanged));

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTree self)
            {
                if (e.OldValue is INotifyPropertyChanged oldDataSource)
                {
                    oldDataSource.PropertyChanged -= self.OnDataSourcePropertyChanged;
                }
                if (e.NewValue is INotifyPropertyChanged newDataSource)
                {
                    newDataSource.PropertyChanged += self.OnDataSourcePropertyChanged;
                }
            }
        }

        private bool _preventSetDataSourceSelectedItem;

        private void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IDataTreeSource.SelectedValue)) return;

            _preventSetDataSourceSelectedItem = true;
            SelectedValue = DataSource.SelectedValue;
            _preventSetDataSourceSelectedItem = false;
        }

        [Category("Main")]
        public IDataTreeSource DataSource
        {
            get { return (IDataTreeSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate), typeof(DataTemplate), typeof(DataTree), new PropertyMetadata(default(DataTemplate)));

        [Category("Main")]
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(
            nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(DataTree), new PropertyMetadata(default(DataTemplateSelector)));

        [Category("Main")]
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(
            nameof(ItemContainerStyle), typeof(Style), typeof(DataTree), new PropertyMetadata(default(Style)));

        [Category("Main")]
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemContainerStyleSelectorProperty = DependencyProperty.Register(
            nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(DataTree), new PropertyMetadata(default(StyleSelector)));

        [Category("Main")]
        public StyleSelector ItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
            set { SetValue(ItemContainerStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
            nameof(SelectedValue), typeof(object), typeof(DataTree), new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChanged));

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTree { _preventSetDataSourceSelectedItem: false, DataSource: { } } self)
            {
                self.DataSource.SelectedValue = e.NewValue;
            }
        }

        [Category("Main")]
        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public static readonly DependencyProperty DisabledExpanderVisibilityProperty = DependencyProperty.Register(
            nameof(DisabledExpanderVisibility), typeof(Visibility), typeof(DataTree), new PropertyMetadata(default(Visibility)));

        [Category("Main")]
        public Visibility DisabledExpanderVisibility
        {
            get { return (Visibility)GetValue(DisabledExpanderVisibilityProperty); }
            set { SetValue(DisabledExpanderVisibilityProperty, value); }
        }

        #region AdditionalContent

        public static readonly DependencyProperty BottomAdditionalContentProperty = DependencyProperty.Register(
            nameof(BottomAdditionalContent), typeof(object), typeof(DataTree), new PropertyMetadata(default(object)));

        public object BottomAdditionalContent
        {
            get { return GetValue(BottomAdditionalContentProperty); }
            set { SetValue(BottomAdditionalContentProperty, value); }
        }

        #endregion

        #endregion

        #region TreeView Events

        /// <summary>
        /// Invoke the OnKeyDownCommand for Key Gesture
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            DataSource?.OnKeyDown(e);
        }

        #endregion

        #region Drag And Drop Events

        private FrameworkElement _currentMouseOverItem;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // Return if there is no dragged element
            var dragDropAction = DataSource?.DragDropBehavior.CurrentDragDropAction;
            var draggedTreeElement = dragDropAction?.DraggedNode;
            if (draggedTreeElement == null) return;

            if (!_itemsControl.IsMouseCaptured) _itemsControl.CaptureMouse();

            // Get the item behind the cursor
            _currentMouseOverItem = _itemsControl.GetItemByMousePos(e.GetPosition);
            var targetTreeElement = GetTreeNode(_currentMouseOverItem);

            // Return if the target element is null or the source
            if (targetTreeElement == null || targetTreeElement == draggedTreeElement)
            {
                dragDropAction.Update(null, -1, DragDropAction.None);
                RefreshSeparatorDisplay();
                return;
            }

            var currentIndex = DataSource.DragDropBehavior.GetIndex(targetTreeElement);
            if (currentIndex == null) return;

            int nextIndex;
            DragDropAction dragDropActionType;
            ITreeNode newParent;

            var nextIndexFromMouse = GetNextIndexFromMousePos(_currentMouseOverItem, e.GetPosition, currentIndex.Value);
            if (nextIndexFromMouse != null)
            {
                nextIndex = nextIndexFromMouse.Value;

                // [TLa] Prevent wrong drop action
                // If we hover the bottom of an expanded item, we must remember that we add the element as a child at index 0.
                if (nextIndexFromMouse == currentIndex.Value + 1 && targetTreeElement.IsExpanded && targetTreeElement.VisibleChildCount > 0)
                {
                    // It is necessary to change the _currentMouseOverItem to correctly apply the location of the separator.
                    _currentMouseOverItem = _itemsControl.GetNextItemByMousePos(e.GetPosition);

                    nextIndex = 0;
                    newParent = targetTreeElement;
                    dragDropActionType = DragDropAction.ChangeOrderAndParent;
                }
                else if (targetTreeElement.Parent != draggedTreeElement.Parent)
                {
                    newParent = targetTreeElement.Parent;
                    dragDropActionType = DragDropAction.ChangeOrderAndParent;
                }
                else
                {
                    newParent = draggedTreeElement.Parent;
                    dragDropActionType = DragDropAction.ChangeOrder;
                }
            }
            else
            {
                nextIndex = currentIndex.Value;
                newParent = targetTreeElement;
                dragDropActionType = DragDropAction.ChangParent;
            }

            // Define the dragDrop action
            dragDropAction.Update(newParent, nextIndex, dragDropActionType);

            RefreshSeparatorDisplay();
        }

        private static int? GetNextIndexFromMousePos(FrameworkElement self, Func<IInputElement, Point> getPosition, int currentIndex)
        {
            const int activeBorderThis = 5;
            var mouseYPosition = getPosition(self).Y;

            if (mouseYPosition < activeBorderThis) return currentIndex;
            if (mouseYPosition > self.ActualHeight - activeBorderThis) return currentIndex + 1;
            return null;
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DataSource == null) return;
            DataSource.DragDropBehavior.ApplyCurrentDragDropAction();
            RefreshSeparatorDisplay();
            _itemsControl?.ReleaseMouseCapture();
        }

        #endregion

        #region Separator

        private void RefreshSeparatorDisplay()
        {
            var canBeApplied = DataSource?.DragDropBehavior.RefreshCanApplyDragDropAction();
            if (!canBeApplied.HasValue || !canBeApplied.Value)
            {
                RemoveSeparator();
                return;
            }

            var dragDropAction = DataSource?.DragDropBehavior.CurrentDragDropAction;
            if (dragDropAction != null && (dragDropAction.ActionType == DragDropAction.ChangeOrder || dragDropAction.ActionType == DragDropAction.ChangeOrderAndParent))
                MoveSeparator(dragDropAction);
            else
                RemoveSeparator();
        }

        private void MoveSeparator(IDragDropAction dragDropAction)
        {
            var targetTreeElement = GetTreeNode(_currentMouseOverItem);

            var relativePoint = _currentMouseOverItem.TransformToAncestor(this).Transform(new Point(0, 0));

            _dragGridTranslateTransform.X = relativePoint.X;
            _dragGrid.Width = _currentMouseOverItem.ActualWidth;

            var shift = _dragGrid.ActualHeight / 2;

            if (dragDropAction.NewIndex <= DataSource.DragDropBehavior.GetIndex(targetTreeElement))
                _dragGridTranslateTransform.Y = relativePoint.Y - shift;
            else
                _dragGridTranslateTransform.Y = relativePoint.Y + _currentMouseOverItem.ActualHeight - shift;
            _dragGrid.Visibility = Visibility.Visible;
        }

        private void RemoveSeparator()
        {
            _dragGrid.Visibility = Visibility.Hidden;
            _dragGridTranslateTransform.X = 0;
            _dragGridTranslateTransform.Y = 0;
            _dragGrid.Width = 0;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get the DataContext as ITreeNode of the specified element
        /// </summary>
        /// <param name="element">Framework element</param>
        private static ITreeNode GetTreeNode(FrameworkElement element) => element?.DataContext as ITreeNode;

        #endregion

        #region GridView

        public void SortBy(GridViewColumnHeader header)
        {
            var column = GridView?.Columns.OfType<DataTableColumn>().FirstOrDefault(c => ReferenceEquals(c.Header, header));

            if (column == null || string.IsNullOrEmpty(column.SortKey)) return;

            var command = DataSource?.SortCommand;
            if (command == null) return;

            if (command.CanExecute(column.SortKey))
            {
                command.Execute(column.SortKey);
            }
        }

        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            SortBy(headerClicked);
        }

        public static readonly DependencyProperty GridViewProperty = DependencyProperty.Register(
            nameof(GridView),
            typeof(GridView),
            typeof(DataTree),
            new FrameworkPropertyMetadata(default(GridView), FrameworkPropertyMetadataOptions.AffectsRender, GridViewChanged));

        private static void GridViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataTree dataTree || e.NewValue is not GridView gridView) return;

            foreach (var gridViewColumn in gridView.Columns)
            {
                if (gridViewColumn is not DataTableColumn column) continue;
                if (gridViewColumn.Header is not GridViewColumnHeader header) continue;

                var sortingEnabled = GridViewColumnHeaderSortBehaviors.GetEnableSorting(header);
                if (!sortingEnabled) continue;

                // If sorting for the column is enabled, automatically creates
                // the link with the sort definition in the IGridViewSource

                var columnSortKey = column.SortKey;

                header.SetBinding(GridViewColumnHeaderSortBehaviors.IsActiveProperty, new Binding
                {
                    Source = dataTree,
                    Path = new PropertyPath($"{nameof(DataSource)}.{nameof(IDataTreeSource.Sort)}[{columnSortKey}].{nameof(SortDefinition.IsActive)}", Array.Empty<object>())
                });

                header.SetBinding(GridViewColumnHeaderSortBehaviors.DirectionProperty, new Binding
                {
                    Source = dataTree,
                    Path = new PropertyPath($"{nameof(DataSource)}.{nameof(IDataTreeSource.Sort)}[{columnSortKey}].{nameof(SortDefinition.Direction)}", Array.Empty<object>())
                });
            }
        }

        [Category("Main")]
        public GridView GridView
        {
            get { return (GridView)GetValue(GridViewProperty); }
            set { SetValue(GridViewProperty, value); }
        }

        #endregion
    }
}
