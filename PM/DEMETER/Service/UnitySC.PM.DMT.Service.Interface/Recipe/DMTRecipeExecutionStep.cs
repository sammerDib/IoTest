using System.Runtime.Serialization;

namespace UnitySC.PM.DMT.Service.Interface.Recipe
{
    [DataContract]
    public enum DMTRecipeExecutionStep
    {
        [EnumMember] 
        Acquisition,
        [EnumMember] 
        Computation,
    }
}
