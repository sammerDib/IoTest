using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.GUI.Common.Vendor.Configuration.DataCollection
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class DataCollectionPlansConfiguration
    {
        public DataCollectionPlansConfiguration()
        {
            OnCreated();
        }

        [XmlArray]
        [XmlArrayItem(typeof(DCPConfiguration))]
        [DataMember]
        public List<DCPConfiguration> DataCollectionPlanConfigurations { get; set; }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext c)
        {
            OnCreated();
        }

        private void OnCreated()
        {
            DataCollectionPlanConfigurations = new List<DCPConfiguration>();
        }
    }
}
