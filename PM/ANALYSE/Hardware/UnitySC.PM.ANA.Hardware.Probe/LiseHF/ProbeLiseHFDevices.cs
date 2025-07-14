using System;

using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Laser;
using UnitySC.PM.Shared.Hardware.Shutter;
using UnitySC.PM.Shared.Hardware.Spectrometer;

namespace UnitySC.PM.ANA.Hardware.Probe.LiseHF
{
    public class ProbeLiseHFDevices
    {
        public ShutterBase Shutter { get; set; }

        public LaserBase Laser { get; set; }

        public LiseHfAxes OpticalRackAxes { get; set; }

        public SpectrometerBase Spectrometer { get; set; }


    }
}
