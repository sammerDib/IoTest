using System;
using System.Windows.Controls;

using UnitySC.Shared.ResultUI.Common.Controls;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.EdgeTrim;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.EdgeTrim
{
    /// <summary>
    /// Interaction logic for EdgeTrimResultView.xaml
    /// </summary>
    public partial class EdgeTrimResultView : UserControl
    {
        public EdgeTrimResultView()
        {
            InitializeComponent();
        }

        private void OnDieMapSelected(object sender, EventArgs e)
        {
            if (!(sender is AdvancedTabItem tabItem) || !tabItem.IsSelected) return;
            if (DataContext is EdgeTrimResultVM viewModel)
            {
                viewModel.DieMap.Chart.ViewXY.ZoomToFit();
            }
        }
    }
}
