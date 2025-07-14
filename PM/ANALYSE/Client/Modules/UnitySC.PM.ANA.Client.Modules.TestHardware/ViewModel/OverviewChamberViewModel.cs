using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.Shared.Hardware.ClientProxy.Ffu;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.PM.Shared.Hardware.ClientProxy.Plc;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class OverviewChamberViewModel : TabViewModelBase
    {
        private FfuSupervisor _ffuSupervisor;
        private PlcSupervisor _plcSupervisor;

        public FfuVM FfuVM => FfuSupervisor.FfuVM;

        public PlcVM PlcVM => PlcSupervisor.PlcVM;

        public string Header { get; protected set; }

        public OverviewChamberViewModel()
        {
            Header = "Overview";

            Refresh();
        }

        public void Refresh()
        {
            Task.Run(() => FfuSupervisor.TriggerUpdateEvent());
            Task.Run(() => PlcSupervisor.TriggerUpdateEvent());
        }

        public FfuSupervisor FfuSupervisor
        {
            get
            {
                if (_ffuSupervisor == null)
                {
                    _ffuSupervisor = ClassLocator.Default.GetInstance<FfuSupervisor>();
                }

                return _ffuSupervisor;
            }
        }

        public PlcSupervisor PlcSupervisor
        {
            get
            {
                if (_plcSupervisor == null)
                {
                    _plcSupervisor = ClassLocator.Default.GetInstance<PlcSupervisor>();
                }

                return _plcSupervisor;
            }
        }
    }
}
