using System;
using System.Windows.Controls;

using AdvancedModules.Edition.VID.ViewModel;

namespace AdvancedModules.Edition.VID.View
{
    /// <summary>
    /// Interaction logic for CrownParameterView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class CrownParameterView : UserControl
    {
        private CrownParameterViewModel ViewModel { get { return (CrownParameterViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public CrownParameterView()
        {
            InitializeComponent();
        }
        public CrownParameterView(CrownParameterViewModel viewmodel)
        {
            DataContext = viewmodel;
            viewmodel.Init();
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

    }
}
