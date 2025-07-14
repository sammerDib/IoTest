using System;
using System.Windows;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Dialogs
{
    /// <summary>
    /// Interaction logic for GenericMvvmDialog.xaml
    /// </summary>
    public partial class GenericMvvmDialog : Window
    {
        public GenericMvvmDialog()
        {
            InitializeComponent();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            if (DataContext is GenericMvvmDialogViewModel viewModel)
            {
                viewModel.OnClosed();
            }
        }
    }
}
