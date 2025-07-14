using System;
using System.Windows.Controls;

using AdvancedModules.Edition.VID.ViewModel;

namespace AdvancedModules.Edition.VID.View
{
    /// <summary>
    /// Interaction logic for BF3DParameterView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class BF3DParameterView : UserControl
    {
        private BF3DParameterViewModel ViewModel { get { return (BF3DParameterViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public BF3DParameterView()
        {
            InitializeComponent();
        }
        internal BF3DParameterView(BF3DParameterViewModel viewmodel)
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
