using System.Runtime.Serialization;

namespace UnitySC.PM.HLS.Service.Interface
{
    [DataContract]
    public enum HLSState
    {
        [EnumMember] NotInitialized,
        [EnumMember] Initializing,
        [EnumMember] Running,
        [EnumMember] Error
    };
}
