using System.Runtime.Serialization;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    [DataContract]
    public class WaferConfiguration
    {        
        [DataMember]
        public Length WaferDiameter { get; set; }

        [DataMember]
        public WaferReferentialSettings WaferReferentialSettings { get; set; }
    }
}
