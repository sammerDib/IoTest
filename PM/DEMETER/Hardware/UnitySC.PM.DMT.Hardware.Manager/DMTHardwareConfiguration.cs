using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Service.Interface.OpticalMount;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Hardware.Manager
{
    [Serializable]
    public class DMTHardwareConfiguration : HardwareConfiguration
    {
        [Serializable]
        public class OpticalMount
        {
            [XmlAttribute]
            public Side Side;

            public string CameraId;

            public string ScreenId;

            public OpticalMountShape MountShape;
        }

        [XmlElement("OpticalMount")]
        public List<OpticalMount> OpticalMounts { get; set; } = new List<OpticalMount>();

        [XmlArrayItem(typeof(DMTScreenConfig))]
        public List<DMTScreenConfig> DMTScreenConfigs { get; set; }

        public void InitializeObjectsFromIDs()
        {
        }

        public override void SetAllHardwareInSimulation()
        {
            var subDeviceConfigs = SubObjectFinder.GetAllSubObjectOfTypeT<IDeviceConfiguration>(this, 2);
            foreach (var deviceConfig in subDeviceConfigs)
            {
                deviceConfig.Value.IsSimulated = true;
            }
        }
    }
}
