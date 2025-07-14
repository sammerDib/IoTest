using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    /// <summary>
    /// Comprise move orders for one axis.
    /// </summary>
    ///

    [DataContract]
    public class OneAxisMove : IncrementalMoveBase, IEquatable<OneAxisMove>
    {
        [DataMember]
        public double Distance { get; set; }

        [DataMember]
        public String AxisID { get; set; }

        public OneAxisMove(String axisID, double distance) :
            base(new MotorReferential())

        {
            AxisID = axisID;
            Distance = distance;
        }

        public bool Equals(OneAxisMove other)
        {
            return (
                (Referential == other.Referential) &&
                (Distance == other.Distance) &&
                (AxisID == other.AxisID)
            );
        }

        public override int GetHashCode() => (Referential, Distance, AxisID).GetHashCode();

        public override bool Equals(object other)
        {
            if (other is OneAxisMove otherOfGoodType)
            {
                return Equals(otherOfGoodType);
            }
            return false;
        }

        public static bool operator ==(OneAxisMove left, OneAxisMove right)
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

        public static bool operator !=(OneAxisMove left, OneAxisMove right)
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

        public override object Clone()
        {
            var clone = new OneAxisMove(AxisID, Distance);
            return clone;
        }
    }
}
