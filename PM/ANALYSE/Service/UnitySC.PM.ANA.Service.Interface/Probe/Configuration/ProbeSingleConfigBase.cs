using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeSingleConfigBase : ProbeConfigBase, ISingleProbeConfig
    {
        [XmlIgnore]
        [DataMember]
        public List<LightConfig> Lights { get; set; }

        [XmlIgnore]
        [DataMember]
        public List<CameraConfigBase> Cameras { get; set; }

        [XmlIgnore]
        [DataMember]
        public string ModuleID { get; set; }

        [XmlIgnore]
        [DataMember]
        public string ModuleName { get; set; }

        [XmlIgnore]
        [DataMember]
        public ModulePositions ModulePosition { get; set; }

        public override void InitializeObjectsFromIDs(List<ProbeModuleConfig> probeModulesConfigs, List<CameraConfigBase> camerasConfigs, List<LightModuleConfig> lightModuleConfigs, List<ObjectivesSelectorConfigBase> objectivesSelectorsConfigs)
        {
            if (string.IsNullOrEmpty(ModuleID))
                throw (new Exception("ModuleID must be set before calling InitializeObjectsFromIDs"));

            var probeModuleConfig = probeModulesConfigs.First(p => p.DeviceID == ModuleID);

            Lights = new List<LightConfig>();
            foreach (var lightID in probeModuleConfig.LightsID)
            {
                foreach (var x in lightModuleConfigs)
                {
                    var lights = x.Lights.Where(c => c.DeviceID == lightID);
                    if (lights.Any())
                    {
                        Lights.AddRange(lights);
                        break;
                    }
                }
            }

            Cameras = new List<CameraConfigBase>();
            foreach (var cameraID in probeModuleConfig.CamerasID)
            {
                var cameras = camerasConfigs.Where(c => c.DeviceID == cameraID);
                if (cameras.Any())
                {
                    foreach (var camera in cameras)
                        camera.ObjectivesSelectorID = probeModuleConfig.ObjectivesSelectorID;
                    Cameras.AddRange(cameras);
                }
            }
        }
    }
}
