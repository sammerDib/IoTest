using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables
{
    public class DataTableColumnSelector : Control
    {
        static DataTableColumnSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTableColumnSelector), new FrameworkPropertyMetadata(typeof(DataTableColumnSelector)));
        }

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            nameof(Columns), typeof(IEnumerable), typeof(DataTableColumnSelector), new PropertyMetadata(default(IEnumerable)));

        public IEnumerable Columns
        {
            get { return (IEnumerable)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
    }

    public class DataTableColumn : GridViewColumn
    {
        public DataTableColumn()
        {
            SetupDefaultHeader();
        }

        #region Overrides of DependencyObject

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == HeaderProperty.Name)
            {
                if (Header == null)
                {
                    SetupDefaultHeader();
                    return;
                }

                SetEnableSorting();
            }
            base.OnPropertyChanged(e);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == SortKeyProperty)
            {
                SetEnableSorting();
            }
            if (e.Property == DisplayNameProperty && string.IsNullOrEmpty(DisplayName))
            {
                throw new InvalidOperationException($"{nameof(DisplayName)} must be specified.");
            }
            if (e.Property == CollapsedProperty)
            {
                GridViewColumnBehaviors.SetCollapsed(this, Collapsed);
            }
            if (e.Property == IsVisibleProperty && !IsVisible)
            {
                Collapsed = true;
            }
            base.OnPropertyChanged(e);
        }

        #endregion

        private void SetEnableSorting()
        {
            if (!(Header is GridViewColumnHeader))
            {
                throw new InvalidOperationException($"{nameof(Header)} of {nameof(DataTableColumn)} must be an instance of type {nameof(GridViewColumnHeader)}.");
            }
            GridViewColumnHeaderSortBehaviors.SetEnableSorting(Header as DependencyObject, !string.IsNullOrEmpty(SortKey));
        }

        private void SetupDefaultHeader()
        {
            var textBlock = new TextBlock();
            var headerText = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(DisplayName), Array.Empty<object>())
            };
            textBlock.SetBinding(TextBlock.TextProperty, headerText);
            Header = new GridViewColumnHeader
            {
                Content = textBlock
            };
            SetEnableSorting();
        }

        public static readonly DependencyProperty SortKeyProperty = DependencyProperty.Register(
            nameof(SortKey), typeof(string), typeof(DataTableColumn), new PropertyMetadata(default(string)));

        [Category("Main")]
        public string SortKey
        {
            get { return (string)GetValue(SortKeyProperty); }
            set { SetValue(SortKeyProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register(
            nameof(DisplayName), typeof(string), typeof(DataTableColumn), new PropertyMetadata(nameof(DisplayName)));

        [Category("Main")]
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty CollapsedProperty = DependencyProperty.Register(
            nameof(Collapsed), typeof(bool), typeof(DataTableColumn), new PropertyMetadata(false));

        [Category("Main")]
        public bool Collapsed
        {
            get { return (bool)GetValue(CollapsedProperty); }
            set { SetValue(CollapsedProperty, value); }
        }

        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
            nameof(IsVisible), typeof(bool), typeof(DataTableColumn), new PropertyMetadata(true));

        [Category("Main")]
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }
    }

    [ContentProperty(nameof(GridView))]
    public class DataTable : Control
    {
        static DataTable()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTable), new FrameworkPropertyMetadata(typeof(DataTable)));
        }

        public DataTable()
        {
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
        }

        private ListView _listView;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_listView != null)
            {
                _listView.PreviewMouseDown -= ListView_OnPreviewMouseDown;
                _listView.SelectionChanged -= ListView_OnSelectionChanged;
            }

            _listView = GetTemplateChild("PART_ListView") as ListView;

            if (_listView != null)
            {
                _listView.PreviewMouseDown += ListView_OnPreviewMouseDown;
                _listView.SelectionChanged += ListView_OnSelectionChanged;
            }
        }

        #region Columns

        public static readonly DependencyProperty GridViewProperty = DependencyProperty.Register(
            nameof(GridView), typeof(GridView), typeof(DataTable), new FrameworkPropertyMetadata(default(GridView), FrameworkPropertyMetadataOptions.AffectsRender, GridViewChanged));

        private static void GridViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataTable dataTable || e.NewValue is not GridView gridView) return;

            var generatedColumn = new GridViewColumn
            {
                Width = 43,
                Header = new GridViewColumnHeader
                {
                    Content = new DataTableColumnSelector
                    {
                        Columns = gridView.Columns.OfType<DataTableColumn>().ToList()
                    }
                }
            };

            BindingOperations.SetBinding(generatedColumn, GridViewColumn.CellTemplateProperty, new Binding
            {
                Source = dataTable,
                Path = new PropertyPath(nameof(FirstColumnCellTemplate), Array.Empty<object>())
            });

            gridView.Columns.Insert(0, generatedColumn);


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
                    Source = dataTable,
                    Path = new PropertyPath($"{nameof(DataSource)}.{nameof(IDataTableSource.Sort)}[{columnSortKey}].{nameof(SortDefinition.IsActive)}", Array.Empty<object>())
                });

                header.SetBinding(GridViewColumnHeaderSortBehaviors.DirectionProperty, new Binding
                {
                    Source = dataTable,
                    Path = new PropertyPath($"{nameof(DataSource)}.{nameof(IDataTableSource.Sort)}[{columnSortKey}].{nameof(SortDefinition.Direction)}", Array.Empty<object>())
                });
            }
        }

        [Category("Main")]
        public GridView GridView
        {
            get { return (GridView)GetValue(GridViewProperty); }
            set { SetValue(GridViewProperty, value); }
        }

        public static readonly DependencyProperty FirstColumnCellTemplateProperty = DependencyProperty.Register(nameof(FirstColumnCellTemplate), typeof(DataTemplate), typeof(DataTable), new PropertyMetadata(new DataTemplate()));

        public DataTemplate FirstColumnCellTemplate
        {
            get { return (DataTemplate)GetValue(FirstColumnCellTemplateProperty); }
            set { SetValue(FirstColumnCellTemplateProperty, value); }
        }

        #endregion

        private void ListView_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var filterPanel = DataSource?.Filter;
            if (filterPanel != null)
            {
                filterPanel.IsOpen = false;
            }
        }

        #region Events

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(DataTable));

        /// <summary>Occurs when the selection of the <see cref="DataTable" /> changes.</summary>
        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        #endregion

        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var eventArg = new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems)
            {
                Source = this
            };
            RaiseEvent(eventArg);
        }

        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
            nameof(DataSource), typeof(IDataTableSource), typeof(DataTable), new PropertyMetadata(default(IDataTableSource)));

        [Category("Main")]
        public IDataTableSource DataSource
        {
            get { return (IDataTableSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem), typeof(object), typeof(DataTable), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        [Category("Main")]
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>Gets the currently selected items.  </summary>
        /// <returns>Returns a collection of the currently selected items.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Windows.Controls.ListBox.SelectionMode" /> property is set to <see cref="F:System.Windows.Controls.SelectionMode.Single" />.</exception>
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList SelectedItems => _listView != null ? _listView.SelectedItems : Array.Empty<object>();

        public ListView ListView => _listView;

        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            nameof(SelectionMode), typeof(SelectionMode), typeof(DataTable), new PropertyMetadata(SelectionMode.Single));

        public SelectionMode SelectionMode
        {
            get => (SelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        #region Sort

        public void SortBy(GridViewColumnHeader header)
        {
            var column = GridView.Columns.OfType<DataTableColumn>().FirstOrDefault(c => ReferenceEquals(c.Header, header));

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

        #endregion

        #region Additional Content

        public static readonly DependencyProperty LeftAdditionalContentProperty = DependencyProperty.Register(nameof(LeftAdditionalContent), typeof(object), typeof(DataTable), new PropertyMetadata(default(object)));

        public object LeftAdditionalContent
        {
            get { return GetValue(LeftAdditionalContentProperty); }
            set { SetValue(LeftAdditionalContentProperty, value); }
        }

        public static readonly DependencyProperty BottomAdditionalContentProperty = DependencyProperty.Register(nameof(BottomAdditionalContent), typeof(object), typeof(DataTable), new PropertyMetadata(default(object)));

        public object BottomAdditionalContent
        {
            get { return GetValue(BottomAdditionalContentProperty); }
            set { SetValue(BottomAdditionalContentProperty, value); }
        }

        public static readonly DependencyProperty FilterPanelAdditionalContentProperty = DependencyProperty.Register(nameof(FilterPanelAdditionalContent), typeof(object), typeof(DataTable), new PropertyMetadata(default(object)));

        public object FilterPanelAdditionalContent
        {
            get { return GetValue(FilterPanelAdditionalContentProperty); }
            set { SetValue(FilterPanelAdditionalContentProperty, value); }
        }

        #endregion

        #region AutoScrollToSelectedItem

        public static readonly DependencyProperty EnableAutoScrollToSelectedItemProperty = DependencyProperty.Register(nameof(EnableAutoScrollToSelectedItem), typeof(bool), typeof(DataTable), new PropertyMetadata(default(bool)));

        [Category("Main")]
        public bool EnableAutoScrollToSelectedItem
        {
            get { return (bool)GetValue(EnableAutoScrollToSelectedItemProperty); }
            set { SetValue(EnableAutoScrollToSelectedItemProperty, value); }
        }

        #endregion

        #region AutoScrollToEnd

        public static readonly DependencyProperty EnableAutoScrollToEndProperty = DependencyProperty.Register(nameof(EnableAutoScrollToEnd), typeof(bool), typeof(DataTable), new PropertyMetadata(default(bool)));

        [Category("Main")]
        public bool EnableAutoScrollToEnd
        {
            get { return (bool)GetValue(EnableAutoScrollToEndProperty); }
            set { SetValue(EnableAutoScrollToEndProperty, value); }
        }

        public static readonly DependencyProperty AutoSelectLastItemOnAutoScrollProperty = DependencyProperty.Register(nameof(AutoSelectLastItemOnAutoScroll), typeof(bool), typeof(DataTable), new PropertyMetadata(default(bool)));

        [Category("Main")]
        public bool AutoSelectLastItemOnAutoScroll
        {
            get { return (bool)GetValue(AutoSelectLastItemOnAutoScrollProperty); }
            set { SetValue(AutoSelectLastItemOnAutoScrollProperty, value); }
        }

        #endregion

        #region ListView Facade

        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(DataTable), new PropertyMetadata(null));

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        #endregion
    }
}
