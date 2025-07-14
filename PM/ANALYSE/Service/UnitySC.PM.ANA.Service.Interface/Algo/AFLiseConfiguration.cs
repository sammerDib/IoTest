using System;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class AFLiseConfiguration : AxesMovementConfiguration
    {
        public int MinDistanceToAvoidInterference { get; set; } = 7; // when taking a measurement with one of Lise probes, the second must be far enough away to avoid interference between the two

        //[Obsolete] correspond to (1.0/LiseEDStepXinMicrons * 1000). the 1000 factor is to pass from um to mm
        public int ZPosToSignalIndexCoeff = 2600; // used to establish the relationship between a displacement on the Z axis and a displacement of the peak in the LISE signal
        
        public Length AutoFocusScanRange { get; set; } = 2.Millimeters();
    }
}
