using System;
using System.Windows.Controls;

namespace BasicModules.KlarfEditor
{
    /// <summary>
    /// Logique d'interaction pour KlarfEditorRoughBinControl.xaml
    /// </summary>
    public partial class KlarfEditorRoughBinControl : UserControl
    {
        private KlarfEditorRoughBinViewModel ViewModel { get { return (KlarfEditorRoughBinViewModel)DataContext; } }
        private int uninitializedItemCount;

        internal KlarfEditorRoughBinControl(KlarfEditorRoughBinViewModel viewmodel)
        {
            DataContext = viewmodel;
            InitializeComponent();
        }
        //=================================================================
        //
        //=================================================================
        private void UserControl_Loaded(object sender, EventArgs e)
        {
            //-------------------------------------------------------------
            // Refresh the view Model
            //-------------------------------------------------------------
            ViewModel.Init();
            uninitializedItemCount = dataGrid.Items.Count;
        }

        //=================================================================
        // 
        //=================================================================
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                ViewModel.ReportChange();
                if (e.Column.DisplayIndex == 1)
                    ViewModel.Parameter.Validate();
            }
        }
    }
}
