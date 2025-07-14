using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataWriter.Chart;

using UnitySC.GUI.Common.Vendor.UIComponents.Extensions;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Core;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization
{
    /// <summary>
    /// Interaction logic for ProcessDataVisualizationView.xaml
    /// </summary>
    public partial class ProcessDataVisualizationView : UserControl
    {
        public ProcessDataVisualizationView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsMaximizableProperty = DependencyProperty.Register("IsMaximizable", typeof(bool), typeof(ProcessDataVisualizationView), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Allow to set the <see cref="ProcessDataVisualizationViewModel.FullScreenCommand"/> visibility.
        /// If true, FullScreen command will be added to chart commands.
        /// Caution : even if command is visible, it will do nothing by default.
        /// Write custom code to interpret the command and maximize chart on the view is needed
        /// </summary>
        public bool IsMaximizable
        {
            get { return (bool)GetValue(IsMaximizableProperty); }
            set { SetValue(IsMaximizableProperty, value); }
        }

        private int _selectedIndex = -1;

        private void DataSourceListView_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _selectedIndex = ActivatedStatus.SelectedIndex;
        }

        private void DataSourceListView_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_selectedIndex == ActivatedStatus.SelectedIndex)
            {
                ActivatedStatus.SelectedIndex = -1;
            }
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            var associatedViewModel = (ProcessDataVisualizationViewModel)DataContext;
            DataCollectionPlan selectedDcp;
            ChartDataWriter selectedChartDataWriter;


            var currentNode = e.NewValue as DataCollectionPlanChartsNode;
            if (currentNode != null)
            {
                selectedDcp = currentNode.DataCollectionPlan;
                selectedChartDataWriter = selectedDcp.DataWriters.FirstOrDefault(writer => writer is ChartDataWriter) as ChartDataWriter;
            }
            else if (e.NewValue is ChartDataWriter writer)
            {
                var selectedTreeViewItem = ((TreeView)sender).GetChildren<TreeViewItem>().SingleOrDefault(treeViewItem => treeViewItem.IsSelected);
                var parent = selectedTreeViewItem?.GetAncestor<TreeViewItem>()?.DataContext;
                selectedDcp = ((DataCollectionPlanChartsNode)parent)?.DataCollectionPlan;
                selectedChartDataWriter = writer;
            }
            else return;

            associatedViewModel.SelectedDcp = selectedDcp;
            associatedViewModel.SelectedChartModel = selectedChartDataWriter;
        }
    }
}
