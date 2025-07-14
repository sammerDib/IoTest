using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeCalibResultsBase : IProbeCalibResult
    {
        [DataMember]
        public string ProbeID { get; set; }

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
