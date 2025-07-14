using System.Runtime.Serialization;

namespace UnitySC.PM.DMT.Service.Interface.Fringe
{
    [DataContract]
    public enum FringeType
    {
        [EnumMember] Standard,
        [EnumMember] Multi
    }
}
