using System;

using System.Runtime.Serialization;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class XPosition : OneAxisPosition
    {
        public XPosition(ReferentialBase referential, double x)
            : base(referential, "X", x)
        {
        }
    }

    [DataContract]
    public class TPosition : OneAxisPosition
    {
        public TPosition(ReferentialBase referential, double t)
            : base(referential, "T", t)
        {
        }
    }

    [DataContract]
    public class YPosition : OneAxisPosition
    {
        public YPosition(ReferentialBase referential, double y)
            : base(referential, "Y", y)
        {
        }
    }

    [DataContract]
    public class ZPosition : OneAxisPosition
    {
        public ZPosition(ReferentialBase referential, double z)
            : base(referential, "Z", z)
        {
        }
    }

    [DataContract]
    public class ZTopPosition : OneAxisPosition
    {
        public ZTopPosition(ReferentialBase referential, double zTop)
            : base(referential, "ZTop", zTop)
        {
        }
    }

    [DataContract]
    public class ZBottomPosition : OneAxisPosition
    {
        public ZBottomPosition(ReferentialBase referential, double zBottom)
            : base(referential, "ZBottom", zBottom)
        {
        }
    }

    [DataContract]
    public class LinearPosition : OneAxisPosition
    {
        public LinearPosition(ReferentialBase referential, double linear)
            : base(referential, "Linear", linear)
        {
        }
    }

    [DataContract]
    public class RotationPosition : OneAxisPosition
    {
        public RotationPosition(ReferentialBase referential, double rotation)
            : base(referential, "Rotation", rotation)
        {
        }
    }

    [DataContract]
    public class UnknownPosition : OneAxisPosition
    {
        public UnknownPosition(ReferentialBase referential, double pos)
            : base(referential, "", pos)
        {
        }
    }

    [DataContract]
    public abstract class OneAxisPosition : PositionBase, IEquatable<OneAxisPosition>
    {
        [DataMember]
        public virtual double Position { get; set; } // FIXME: use Length instead of double

        [DataMember]
        public string AxisID { get; set; }

        public OneAxisPosition(ReferentialBase referential, string axisID, double position) : base(referential)
        {
            AxisID = axisID;
            Position = position;
        }

        public override int GetHashCode() => (base.GetHashCode(), Position, AxisID).GetHashCode();

        public override bool Equals(object other)
        {
            return (other is OneAxisPosition otherOneAxisPosition) ? Equals(otherOneAxisPosition) : false;
        }

        public bool Equals(OneAxisPosition otherOneAxisPosition)
        {
            if (otherOneAxisPosition is null) return false;

            bool hasSameReferential = Referential == otherOneAxisPosition.Referential;
            bool hasSameAxisID = AxisID == otherOneAxisPosition.AxisID;
            bool hasSamePosition = Position == otherOneAxisPosition.Position;

            return hasSameReferential && hasSameAxisID && hasSamePosition;
        }

        public static bool operator ==(OneAxisPosition lPosition, OneAxisPosition rPosition)
        {
            if (lPosition is null && rPosition is null)
            {
                return true;
            }

            if (lPosition is null || rPosition is null)
            {
                return false;
            }

            return lPosition.Equals(rPosition);
        }

        public static bool operator !=(OneAxisPosition lPosition, OneAxisPosition rPosition)
        {
            if (lPosition is null && rPosition is null)
            {
                return false;
            }

            if (lPosition is null || rPosition is null)
            {
                return true;
            }

            return !lPosition.Equals(rPosition);
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public bool Near(OneAxisPosition otherPosition, Length epsilon)
        {
            return Position.Near(otherPosition.Position, epsilon.Millimeters);
        }       
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {            
            if (otherPosition is OneAxisPosition other)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                return Position.Near(other.Position, tolerance.Millimeters);                
            }
            return false;
        }
    }

    [DataContract]
    public class TopXPosition : OneAxisPosition
    {
        public TopXPosition(ReferentialBase referential, double topX)
            : base(referential, "TopX", topX)
        {
        }
    }

    [DataContract]
    public class TopZPosition : OneAxisPosition
    {
        public TopZPosition(ReferentialBase referential, double topY)
            : base(referential, "TopZ", topY)
        {
        }
    }

    [DataContract]
    public class BottomXPosition : OneAxisPosition
    {
        public BottomXPosition(ReferentialBase referential, double bevelTop)
            : base(referential, "BevelTop", bevelTop)
        {
        }
    }

    [DataContract]
    public class BevelTopZPosition : OneAxisPosition
    {
        public BevelTopZPosition(ReferentialBase referential, double bevelBottom)
            : base(referential, "BevelBottom", bevelBottom)
        {
        }
    }

    [DataContract]
    public class ApexZPosition : OneAxisPosition
    {
        public ApexZPosition(ReferentialBase referential, double apex)
            : base(referential, "Apex", apex)
        {
        }
    }
}
