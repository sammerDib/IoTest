using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class DieReferential : ReferentialBase
    {
        public DieReferential() : base(ReferentialTag.Die)
        {
        }

        [DataMember]
        public readonly int DieColumn;

        [DataMember]
        public readonly int DieLine;

        public DieReferential(int column, int line) : base(ReferentialTag.Die)
        {
            DieColumn = column;
            DieLine = line;
        }

        public override bool Equals(object other)
        {
            if (other is DieReferential dieReferential)
            {
                return dieReferential.Tag == Tag
                    && dieReferential.DieColumn == DieColumn
                    && dieReferential.DieLine == DieLine;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Tag, DieColumn, DieLine).GetHashCode();
        }
    }
}
