using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedModules.MultiLayerClusterDispatcher
{
    /// <summary>
    /// Logique d'interaction pour ClusterDispatcherControl.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class MultiLayerClusterDispatcherControl : UserControl
    {
        //=================================================================
        // Constructeur
        //=================================================================
        internal MultiLayerClusterDispatcherControl(MultiLayerClusterDispatcherViewModel datacontext)
        {
            DataContext = datacontext;
            InitializeComponent();
        }

        //=================================================================
        // 
        //=================================================================
        private MultiLayerClusterDispatcherViewModel ViewModel { get { return (MultiLayerClusterDispatcherViewModel)DataContext; } }

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
            // Customization des colonnes de la dataGrid
            //-------------------------------------------------------------
            List<DataGridColumn> columns = ViewModel.ColumnList;

            datagrid.Columns.Clear();
            foreach (DataGridColumn column in columns)
                datagrid.Columns.Add(column);
        }

        //=================================================================
        //
        //=================================================================
        private void ColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            int? columnIndex = (e.OriginalSource as System.Windows.Controls.Primitives.DataGridColumnHeader)?.DisplayIndex;
            if (columnIndex == null)
                return;

            ViewModel.GroupBy(columnIndex.Value);
        }
    }
}
