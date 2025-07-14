using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class ZPiezoPosition : OneAxisPosition
    {
        public Length PiezoPosition => Position.Micrometers();

        public ZPiezoPosition() : base(null, null, double.NaN)
        {
            // Required by XML serialization
        }

        public ZPiezoPosition(ReferentialBase referential, string axisID, Length zPiezoPosition) : base(referential, axisID, zPiezoPosition.Micrometers)
        {
        }

        public override bool Equals(object other)
        {
            return (other is ZPiezoPosition otherZPiezoPosition) ? Equals(otherZPiezoPosition) : false;
        }

        public bool Equals(ZPiezoPosition otherZPiezoPosition)
        {
            if (otherZPiezoPosition is null) return false;

            bool hasSameReferential = Referential == otherZPiezoPosition.Referential;
            bool hasSameAxisID = AxisID == otherZPiezoPosition.AxisID;
            bool hasSamePosition = Position == otherZPiezoPosition.Position;

            return hasSameReferential && hasSameAxisID && hasSamePosition;
        }

        public override int GetHashCode() => (base.GetHashCode(), PiezoPosition).GetHashCode();
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            if (otherPosition is ZPiezoPosition otherZPiezoPosition)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                return PiezoPosition.Near(otherZPiezoPosition.PiezoPosition, tolerance);
            }
            return false;
        }
    }
}
