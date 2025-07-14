using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public enum AutoFocusType
    {
        [EnumMember]
        Camera,

        [EnumMember]
        Lise,

        [EnumMember]
        LiseAndCamera
    }
}
