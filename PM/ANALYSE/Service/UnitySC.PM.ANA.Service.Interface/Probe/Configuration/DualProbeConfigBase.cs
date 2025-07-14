using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class DualProbeConfigBase : ProbeConfigBase, IDualProbeConfig
    {
        [DataMember]
        public string ProbeUpID { get; set; }

        [DataMember]
        public string ProbeDownID { get; set; }

        [XmlIgnore]
        [DataMember]
        public ProbeSingleConfigBase ProbeUp { get; set; }

        [XmlIgnore]
        [DataMember]
        public ProbeSingleConfigBase ProbeDown { get; set; }

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
            // We look for the module that contains the lise up
            foreach (var probeModuleConfig in probeModulesConfigs)
            {
                var probeUp = probeModuleConfig.GetProbeFromID(ProbeUpID);
                if (probeUp != null) ProbeUp = probeUp as ProbeSingleConfigBase;

                var probeDown = probeModuleConfig.GetProbeFromID(ProbeDownID);
                if (probeDown != null) ProbeDown = probeDown as ProbeSingleConfigBase;
            }
        }
    }
}
