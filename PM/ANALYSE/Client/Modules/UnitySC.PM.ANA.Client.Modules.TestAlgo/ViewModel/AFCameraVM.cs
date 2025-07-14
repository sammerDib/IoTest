using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel
{
    public class AFCameraVM : AlgoBaseVM
    {
        public IEnumerable<ScanRangeType> ScanRanges { get; private set; }

        public AFCameraVM() : base("AF Image")
        {
            ScanRanges = Enum.GetValues(typeof(ScanRangeType)).Cast<ScanRangeType>().Where(e => !e.Equals(ScanRangeType.Configured));
            SelectedRange = ScanRangeType.Small;
            UseCurrentZPosition = true;
            ClassLocator.Default.GetInstance<AlgosSupervisor>().AFCameraChangedEvent += AFCameraVM_AFCameraChangedEvent;
            CanDoAutofocus = false;
        }

        private AFCameraResult _result;

        public AFCameraResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }

        private bool _useCurrentZPosition;

        public bool UseCurrentZPosition
        {
            get => _useCurrentZPosition; set { if (_useCurrentZPosition != value) { _useCurrentZPosition = value; OnPropertyChanged(); } }
        }

        private ScanRangeType _selectedRange;

        public ScanRangeType SelectedRange
        {
            get => _selectedRange; set { if (_selectedRange != value) { _selectedRange = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _startAF;

        public AutoRelayCommand StartAF
        {
            get
            {
                return _startAF ?? (_startAF = new AutoRelayCommand(
              () =>
              {
                  if (!ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.IsGrabbing)
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Camera must be started", "Camera", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                      return;
                  }
                  Result = null;
                  IsBusy = true;

                  var aoi = new Rect(0, 448, 1280, 128); // TODO FRANCIS/FOUCAULD: get dimensions and positions from user interface
                  var afCameraInput = new AFCameraInput(ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.Configuration.DeviceID, SelectedRange)
                  {
                      UseCurrentZPosition = UseCurrentZPosition,
                      Aoi = aoi,
                      // TODO : Set Initial context if needed
                  };
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().StartAFCamera(afCameraInput);
              },
              () => { return true; }));
            }
        }

        private void AFCameraVM_AFCameraChangedEvent(AFCameraResult afCameraResult)
        {
            UpdateResult(afCameraResult);
        }

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelAFCamera();
              },
              () => { return true; }));
            }
        }

        public void UpdateResult(AFCameraResult afResult)
        {
            Result = afResult;
            if (afResult.Status.IsFinished)
                IsBusy = false;
        }

        public override void Dispose()
        {
        }
    }
}
