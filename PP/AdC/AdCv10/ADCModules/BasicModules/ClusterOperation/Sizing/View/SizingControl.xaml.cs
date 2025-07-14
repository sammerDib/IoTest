using System;
using System.Windows.Controls;

namespace BasicModules.Sizing
{
    /// <summary>
    /// Interaction logic for SizingControl.xaml
    /// </summary>
    public partial class SizingControl : UserControl
    {
        private SizingViewModel ViewModel { get { return (SizingViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        internal SizingControl(SizingViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        //=================================================================
        //
        //=================================================================
        private void UserControl_Loaded(object sender, EventArgs e)
        {
            ViewModel.Synchronize();
        }

    }
}
