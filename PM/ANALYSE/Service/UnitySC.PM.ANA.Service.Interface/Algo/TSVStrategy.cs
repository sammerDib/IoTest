using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public enum TSVAcquisitionStrategy
    {
        // Movement along a segment (when point position is accurate)
        [EnumMember]
        Standard,

        // Movement along a spiral (when point position is not very accurate)
        [EnumMember]
        Spiral
    }

    [DataContract]
    public enum TSVMeasurePrecision
    {
        // Average on several measures
        [EnumMember]
        Acccurate,

        // Stop for first good measure
        [EnumMember]
        Fast
    }
}
