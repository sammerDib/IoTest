using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    [KnownType(typeof(DieReferential))]
    [KnownType(typeof(MotorReferential))]
    [KnownType(typeof(StageReferential))]
    [KnownType(typeof(WaferReferential))]
    public abstract class ReferentialBase
    {
        [DataMember]
        public readonly ReferentialTag Tag;

        public ReferentialBase(ReferentialTag tag)
        {
            Tag = tag;
        }

        public abstract override bool Equals(object other);

        public static bool operator ==(ReferentialBase left, ReferentialBase right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(ReferentialBase left, ReferentialBase right)
        {
            if (left is null && right is null)
            {
                return false;
            }

            if (left is null || right is null)
            {
                return true;
            }
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode();
        }
    }
}
