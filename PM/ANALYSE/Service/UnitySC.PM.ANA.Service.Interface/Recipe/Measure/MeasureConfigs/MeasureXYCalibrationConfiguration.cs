using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class MeasureXYCalibrationConfiguration : MeasureConfigurationBase
    {
        [DataMember]
        public int PreAlignmentNbDiesPerBranch { get; set; }

        [DataMember]
        public int PreAlignmentDiesPeriodicityFromCenter { get; set; }
    }
}
