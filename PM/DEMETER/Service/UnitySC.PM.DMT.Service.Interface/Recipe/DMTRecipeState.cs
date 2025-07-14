using System.Runtime.Serialization;

namespace UnitySC.PM.DMT.Service.Interface.Recipe
{
    [DataContract]
    public enum DMTRecipeState
    {
        [EnumMember]
        Preparing,

        [EnumMember]
        Executing,

        [EnumMember]
        AcquisitionComplete,

        [EnumMember]
        ExecutionComplete,

        [EnumMember]
        Aborted,

        [EnumMember]
        Failed
    };
}
