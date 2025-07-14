using System.Runtime.Serialization;

using UnitySC.Shared.Data;

namespace UnitySC.DataAccess.Dto
{
    public partial class Material
    {
        [DataMember]
        public MaterialCharacteristic Characteristic { get; set; }
    }
}
