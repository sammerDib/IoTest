using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using UnitySC.Shared.UI.ViewModel.AdvancedGridView;

namespace UnitySC.Shared.UI.Behaviors
{
    /*
    In this example, when the header named "Column A" is clicked,
    the "SortCommand" ICommand of the ViewModel will be called with the "SortDefinition" property as a parameter.
    You can use an instance of DataTableSource<T> which will provide you a generic implementation of the sort command and the sorts application.

    <ListView behaviors:GridViewSortBehaviors.Command="{Binding SortCommand}">
     	<ListView.View>
            <GridView>
                <GridViewColumn>
					<GridViewColumn.Header>
						<GridViewColumnHeader
							behaviors:GridViewColumnHeaderSortBehaviors.SortDefinition="{Binding SortDefinition}"
							Content="Column A" />
					</GridViewColumn.Header>
				</GridViewColumn>
            </GridView>
        </ListView.View>
    </ListView>
    */

    /// <summary>
    /// This behavior is used to invoke a <see cref="ICommand"/> when the header of the <see cref="GridView"/> associated with the <see cref="ListView"/> is clicked.
    /// The property <see cref="GridViewColumnHeaderSortBehaviors.SortDefinitionProperty"/> will be passed as a parameter of the command.
    /// </summary>
    public static class GridViewSortBehaviors
    {
        #region Attached properties

        #region Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(GridViewSortBehaviors), new UIPropertyMetadata(null, CommandChangedCallback));

        private static void CommandChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(o is ItemsControl listView)) return;

            if (e.OldValue != null && e.NewValue == null)
            {
                listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
            }

            if (e.OldValue == null && e.NewValue != null)
            {
                listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
            }
        }

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        #endregion Command

        #endregion Attached properties

        #region Column header click event handler

        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is GridViewColumnHeader headerClicked)) return;

            var sortDefinition = GridViewColumnHeaderSortBehaviors.GetSortDefinition(headerClicked);
            if (sortDefinition == null) return;

            var listView = GetAncestor<ListView>(headerClicked);
            if (listView == null) return;

            var command = GetCommand(listView);
            if (command != null)
            {
                if (command.CanExecute(sortDefinition))
                {
                    command.Execute(sortDefinition);
                }
            }
        }

        #endregion Column header click event handler

        #region Helper methods

        private static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
            {
                if (parent != null) parent = VisualTreeHelper.GetParent(parent);
            }
            return (T)parent;
        }

        #endregion Helper methods
    }

    public static class GridViewColumnHeaderSortBehaviors
    {
        /// <summary>
        /// Allows you to link an instance of <see cref="SortDefinition"/> with a <see cref="GridViewColumnHeader"/>.
        /// Do not make any binding or explicitly modify the other properties of this behavior because they will be linked automatically with the instance of the <see cref="SortDefinition"/>.
        /// </summary>
        public static readonly DependencyProperty SortDefinitionProperty = DependencyProperty.RegisterAttached(
            "SortDefinition", typeof(SortDefinition), typeof(GridViewColumnHeaderSortBehaviors), new PropertyMetadata(default(SortDefinition), SortDefinitionChangedCallback));

        private static void SortDefinitionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is SortDefinition sort)
            {
                var directionBinding = new Binding { Source = sort, Path = new PropertyPath(nameof(SortDefinition.Direction)) };
                BindingOperations.SetBinding(d, DirectionProperty, directionBinding);

                var isActiveBinding = new Binding { Source = sort, Path = new PropertyPath(nameof(SortDefinition.IsActive)) };
                BindingOperations.SetBinding(d, IsActiveProperty, isActiveBinding);
            }

            SetEnableSorting(d, e.NewValue is SortDefinition);
        }

        public static void SetSortDefinition(DependencyObject element, SortDefinition value)
        {
            element.SetValue(SortDefinitionProperty, value);
        }

        public static SortDefinition GetSortDefinition(DependencyObject element)
        {
            return (SortDefinition)element.GetValue(SortDefinitionProperty);
        }

        public static readonly DependencyProperty EnableSortingProperty = DependencyProperty.RegisterAttached(
            "EnableSorting", typeof(bool), typeof(GridViewColumnHeaderSortBehaviors), new PropertyMetadata(default(bool)));

        public static void SetEnableSorting(DependencyObject element, bool value)
        {
            element.SetValue(EnableSortingProperty, value);
        }

        public static bool GetEnableSorting(DependencyObject element)
        {
            return (bool)element.GetValue(EnableSortingProperty);
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
            "IsActive", typeof(bool), typeof(GridViewColumnHeaderSortBehaviors), new PropertyMetadata(default(bool)));

        public static void SetIsActive(DependencyObject element, bool value)
        {
            element.SetValue(IsActiveProperty, value);
        }

        public static bool GetIsActive(DependencyObject element)
        {
            return (bool)element.GetValue(IsActiveProperty);
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.RegisterAttached(
            "Direction", typeof(ListSortDirection), typeof(GridViewColumnHeaderSortBehaviors), new PropertyMetadata(default(ListSortDirection)));

        public static void SetDirection(DependencyObject element, ListSortDirection value)
        {
            element.SetValue(DirectionProperty, value);
        }

        public static ListSortDirection GetDirection(DependencyObject element)
        {
            return (ListSortDirection)element.GetValue(DirectionProperty);
        }
    }

    public static class GridViewColumnBehaviors
    {
        public static readonly DependencyProperty CollapsedProperty = DependencyProperty.RegisterAttached(
            "Collapsed", typeof(bool), typeof(GridViewColumnBehaviors), new PropertyMetadata(false, CollapsedCallback));

        public static void SetCollapsed(DependencyObject element, bool value)
        {
            element.SetValue(CollapsedProperty, value);
        }

        public static bool GetCollapsed(DependencyObject element)
        {
            return (bool)element.GetValue(CollapsedProperty);
        }

        private static void CollapsedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GridViewColumn column)) return;

            if ((bool)e.NewValue)
            {
                SetLastWidth(column, column.Width);
                column.Width = 0;
            }
            else
            {
                column.Width = GetLastWidth(column);
            }
        }

        public static readonly DependencyProperty LastWidthProperty = DependencyProperty.RegisterAttached(
            "LastWidth", typeof(double), typeof(GridViewColumnBehaviors), new PropertyMetadata(double.NaN));

        public static void SetLastWidth(DependencyObject element, double value)
        {
            element.SetValue(LastWidthProperty, value);
        }

        public static double GetLastWidth(DependencyObject element)
        {
            return (double)element.GetValue(LastWidthProperty);
        }
    }
}
