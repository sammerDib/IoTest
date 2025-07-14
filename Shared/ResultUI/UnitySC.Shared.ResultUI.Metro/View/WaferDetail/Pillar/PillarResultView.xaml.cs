using System;

using UnitySC.Shared.ResultUI.Common.Controls;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Pillar
{
    /// <summary>
    /// Interaction logic for NanotopoResultView.xaml
    /// </summary>
    public partial class PillarResultView
    {
        public PillarResultView()
        {
            InitializeComponent();
        }

        private void OnDieMapSelected(object sender, EventArgs e)
        {
            if (!(sender is AdvancedTabItem tabItem) || !tabItem.IsSelected) return;
            if (DataContext is PillarResultVM viewModel)
            {
                viewModel.DieMap.Chart.ViewXY.ZoomToFit();
            }
        }
    }
}
