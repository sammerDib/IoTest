using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Common.Components.DataTree.Interfaces;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree.Controls
{
    public class DataTreeItem : Control
    {
        private const string ElementHeaderGrid = "PART_HeaderGrid";
        private const double DragDropOffset = 10;

        private UIElement _headerGrid;
        
        static DataTreeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTreeItem), new FrameworkPropertyMetadata(typeof(DataTreeItem)));
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _headerGrid = GetTemplateChild(ElementHeaderGrid) as UIElement;
            if (_headerGrid != null)
            {
                _headerGrid.MouseEnter += HeaderGrid_MouseEnter;
                _headerGrid.MouseLeave += HeaderGrid_OnMouseLeave;
                _headerGrid.MouseDown += HeaderGrid_MouseDown;
                _headerGrid.MouseUp += HeaderGrid_OnMouseUp;
                _headerGrid.MouseMove += HeaderGrid_MouseMove;
            }

            KeyDown += OnKeyDown;
        }
        
        #region Private

        private void ChangeVisualState(bool useTransitions) => VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", useTransitions);

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled) return;
            if (!(DataContext is ITreeNode treeNode)) return;

            var dataTreeSource = treeNode.DataTreeSource;
            dataTreeSource?.OnKeyDown(e);
        }

        #endregion

        #region HeaderGrid Events

        private void HeaderGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsClicked) return;

            if (_clickedPoint != null)
            {
                var currentPoint = e.GetPosition(this);
                var canDragDrop = Math.Abs(currentPoint.X - _clickedPoint.Value.X) > DragDropOffset || Math.Abs(currentPoint.Y - _clickedPoint.Value.Y) > DragDropOffset;
                if (!canDragDrop) return;
            }

            var onDragCommand = OnDragCommand;
            if (onDragCommand == null) return;
            if (onDragCommand.CanExecute(null)) onDragCommand.Execute(null);
        }

        private Point? _clickedPoint;

        private void HeaderGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                if (IsExpandable) IsExpanded = !IsExpanded;
            }
            else
            {
                IsClicked = true;
                _clickedPoint = e.GetPosition(this);
            }
        }

        private void HeaderGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsClicked)
            {
                var selectCommand = SelectedCommand;
                if (selectCommand != null)
                    if (selectCommand.CanExecute(null))
                        selectCommand.Execute(null);
            }

            IsClicked = false;
        }

        private void HeaderGrid_OnMouseLeave(object sender, MouseEventArgs e)
        {
            IsMouseOverHeader = false;
            IsClicked = false;
        }

        private void HeaderGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            IsMouseOverHeader = true;
        }

        #endregion

        #region Properties

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            nameof(IsExpanded), typeof(bool), typeof(DataTreeItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsExpandedChangedCallback));

        private static void OnIsExpandedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTreeItem self)
            {
                self.ChangeVisualState(self.IsInitialized);
            }
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public static readonly DependencyProperty IsExpandableProperty = DependencyProperty.Register(
            nameof(IsExpandable), typeof(bool), typeof(DataTreeItem), new PropertyMetadata(default(bool)));

        public bool IsExpandable
        {
            get => (bool) GetValue(IsExpandableProperty);
            set => SetValue(IsExpandableProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(DataTreeItem), new PropertyMetadata(false));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsMouseOverHeaderProperty = DependencyProperty.Register(
            nameof(IsMouseOverHeader), typeof(bool), typeof(DataTreeItem), new PropertyMetadata(default(bool)));

        public bool IsMouseOverHeader
        {
            get => (bool)GetValue(IsMouseOverHeaderProperty);
            set => SetValue(IsMouseOverHeaderProperty, value);
        }

        public static readonly DependencyProperty IsClickedProperty = DependencyProperty.Register(
            nameof(IsClicked), typeof(bool), typeof(DataTreeItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsClickedPropertyChangedCallback));

        private static void IsClickedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTreeItem self)
            {
                if (!self.IsClicked) self._clickedPoint = null;
            }
        }

        public bool IsClicked
        {
            get => (bool)GetValue(IsClickedProperty);
            set => SetValue(IsClickedProperty, value);
        }

        public static readonly DependencyProperty IsDragDropDestinationProperty = DependencyProperty.Register(
            nameof(IsDragDropDestination), typeof(bool), typeof(DataTreeItem), new PropertyMetadata(default(bool)));

        public bool IsDragDropDestination
        {
            get => (bool)GetValue(IsDragDropDestinationProperty);
            set => SetValue(IsDragDropDestinationProperty, value);
        }

        #endregion

        #region Items
        
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate), typeof(DataTemplate), typeof(DataTreeItem), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(
            nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(DataTreeItem), new PropertyMetadata(default(DataTemplateSelector)));

        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(
            nameof(IconTemplate),
            typeof(DataTemplate),
            typeof(DataTreeItem),
            new PropertyMetadata(default(DataTemplate)));

        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }
        
        #endregion

        #region ICommands

        public static readonly DependencyProperty SelectedCommandProperty = DependencyProperty.Register(
            nameof(SelectedCommand), typeof(ICommand), typeof(DataTreeItem), new PropertyMetadata(default(ICommand)));

        public ICommand SelectedCommand
        {
            get => (ICommand)GetValue(SelectedCommandProperty);
            set => SetValue(SelectedCommandProperty, value);
        }

        public static readonly DependencyProperty OnDragCommandProperty = DependencyProperty.Register(
            nameof(OnDragCommand), typeof(ICommand), typeof(DataTreeItem), new PropertyMetadata(default(ICommand)));

        public ICommand OnDragCommand
        {
            get => (ICommand)GetValue(OnDragCommandProperty);
            set => SetValue(OnDragCommandProperty, value);
        }

        #endregion
    }
}
