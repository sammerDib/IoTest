using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class MotorReferential : ReferentialBase
    {
        public MotorReferential() : base(ReferentialTag.Motor)
        {
        }

        public override bool Equals(object other)
        {
            if (other is MotorReferential motorReferential)
            {
                return motorReferential.Tag == Tag;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode();
        }
    }
}
