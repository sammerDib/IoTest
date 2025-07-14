using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Probe.Configuration.ProbeLiseHF;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeLiseHFConfig : ProbeSingleConfigBase
    {
        public ProbeLiseHFConfig()
        {
        }

        [DataMember]
        public int HighSaturationThreshold { get; set; } = 80;

        [DataMember]
        public int LowSaturationThreshold { get; set; } = 30;
        
        [DataMember]
        public double HighQualityThreshold { get; set; } = 20.0;

        [DataMember]
        public double LowQualityThreshold { get; set; } = 8.0;
        
        [DataMember]
        public double QualityNonNormalizedMax { get; set; } = 1.0;

        [DataMember]
        public double NormalScaleFactor { get; set; } = 1.0; //  Use for LiseHF Spectrum Signal Normalisation for TTM purpose => NormalizeSignal = RawSignal / NormalScaleFactor

        [DataMember]
        public string ShutterID { get; set; }

        [DataMember]
        public string LaserID { get; set; }

        [DataMember]
        public string OpticalRackAxesID { get; set; }

        [DataMember]
        public string SpectrometerID { get; set; }

        [DataMember]
        [XmlArray("AttenuationFilters")]
        [XmlArrayItem("Filter")]
        public List<ProbeLiseHFFilter> Filters { get; set; } = new List<ProbeLiseHFFilter>();

        [DataMember]
        public bool ComputePeakDetectionOnRight { get; set; } = true;

        [DataMember]
        public bool ComputeNewPeakDetection { get; set; } = true;

        [DataMember]
        public int CalibrationValidityPeriodMinutes { get; set; } = 60;
        
        [DataMember]
        public int CalibrationNbAverage{ get; set; } = 128;

    }
}
