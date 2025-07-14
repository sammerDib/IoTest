using System.Windows;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    public interface ICameraDisplayPoint
    {
        string Name { get; }

        // Position of the point in wafer coordinates
        Point DisplayPosition { get; }
    }
}
