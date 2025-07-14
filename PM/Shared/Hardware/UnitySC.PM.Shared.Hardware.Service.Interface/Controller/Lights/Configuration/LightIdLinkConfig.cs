using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class LightSource : LightIdLink
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }
    }


    [Serializable]
    [DataContract]
    public class LightIdLink
    {
        [DataMember]
        public string LightID { get; set; }

        [DataMember]
        public int Address { get; set; }
    }
}
