using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace AdvancedModules.ClassificationMultiLayer
{
    /// <summary>
    /// Interaction logic for ClassificationMultiLayerControl.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class ClassificationMultiLayerControl : UserControl
    {
        private ClassificationMultiLayerViewModel ViewModel { get { return (ClassificationMultiLayerViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        internal ClassificationMultiLayerControl(ClassificationMultiLayerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
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

            //-------------------------------------------------------------
            // Customization des colonnes de la DataGrid
            //-------------------------------------------------------------
            List<DataGridColumn> columns = ViewModel.Columns;

            dataGrid.Columns.Clear();
            foreach (DataGridColumn column in columns)
                dataGrid.Columns.Add(column);
        }

        //=================================================================
        // 
        //=================================================================
        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            int newIndex = ViewModel.MoveRowUp(dataGrid.SelectedIndex);
            if (newIndex >= 0)
                dataGrid.SelectedIndex = newIndex;
        }

        //=================================================================
        // 
        //=================================================================
        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            int newIndex = ViewModel.MoveRowDown(dataGrid.SelectedIndex);
            if (newIndex >= 0)
                dataGrid.SelectedIndex = newIndex;
        }

    }
}
