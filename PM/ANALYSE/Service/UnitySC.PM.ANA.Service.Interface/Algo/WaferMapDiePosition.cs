using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [Serializable]
    public class WaferMapDiePosition
    {
        [DataMember]
        public int DiePositionX { get; set; }

        [DataMember]
        public int DiePositionY { get; set; }

        [DataMember]
        // Position in mm in the wafer Referential
        public double TopLeftCornerX { get; set; }

        [DataMember]
        // Position in mm in the wafer Referential
        public double TopLeftCornerY { get; set; }
    }
}
