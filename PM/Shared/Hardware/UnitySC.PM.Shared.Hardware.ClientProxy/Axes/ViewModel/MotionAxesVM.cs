using System;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel
{
    public enum AxesMoveTypes
    {
        XPlus,
        XMinus,
        TPlus,
        TMinus
    }

    public class MotionAxesVM : ViewModelBaseExt
    {
        private readonly ILogger _logger;
        private readonly IMotionAxesService _motionAxesSupervisor;
        private readonly IDialogOwnerService _dialogService;

        private AxesConfig _axesConfiguration;

        private string _deviceStatus;
        private double _linearPosition;
        private double _rotationPosition;
        private double _step;

        private AutoRelayCommand<string> _move;
        private AutoRelayCommand<string> _relativeMove;
        private AutoRelayCommand _gotoHome;
        private AutoRelayCommand _stop;
        private AutoRelayCommand _getAxesConfiguration;

        public double Step
        {
            get { return _step; }
            set
            {
                _step = value;
                OnPropertyChanged();
            }
        }

        public AxesConfig AxesConfiguration
        {
            get
            {
                if (_axesConfiguration == null)
                {
                    _axesConfiguration = _motionAxesSupervisor.GetAxesConfiguration()?.Result;
                }
                return _axesConfiguration;
            }

            set
            {
                if (_axesConfiguration == value)
                {
                    return;
                }
                _axesConfiguration = value;
                OnPropertyChanged();
            }
        }

        public AxisConfig ConfigurationAxisLinear => AxesConfiguration?.AxisConfigs?.FirstOrDefault<AxisConfig>(a => a.MovingDirection == MovingDirection.Linear);
        public AxisConfig ConfigurationAxisRotation => AxesConfiguration?.AxisConfigs?.FirstOrDefault<AxisConfig>(a => a.MovingDirection == MovingDirection.Rotation);

        public MotionAxesVM(IMotionAxesService motionAxeSupervisor, IDialogOwnerService dialogService, ILogger logger)
        {
            _logger = logger;
            _motionAxesSupervisor = motionAxeSupervisor;
            _dialogService = dialogService;

            Init();
        }

        public void Init()
        {
            try
            {
                if (_axesConfiguration == null)
                {
                    AxesConfiguration = _motionAxesSupervisor.GetAxesConfiguration()?.Result;
                    OnPropertyChanged(nameof(ConfigurationAxisLinear));
                    OnPropertyChanged(nameof(ConfigurationAxisRotation));
                }

                // Initialize position
                var resultPos = _motionAxesSupervisor.GetCurrentPosition()?.Result;
                if (resultPos != null && resultPos is PositionBase position)
                {
                    UpdatePosition(position);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"MotionAxesVM Init Failed");
            }
        }

        public void UpdatePosition(PositionBase position)
        {
            if (position is LinearPosition linearPos)
            {
                LinearPosition = linearPos.Position;
            }
            else if (position is RotationPosition rotationPos)
            {
                RotationPosition = rotationPos.Position;
            }
            else if (position is XTPosition pos)
            {
                LinearPosition = pos.X;
                RotationPosition = pos.T;
            }
        }

        public string DeviceStatus
        {
            get => _deviceStatus;
            set
            {
                if (_deviceStatus != value)
                {
                    _deviceStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public double LinearPosition
        {
            get { return _linearPosition; }
            set
            {
                if (_linearPosition != value)
                {
                    _linearPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        public double RotationPosition
        {
            get { return _rotationPosition; }
            set
            {
                if (_rotationPosition != value)
                {
                    _rotationPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        public AutoRelayCommand<string> Move
        {
            get
            {
                return _move ?? (_move = new AutoRelayCommand<string>(
                (axis) =>
                {
                    var pos = new Length(Step, LengthUnit.Millimeter);
                    var newPosition = new PMAxisMove(axis, pos);
                    MoveIncremental(newPosition);

                    OnPropertyChanged(nameof(LinearPosition));
                    OnPropertyChanged(nameof(RotationPosition));
                },
                (axis) => true));
            }
        }

        public void MoveIncremental(PMAxisMove position)
        {
            _motionAxesSupervisor.Move(position);
            _motionAxesSupervisor.WaitMotionEnd(1500);
        }

        public AutoRelayCommand<string> RelativeMove
        {
            get
            {
                return _relativeMove ?? (_relativeMove = new AutoRelayCommand<string>(
                (axis) =>
                {
                    var pos = new Length(Step, LengthUnit.Millimeter);
                    var newPosition = new PMAxisMove(axis, pos);

                    _motionAxesSupervisor.RelativeMove(newPosition);
                    OnPropertyChanged(nameof(LinearPosition));
                    OnPropertyChanged(nameof(RotationPosition));
                },
                (axis) => true));
            }
        }

        public AutoRelayCommand GotoHome
        {
            get
            {
                return _gotoHome ?? (_gotoHome = new AutoRelayCommand(
                () =>
                {
                    _motionAxesSupervisor.GoToHome(0);
                    OnPropertyChanged(nameof(LinearPosition));
                    OnPropertyChanged(nameof(RotationPosition));
                },
                () => true));
            }
        }

        public AutoRelayCommand Stop
        {
            get
            {
                return _stop ?? (_stop = new AutoRelayCommand(
                () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        response = _motionAxesSupervisor.StopAllMotion();
                        OnPropertyChanged(nameof(LinearPosition));
                        OnPropertyChanged(nameof(RotationPosition));
                    }
                    catch (Exception e)
                    {
                        _dialogService.ShowException(e, $"Stop {response}");
                    }
                },
                () => true));
            }
        }

        public AutoRelayCommand GetAxesConfiguration
        {
            get
            {
                return _getAxesConfiguration ?? (_getAxesConfiguration = new AutoRelayCommand(
              () =>
              {
                  AxesConfiguration = _motionAxesSupervisor.GetAxesConfiguration()?.Result;
              },
              () => true));
            }
        }
    }
}
