using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using AdcBasicObjects.DefectLabels.ViewModel;

namespace AdcBasicObjects.DefectLabels.View
{
    /// <summary>
    /// Interaction logic for DefectLabelStoreView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class DefectLabelStoreView : Window
    {
        private DefectLabelStoreViewModel ViewModel { get { return (DefectLabelStoreViewModel)DataContext; } }
        private DispatcherTimer timer = new DispatcherTimer();

        public bool MultipleSelectionMode
        {
            get { return datagrid.SelectionMode == DataGridSelectionMode.Extended; }
            set { datagrid.SelectionMode = value ? DataGridSelectionMode.Extended : DataGridSelectionMode.Single; }
        }

        public DefectLabelStoreView()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            timer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: 1);
            timer.Tick += Timer_Tick;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            DefectLabelStoreViewModel vm = (DefectLabelStoreViewModel)DataContext;
            vm.CreateCommand.Execute(null);

            timer.IsEnabled = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.IsEnabled = false;

            // Je n'ai pas trouvé comment faire ça directment depuis la commande, même en passant par des message mvvmlight
            // Donc j'utilise un timer. Ça marche mais ce n'est pas joli :-(
            if (datagrid.SelectedIndex < 0)
                return;
            DataGridCell cell = GetCell(datagrid, datagrid.SelectedIndex, 0);
            if (cell == null)
                return;

            cell.Focus();
            datagrid.BeginEdit();
        }

        private DataGridCell GetCell(DataGrid dg, int rowIndex, int columnIndex)
        {
            var dr = dg.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (dr == null)
                return null;
            var dc = dg.Columns[columnIndex].GetCellContent(dr);
            if (dc == null)
                return null;
            return dc.Parent as DataGridCell;
        }

    }
}
