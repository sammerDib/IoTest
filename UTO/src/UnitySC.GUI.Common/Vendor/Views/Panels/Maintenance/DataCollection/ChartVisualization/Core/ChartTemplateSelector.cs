using System.Windows;
using System.Windows.Controls;

using Agileo.DataMonitoring.DataWriter.Chart;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Core
{
    public class ChartTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Get the template for the data collection plan.
        /// </summary>
        public DataTemplate DataCollectionPlanNodeTemplate { get; set; }

        /// <summary>
        /// Get the template for the chart data writer.
        /// </summary>
        public DataTemplate ChartDataWriterNodeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            if (item is DataCollectionPlanChartsNode)
            {
                return DataCollectionPlanNodeTemplate;
            }

            return item is ChartDataWriter ? ChartDataWriterNodeTemplate : null;
        }
    }
}
