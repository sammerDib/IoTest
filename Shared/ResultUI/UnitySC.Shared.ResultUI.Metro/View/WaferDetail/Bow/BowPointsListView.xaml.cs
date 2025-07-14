using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Bow
{
    /// <summary>
    /// Interaction logic for MeasureResultPointsListView.xaml
    /// </summary>
    public partial class BowPointsListView
    {
        public BowPointsListView()
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

        private BowPointsListVM ViewModel => DataContext as BowPointsListVM;

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
