using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    [KnownType(typeof(WaferReferential))]
    [KnownType(typeof(MotorReferential))]
    [KnownType(typeof(DieReferential))]
    [KnownType(typeof(StageReferential))]
    public abstract class IncrementalMoveBase : ICloneable
    {
        public IncrementalMoveBase(ReferentialBase referential)
        {
            Referential = referential;
        }

        [DataMember]
        public readonly ReferentialBase Referential;

        public abstract object Clone();
    }
}
