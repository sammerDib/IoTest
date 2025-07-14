using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.EME.Service.Interface.FilterWheel
{
    [Serializable]
    [DataContract]
    public class FilterSlot
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Position { get; set; }
    }
}
