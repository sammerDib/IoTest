using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using AdcBasicObjects;

namespace BasicModules.Classification
{
    /// <summary>
    /// Interaction logic for MilClassificationControl.xaml
    /// </summary>
    public partial class ClassificationControl : UserControl
    {
        //=================================================================
        // Propriétés
        //=================================================================
        private ClassificationViewModel ViewModel { get { return (ClassificationViewModel)DataContext; } }

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
        // Constructeur
        //=================================================================
        internal ClassificationControl(ClassificationViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        //=================================================================
        // Loaded/Unloaded
        //=================================================================
        private void UserControl_Loaded(object sender, EventArgs e)
        {
            ViewModel.Init();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Release();
        }

        //=================================================================
        // 
        //=================================================================
        private void ButtonAddDefectClass_Click(object sender, RoutedEventArgs e)
        {
            bool ok = ViewModel.AddDefectClass();
            if (ok)
            {
                //TODO focus sur le nouvel élement
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void ButtonDeleteDefectClass_Click(object sender, RoutedEventArgs e)
        {
            DataTable table = DataTable;
            var selectedRows = dataGrid.SelectedItems;

            // Creation de la liste de rows à détruire
            // On ne pas itérer sur SelectItems pendant qu'on détruit ces même items
            //......................................................................
            List<DataRow> rowsToDelete = new List<DataRow>();
            foreach (DataRowView view in selectedRows)
                rowsToDelete.Add(view.Row);

            // Suppression des éléments
            //.........................
            foreach (DataRow row in rowsToDelete)
                table.Rows.Remove(row);
        }

        //=================================================================
        // 
        //=================================================================
        private void DelRowButton_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;
            while (!(obj is DataGridRow) && obj != null) obj = VisualTreeHelper.GetParent(obj);
            if (obj is DataGridRow)
            {
                DataGridRow dgr = (DataGridRow)obj;
                DataRowView drv = (DataRowView)dgr.DataContext;
                DataRow row = drv.Row;
                DataTable table = DataTable;
                table.Rows.Remove(row);
            }
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


        //=================================================================
        // 
        //=================================================================
        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var rowView = (DataRowView)e.Row.Item;
            DataRow row = rowView.Row;
            int columnIndex = e.Column.DisplayIndex;

            bool isEdited = ViewModel.EditCell(row, columnIndex);
            if (isEdited)
                e.Cancel = true;
        }

        //=================================================================
        // 
        //=================================================================
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                DataTable table = DataTable;
                var rowView = (DataRowView)e.Row.Item;
                var row = rowView.Row;
                var idx = e.Column.DisplayIndex;
                var defectClass = (DefectClass)row[0];
                var item = row[idx];
                if (item == DBNull.Value)
                    item = null;
                Characteristic carac = (Characteristic)table.ExtendedProperties[e.Column.Header];

                if (idx == 0)
                {
                    TextBox tb = (TextBox)e.EditingElement;
                    defectClass.label = tb.Text;
                    row[0] = defectClass;
                }
            }
        }

    }
}
