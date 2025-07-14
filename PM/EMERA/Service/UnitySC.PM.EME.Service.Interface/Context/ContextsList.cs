using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace UnitySC.PM.EME.Service.Interface.Context
{
    [DataContract]
    public class ContextsList : EMEContextBase
    {
        [DataMember]
        public List<EMEContextBase> Contexts { get; set; } = new List<EMEContextBase>();

        // For serialization
        public ContextsList()
        {
        }

        public ContextsList(params EMEContextBase[] contexts)
        {
            Contexts = contexts.ToList();
        }
    }
}
