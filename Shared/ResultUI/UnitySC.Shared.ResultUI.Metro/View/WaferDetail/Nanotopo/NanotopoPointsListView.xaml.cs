using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Nanotopo
{
    /// <summary>
    /// Interaction logic for MeasureResultPointsListView.xaml
    /// </summary>
    public partial class NanotopoPointsListView
    {
        public NanotopoPointsListView()
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
        
        private NanotopoPointsListVM ViewModel => DataContext as NanotopoPointsListVM;

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
