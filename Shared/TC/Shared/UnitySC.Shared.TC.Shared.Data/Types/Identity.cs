using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.TC.Shared.Data
{
    [DataContract(Namespace = "")]
    public class Identity : IComparable<Identity>
    {
        public Identity(int toolkey, ActorType actorType, int chamberID)
        {
            ToolKey = toolkey;
            ActorType = actorType;
            ChamberID = chamberID;
        }

        [DataMember]
        public int ToolKey { get; set; }
        [DataMember]
        public ActorType ActorType { get; set; }
        [DataMember]
        public int ChamberID { get; set; }

        public override string ToString()
        {
            return string.Format("[PM/PP Identity] ActorType={0} ChamberID={1} Toolkey={2}", ActorType, ChamberID, ToolKey);
        }

        public Identity Clone()
        {
            return (Identity)MemberwiseClone();
        }
        public int CompareTo(Identity other)
        {
            if (other == null)
                return 1;

            int toolKeyComparison = ToolKey.CompareTo(other.ToolKey);
            if (toolKeyComparison != 0)
                return toolKeyComparison;

            int actorTypeComparison = ActorType.CompareTo(other.ActorType);
            if (actorTypeComparison != 0)
                return actorTypeComparison;

            int chamberIDComparison = ChamberID.CompareTo(other.ChamberID);
            if (chamberIDComparison != 0)
                return chamberIDComparison;


            return 0;
        }

        public static bool operator ==(Identity identity1, Identity identity2)
        {
            if (ReferenceEquals(identity1, null) && ReferenceEquals(identity2, null))
                return true;

            if (ReferenceEquals(identity1, null) || ReferenceEquals(identity2, null))
                return false;

            return identity1.ToolKey == identity2.ToolKey &&
                   identity1.ActorType == identity2.ActorType &&
                   identity1.ChamberID == identity2.ChamberID;
        }

        public static bool operator !=(Identity identity1, Identity identity2)
        {
            return !(identity1 == identity2);
        }

        public override bool Equals(object obj)
        {
            return obj is Identity identity &&
                   ToolKey == identity.ToolKey &&
                   ActorType == identity.ActorType &&
                   ChamberID == identity.ChamberID;
        }

        public override int GetHashCode()
        {
            return (ToolKey, ActorType, ChamberID).GetHashCode();
        }
    }
}
