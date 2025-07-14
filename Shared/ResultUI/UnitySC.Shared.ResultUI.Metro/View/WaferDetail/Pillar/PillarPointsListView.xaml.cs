using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Pillar
{
    /// <summary>
    /// Interaction logic for MeasureResultPointsListView.xaml
    /// </summary>
    public partial class PillarPointsListView
    {
        public PillarPointsListView()
        {
            InitializeComponent();
        }

        #region Overrides

        protected override void OnGenerateColumnsRequested()
        {
            base.OnGenerateColumnsRequested();
            OnGeneratedColumnsChanged();
        }

        #endregion
        
        private PillarPointsListVM ViewModel => DataContext as PillarPointsListVM;

        private void OnGeneratedColumnsChanged()
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;

            ColumnGenerator.GenerateColumns(GridView, viewModel.GeneratedColumns);
        }

        private void ListView_OnKeyDown(object sender, KeyEventArgs e) => ViewModel?.OnKeyDown(e, ListView);

        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => ViewModel?.SynchronizeSelectedItems(ListView);
    }
}
