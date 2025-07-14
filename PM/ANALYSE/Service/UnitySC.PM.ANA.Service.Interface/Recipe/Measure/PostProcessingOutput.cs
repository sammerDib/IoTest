using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class PostProcessingOutput
    {
        [DataMember]
        public string Unit { get; set; }

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Target { get; set; }

        [DataMember]
        public double Tolerance { get; set; }

        [DataMember]
        public bool IsUsed { get; set; }

        [DataMember]
        public ResultCorrectionAnyUnitSettings Correction { get; set; } = new ResultCorrectionAnyUnitSettings();

    }
}
