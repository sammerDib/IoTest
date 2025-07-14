using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [DataContract]
    public class ServicePoint : IEquatable<ServicePoint>
    {
        public ServicePoint()
        {
        }

        public ServicePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        [DataMember]
        public double X;

        [DataMember]
        public double Y;

        public override string ToString()
        {
            return "[" + X + "," + Y + "]";
        }

        public override bool Equals(object other)
        {
            if (other is ServicePoint otherServicePoint)
            {
                return Equals(otherServicePoint);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(ServicePoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y).GetHashCode();
        }
    }
}
