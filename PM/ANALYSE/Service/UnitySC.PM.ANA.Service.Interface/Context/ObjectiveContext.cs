using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public class ObjectiveContext : ANAContextBase
    {
        // Needed for XML serialization
        public ObjectiveContext()
        {
        }

        public ObjectiveContext(string objectiveId)
        {
            ObjectiveId = objectiveId;
        }

        [DataMember]
        public string ObjectiveId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ObjectiveContext context &&
                   ObjectiveId == context.ObjectiveId;
        }

        public override int GetHashCode()
        {
            return 669852973 + EqualityComparer<string>.Default.GetHashCode(ObjectiveId);
        }
    }

    [DataContract]
    public class TopObjectiveContext : ObjectiveContext
    {
        // Needed for XML serialization
        public TopObjectiveContext()
        {
        }

        public TopObjectiveContext(string objectiveId) : base(objectiveId)
        {
        }
    }

    [DataContract]
    public class BottomObjectiveContext : ObjectiveContext
    {
        // Needed for XML serialization
        public BottomObjectiveContext()
        {
        }

        public BottomObjectiveContext(string objectiveId) : base(objectiveId)
        {
        }
    }
}
