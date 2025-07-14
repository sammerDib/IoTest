using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class MeasureTrenchConfiguration : MeasureConfigurationBase
    {
        [DataMember]
        public ResultCorrectionType CorrectionTypeForDepth { get; set; } = ResultCorrectionType.None;

        [DataMember]
        public ResultCorrectionType CorrectionTypeForWidth { get; set; } = ResultCorrectionType.None;

        /// <summary>
        /// The number of acquisition to perform to have one single lise signal.
        /// These acquisitions' air gap and thicknesses measures are averaged to 
        /// get a more accurate result.
        /// </summary>
        [DataMember]
        public int NbAveragingLise { get; set; } = 16;
    }
}
