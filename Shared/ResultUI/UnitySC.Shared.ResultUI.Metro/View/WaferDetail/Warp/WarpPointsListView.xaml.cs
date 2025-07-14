using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Warp
{
    /// <summary>
    /// Interaction logic for WarpPointsListView.xaml
    /// </summary>
    public partial class WarpPointsListView
    {
        public WarpPointsListView()
        {
            InitializeComponent();
        }
        private WarpPointsListVM ViewModel => DataContext as WarpPointsListVM;

        #region Overrides

        protected override void OnGenerateColumnsRequested()
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;

            ColumnGenerator.GenerateColumns(GridView, viewModel.GeneratedColumns);
        }

        #endregion


        private void ListView_OnKeyDown(object sender, KeyEventArgs e) => ViewModel?.OnKeyDown(e, ListView);

        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => ViewModel?.SynchronizeSelectedItems(ListView);

    }
}
