using System;
using System.Windows.Controls;

using AdvancedModules.Edition.Apc.ViewModel;

namespace AdvancedModules.Edition.Apc.View
{
    /// <summary>
    /// Interaction logic for ApcParameterView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class ApcParameterView : UserControl
    {
        private ApcParameterViewModel ViewModel { get { return (ApcParameterViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public ApcParameterView()
        {
            InitializeComponent();
        }
        public ApcParameterView(ApcParameterViewModel viewmodel)
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
