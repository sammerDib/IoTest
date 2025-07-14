using System.Drawing;

namespace UnitySC.PM.DMT.Hardware.Service.Interface.Screen
{
    public class ScreenCorrectionData
    {
        public Size ScreenSize { get; set; }
        public double Gamma { get; set; }
        public double Alpha { get; set; }
        public double MCoefficient { get; set; }
        public string CorrectedImage { get; set; }

        public double BrightnessFactor { get; set; } = 1;
    }
}
