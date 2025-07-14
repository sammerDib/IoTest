using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.DMT.Client.Proxy.Chamber;
using UnitySC.PM.DMT.Client.Proxy.Chuck;
using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.Shared.Hardware.ClientProxy.Plc;
using UnitySC.PM.Shared.Hardware.ClientProxy.Ffu;
using UnitySC.PM.Shared.Hardware.ClientProxy.Ffu;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class OverviewChamberVM : ViewModelBaseExt, ITabManager
    {
        private readonly ChuckSupervisor _chuckSupervisor;
        private readonly ChamberSupervisor _chamberSupervisor;
        private readonly PlcSupervisor _plcSupervisor;
        private readonly FfuSupervisor _ffuSupervisor;

        public string Header { get; protected set; }

        public ChamberVM ChamberVM => _chamberSupervisor.ChamberVM;
        public ChuckVM ChuckVM => _chuckSupervisor.ChuckVM;
        public PlcVM PlcVM => _plcSupervisor.PlcVM;
        public FfuVM FfuVM => _ffuSupervisor.FfuVM;

        public OverviewChamberVM(ChamberSupervisor chamberSupervisor, ChuckSupervisor chuckSupervisor, PlcSupervisor plcSupervisor, FfuSupervisor ffuSupervisor)
        {
            Header = "Chamber";
            
            _chamberSupervisor = chamberSupervisor;
            _chuckSupervisor = chuckSupervisor;
            _plcSupervisor = plcSupervisor; 
            _ffuSupervisor = ffuSupervisor;
            Refresh();

            Refresh();
        }

        public void Refresh()
        {
            Task.Run(() => _chamberSupervisor.TriggerUpdateEvent());
            Task.Run(() => _chuckSupervisor.RefreshAllValues());
            Task.Run(() => _chuckSupervisor.GetTag());
            Task.Run(() => _plcSupervisor.TriggerUpdateEvent());
            Task.Run(() => _ffuSupervisor.TriggerUpdateEvent());
        }

        void ITabManager.Display()
        {
            Refresh();
        }

        bool ITabManager.CanHide()
        {
            return true;
        }

        void ITabManager.Hide()
        {
        }

        private bool _fsTurnOnSelected;

        public bool FSTurnOnSelected
        {
            get => _fsTurnOnSelected;
            set
            {
                if (value != _fsTurnOnSelected)
                {
                    _fsTurnOnSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand _fsTurnOnCommand;

        public RelayCommand FSTurnOnCommand
        {
            get => _fsTurnOnCommand ?? (_fsTurnOnCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FSTurnOnSelected = true;
                });
            }));
        }

        private RelayCommand _fsTurnOffCommand;

        public RelayCommand FSTurnOffCommand
        {
            get => _fsTurnOffCommand ?? (_fsTurnOffCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FSTurnOnSelected = false;
                });
            }));
        }

        private bool _bsTurnOnSelected;

        public bool BSTurnOnSelected
        {
            get => _bsTurnOnSelected;
            set
            {
                if (value != _bsTurnOnSelected)
                {
                    _bsTurnOnSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand _bsTurnOnCommand;

        public RelayCommand BSTurnOnCommand
        {
            get => _bsTurnOnCommand ?? (_bsTurnOnCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    BSTurnOnSelected = true;
                });
            }));
        }

        private RelayCommand _bsTurnOffCommand;

        public RelayCommand BSTurnOffCommand
        {
            get => _bsTurnOffCommand ?? (_bsTurnOffCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    BSTurnOnSelected = false;
                });
            }));
        }

        /*private bool _fanTurnOnSelected;

        public bool FanTurnOnSelected
        {
            get => _fanTurnOnSelected;
            set
            {
                if (value != _fanTurnOnSelected)
                {
                    _fanTurnOnSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand _fanTurnOnCommand;

        public RelayCommand FanTurnOnCommand
        {
            get => _fanTurnOnCommand ?? (_fanTurnOnCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FanTurnOnSelected = true;
                });
            }));
        }

        private RelayCommand _fanTurndOffCommand;

        public RelayCommand FanTurnOffCommand
        {
            get => _fanTurndOffCommand ?? (_fanTurndOffCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FanTurnOnSelected = false;
                });
            }));
        }*/
    }
}
