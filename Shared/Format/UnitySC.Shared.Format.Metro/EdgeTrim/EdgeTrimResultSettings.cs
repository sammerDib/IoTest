using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.EdgeTrim
{
    [DataContract]
    public class EdgeTrimResultSettings
    {
        [DataMember]
        public LengthTolerance HeightTolerance { get; set; }

        [DataMember]
        public Length HeightTarget { get; set; }

        [DataMember]
        public LengthTolerance WidthTolerance { get; set; }

        [DataMember]
        public Length WidthTarget { get; set; }

        [DataMember]
        public bool StepUp { get; set; } = false;
    }
}
