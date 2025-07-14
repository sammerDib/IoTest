using System.Runtime.Serialization;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [DataContract]
    public enum LightspeedState
    {
        [EnumMember] NotInitialized,
        [EnumMember] Initializing,
        [EnumMember] Running,
        [EnumMember] Error
    };
}
