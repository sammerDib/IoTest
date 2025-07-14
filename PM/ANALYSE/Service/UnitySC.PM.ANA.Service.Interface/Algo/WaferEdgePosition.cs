using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public enum WaferEdgePositions
    {
        [EnumMember]
        Top,

        [EnumMember]
        Right,

        [EnumMember]
        Bottom,

        [EnumMember]
        Left,
    }
}
