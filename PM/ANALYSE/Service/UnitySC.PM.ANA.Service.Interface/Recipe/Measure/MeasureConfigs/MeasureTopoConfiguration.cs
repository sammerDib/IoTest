using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class MeasureTopoConfiguration : MeasureConfigurationBase
    {
        [DataMember]
        public Length VSIStepSize { get; set; } = 40.Micrometers();

        [DataMember]
        public Length VSIMarginConstant { get; set; } = 5.Micrometers();

        [DataMember]
        public ResultCorrectionType CorrectionType { get; set; } = ResultCorrectionType.None;
    }
}
