using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Core
{
    public class DataSourceTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Get the tree node template.
        /// </summary>
        public DataTemplate TreeNodeTemplate { get; set; }

        /// <summary>
        /// Get the template for the entire data source.
        /// </summary>
        public DataTemplate DataSourceTemplate { get; set; }

        /// <summary>
        /// Get the template for data source information (without the source data).
        /// </summary>
        public DataTemplate DataSourceInformationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            if (item is UnitTreeNode)
            {
                return TreeNodeTemplate;
            }

            var sourceViewModel = item as DataSourceViewModel;
            if (sourceViewModel != null)
            {
                return sourceViewModel.DataSource != null ? DataSourceTemplate : DataSourceInformationTemplate;
            }
            return null;
        }
    }
}
