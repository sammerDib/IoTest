using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Thickness
{
    /// <summary>
    /// Interaction logic for ThicknessPointsListView.xaml
    /// </summary>
    public partial class ThicknessPointsListView
    {
        public ThicknessPointsListView()
        {
            InitializeComponent();
        }

        #region Overrides

        protected override void OnGenerateColumnsRequested()
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;

            ColumnGenerator.GenerateColumns(GridView, viewModel.GeneratedColumns);
        }

        #endregion

        private ThicknessPointsListVM ViewModel => DataContext as ThicknessPointsListVM;

        private void ListView_OnKeyDown(object sender, KeyEventArgs e) => ViewModel?.OnKeyDown(e, ListView);

        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => ViewModel?.SynchronizeSelectedItems(ListView);
    }
}
