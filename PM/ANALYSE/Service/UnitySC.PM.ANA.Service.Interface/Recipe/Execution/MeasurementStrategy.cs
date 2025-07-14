using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Execution
{
    [DataContract]
    public enum MeasurementStrategy
    {
        [EnumMember]
        PerMeasurementType,
        [EnumMember]
        PerPoint,
    }

}
