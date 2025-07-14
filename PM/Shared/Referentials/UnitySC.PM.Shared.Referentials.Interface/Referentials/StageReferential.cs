using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class StageReferential : ReferentialBase
    {
        public StageReferential() : base(ReferentialTag.Stage)
        {
        }

        public override bool Equals(object other)
        {
            if (other is StageReferential stageReferential)
            {
                return stageReferential.Tag == Tag;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode();
        }
    }
}
