using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.AlignmentMarks
{
    [DataContract]
    public class AlignmentMarksSettings
    {
        [DataMember]
        public List<PositionWithPatternRec> AlignmentMarksSite1 { get; set; }

        [DataMember]
        public List<PositionWithPatternRec> AlignmentMarksSite2 { get; set; }

        [DataMember]
        public AutoFocusSettings AutoFocus { get; set; }

        [DataMember]
        public ObjectiveContext ObjectiveContext { get; set; }
    }
}
