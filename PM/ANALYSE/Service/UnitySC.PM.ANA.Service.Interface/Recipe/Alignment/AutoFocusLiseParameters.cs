using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Alignment
{
    [DataContract]
    public class AutoFocusLiseParameters
    {
        [DataMember]
        public bool ZIsDefinedByUser { get; set; }

        [DataMember]
        public Length ZTopFocus { get; set; }

        [DataMember]
        public bool LiseParametersAreDefinedByUser { get; set; }

        [DataMember]
        public double LiseGain { get; set; }

        [DataMember]
        public Length ZMin { get; set; }

        [DataMember]
        public Length ZMax { get; set; }

        [DataMember]
        public ObjectiveContext LiseObjectiveContext { get; set; }
    }
}
