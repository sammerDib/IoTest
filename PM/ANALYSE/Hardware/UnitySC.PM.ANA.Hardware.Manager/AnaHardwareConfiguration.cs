using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware
{
    [Serializable]
    public class AnaHardwareConfiguration : HardwareConfiguration
    {
        public List<ProbeModuleConfig> ProbeModulesConfigs { get; set; }

        [XmlArrayItem(typeof(LineMotObjectivesSelectorConfig))]
        [XmlArrayItem(typeof(SingleObjectiveSelectorConfig))]
        public List<ObjectivesSelectorConfigBase> ObjectivesSelectorConfigs { get; set; }        

        public void InitializeObjectsFromIDs()
        {
            foreach (var probeModuleConfig in ProbeModulesConfigs)
            {
                foreach (var probe in probeModuleConfig.Probes)
                {
                    // We set the module information to the probe
                    (probe as IModuleInformation).ModuleID = probeModuleConfig.DeviceID;
                    (probe as IModuleInformation).ModuleName = probeModuleConfig.Name;
                    (probe as IModuleInformation).ModulePosition = probeModuleConfig.ProbeModuleSettings.Position;
                    probe.InitializeObjectsFromIDs(ProbeModulesConfigs, CameraConfigs, LightModuleConfigs, ObjectivesSelectorConfigs);
                }

                foreach (var cameraID in probeModuleConfig.CamerasID)
                {
                    foreach (var camera in CameraConfigs.Where(c => c.DeviceID == cameraID))
                    {
                        // We set the module information to the camera
                        camera.ModuleID = probeModuleConfig.DeviceID;
                        camera.ModuleName = probeModuleConfig.Name;
                        camera.ModulePosition = probeModuleConfig.ProbeModuleSettings.Position;
                    }
                }

                foreach (var lightID in probeModuleConfig.LightsID)
                {
                    foreach (var lightModuleConfigs in LightModuleConfigs)
                    {
                        foreach (var light in lightModuleConfigs.Lights.Where(x => x.DeviceID == lightID))
                        {
                            // We set the module position in the light position
                            light.Position = probeModuleConfig.ProbeModuleSettings.Position;
                        }
                    }
                }
            }
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
