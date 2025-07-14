using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Agileo.Semi.Gem300.Abstractions.E90;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking
{
    /// <summary>
    /// Interaction logic for SubstrateTrackingView.xaml
    /// </summary>
    public partial class SubstrateTrackingView
    {
        public SubstrateTrackingView()
        {
            InitializeComponent();
        }

        #region Event Handler

        private void DataTable_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is SubstrateTrackingPanelModel model && model.SelectionMode != SelectionMode.Single)
            {
                model.SelectedSubstrateList = SubstrateDataTable.SelectedItems.OfType<Substrate>().ToList();
            }
        }

        private void ButtonSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is SubstrateTrackingPanelModel model && model.SelectionMode != SelectionMode.Single)
            {
                SubstrateDataTable.ListView.SelectAll();
            }
        }

        private void ButtonDeselectAll_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is SubstrateTrackingPanelModel model && model.SelectionMode != SelectionMode.Single)
            {
                SubstrateDataTable.ListView.UnselectAll();
            }
        }

        private void ExpandDown_Click(object sender, RoutedEventArgs e) => CollapsableHorizontalPanel.SecondRowIsExpanded = false;

        #endregion

    }
}
