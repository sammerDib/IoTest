using System;
using System.Runtime.Serialization;
using UnitySC.Shared.Data;
using UnitySC.Shared.Image;

namespace UnitySC.PM.EME.Service.Interface.Recipe.Execution
{
    [DataContract]
    public class RecipeExecutionMessage
    {
        [DataMember]
        public ExecutionStatus Status { get; set; }

        [DataMember] 
        public int TotalImages { get; set; }

        [DataMember] 
        public int ImageIndex { get; set; }

        [DataMember] 
        public string ErrorMessage { get; set; }
        [DataMember]
        public RemoteProductionInfo CurrentRemoteProductionInfo;
        [DataMember]
        public Guid? RecipeKey { get; set; }
        [DataMember]
        public ServiceImage Thumbnail { get; set; }
        [DataMember]
        public string ImageFilePath { get; set; }
    }
}
