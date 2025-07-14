using System;
using System.Collections.Generic;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using UnitySC.PM.AGS.Hardware.Manager;
using UnitySC.PM.AGS.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.AGS.Modules.TestHardware.ViewModel
{
    public class StageViewModel : SettingVM
    {
        #region Fields

        private readonly IDialogOwnerService _dialogService;
        private ArgosHardwareManager _hardwareManager;
        private string _aerotechController = "AerotechController";
        private AxesConfig _axesConfiguration;
        private ChuckConfig _chuckConfiguration;
        private IChuckService _chuckService;
        private AxisSpeed _selectedAxisSpeed = AxisSpeed.Normal;

        private AxisSpeed _selectedXAxisSpeed = AxisSpeed.Normal;
        private AxisSpeed _selectedTAxisSpeed = AxisSpeed.Normal;
        private ArgosAxesBase _axes;

        #endregion Fields

        #region Properties

        private double _xPosition;

        public double XPosition
        {
            get
            {
                return _xPosition;
            }
            set
            {
                _xPosition = value;
                RaisePropertyChanged();
            }
        }

        private double _tPosition;

        public double TPosition
        {
            get
            {
                return _tPosition;
            }
            set
            {
                _tPosition = value;
                RaisePropertyChanged();
            }
        }

        private bool _isConnected;

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                RaisePropertyChanged();
            }
        }

        public AxisSpeed SelectedXAxisSpeed
        {
            get { return _selectedXAxisSpeed; }
            set
            {
                _selectedXAxisSpeed = value;
                RaisePropertyChanged();
            }
        }

        private bool _isWaferClamped;

        public bool IsWaferClamped
        {
            get
            {
                return _isWaferClamped;
            }
            set
            {
                if (_isWaferClamped == value)
                    return;
                _isWaferClamped = value;
                RaisePropertyChanged();
            }
        }

        private bool _isWaferLloaded;

        public bool IsWaferLloaded
        {
            get
            {
                return _isWaferLloaded;
            }
            set
            {
                if (_isWaferLloaded == value)
                    return;
                _isWaferLloaded = value;
                RaisePropertyChanged();
            }
        }

        private bool _isVacuumOn;

        public bool IsVacuumOn
        {
            get
            {
                return _isVacuumOn;
            }
            set
            {
                if (_isVacuumOn == value)
                    return;
                _isVacuumOn = value;
                RaisePropertyChanged();
            }
        }

        private bool _isLiftPinUp;

        public bool IsLiftPinUp
        {
            get
            {
                return _isLiftPinUp;
            }
            set
            {
                if (_isLiftPinUp == value)
                    return;
                _isLiftPinUp = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<AxisSpeed> SpeedXAxisValues
        {
            get
            {
                return Enum.GetValues(typeof(AxisSpeed))
                    .Cast<AxisSpeed>();
            }
        }

        public AxisSpeed SelectedTAxisSpeed
        {
            get { return _selectedTAxisSpeed; }
            set
            {
                _selectedTAxisSpeed = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<AxisSpeed> SpeedTAxisValues
        {
            get
            {
                return Enum.GetValues(typeof(AxisSpeed))
                    .Cast<AxisSpeed>();
            }
        }

        #endregion Properties

        #region Command

        private RelayCommand _xmove;

        public RelayCommand XMove
        {
            get
            {
                return _xmove ?? (_xmove = new RelayCommand(
              () =>
              {
                  XMoveIncremental();
              },
              () => { return true; }));
            }
        }

        private RelayCommand _tmove;

        public RelayCommand TMove
        {
            get
            {
                return _tmove ?? (_tmove = new RelayCommand(
              () =>
              {
                  TMoveIncremental();
              },
              () => { return true; }));
            }
        }

        private RelayCommand _changeClampStatus;

        public RelayCommand ChangeClampStatusCmd
        {
            get
            {
                return _changeClampStatus ?? (_changeClampStatus = new RelayCommand(
              () =>
              {
                  ChangeClampStatus();
              },
              () => { return true; }));
            }
        }

        private RelayCommand _changeLoadStatus;

        public RelayCommand ChangeLoadStatusCmd
        {
            get
            {
                return _changeLoadStatus ?? (_changeLoadStatus = new RelayCommand(
              () =>
              {
                  ChangeLoadStatus();
              },
              () => { return true; }));
            }
        }

        private RelayCommand _changeVacuumStatus;

        public RelayCommand ChangeVacuumStatusCmd
        {
            get
            {
                return _changeVacuumStatus ?? (_changeVacuumStatus = new RelayCommand(
              () =>
              {
                  ChangeVacuumStatus();
              },
              () => { return true; }));
            }
        }

        private RelayCommand _changeLiftPinStatus;

        public RelayCommand ChangeLiftPinStatusCmd
        {
            get
            {
                return _changeLiftPinStatus ?? (_changeLiftPinStatus = new RelayCommand(
              () =>
              {
                  ChangeLiftPinStatus();
              },
              () => { return true; }));
            }
        }

        private RelayCommand _gotoHomePosition;

        public RelayCommand GotoHomePositionCmd
        {
            get
            {
                return _gotoHomePosition ?? (_gotoHomePosition = new RelayCommand(
              () =>
              {
                  GotoHomePosition();
              },
              () => { return true; }));
            }
        }

        #endregion Command

        #region Constructor

        public StageViewModel()
        {
            Header = "Stage";
            IsEnabled = true;
            IsWaferClamped = false;
            IsWaferLloaded = false;
            IsLiftPinUp = false;
            IsVacuumOn = false;

            _hardwareManager = ClassLocator.Default.GetInstance<ArgosHardwareManager>();
            _axes = _hardwareManager.Axes as ArgosAxesBase;
            if (!IsInDesignModeStatic)
            {
                _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            }

            var AerotechControllerId = "";

            foreach (var item in _hardwareManager.MotionControllers)
            {
                if (item.Value.Name == "Aerotech Controller")
                {
                    AerotechControllerId = item.Value.DeviceID;
                }
            }

            IsConnected = true;
        }

        #endregion Constructor

        #region Method

        public void XMoveIncremental()
        {
            var AerotechControllerId = "4VIS2207_DEMO";
            foreach (var item in _hardwareManager.MotionControllers)
            {
                if (item.Value.Name == "Aerotech Controller")
                {
                    AerotechControllerId = item.Value.DeviceID;
                }
            }

            var motionController = _hardwareManager.MotionControllers[AerotechControllerId] as IMotion;
            var axisXConf = _axes.GetAxisConfigById("X");
            var speedX = ArgosAxesBase.ConvertAxisSpeed(SelectedXAxisSpeed, axisXConf);
            var pos = new Length(XPosition, LengthUnit.Millimeter);
            motionController.RelativeMove(new PMAxisMove("X", pos, speedX));
        }

        public void TMoveIncremental()
        {
            var AerotechControllerId = "4VIS2207_DEMO";

            foreach (var item in _hardwareManager.MotionControllers)
            {
                if (item.Value.Name == "Aerotech Controller")
                {
                    AerotechControllerId = item.Value.DeviceID;
                }
            }

            //recuperer axes
            var motionController = _hardwareManager.MotionControllers[AerotechControllerId] as IMotion;
            var axisTConf = _axes.GetAxisConfigById("T");
            var speedT = ArgosAxesBase.ConvertAxisSpeed(SelectedTAxisSpeed, axisTConf);
            var pos = new Length(TPosition, LengthUnit.Millimeter);
            motionController.RelativeMove(new PMAxisMove("T", pos, speedT));
        }

        public void ChangeClampStatus()
        {
            //TODO Use WaferDimensionalCharacteristic stored in recipe.step.product and implement clamp in WagoChuck. 
            WaferDimensionalCharacteristic wafer = new WaferDimensionalCharacteristic();
            if (!IsWaferClamped)
            {
                _hardwareManager.Chuck.ClampWafer(wafer);
            }
            else
                _hardwareManager.Chuck.ReleaseWafer(wafer);
        }

        public void ChangeLoadStatus()
        {
        }

        public void ChangeVacuumStatus()
        {
        }

        public void ChangeLiftPinStatus()
        {
        }

        public void GotoHomePosition()
        {
            var AerotechControllerId = "";

            foreach (var item in _hardwareManager.MotionControllers)
            {
                if (item.Value.Name == "Aerotech Controller")
                {
                    AerotechControllerId = item.Value.DeviceID;
                }
            }
        }

        #endregion Method
    }
}
