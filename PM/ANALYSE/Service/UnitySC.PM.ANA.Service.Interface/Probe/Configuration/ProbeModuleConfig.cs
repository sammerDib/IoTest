using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    [Serializable]
    public class ProbeModuleConfig : IProbeModuleConfig
    {
        public string DeviceID { get; set; }
        public string Name { get; set; }
        public ProbeModuleSettings ProbeModuleSettings { get; set; }

        [XmlArray("Lights")]
        [XmlArrayItem("Light")]
        public List<string> LightsID { get; set; }

        public string ObjectivesSelectorID { get; set; }

        [XmlArray("Cameras")]
        [XmlArrayItem("Camera")]
        public List<string> CamerasID { get; set; }

        [XmlArrayItem(typeof(ProbeLiseConfig))]
        [XmlArrayItem(typeof(ProbeDualLiseConfig))]
        [XmlArrayItem(typeof(ProbeLiseHFConfig))]
        public List<ProbeConfigBase> Probes { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsSimulated { get; set; }
        public DeviceLogLevel LogLevel { get; set; }

        public ProbeConfigBase GetProbeFromID(string probeID)
        {
            return Probes.FirstOrDefault(p => (p as IProbeConfig).DeviceID == probeID);
        }
    }
}
