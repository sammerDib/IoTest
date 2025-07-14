using System.Windows;

using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Common.HeatMap
{
    /// <summary>
    /// Interaction logic for DieDetailsView.xaml
    /// </summary>
    public partial class DieMapView
    {
        public DieMapView()
        {
            InitializeComponent();
        }

        private void OnChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is BaseDieMapVM viewModel)
            {
                viewModel.UpdateLegendHeight(e.NewSize.Height);
            }
        }
    }
}

