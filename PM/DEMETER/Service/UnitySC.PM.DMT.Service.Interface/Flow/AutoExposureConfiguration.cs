using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class AutoExposureConfiguration : FlowConfigurationBase
    {
        [XmlElement] public List<Config> DefaultAutoExposureSetting;

        public override FlowReportConfiguration WriteReportMode { get; set; }

        public bool IgnoreAutoExposureFailure { get; set; } = false;
        
        public Length DefaultEdgeExclusionLength { get; set; } = 3.Millimeters();

        [Serializable]
        public class Config
        {
            [XmlAttribute] public double InitialExposureTimeMs = 20;

            [XmlAttribute] public double DefaultExposureTimeMsIfFailure = 85;

            [XmlAttribute] public MeasureType Measure;

            [XmlAttribute] public double RatioSaturated = 0.03;

            [XmlAttribute] public int SaturationTolerance = 10;

            [XmlAttribute] public int TargetSaturation = 220;

            [XmlAttribute] public Side WaferSide;
        }

        public Config GetDefaultAutoExposureSettingForMeasure(MeasureBase measure)
        {
            return DefaultAutoExposureSetting.SingleOrDefault(setting => setting.Measure == measure.MeasureType && setting.WaferSide == measure.Side);
        }
    }
}
