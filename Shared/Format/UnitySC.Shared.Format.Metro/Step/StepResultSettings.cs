using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Step
{
    [DataContract]
    public class StepResultSettings
    {
        [DataMember]
        public LengthTolerance StepHeightTolerance { get; set; }
        [DataMember]
        public Length StepHeightTarget { get; set; }
        [DataMember]
        public bool StepUp { get; set; } = true;
    }
}
