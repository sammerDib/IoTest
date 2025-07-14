using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.ResultUI.Metro.AttachedProperties;
using UnitySC.Shared.UI.Behaviors;

namespace UnitySC.Shared.ResultUI.Metro.Utilities
{
    public static class ColumnGenerator
    {
        public static void GenerateColumns<T>(GridView gridView, List<T> generatedColumns, Func<T, DataTemplate> getDataTemplate) where T : GeneratedColumn
        {
            // Clear previous generated columns
            var oldGeneratedColumns = gridView.Columns.Where(GridViewColumnAttachedProperties.GetIsGenerated).ToList();

            foreach (var generatedColumn in oldGeneratedColumns)
            {
                gridView.Columns.Remove(generatedColumn);
            }

            foreach (var column in generatedColumns)
            {
                var gridViewColumnHeader = new GridViewColumnHeader
                {
                    Content = column.HeaderName
                };
                gridViewColumnHeader.SetValue(GridViewColumnHeaderSortBehaviors.SortDefinitionProperty, column.SortDefinition);

                var cellTemplate = getDataTemplate(column);

                var gridViewColumn = new GridViewColumn
                {
                    Header = gridViewColumnHeader,
                    CellTemplate = cellTemplate,
                    Width = double.NaN
                };

                // Store IsGenerated property to allow the GridView to be cleaned up the next time the generatedColumns changed.
                gridViewColumn.SetValue(GridViewColumnAttachedProperties.IsGeneratedProperty, true);

                gridView.Columns.Add(gridViewColumn);
            }
        }

        public static void GenerateColumns(GridView gridView, List<GeneratedColumn> generatedColumns)
        {
            DataTemplate GetDataTemplate(GeneratedColumn column)
            {
                var cellTemplate = new DataTemplate();
                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetBinding(TextBlock.TextProperty, column.ValueBinding);
                textBlockFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right);
                cellTemplate.VisualTree = textBlockFactory;
                return cellTemplate;
            }

            GenerateColumns(gridView, generatedColumns, GetDataTemplate);
        }
    }
}
