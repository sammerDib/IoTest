using System;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Dataflow.Shared
{
    [DataContract]
    public class DAPData
    {
        [DataMember]
        public Guid Token { get; set; }

        [DataMember]
        public string Data { get; set; }

        /// <summary>
        /// Tag if the data can be deleted after use
        /// </summary>
        [DataMember]
        public bool DataDeletable { get; set; } = true;
    }
}
