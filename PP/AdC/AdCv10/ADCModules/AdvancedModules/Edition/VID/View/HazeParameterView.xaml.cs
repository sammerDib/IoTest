using System;
using System.Windows.Controls;

using AdvancedModules.Edition.VID.ViewModel;

namespace AdvancedModules.Edition.VID.View
{
    /// <summary>
    /// Interaction logic for HazeParameterView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class HazeParameterView : UserControl
    {
        private HazeParameterViewModel ViewModel { get { return (HazeParameterViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public HazeParameterView()
        {
            InitializeComponent();
        }
        public HazeParameterView(HazeParameterViewModel viewmodel)
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
