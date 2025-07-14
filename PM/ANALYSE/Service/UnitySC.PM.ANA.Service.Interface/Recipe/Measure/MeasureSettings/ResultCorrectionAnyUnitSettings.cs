using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class ResultCorrectionAnyUnitSettings
    {
        [DataMember]
        public double Offset { get; set; } = 0.0;

        [DataMember]
        public string Unit { get; set; } = string.Empty;

        [DataMember]
        public double Coef { get; set; } = 1;

        public double ApplyCorrection(double originalResult)
        {
            return originalResult * Coef + Offset;
        }
    }
}
