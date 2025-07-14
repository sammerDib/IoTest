using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public class LightContext : ANAContextBase
    {
        // Needed for XML serialization
        public LightContext()
        {
        }

        public LightContext(string deviceID, double intensity)
        {
            DeviceID = deviceID;
            Intensity = intensity;
        }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        public double Intensity { get; set; }
    }
}
