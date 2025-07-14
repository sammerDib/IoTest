using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public class ContextsList : ANAContextBase
    {
        [DataMember]
        public List<ANAContextBase> Contexts { get; set; } = new List<ANAContextBase>();

        // For serialization
        public ContextsList()
        {
        }

        public ContextsList(params ANAContextBase[] contexts)
        {
            Contexts = contexts.ToList();
        }
    }
}
