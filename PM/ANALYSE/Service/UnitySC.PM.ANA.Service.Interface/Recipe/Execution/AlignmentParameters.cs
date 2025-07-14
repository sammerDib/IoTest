using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Execution
{
    [DataContract]
    public class AlignmentParameters
    {
        [DataMember]
        public bool RunAutoFocus { get; set; }

        [DataMember]
        public bool RunAutoLight { get; set; }

        [DataMember]
        public bool RunBwa { get; set; }

        [DataMember]
        public bool RunMarkAlignment { get; set; }
    }
}
