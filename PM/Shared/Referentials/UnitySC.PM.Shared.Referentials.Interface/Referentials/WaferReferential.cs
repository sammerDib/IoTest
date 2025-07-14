using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class WaferReferential : ReferentialBase
    {
        public WaferReferential() : base(ReferentialTag.Wafer)
        {
        }

        public override bool Equals(object other)
        {
            if (other is WaferReferential waferReferential)
            {
                return waferReferential.Tag == Tag;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode();
        }
    }
}
