using System.Runtime.Serialization;

using UnitySC.Shared.Data;

namespace UnitySC.DataAccess.Dto
{
    public partial class WaferCategory
    {
        [DataMember]
        public WaferDimensionalCharacteristic DimentionalCharacteristic { get; set; }
    }
}