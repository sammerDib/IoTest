using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Execution
{
    [DataContract]
    public class ExecutionSettings
    {
        [DataMember]
        public AlignmentParameters Alignment { get; set; }

        [DataMember]
        public MeasurementStrategy Strategy { get; set; } = MeasurementStrategy.PerMeasurementType;

    }
}
