using System.Windows;

using UnitySC.PM.ANA.Client.Controls.Camera;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class CameraDisplayPoint : ICameraDisplayPoint
    {
        public CameraDisplayPoint(string name, Point position)
        {
            Name = name;
            DisplayPosition = position;
        }

        public string Name { get; set; }

        // Position of the point in wafer coordinates
        public Point DisplayPosition { get; set; }
    }
}
