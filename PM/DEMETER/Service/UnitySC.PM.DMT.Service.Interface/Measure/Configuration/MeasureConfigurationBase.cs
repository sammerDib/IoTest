using System.Runtime.Serialization;

namespace UnitySC.PM.DMT.Service.Interface.Measure.Configuration
{
    [DataContract]
    [KnownType(typeof(BackLightMeasureConfiguration))]
    [KnownType(typeof(BrightFieldMeasureConfiguration))]
    [KnownType(typeof(DeflectometryMeasureConfiguration))]
    [KnownType(typeof(HighAngleDarkFieldMeasureConfiguration))]
    public abstract class MeasureConfigurationBase
    {
        [DataMember]
        public bool IsAvailable { get; set; } = false;
    }
}
