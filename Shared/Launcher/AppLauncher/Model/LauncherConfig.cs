using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AppLauncher
{
    [Serializable]
    public class LauncherConfig
    {
        [XmlArray("Applications")]
        [XmlArrayItem("Application")]
        public List<LauncherApplicationConfig> Applications { get; set; }

        [XmlArray("Services")]
        [XmlArrayItem("Service")]
        public List<LauncherServiceConfig> Services { get; set; }

        public bool DisplayStopAll { get; set; }
    }
}
