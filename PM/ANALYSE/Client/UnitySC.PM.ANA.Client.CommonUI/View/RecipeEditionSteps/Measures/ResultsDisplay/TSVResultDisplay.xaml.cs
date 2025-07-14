using System.Windows;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps
{
    public partial class TSVResultDisplay : Window
    {
        public TSVResultDisplay()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as TSVResultDisplayVM).StopStaticRepetaCommand.Execute(this);
        }
    }
}
