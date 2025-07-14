using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Common;

namespace UnitySC.PM.Shared.Hardware.OpticalPowermeter
{
    public class PM101OpticalPowermeterConfig : OpticalPowermeterConfig
    {
        public SerialCom SerialCommunication;
        public OpcCom OpcCommunication;

        [XmlArrayItem("Wavelength")]
        public List<string> Wavelengths;

        [XmlArrayItem("Range")]
        public List<string> Ranges;

        [XmlArrayItem("BeamDiameter")]
        public List<string> BeamDiameters;
    }
}
