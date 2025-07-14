using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [Serializable]
    [DataContract]
    public abstract class ProbeConfigBase
    {
        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public bool IsSimulated { get; set; } //Use a dummy probe instead of the real device

        [DataMember]
        public DeviceLogLevel LogLevel { get; set; }

        [DataMember]
        public Length ThicknessThresholdInTheAir { get; set; }

        public abstract void InitializeObjectsFromIDs(List<ProbeModuleConfig> probeModulesConfigs, List<CameraConfigBase> camerasConfigs, List<LightModuleConfig> lightModuleConfigs, List<ObjectivesSelectorConfigBase> objectivesSelectorsConfigs);
    }
}
