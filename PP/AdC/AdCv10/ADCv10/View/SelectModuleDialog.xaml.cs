using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ADC.View
{
    /// <summary>
    /// Logique d'interaction pour SelectModuleDialog.xaml
    /// </summary>
    public partial class SelectModuleDialog : Window
    {
        // Résultat du Dialog
        public Object SelectedModuleFactory
        {
            get
            {
                return ModuleListView.SelectedValue;
            }
        }

        //=================================================================
        // Constructor
        //=================================================================
        public SelectModuleDialog(SelectModuleViewModel viewModel)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            DataContext = viewModel;
            CategoryListView.SelectedIndex = 0;
            CategoryListView.Items.SortDescriptions.Clear();
            CategoryListView.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", ListSortDirection.Ascending));
        }

        //=================================================================
        // 
        //=================================================================
        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        //=================================================================
        // 
        //=================================================================
        private void ModuleListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Select();
        }

        //=================================================================
        // 
        //=================================================================
        private void Select()
        {
            if (SelectedModuleFactory != null)
            {
                DialogResult = true;
                Close();
            }
        }

    }
}
