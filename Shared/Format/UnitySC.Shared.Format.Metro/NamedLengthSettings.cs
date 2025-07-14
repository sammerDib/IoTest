using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using System.Runtime.Serialization;
using UnitySC.Shared.Format.Metro.Thickness;

namespace UnitySC.Shared.Format.Metro
{

    [KnownType(typeof(ThicknessLengthSettings))]
    [DataContract]
    public class NamedLengthSettings
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Length Target { get; set; }
        [DataMember]
        public LengthTolerance Tolerance { get; set; }
    }


}
