using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class ProbeLiseViewModelBase : ViewModelBaseExt
    {
        private ProbeLiseBaseVM _probe = null;

        public ProbeLiseBaseVM Probe
        {
            get => _probe;
            set { if (_probe != value) { _probe = value; OnPropertyChanged(); } }
        }
    }
}
