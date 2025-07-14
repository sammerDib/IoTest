using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public class PMContext : ANAContextBase
    {
        [DataMember]
        public ChamberContext Context { get; set; }

        [DataMember]
        public XYPositionContext XyPosition { get; set; }

        [DataMember]
        public LightsContext Lights { get; set; }

        [DataMember]
        public ObjectivesContext Objectives { get; set; }
    }
}
