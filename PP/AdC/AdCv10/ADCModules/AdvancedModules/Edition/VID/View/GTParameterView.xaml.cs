using System;
using System.Windows.Controls;

using AdvancedModules.Edition.VID.ViewModel;

namespace AdvancedModules.Edition.VID.View
{
    /// <summary>
    /// Interaction logic for GTParameterView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class GTParameterView : UserControl
    {
        private GTParameterViewModel ViewModel { get { return (GTParameterViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public GTParameterView()
        {
            InitializeComponent();
        }
        public GTParameterView(GTParameterViewModel viewmodel)
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
