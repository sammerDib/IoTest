using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class MeasureTSVConfiguration : MeasureConfigurationBase
    {
        [DataMember]
        public ResultCorrectionType CorrectionTypeForDepth { get; set; } = ResultCorrectionType.None;
        
        [DataMember]
        public ResultCorrectionType CorrectionTypeForCDWidth { get; set; } = ResultCorrectionType.None;
        
        [DataMember]
        public ResultCorrectionType CorrectionTypeForCDLength { get; set; } = ResultCorrectionType.None;

        [DataMember]
        public string DColTSVDepthDefaultLabel { get; set; }

        [DataMember]
        public string DColTSVCDWidthDefaultLabel { get; set; }

        [DataMember]
        public string DColTSVCDLengthDefaultLabel { get; set; }

        [DataMember]
        public bool CanChangeDColLabels { get; set; }

    }
}
