using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto
{
    public partial class Result
    {
        [DataMember]
        public ActorType ActorTypeEnum
        {
            get
            {
                return (ActorType)ActorType;
            }
            set
            {
                ActorType = (int)value;
            }
        }
    }
}
