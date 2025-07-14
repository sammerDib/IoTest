using System;

using UnitySC.Shared.ResultUI.Common.Controls;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Topography
{
    /// <summary>
    /// Interaction logic for TopographyResultView.xaml
    /// </summary>
    public partial class TopographyResultView
    {
        public TopographyResultView()
        {
            InitializeComponent();
        }

        private void OnDieMapSelected(object sender, EventArgs e)
        {
            if (!(sender is AdvancedTabItem tabItem) || !tabItem.IsSelected) return;
            if (DataContext is TopographyResultVM viewModel)
            {
                viewModel.DieMap.Chart.ViewXY.ZoomToFit();
            }
        }
    }
}
