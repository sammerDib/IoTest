using System;

using UnitySC.Shared.ResultUI.Common.Controls;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Trench
{
    /// <summary>
    /// Interaction logic for NanotopoResultView.xaml
    /// </summary>
    public partial class TrenchResultView
    {
        public TrenchResultView()
        {
            InitializeComponent();
        }

        private void OnDieMapSelected(object sender, EventArgs e)
        {
            if (!(sender is AdvancedTabItem tabItem) || !tabItem.IsSelected) return;
            if (DataContext is TrenchResultVM viewModel)
            {
                viewModel.DieMap.Chart.ViewXY.ZoomToFit();
            }
        }
    }
}
