using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public enum Protocol
    {
        OPC = 0,
        RS232,
        Ethernet,
        USB
    }

    [XmlInclude(typeof(ACSControllerConfig))]
    [XmlInclude(typeof(AerotechControllerConfig))]
    [XmlInclude(typeof(IOControllerConfig))]
    [XmlInclude(typeof(MCCControllerConfig))]
    [XmlInclude(typeof(NICouplerControllerConfig))]
    [XmlInclude(typeof(PIE709ControllerConfig))]
    [XmlInclude(typeof(PiezoControllerConfig))]
    [XmlInclude(typeof(RCMControllerConfig))]
    [XmlInclude(typeof(SMC100ControllerConfig))]
    [XmlInclude(typeof(DummyControllerConfig))]
    [XmlInclude(typeof(BeckhoffPlcControllerConfig))]
    [XmlInclude(typeof(LaserPiano450ControllerConfig))]
    [XmlInclude(typeof(LaserSMD12ControllerConfig))]
    [XmlInclude(typeof(ShutterSh10pilControllerConfig))]
    [XmlInclude(typeof(SpectrometerAvantesControllerConfig))]
    [XmlInclude(typeof(ScreenDensitronDM430GNControllerConfig))]
    [KnownType(typeof(ACSControllerConfig))]
    [KnownType(typeof(AerotechControllerConfig))]
    [KnownType(typeof(IOControllerConfig))]
    [KnownType(typeof(MCCControllerConfig))]
    [KnownType(typeof(NICouplerControllerConfig))]
    [KnownType(typeof(OpcControllerConfig))]
    [KnownType(typeof(PIE709ControllerConfig))]
    [KnownType(typeof(PiezoControllerConfig))]
    [KnownType(typeof(RCMControllerConfig))]
    [KnownType(typeof(SMC100ControllerConfig))]
    [KnownType(typeof(DummyControllerConfig))]
    [KnownType(typeof(BeckhoffPlcControllerConfig))]
    [KnownType(typeof(LaserPiano450ControllerConfig))]
    [KnownType(typeof(LaserSMD12ControllerConfig))]
    [KnownType(typeof(ShutterSh10pilControllerConfig))]
    [KnownType(typeof(SpectrometerAvantesControllerConfig))]
    [XmlInclude(typeof(ACSLightControllerConfig))]
    [XmlInclude(typeof(ENTTECLightControllerConfig))]
    [KnownType(typeof(ScreenDensitronDM430GNControllerConfig))]
    [KnownType(typeof(ArduinoLightControllerConfig))]
    [KnownType(typeof(EvosensLightControllerConfig))]
    [DataContract]
    public class ControllerConfig : IDeviceConfiguration
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public bool IsSimulated { get; set; }

        [DataMember]
        public DeviceLogLevel LogLevel { get; set; }

        [DataMember]
        public string ControllerTypeName { get; set; }

        [DataMember]
        public Protocol Protocol { get; set; }
    }
}
