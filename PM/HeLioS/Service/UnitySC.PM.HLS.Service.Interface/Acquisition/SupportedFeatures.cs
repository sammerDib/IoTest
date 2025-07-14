using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UnitySC.PM.HLS.Service.Interface.Acquisition
{
    [Serializable]
    public class SupportedFeatures
    {
        [XmlArrayItem("Mode")]
        public List<string> AvailableModes;

        [XmlArrayItem("PolarIN")]
        public List<string> AvailablePolarIN;

        [XmlArrayItem("PolarOUT")]
        public List<string> AvailablePolarOUT;

        // note de rti à voir si utile
        [XmlArrayItem("CalibId")]
        public List<string> AvailableCalibrations;

        [XmlAttribute]
        public int OpticalRackVersion;
    }
}
