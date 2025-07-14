using System.Runtime.Serialization;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
    
namespace UnitySC.Shared.Format.Metro.Bow
{
    public class BowResultSettings
    {
        [DataMember]
        public Length BowTargetMin{ get; set; }
        [DataMember]
        public Length BowTargetMax { get; set; }
    }
}
