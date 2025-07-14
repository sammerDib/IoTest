using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto
{
    public partial class Chamber
    {
        [DataMember]
        public ActorType Actor
        {
            get
            {
                return (ActorType)this.ActorType;
            }
            set
            {
                this.ActorType = (int)value;
            }
        }
    }
}