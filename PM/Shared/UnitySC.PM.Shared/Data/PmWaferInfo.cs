using System.Runtime.Serialization;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Data
{
    [DataContract(Namespace = "")]
    public class PmWaferInfo
    {
        private PMConfiguration _pmConfiguration;

        public PmWaferInfo()
        {
            _pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            LotName = _pmConfiguration.DefaultLotName;
            JobName = _pmConfiguration.DefaultJobName;
            SlotId = _pmConfiguration.DefaultSlotId;
            WaferName = _pmConfiguration.DefaultWaferName;
            LoadPort = _pmConfiguration.DefaultLoadPort;
        }

        [DataMember]
        public string LotName { get; set; }

        [DataMember]
        public string JobName { get; set; }

        [DataMember]
        public string WaferName { get; set; }

        [DataMember]
        public int SlotId { get; set; }

        [DataMember]
        public string LoadPort { get; set; }

        [DataMember]
        public string TCRecipeName { get; set; }
    }
}
