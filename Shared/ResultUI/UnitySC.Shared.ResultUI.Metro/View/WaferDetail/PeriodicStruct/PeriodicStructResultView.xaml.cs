using System;

using UnitySC.Shared.ResultUI.Common.Controls;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.PeriodicStruct
{
    /// <summary>
    /// Interaction logic for NanotopoResultView.xaml
    /// </summary>
    public partial class PeriodicStructResultView
    {
        public PeriodicStructResultView()
        {
            InitializeComponent();
        }

        private void OnDieMapSelected(object sender, EventArgs e)
        {
            if (!(sender is AdvancedTabItem tabItem) || !tabItem.IsSelected) return;
            if (DataContext is PeriodicStructResultVM viewModel)
            {
                viewModel.DieMap.Chart.ViewXY.ZoomToFit();
            }
        }
    }
}
