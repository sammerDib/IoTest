using System;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class MultiSizeChuckConfiguration : DefaultConfiguration
    {
        public String WaferCenterImageCalibrationPath { get; set; }
        public RegionOfInterest RegionOfInterest { get; set; }
    }
}
