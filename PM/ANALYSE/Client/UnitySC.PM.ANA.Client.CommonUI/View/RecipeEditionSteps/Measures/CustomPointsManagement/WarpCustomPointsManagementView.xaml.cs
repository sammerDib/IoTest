using System.Windows.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures.CustomPointsManagement
{
    /// <summary>
    /// Interaction logic for WarpCustomPointsManagementView.xaml
    /// </summary>
    public partial class WarpCustomPointsManagementView : UserControl
    {
        public WarpCustomPointsManagementView()
        {
            InitializeComponent();
        }

        private void PresetsList_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PresetsList.SelectedItem != null)
            {
                PopupPresets.IsOpen = false;
            }
        }
    }
}
