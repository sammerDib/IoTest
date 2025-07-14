using System.Windows;
using System.Windows.Controls;


namespace BasicModules.AsoEditor
{
    /// <summary>
    /// Logique d'interaction pour AsoControl.xaml
    /// </summary>
    public partial class AsoControl : UserControl
    {
        private AsoEditorViewModel ViewModel { get { return (AsoEditorViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        internal AsoControl(AsoEditorViewModel viewmodel)
        {
            DataContext = viewmodel;
            InitializeComponent();
        }

        //=================================================================
        //
        //=================================================================
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
            DefectViewModel defectClassVM = (DefectViewModel)e.Row.Item;
            int columnIndex = e.Column.DisplayIndex;

            if (columnIndex == 1)
            {
                ViewModel.SelectDefectCategory(defectClassVM);
                e.Cancel = true;
            }
        }


    }
}
