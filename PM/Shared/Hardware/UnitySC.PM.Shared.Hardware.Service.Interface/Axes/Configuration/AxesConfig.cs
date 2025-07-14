using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{

    [Serializable]
    [XmlInclude(typeof(TMAPAxesConfig))]
    [XmlInclude(typeof(NSTAxesConfig))]
    [XmlInclude(typeof(PSDAxesConfig))]
    [XmlInclude(typeof(LSAxesConfig))]
    [XmlInclude(typeof(LiseHfAxesConfig))]
    [XmlInclude(typeof(PhotoLumAxesConfig))]
    [DataContract]
    public class AxesConfig : IDeviceConfiguration
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        ///<summary>Use a dummy axes instead of the real device</summary>
        public bool IsSimulated { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public DeviceLogLevel LogLevel { get; set; }

        [Browsable(false)]
        [DataMember]
        public List<AxisConfig> AxisConfigs { get; set; }

        [DataMember]
        public Length Precision { get; set; }

        [DataMember]
        public int StabilizationTimems { get; set; }

    }
}
