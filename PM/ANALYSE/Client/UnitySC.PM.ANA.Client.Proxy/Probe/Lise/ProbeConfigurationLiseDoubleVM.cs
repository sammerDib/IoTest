using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeConfigurationLiseDoubleVM : ObservableObject, IProbeDualLiseConfig
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

        public ProbeSingleConfigBase ProbeUp { get; set; }
        public ProbeSingleConfigBase ProbeDown { get; set; }
        public int ConfigLiseDoubleParam { get; set; }

        public string Name { get; set; }
        public string DeviceID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSimulated { get; set; }
        public DeviceLogLevel LogLevel { get; set; }
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
        public ModulePositions ModulePosition { get; set; }

        public string ModuleObjectiveSelectorID { get; set; }
    }
}
