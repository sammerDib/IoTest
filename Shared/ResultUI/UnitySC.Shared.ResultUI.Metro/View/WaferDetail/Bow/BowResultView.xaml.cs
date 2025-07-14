using System;

using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.ResultUI.Common.Controls;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Bow
{
    /// <summary>
    /// Interaction logic for NanotopoResultView.xaml
    /// </summary>
    public partial class BowResultView
    {
        public BowResultView()
        {
            InitializeComponent();
        }

        private void OnDieMapSelected(object sender, EventArgs e)
        {
            if (!(sender is AdvancedTabItem tabItem) || !tabItem.IsSelected) return;
            // we don't really need that one, puisqu'on a pas des Dies dans le Bow
            //if (DataContext is BowResultVM viewModel)
            //{
            //    viewModel.DieMap.Chart.ViewXY.ZoomToFit();
            //}
        }

        private void MetroHeatMapView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
