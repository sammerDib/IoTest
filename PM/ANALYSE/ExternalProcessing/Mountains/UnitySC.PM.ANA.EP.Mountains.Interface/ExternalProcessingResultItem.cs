using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.EP.Mountains.Interface
{
    [DataContract]
    public class ExternalProcessingResultItem
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [DataMember]
        public string StringValue { get; set; }

        [DataMember]
        public double? DoubleValue { get; set; }
    }
}
