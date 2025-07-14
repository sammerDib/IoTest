using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;

namespace BasicModules.ClusterDispatcher
{
    /// <summary>
    /// Logique d'interaction pour ClusterDispatcherControl.xaml
    /// </summary>
    public partial class ClusterDispatcherControl : UserControl
    {
        //=================================================================
        // Constructeur
        //=================================================================
        internal ClusterDispatcherControl(ClusterDispatcherViewModel datacontext)
        {
            DataContext = datacontext;
            InitializeComponent();
        }

        //=================================================================
        // 
        //=================================================================
        private ClusterDispatcherViewModel ViewModel { get { return (ClusterDispatcherViewModel)DataContext; } }

        private DataTable DataTable
        {
            get
            {
                DataView view = (DataView)dataGrid.ItemsSource;
                DataTable table = view.Table;
                return table;
            }
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
            // Customization des colonnes de la dataGrid
            //-------------------------------------------------------------
            ObservableCollection<DataGridColumn> columns = ViewModel.ColumnCollection;

            dataGrid.Columns.Clear();
            foreach (DataGridColumn column in columns)
                dataGrid.Columns.Add(column);
        }

    }
}
