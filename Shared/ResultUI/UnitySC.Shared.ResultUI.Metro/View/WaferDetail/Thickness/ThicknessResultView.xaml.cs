using System;

using UnitySC.Shared.ResultUI.Common.Controls;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Thickness
{
    /// <summary>
    /// Interaction logic for NanotopoResultView.xaml
    /// </summary>
    public partial class ThicknessResultView
    {
        public ThicknessResultView()
        {
            InitializeComponent();
        }

        private void OnDieMapSelected(object sender, EventArgs e)
        {
            if (!(sender is AdvancedTabItem tabItem) || !tabItem.IsSelected) return;
            if (DataContext is ThicknessResultVM viewModel)
            {
                viewModel.DieMap.Chart.ViewXY.ZoomToFit();
            }
        }
    }
}
