using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Trench
{
    [DataContract]
    public class TrenchResultSettings
    {
        [DataMember]
        public LengthTolerance WidthTolerance { get; set; }
        [DataMember]
        public Length WidthTarget { get; set; }

        [DataMember]
        public LengthTolerance DepthTolerance { get; set; }
        [DataMember]
        public Length DepthTarget { get; set; }

    }
}
