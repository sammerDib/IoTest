using System.Runtime.Serialization;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class BareWaferAlignmentSettings
    {
        [DataMember]
        public PositionBase ImagePositionTop { get; set; }
        
        [DataMember]
        public PositionBase ImagePositionRight { get; set; }

        [DataMember]
        public PositionBase ImagePositionBottom { get; set; }

        [DataMember]
        public PositionBase ImagePositionLeft { get; set; }
    }
}
