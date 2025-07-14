using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ObjectiveResult
    {
        #region Properties

        [DataMember]
        public string ObjectiveID { get; set; }

        [DataMember]
        public bool Success { get; set; }

        #endregion Properties

        public ObjectiveResult(string objectiveID, bool success)
        {
            ObjectiveID = objectiveID;
            Success = success;
        }
    }
}