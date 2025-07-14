using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes.Models;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.ViewModel;

using Message = UnitySC.Shared.Tools.Service.Message;

namespace UnitySC.PM.EME.Client.Proxy.Axes
{
    public enum AxesMoveTypes
    {
        XPlus,
        XMinus,
        YPlus,
        YMinus,
        ZPlus,
        ZMinus,
    }
    public class AxesVM : ViewModelBaseExt
    {
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly IEmeraMotionAxesService _motionAxesSupervisor;
        private readonly GlobalStatusSupervisor _globalStatusSupervisor;
        private readonly ChuckVM _chuckVM;
        private readonly ReferentialSupervisor _referentialSupervisor;

        private AxesConfig _axesConfiguration;
        private Dictionary<AxesMoveTypes, bool> _canMoveDictionary = new Dictionary<AxesMoveTypes, bool>()
            {
                { AxesMoveTypes.XMinus, true },
                { AxesMoveTypes.XPlus, true },
                { AxesMoveTypes.YMinus, true },
                { AxesMoveTypes.YPlus, true },
                { AxesMoveTypes.ZMinus, true },
                { AxesMoveTypes.ZPlus, true },
            };

        private bool _isLocked;

        private AxisConfig _xAxisConfig;
        private AxisConfig _yAxisConfig;
        private AxisConfig _zAxisConfig;
        private string _axisIDx;
        private string _axisIDy;
        private string _axisIDz;
        private AsyncRelayCommand<double> _moveX;
        private AsyncRelayCommand<double> _moveY;
        private AsyncRelayCommand<double> _moveZ;

        private const int MotionEndTimeout = 30000;
        private readonly AxisSpeed _selectedAxisSpeed = AxisSpeed.Normal;

        private AsyncRelayCommand<Increment> _incrementalMove;
        private AsyncRelayCommand _gotoHome;
        private AsyncRelayCommand _stop;
        private AsyncRelayCommand _gotoEfemLoad;
        private AsyncRelayCommand _gotoManualLoad;

        public AxisStatus Status { get; private set; }

        public AxesVM(IEmeraMotionAxesService motionAxeSupervisor, GlobalStatusSupervisor globalStatusSupervisor, ReferentialSupervisor referentialSupervisor, ChuckVM chuckVM, ILogger logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _motionAxesSupervisor = motionAxeSupervisor;
            Status = new AxisStatus();
            Position = new Position(new WaferReferential(), 0, 0, 0);
            _globalStatusSupervisor = globalStatusSupervisor;
            _chuckVM = chuckVM;
            _referentialSupervisor = referentialSupervisor;
            Init();
        }

        public void Init()
        {
            try
            {
                var resultState = _motionAxesSupervisor.GetCurrentState().Result;
                if (resultState != null)
                    UpdateStatus(resultState);

                // Initialize Axes
                _xAxisConfig = AxesConfiguration.AxisConfigs.Find(a => a.MovingDirection == MovingDirection.X);
                _yAxisConfig = AxesConfiguration.AxisConfigs.Find(a => a.MovingDirection == MovingDirection.Y);
                _zAxisConfig = AxesConfiguration.AxisConfigs.Find(a => a.MovingDirection == MovingDirection.Z);

                AxisIDx = _xAxisConfig?.AxisID;
                AxisIDy = _yAxisConfig?.AxisID;
                AxisIDz = _zAxisConfig?.AxisID;

                // Initialize position                
                var resultPos = _motionAxesSupervisor.GetCurrentPosition()?.Result;
                var waferPosition = _referentialSupervisor.ConvertTo(resultPos, ReferentialTag.Wafer)?.Result.ToXYZPosition();
                UpdatePosition(waferPosition);

                // Register to update message
                _messenger.Register<PositionBase>(this, (_, p) => UpdatePosition(p));
                _messenger.Register<AxesState>(this, (_, state) =>
                {
                    UpdateStatus(state);
                    UpdateAllCanExecutes();
                });
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "MotionAxesVM Init Failed");
            }
        }

        public AxesConfig AxesConfiguration
        {
            get
            {
                if (_axesConfiguration == null)
                    _axesConfiguration = _motionAxesSupervisor.GetAxesConfiguration().Result;
                return _axesConfiguration;
            }
            set => SetProperty(ref _axesConfiguration, value);
        }

        // Position in WaferReferential
        public Position Position { get; set; }

        public string AxisIDx
        {
            get => _axisIDx;
            set => SetProperty(ref _axisIDx, value);
        }

        public string AxisIDy
        {
            get => _axisIDy;
            set => SetProperty(ref _axisIDy, value);
        }

        public bool IsLocked
        {
            get => _isLocked; set { if (_isLocked != value) { _isLocked = value; UpdateAllCanExecutes(); OnPropertyChanged(); } }
        }

        public bool IsReadyToStartMove => !Status.IsMoving;

        public string AxisIDz
        {
            get => _axisIDz;
            set => SetProperty(ref _axisIDz, value);
        }

        public Dictionary<AxesMoveTypes, bool> CanMoveDictionary
        {
            get => _canMoveDictionary;
            set => SetProperty(ref _canMoveDictionary, value);
        }
        public AxisConfig XAxisConfig
        {
            get => _xAxisConfig;
        }
        public AxisConfig YAxisConfig
        {
            get => _yAxisConfig;
        }
        public AxisConfig ZAxisConfig
        {
            get => _zAxisConfig;
        }

        public void UpdatePosition(PositionBase position)
        {
            // Conversion to Wafer referencial

            var positionOnWafer = position;

            Position.UpdatePosition(positionOnWafer);
            OnPropertyChanged(nameof(Position));
            Length epsilon = 0.001.Millimeters();
            Length defaultPosition = 0.0.Millimeters();

            // Check the limits
            CanMoveDictionary[AxesMoveTypes.XPlus] = Position.X < (_xAxisConfig?.PositionMax - epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.XMinus] = Position.X > (_xAxisConfig?.PositionMin + epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.YPlus] = Position.Y < (_yAxisConfig?.PositionMax - epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.YMinus] = Position.Y > (_yAxisConfig?.PositionMin + epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.ZPlus] = Position.Z < (_zAxisConfig?.PositionMax ?? defaultPosition - epsilon)?.Millimeters;
            CanMoveDictionary[AxesMoveTypes.ZMinus] = Position.Z > (_zAxisConfig?.PositionMin ?? defaultPosition + epsilon).Millimeters;

            OnPropertyChanged(nameof(CanMoveDictionary));
        }
        

        public AsyncRelayCommand<Increment> IncrementalMove =>
            _incrementalMove ?? (_incrementalMove = new AsyncRelayCommand<Increment>(
                async increment =>
                {
                    var targetPosition = GetNextPosition(increment.Axis, increment.Step);
                    await Task.Run(() => _motionAxesSupervisor.GoToPosition(targetPosition, _selectedAxisSpeed));
                    _motionAxesSupervisor.WaitMotionEnd(MotionEndTimeout);
                },
                _ => !Status.IsMoving)
            );

        private XYZPosition GetNextPosition(string axis, double step)
        {
            var currentPos = _motionAxesSupervisor.GetCurrentPosition()?.Result;
            var targetPosition = _referentialSupervisor.ConvertTo(currentPos, ReferentialTag.Motor)
                ?.Result.ToXYZPosition();


            var axisConfig = GetAxisConfig(axis);
            if (axisConfig == null)
            {
                _logger.Warning($"Unrecognized axis move: {axis}.");
                return targetPosition;
            }
            var newTargetPosition = ApplyStepWithBoundsCheck(targetPosition, axis, step, axisConfig);
            return newTargetPosition;
        }
        private AxisConfig GetAxisConfig(string axis)
        {
            switch (axis)
            {
                case "X":
                    return XAxisConfig;
                case "Y":
                    return YAxisConfig;
                case "Z":
                    return ZAxisConfig;
                default:
                    return null;
            }
        }
        private double GetPositionForAxis(XYZPosition position, string axis)
        {
            switch (axis)
            {
                case "X":
                    return position.X;
                case "Y":
                    return position.Y;
                case "Z":
                    return position.Z;
                default:
                    throw new ArgumentException("Invalid axis", nameof(axis));
            }
        }
        private void SetPositionForAxis(XYZPosition position, string axis, double newValue)
        {
            switch (axis)
            {
                case "X":
                    position.X = newValue;
                    break;
                case "Y":
                    position.Y = newValue;
                    break;
                case "Z":
                    position.Z = newValue;
                    break;
                default:
                    throw new ArgumentException("Invalid axis", nameof(axis));
            }
        }
        private XYZPosition ApplyStepWithBoundsCheck(XYZPosition position, string axis, double step, AxisConfig config)
        {
            double newPosition = GetPositionForAxis(position, axis) + step;

            if (newPosition < config.PositionMin.Millimeters || newPosition > config.PositionMax.Millimeters)
            {
                _logger.Warning($"{axis}-axis move out of bounds: {newPosition} mm. Limits are [{config.PositionMin.Millimeters}, {config.PositionMax.Millimeters}] mm.");
                return position;
            }

            SetPositionForAxis(position, axis, newPosition);
            return position;
        }

        public AsyncRelayCommand<double> MoveX
            => _moveX ?? (_moveX = new AsyncRelayCommand<double>(DoMoveX, _ => IsReadyToStartMove));
        public AsyncRelayCommand<double> MoveY
            => _moveY ?? (_moveY = new AsyncRelayCommand<double>(DoMoveY, _ => IsReadyToStartMove));
        public AsyncRelayCommand<double> MoveZ
            => _moveZ ?? (_moveZ = new AsyncRelayCommand<double>(DoMoveZ, _ => IsReadyToStartMove));

        public async Task DoMoveX(double newPositionX)
        {
            try
            {
                if (_xAxisConfig == null)
                    throw new InvalidOperationException("No configuration found for X axis.");

                newPositionX = Math.Max(_xAxisConfig.PositionMin?.Millimeters ?? newPositionX, newPositionX);
                newPositionX = Math.Min(_xAxisConfig.PositionMax?.Millimeters ?? newPositionX, newPositionX);

                var targetPosition = Position.ToXyzPosition();
                targetPosition.X = newPositionX;
                targetPosition.Y = double.NaN;
                targetPosition.Z = double.NaN;

                await Task.Run(() =>
                {
                    try
                    {
                        _motionAxesSupervisor.GoToPosition(targetPosition, _selectedAxisSpeed);
                        _motionAxesSupervisor.WaitMotionEnd(MotionEndTimeout);
                    }
                    catch (Exception e)
                    {
                        _logger?.Error(e, "AxesVM MoveX Failed");
                    }

                    Application.Current?.Dispatcher.Invoke(() => OnPropertyChanged(nameof(Position)));
                });
            }
            catch (Exception e)
            {
                _messenger.Send(new Message(MessageLevel.Error, e.Message));
            }
        }

        public async Task DoMoveY(double newPositionY)
        {
            try
            {
                if (_yAxisConfig == null)
                    throw new InvalidOperationException("No configuration found for Y axis.");

                newPositionY = Math.Max(_yAxisConfig.PositionMin?.Millimeters ?? newPositionY, newPositionY);
                newPositionY = Math.Min(_yAxisConfig.PositionMax?.Millimeters ?? newPositionY, newPositionY);

                var targetPosition = Position.ToXyzPosition();
                targetPosition.Y = newPositionY;
                targetPosition.X = double.NaN;
                targetPosition.Z = double.NaN;

                await Task.Run(() =>
                {
                    try
                    {
                        _motionAxesSupervisor.GoToPosition(targetPosition, _selectedAxisSpeed);
                        _motionAxesSupervisor.WaitMotionEnd(MotionEndTimeout);
                    }
                    catch (Exception e)
                    {
                        _logger?.Error(e, "AxesVM MoveX Failed");
                    }

                    Application.Current?.Dispatcher.Invoke(() => OnPropertyChanged(nameof(Position)));
                });
            }
            catch (Exception e)
            {
                _messenger.Send(new Message(MessageLevel.Error, e.Message));
            }
        }

        private async Task DoMoveZ(double newPositionZ)
        {
            try
            {
                if (_zAxisConfig == null)
                    throw new InvalidOperationException("No configuration found for Z axis.");

                newPositionZ = Math.Max(_zAxisConfig.PositionMin?.Millimeters ?? newPositionZ, newPositionZ);
                newPositionZ = Math.Min(_zAxisConfig.PositionMax?.Millimeters ?? newPositionZ, newPositionZ);

                var targetPosition = Position.ToXyzPosition();
                targetPosition.Z = newPositionZ;
                targetPosition.Y = double.NaN;
                targetPosition.X = double.NaN;
                await Task.Run(() => _motionAxesSupervisor.GoToPosition(targetPosition, _selectedAxisSpeed));
            }
            catch (Exception e)
            {
                _messenger.Send(new Message(MessageLevel.Error, e.Message));
            }
            finally
            {
                _motionAxesSupervisor.WaitMotionEnd(MotionEndTimeout);
            }
        }

        public void DoMoveAxes(XYZPosition targetPosition)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        _motionAxesSupervisor.GoToPosition(targetPosition, _selectedAxisSpeed);
                        _motionAxesSupervisor.WaitMotionEnd(MotionEndTimeout);
                    }
                    catch (Exception e)
                    {
                        _logger?.Error(e, "AxesVM DoMoveAxes Failed");
                    }

                    Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(Position)));
                });
            }
            catch (Exception e)
            {
                _messenger.Send(new Message(MessageLevel.Error, e.Message));
            }
        }

        public AsyncRelayCommand GotoHome
        {
            get
            {
                return _gotoHome ?? (_gotoHome = new AsyncRelayCommand(
                async () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        response = await Task.Run(() => _motionAxesSupervisor.GoToHome(0));
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => IsReadyToStartMove));
            }
        }

        public AsyncRelayCommand Stop
        {
            get
            {
                return _stop ?? (_stop = new AsyncRelayCommand(
                async () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        response = await Task.Run(() => _motionAxesSupervisor.StopAllMotion());
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                }));
            }
        }

        public AsyncRelayCommand GoToEfemLoad =>
            _gotoEfemLoad ?? (_gotoEfemLoad = new AsyncRelayCommand(
                async () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        var waferDiameter = _chuckVM?.SelectedWaferCategory?.DimentionalCharacteristic?.Diameter;
                        if (waferDiameter == null)
                        {
                            throw new Exception("The wafer diameter cannot be found. Check that the dimensional characteristics are correctly defined.");
                        }

                        response = await Task.Run(() =>
                            _motionAxesSupervisor.GoToEfemLoad(waferDiameter, AxisSpeed.Slow));
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => IsReadyToStartMove && !IsLocked));

        public AsyncRelayCommand GoToManualLoad
        {
            get
            {
                return _gotoManualLoad ?? (_gotoManualLoad = new AsyncRelayCommand(
                async () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        var waferDiameter = _chuckVM?.SelectedWaferCategory?.DimentionalCharacteristic?.Diameter;
                        if (waferDiameter == null)
                        {
                            throw new Exception("The wafer diameter cannot be found. Check that the dimensional characteristics are correctly defined.");
                        }
                        response = await Task.Run(() => _motionAxesSupervisor.GoToManualLoad(waferDiameter, AxisSpeed.Slow));
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => IsReadyToStartMove && !IsLocked));
            }
        }

        public void AddMessagesOrExceptionToErrorsList<T>(Response<T> response, Exception e)
        {
            if (response?.Messages.Any() == true)
                response.Messages.Where(x => !string.IsNullOrEmpty(x.UserContent)).ToList().ForEach(x => _globalStatusSupervisor.SendUIMessage(x));
            else if (e != null)
                _globalStatusSupervisor.SendUIMessage(new Message(MessageLevel.Error, e.Message));
        }

        public void UpdateStatus(AxesState state)
        {
            if (Status == null)
                Status = new AxisStatus();

            Status.IsEnabled = state.AllAxisEnabled;
            Status.IsMoving = state.OneAxisIsMoving;
            OnPropertyChanged(nameof(IsReadyToStartMove));
        }

        private XYZPosition GetDestinationPositionFor(AxesMoveTypes moveType)
        {
            double x = double.NaN;
            double y = double.NaN;
            double z = double.NaN;

            switch (moveType)
            {
                case AxesMoveTypes.XPlus:
                    x = _xAxisConfig.PositionMax.Millimeters;
                    break;

                case AxesMoveTypes.XMinus:
                    x = _xAxisConfig.PositionMin.Millimeters;
                    break;

                case AxesMoveTypes.YPlus:
                    y = _yAxisConfig.PositionMax.Millimeters;
                    break;

                case AxesMoveTypes.YMinus:
                    y = _yAxisConfig.PositionMin.Millimeters;
                    break;

                case AxesMoveTypes.ZPlus:
                    z = _zAxisConfig.PositionMax.Millimeters;
                    break;

                case AxesMoveTypes.ZMinus:
                    z = _zAxisConfig.PositionMin.Millimeters;
                    break;
            }
            return new XYZPosition(new MotorReferential(), x, y, z);
        }

        private XYZPosition CreateSteppedDestinationFor(AxesMoveTypes moveType)
        {
            double x = Position.X;
            double y = Position.Y;
            double z = Position.Z;
            Length stepSize = 0.05.Millimeters();

            switch (moveType)
            {
                case AxesMoveTypes.XPlus:
                    x += stepSize.Millimeters;
                    break;
                case AxesMoveTypes.XMinus:
                    x -= stepSize.Millimeters;
                    break;
                case AxesMoveTypes.YPlus:
                    y += stepSize.Millimeters;
                    break;
                case AxesMoveTypes.YMinus:
                    y -= stepSize.Millimeters;
                    break;
                case AxesMoveTypes.ZPlus:
                    z += stepSize.Millimeters;
                    break;
                case AxesMoveTypes.ZMinus:
                    z -= stepSize.Millimeters;
                    break;
            }

            return new XYZPosition(new WaferReferential(), x, y, z);
        }

        public void PerformLongMove(AxesMoveTypes moveType)
        {
            Response<VoidResult> response = null;
            try
            {
                var destination = GetDestinationPositionFor(moveType);
                response = _motionAxesSupervisor.GoToPosition(destination, _selectedAxisSpeed);
            }
            catch (Exception e)
            {
                AddMessagesOrExceptionToErrorsList(response, e);
            }
        }

        public void PerformShortMove(AxesMoveTypes moveType)
        {
            Response<VoidResult> response = null;
            try
            {
                var stepAhead = CreateSteppedDestinationFor(moveType);
                response = _motionAxesSupervisor.GoToPosition(stepAhead, AxisSpeed.Normal);
            }
            catch (Exception e)
            {
                AddMessagesOrExceptionToErrorsList(response, e);
            }
        }
    }
}
