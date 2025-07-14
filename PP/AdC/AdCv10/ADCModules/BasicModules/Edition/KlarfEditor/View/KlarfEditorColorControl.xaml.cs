using System;
using System.Windows.Controls;

namespace BasicModules.KlarfEditor
{
    /// <summary>
    /// Logique d'interaction pour KlarfEditorColorControl.xaml
    /// </summary>
    public partial class KlarfEditorColorControl : UserControl
    {
        private KlarfEditorColorViewModel ViewModel { get { return (KlarfEditorColorViewModel)DataContext; } }

        internal KlarfEditorColorControl(KlarfEditorColorViewModel viewmodel)
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
        }

        //=================================================================
        // 
        //=================================================================
        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            DefectColorViewModel defectColorCat = (DefectColorViewModel)e.Row.Item;
            int columnIndex = e.Column.DisplayIndex;

            if (columnIndex == 1)
            {
                ViewModel.SelectDefectColorCategory(defectColorCat);
                e.Cancel = true;
            }
        }
    }
}
