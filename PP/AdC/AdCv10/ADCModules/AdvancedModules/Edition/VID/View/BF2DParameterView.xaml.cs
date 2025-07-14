using System;
using System.Windows.Controls;

using AdvancedModules.Edition.VID.ViewModel;

namespace AdvancedModules.Edition.VID.View
{
    /// <summary>
    /// Interaction logic for BF2DParameterView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class BF2DParameterView : UserControl
    {
        private BF2DParameterViewModel ViewModel { get { return (BF2DParameterViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public BF2DParameterView()
        {
            InitializeComponent();
        }
        internal BF2DParameterView(BF2DParameterViewModel viewmodel)
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
