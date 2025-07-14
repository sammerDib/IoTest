using System;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data
{
    // For Result File or Serialization
    // Warning : be adviced that modifing existing properties will affect retro compatibilty in result already record
    [DataContract]
    public class RemoteProductionResultInfo
    {
        [DataMember]
        public string DFRecipeName { get; set; } = string.Empty;

        [DataMember]
        public string PMRecipeName { get; set; } = string.Empty;

        [DataMember]
        public string ProcessJobID { get; set; } = string.Empty;

        [DataMember]
        public string LotID { get; set; } = string.Empty;

        [DataMember]
        public string CarrierID { get; set; } = string.Empty;

        [DataMember]
        public int SlotID { get; set; }

        [DataMember]
        public DateTime StartRecipeTime { get; set; } = DateTime.MinValue;

        public RemoteProductionResultInfo()
        {
            
        }

        public RemoteProductionResultInfo(RemoteProductionInfo remoteprodInfo)
        {
            DFRecipeName = remoteprodInfo.DFRecipeName;
            PMRecipeName = remoteprodInfo.ModuleRecipeName;
            ProcessJobID = remoteprodInfo.ProcessedMaterial.ProcessJobID;
            LotID = remoteprodInfo.ProcessedMaterial.LotID;
            CarrierID = remoteprodInfo.ProcessedMaterial.CarrierID;
            SlotID = remoteprodInfo.ProcessedMaterial.SlotID;
            StartRecipeTime = remoteprodInfo.ModuleStartRecipeTime;
        }
    }
}
