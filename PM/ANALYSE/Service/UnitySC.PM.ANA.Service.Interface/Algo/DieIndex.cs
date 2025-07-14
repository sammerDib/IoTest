using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    
    public class DieIndex : IEquatable<DieIndex>
    {
        public DieIndex()
        {
        }

        public DieIndex(int column, int row)
        {
            Row = row;
            Column = column;
        }

        [DataMember]
        [XmlAttribute(AttributeName = "Row")]
        public int Row { get; set; }

        [DataMember]
        [XmlAttribute(AttributeName = "Column")]
        public int Column { get; set; }

        public override int GetHashCode() => (Column, Row).GetHashCode();

        public static bool operator ==(DieIndex lhs, DieIndex rhs)
        {
            if (lhs is null && rhs is null)
                return true;

            if (lhs is null)
                return false;

            return lhs.Equals(rhs);
        }

        public static bool operator !=(DieIndex lhs, DieIndex rhs) => !(lhs == rhs);

        public bool Equals(DieIndex other)
        {
            return !(other is null) &&
               other.Row == Row &&
               other.Column == Column;
        }

        public override bool Equals(object obj) => Equals(obj as DieIndex);
    }
}
