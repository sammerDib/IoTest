using System;
using System.Windows.Controls;

namespace BasicModules.YieldmapEditor
{
    /// <summary>
    /// Logique d'interaction pour YieldEditorKillerDefectControl.xaml
    /// </summary>
    public partial class YieldEditorKillerDefectControl : UserControl
    {
        private YieldEditorKillerDefectViewModel ViewModel { get { return (YieldEditorKillerDefectViewModel)DataContext; } }
        private int uninitializedItemCount;

        internal YieldEditorKillerDefectControl(YieldEditorKillerDefectViewModel viewmodel)
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
                ViewModel.ReportChange();
        }
    }
}
