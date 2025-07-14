using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.CustomPointsManagement
{
    public class CustomPointsManagementVM:ObservableObject
    {
        public MeasurePointsVM MeasurePoints { get; set; }

        public CustomPointsManagementVM(MeasurePointsVM measurePoints)
        {
            MeasurePoints = measurePoints;
        }
    }
}
