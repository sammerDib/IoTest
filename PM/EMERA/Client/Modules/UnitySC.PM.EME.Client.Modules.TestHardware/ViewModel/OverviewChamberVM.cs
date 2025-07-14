using System.Threading.Tasks;

using UnitySC.PM.EME.Client.Proxy.Chamber;
using UnitySC.PM.EME.Client.Proxy.Chiller;
using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.PM.Shared.Hardware.ClientProxy.DistanceSensor;
using UnitySC.PM.Shared.Hardware.ClientProxy.Plc;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.EME.Client.Modules.TestHardware.ViewModel
{
    public class OverviewChamberVM : ViewModelBaseExt
    {
        private ChamberVM _chamberVM;
        private ChuckVM _chuckVM;        
        private PlcSupervisor _plcSupervisor;
        private DistanceSensorSupervisor _distanceSensorSupervisor;
        private ChillerViewModel _chillerViewModel;


        public DistanceSensorVM DistanceSensorVM => _distanceSensorSupervisor.DistanceSensorVM;

        public OverviewChamberVM()
        {
            Init();
        }

        private void Init()
        {
            Task.Run(() => PlcSupervisor.TriggerUpdateEvent());
            Task.Run(() => DistanceSensorSupervisor.TriggerUpdateEvent());
        }
        

        public ChamberVM ChamberVM
        {
            get
            {
                if (_chamberVM == null)
                {
                    _chamberVM = ClassLocator.Default.GetInstance<ChamberVM>();
                }

                return _chamberVM;
            }
        }

        public ChuckVM ChuckVM
        {
            get
            {
                if (_chuckVM == null)
                {
                    _chuckVM = ClassLocator.Default.GetInstance<ChuckVM>();
                }

                return _chuckVM;
            }
        }

        private PlcSupervisor PlcSupervisor
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
        
        private DistanceSensorSupervisor DistanceSensorSupervisor
        {
            get
            {
                if (_distanceSensorSupervisor == null)
                {
                    _distanceSensorSupervisor = ClassLocator.Default.GetInstance<DistanceSensorSupervisor>();
                }

                return _distanceSensorSupervisor;
            }
        }
        
        public ChillerViewModel ChillerViewModel
            => _chillerViewModel ?? (_chillerViewModel = ClassLocator.Default.GetInstance<ChillerViewModel>());
    }
}
