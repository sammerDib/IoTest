using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun;
using UnitySC.Shared.UI.Graph.Utils;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.RecipeRun
{
    /// <summary>
    /// Interaction logic for RecipeRunDashboardControl.xaml
    /// </summary>
    public partial class RecipeRunMeasureResultsControl : UserControl
    {
        public RecipeRunMeasureResultsControl()
        {
            InitializeComponent();
        }

        private void MeasurePointResultVM_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange == 0)
                return;

            var scrollViewer = e.OriginalSource as ScrollViewer;
            if (scrollViewer is null)
                return;
            ScrollBar verticalScrollBar =
                       WpfUtils.FindChildren<ScrollBar>(scrollViewer).FirstOrDefault(s => s.Orientation == Orientation.Vertical);
            if (verticalScrollBar != null)
            {
                DataGrid dataGrid = sender as DataGrid;

                int totalCount = dataGrid.Items.Count;
                int firstVisible = (int)verticalScrollBar.Value;
                (DataContext as RecipeRunDashboardVM).LastVisibleResultIndex = (int)(firstVisible + totalCount - verticalScrollBar.Maximum);


            }
        }

    }

}
