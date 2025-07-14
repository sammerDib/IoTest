using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.LIGHTSPEED.Client.CommonUI.ViewModel.Maintenance
{
    public class DoorSlitVM : ViewModelBase
    {
        private IAcquisitionService _acquistionSupervisor;
        public List<IoVm> Inputs { private get; set; }
        public List<IoVm> Outputs { private get; set; }

        public DoorSlitVM(IAcquisitionService acquisitionSupervisor)
        {
            _acquistionSupervisor = acquisitionSupervisor;
            Init();
        }

        public void Init()
        {
            Inputs = new List<IoVm>() { new IoVm { IsEnabled = true, Name = "DoorClose" } };
            Outputs = new List<IoVm>() { new IoVm { IsEnabled = false, Name = "OpenDoor" } };
            Log = "Yo zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        }

        private string _log;

        public string Log
        {
            get => _log; set { if (_log != value) { _log = value; RaisePropertyChanged(); } }
        }

        private bool _isStarted;

        public bool IsStarted
        {
            get => _isStarted; set { if (_isStarted != value) { _isStarted = value; RaisePropertyChanged(); } }
        }

        private int _nbCycles;

        public int NbCycles
        {
            get => _nbCycles; set { if (_nbCycles != value) { _nbCycles = value; RaisePropertyChanged(); } }
        }

        private RelayCommand _openDoor;

        public RelayCommand OpenDoor
        {
            get
            {
                return _openDoor ?? (_openDoor = new RelayCommand(
              () =>
              {
                  _acquistionSupervisor.Open();
                  ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Open");
              },
              () => { return true; }));
            }
        }

        private RelayCommand _closeDoor;

        public RelayCommand CloseDoor
        {
            get
            {
                return _closeDoor ?? (_closeDoor = new RelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Close");
              },
              () => { return true; }));
            }
        }

        private RelayCommand _startCycling;

        public RelayCommand StartCycling
        {
            get
            {
                return _startCycling ?? (_startCycling = new RelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Open");
                  IsStarted = true;
              },
              () => { return true; }));
            }
        }

        private RelayCommand _stopCycling;

        public RelayCommand StopCycling
        {
            get
            {
                return _stopCycling ?? (_stopCycling = new RelayCommand(
              () =>
              {
                  IsStarted = false;
              },
              () => { return true; }));
            }
        }
    }
}
