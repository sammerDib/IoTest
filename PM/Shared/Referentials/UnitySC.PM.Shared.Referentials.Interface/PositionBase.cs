using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    [KnownType(typeof(WaferReferential))]
    [KnownType(typeof(MotorReferential))]
    [KnownType(typeof(DieReferential))]
    [KnownType(typeof(StageReferential))]
    [XmlInclude(typeof(WaferReferential))]
    [XmlInclude(typeof(MotorReferential))]
    [XmlInclude(typeof(DieReferential))]
    [XmlInclude(typeof(StageReferential))]
    public abstract class PositionBase : ICloneable
    {
        public PositionBase()
        {
        }

        public PositionBase(ReferentialBase referential)
        {
            Referential = referential;
        }

        [DataMember]
        public ReferentialBase Referential;

        public abstract object Clone();

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine($"\tReferential = {Referential.Tag}")
                .ToString();
        }

        public override int GetHashCode() => Referential.GetHashCode();
        public abstract bool Near(PositionBase otherPosition, Length tolerance = null);        
    }
}
