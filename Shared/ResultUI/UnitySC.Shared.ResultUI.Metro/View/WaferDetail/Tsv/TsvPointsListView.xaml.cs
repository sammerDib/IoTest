using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Tsv
{
    /// <summary>
    /// Interaction logic for MeasureResultPointsListView.xaml
    /// </summary>
    public partial class TsvPointsListView
    {
        public TsvPointsListView()
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

        private TsvPointsListVM ViewModel => DataContext as TsvPointsListVM;

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
