using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    public class ArgosPosition : PositionBase, IEquatable<ArgosPosition>
    {
        [DataMember]
        public double TopX { get; set; }

        [DataMember]
        public double TopZ { get; set; }

        [DataMember]
        public double BottomX { get; set; }

        [DataMember]
        public double BevelTopZ { get; set; }

        [DataMember]
        public double ApexZ { get; set; }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double T { get; set; }

        public ArgosPosition(ReferentialBase referential) : base(referential)
        {
        }

        public ArgosPosition(ReferentialBase referential, double topX, double topZ, double bottomX, double bevelTopZ, double apexZ, double x, double t) : base(referential)
        {
            TopX = topX;
            TopZ = topZ;
            BottomX = bottomX;
            BevelTopZ = bevelTopZ;
            ApexZ = apexZ;
            X = x;
            T = t;
        }

        public bool IsValid { get => !(double.IsNaN(BevelTopZ) || double.IsNaN(BottomX) || double.IsNaN(TopZ) || double.IsNaN(TopX) || double.IsNaN(ApexZ) || double.IsNaN(X) || double.IsNaN(T)); }

        public bool Equals(ArgosPosition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TopX.Equals(other.TopX) && TopZ.Equals(other.TopZ) && BottomX.Equals(other.BottomX) && BevelTopZ.Equals(other.BevelTopZ) && ApexZ.Equals(other.ApexZ) && X.Equals(other.X) && T.Equals(other.T);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ArgosPosition)obj);
        }

        public override object Clone()
        {
            return new ArgosPosition(Referential, TopX, TopZ, BottomX, BevelTopZ, ApexZ, X, T);
        }

        public override int GetHashCode() => (Referential, TopX, TopZ, BottomX, BevelTopZ, ApexZ, X, T).GetHashCode();
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            if (otherPosition is ArgosPosition otherArgosPosition)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                bool isNearTopX = TopX.Near(otherArgosPosition.TopX, tolerance.Millimeters);
                bool isNearTopZ = TopZ.Near(otherArgosPosition.TopZ, tolerance.Millimeters);
                bool isNearBottomX = BottomX.Near(otherArgosPosition.BottomX, tolerance.Millimeters);
                bool isNearBevelTopZ = BevelTopZ.Near(otherArgosPosition.BevelTopZ, tolerance.Millimeters);
                bool isNearApexZ = ApexZ.Near(otherArgosPosition.ApexZ, tolerance.Millimeters);
                bool isNearX = X.Near(otherArgosPosition.X, tolerance.Millimeters);
                bool isNearT = T.Near(otherArgosPosition.T, tolerance.Millimeters);

                return isNearTopX && isNearTopZ && isNearBottomX && isNearBevelTopZ && isNearApexZ && isNearX && isNearT;
            }
            return false;
        }
    }
}
