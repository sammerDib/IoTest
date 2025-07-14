using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class BareWaferAlignmentConfiguration : DefaultConfiguration
    {
        // Distance between Z top and Z bottom axis where the bottom probe is the less visible for the top one with 5X objective.
        // DistanceToAvoidCameraInterference = |ZTop| + |ZBottom| with ZTop at focus position and ZBottom not visible for ZTop
        // For example  on NST7 2238, with ZTop = 13mm, ZBottom is the less visible at ZBottom = -14mm.
        public Length DistanceToAvoidCameraInterference { get; set; } = 27.Millimeters();

        //Version to use for the edge detection
        //
        //Version 1 : Works on most wafers but struggles with wafers that have patterned borders ( a high light boost should be used with this option 80-200)
        //Version 2 : Works on most wafers and wafers with patterned borders ( a low / medium light boost should be used with this option 20-80)
        public int EdgeDetectionVersion { get; set; } = 2;

        public int NotchDetectionVersion { get; set; } = 3;

        public Length NotchWidth { get; set; } = 3000.Micrometers();

        public ReportOption ReportOption { get; set; } = ReportOption.OverlayReport;

        //Deprecated parameter that was used before the edge detection v2 (only kept for retro-compatibility reasons)
        public bool ReportOverlayImages { get; set; } = true;

        public int LightBoostPercentage { get; set; } = 60;

        public int CannyThreshold { get; set; } = 200;
    }
}
