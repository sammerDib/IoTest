using System;
using System.Runtime.Serialization;
using System.Text;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class XTPosition : PositionBase, IEquatable<XTPosition>
    {
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double T { get; set; }

        // Required by XmlSerializer
        public XTPosition() : base(null)
        {
        }

        public XTPosition(ReferentialBase referential) : base(referential)
        {
        }

        public XTPosition(ReferentialBase referential, double x, double t)
            : base(referential)
        {
            X = x;
            T = t;
        }

        public override int GetHashCode() => (base.GetHashCode(), X, T).GetHashCode();

        public override bool Equals(object other)
        {
            if (other is XTPosition otherOfGoodType)
            {
                return Equals(otherOfGoodType);
            }
            return false;
        }

        public bool Equals(XTPosition other)
        {
            if (other is null)
                return false;

            return (
                (Referential == other.Referential) &&
                (X == other.X || (X.IsNaN() && other.X.IsNaN())) &&
                (T == other.T || (T.IsNaN() && other.T.IsNaN()))

            );
        }

        public static bool operator ==(XTPosition lPosition, XTPosition rPosition)
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

        public static bool operator !=(XTPosition lPosition, XTPosition rPosition)
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

        /// <summary>
        /// Set not NaN values X, Y, ZTop, ZBottom and ZPiezoPositions given in parameter as current property values.
        /// </summary>
        public void Merge(XTPosition xtPosition)
        {
            X = double.IsNaN(xtPosition.X) ? X : xtPosition.X;
            T = double.IsNaN(xtPosition.T) ? T : xtPosition.T;
        }


        public override object Clone()
        {
            return new XTPosition(Referential, X, T);
        }

        public override string ToString()
        {
            return new StringBuilder(base.ToString())
                .AppendLine($"\tX = {X}")
                .AppendLine($"\tT = {T}")
                .ToString();
        }       
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            if (otherPosition is XTPosition other)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                return X.Near(other.X, tolerance.Millimeters) && T.Near(other.T, tolerance.Millimeters);
            }
            return false;
        }
    }
}
