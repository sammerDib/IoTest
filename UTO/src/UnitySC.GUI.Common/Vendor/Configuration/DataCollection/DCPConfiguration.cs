using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Agileo.DataMonitoring.Configuration;

namespace UnitySC.GUI.Common.Vendor.Configuration.DataCollection
{
    [Serializable]
    [DataContract(Namespace = "")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "False positive")]
    public class DCPConfiguration : DataCollectionPlanConfiguration
    {
        public DCPConfiguration()
        {
            OnCreated();
        }

        #region Properties

        [XmlArray]
        [XmlArrayItem(typeof(string))]
        [DataMember]
        public List<SourceColorConfiguration> SeriesColors { get; set; }

        [XmlArray]
        [XmlArrayItem(typeof(AxisMinMaxLogConfiguration))]
        [DataMember]
        public List<AxisMinMaxLogConfiguration> AxesMinMaxLog { get; set; }

        #endregion Properties

        [OnDeserializing]
        private void OnDeserializing(StreamingContext c)
        {
            OnCreated();
        }

        private void OnCreated()
        {
            SeriesColors = new List<SourceColorConfiguration>();
            AxesMinMaxLog = new List<AxisMinMaxLogConfiguration>();
        }
    }
}
