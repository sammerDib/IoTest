using System;
using System.Runtime.Serialization;

using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.Shared.Data;

namespace UnitySC.PM.DMT.Service.Interface.Recipe
{
    [DataContract]
    [KnownType(typeof(AutoExposureStatus))]
    public class RecipeStatus
    {
        [DataMember]
        public DMTRecipeState State = DMTRecipeState.Preparing;

        [DataMember]
        public DMTRecipeExecutionStep Step = DMTRecipeExecutionStep.Acquisition;

        [DataMember]
        public int TotalSteps;

        [DataMember]
        public int CurrentStep;

        [DataMember]
        public string Message;

        [DataMember]
        public RemoteProductionInfo CurrentRemoteProductionInfo;

        [DataMember]
        public Guid RecipeKey { get; set; }
    }

}
