using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    /// <summary>
    /// Context of all selected objectives in each objective selector.
    /// </summary>
    [DataContract]
    public class ObjectivesContext : ANAContextBase
    {
        [DataMember]
        public List<ObjectiveContext> Objectives { get; set; } = new List<ObjectiveContext>();
    }
}
