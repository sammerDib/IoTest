using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeConfigurationLiseVM : ObservableObject, ISingleProbeConfig, IProbeLiseConfig
    {
        
        private int _LiseProperty;

        public int LiseProperty
        {
            get { return _LiseProperty; }
            set
            {
                _LiseProperty = value;
                OnPropertyChanged();
            }
        }

        public int ConfigLise1 { get; set; }
        public int ConfigLise2 { get; set; }
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSimulated { get; set; }
        public DeviceLogLevel LogLevel { get; set; }
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
        public ModulePositions ModulePosition { get; set; }

        public List<LightConfig> Lights { get; set; }
        public List<CameraConfigBase> Cameras { get; set; }

        public float MinimumGain { get; set; }
        public float MaximumGain { get; set; }
        public ObjectivesSelectorConfigBase ObjectivesSelector { get; set; }
    }
}
