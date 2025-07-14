using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using UnitySC.PM.ANA.Client.Proxy.Axes.Models;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

using static UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck.SubstrateSlotConfig;

namespace UnitySC.PM.ANA.Client.Proxy.Axes
{
    public struct AxesSteps
    {
        public double Little;
        public double Normal;
        public double Big;
    }

    public enum AxesMoveTypes
    {
        XPlus,
        XMinus,
        YPlus,
        YMinus,
        ZTopPlus,
        ZTopMinus,
        ZBottomPlus,
        ZBottomMinus
    }

    public class AxesVM : ViewModelBaseExt
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IAxesService _axesSupervisor;
        private Position _position;
        private WaferMapResult _waferMap;
        private DieIndex _currentDieUserIndex;
        private Position _positionOnDie;

        private AxesConfig _axesConfiguration;
        private AxisSpeed _selectedAxisSpeed = AxisSpeed.Normal;
        private Status _status;

        //is true because an initialization phase starts in the launch of app. At the end of this movements, we want to update the targetposition
        private bool _isGoingToParticularPosition = true;

        private AutoRelayCommand<Position> _gotoPoint;
        private AutoRelayCommand<Increment> _moveIncremental;
        private AutoRelayCommand<double> _moveX;
        private AutoRelayCommand<double> _moveY;
        private AutoRelayCommand<double> _moveXOnDie;
        private AutoRelayCommand<double> _moveYOnDie;
        private AutoRelayCommand<double> _moveZTop;
        private AutoRelayCommand<double> _moveZBottom;
        private AutoRelayCommand<AxesMoveTypes> _moveStepStart;
        private AutoRelayCommand<AxesMoveTypes> _moveStepStop;

        private AutoRelayCommand _stop;
        private AutoRelayCommand _gotoHome;
        private AutoRelayCommand _gotoPark;
        private AutoRelayCommand _gotoChuckCenter;
        private AutoRelayCommand _gotoManualLoad;
        private AutoRelayCommand<SpecificPositions> _gotoGotoSpecificPosition;
        private AutoRelayCommand<OpticalReferenceDefinition> _gotoRefPos;
        private AutoRelayCommand _getAxesConfiguration;
        private AutoRelayCommand _changeLandStatus;
        private GlobalStatusSupervisor _globalStatusSupervisor;

        private int _delayBeforeContinuousMove = 500;
        private bool _isWaitingMove = false;
        private bool _continuousMoveStarted = false;
        private bool _isSpeedLimitedWhenUnclamped = false;

        private CancellationTokenSource _continuousMoveCTSource;

        #endregion Fields

        #region Constructors

        public AxesVM(IAxesService axesSupervisor, IDialogOwnerService dialogService, ILogger logger)
        {
            _logger = logger;
            _axesSupervisor = axesSupervisor;

            _status = new Status();
            _messageAlreadyReceived = new List<string>();

            _canMoveDictionary = new Dictionary<AxesMoveTypes, bool>()
            {
                { AxesMoveTypes.XMinus, true },
                { AxesMoveTypes.XPlus, true },
                { AxesMoveTypes.YMinus, true },
                { AxesMoveTypes.YPlus, true },
                { AxesMoveTypes.ZTopMinus, true },
                { AxesMoveTypes.ZTopPlus, true },
                { AxesMoveTypes.ZBottomMinus, true },
                { AxesMoveTypes.ZBottomPlus, true },
            };

            // We subscribe to the global key events
            ServiceLocator.KeyboardMouseHook.KeyUpDownEvent += KeyboardMouseHook_KeyEvent;

            Position.PropertyChanged += Position_PropertyChanged;
            Status.PropertyChanged += Status_PropertyChanged;

            ServiceLocator.GlobalStatusSupervisor.OnNewMessage += GlobalStatusSupervisor_OnNewMessage;
        }

        private const int MaxStoredErrorMessages = 1;
        private readonly object _messageLock = new object();

        private List<string> _messageAlreadyReceived;

        private async void GlobalStatusSupervisor_OnNewMessage(Message message)
        {
            if (_messageAlreadyReceived == null)
            {
                //If the messageAlreadyReceived is null, it means the message is received before the instantiation of
                //the class. For example, the client is not initialized yet.
                return;
            }

            ApplicationMode mode;

            try
            {
                mode = ClassLocator.Default.GetInstance<PMViewModel>().Mode;
            }
            catch (Exception)
            {

                return;
            }


            if (mode == ApplicationMode.Production)
            {
                return;
            }
            lock (_messageLock)
            {
                if (message.UserContent.Contains("Axis blocked"))
                {
                    _messageAlreadyReceived.Add(message.UserContent);
                }

                //Here we limit the display of a popup because potentially, if a client is not connected,
                //the low-level task will continue to inform that the action to reset the axes needs to be performed.
                if (_messageAlreadyReceived.Count > MaxStoredErrorMessages)
                {
                    return;
                }
            }
            if (IsAxisBlockedError(message))
            {
                try
                {
                    await ExecuteResetAxis();
                    lock (_messageLock)
                    {
                        _messageAlreadyReceived.Clear();
                    }
                }
                catch
                {
                    _logger.Error("The axes were not resolved successfully.");
                }
            }
        }

        private bool IsAxisBlockedError(Message message)
        {
            //TODO: Handling error messages in a way other than using strings...
            return message.Level == MessageLevel.Error && message.UserContent.Contains("Axis blocked") && ServiceLocator.GlobalStatusSupervisor.CurrentState != PMGlobalStates.Error;
        }

        private async Task ExecuteResetAxis()
        {
            var pmViewModel = ClassLocator.Default.GetInstance<PMViewModel>();
            bool previousBusySate = pmViewModel.IsBusy;
            string previousBusyContent = pmViewModel.BusyContent;

            var result = await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Axes are blocked. \nPress the Yes button when you are ready to reset them.", "Axes", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.None);
            });

            if (result == MessageBoxResult.No)
            {
                _logger.Warning("The axes were not reset. You may encounter problems moving the axes");
                bool resultAkn = _axesSupervisor.AcknowledgeResetAxis()?.Result ?? false;
                _globalStatusSupervisor.CurrentState = PMGlobalStates.Error;
                _globalStatusSupervisor.GlobalStatusChangedCallback(new GlobalStatus(PMGlobalStates.Error));
            }
            else
            {
                pmViewModel.IsBusy = true;
                pmViewModel.BusyContent = "Resetting axis, please wait...";

                var probeSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
                bool resInit = _axesSupervisor.ResetAxis()?.Result ?? false;
                resInit &= probeSupervisor.ResetObjectivesSelectors()?.Result ?? false;
                if (!resInit)
                {
                    pmViewModel.IsBusy = false;
                    pmViewModel.BusyContent = previousBusyContent;
                    string message = "[AxesVM][RobotIsOutService][Axis blocked] Robot is out service. Stop movement. Please initialise the EEFM.";
                    _globalStatusSupervisor.SendUIMessage(new Message(MessageLevel.Fatal, message));
                    _globalStatusSupervisor.CurrentState = PMGlobalStates.ErrorHandling;
                    _globalStatusSupervisor.GlobalStatusChangedCallback(new GlobalStatus(PMGlobalStates.ErrorHandling));

                    return;
                }
            }
            pmViewModel.IsBusy = previousBusySate;
            pmViewModel.BusyContent = previousBusyContent;
        }

        #endregion Constructors

        public void Init()
        {
            try
            {
                _globalStatusSupervisor = ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.ANALYSE);

                // Initialize status
                var resultState = _axesSupervisor.GetCurrentState()?.Result;
                if (resultState != null)
                {
                    UpdateStatus(resultState);
                }

                // Initialize configuration
                if (_axesConfiguration == null)
                {
                    AxesConfiguration = _axesSupervisor.GetAxesConfiguration()?.Result;
                    OnPropertyChanged(nameof(ConfigurationAxisX));
                    OnPropertyChanged(nameof(ConfigurationAxisY));
                    OnPropertyChanged(nameof(ConfigurationAxisZTop));
                    OnPropertyChanged(nameof(ConfigurationAxisZBottom));
                    Piezos = new List<PiezoVM>();
                    foreach (var piezoConfig in AxesConfiguration?.AxisConfigs.OfType<PiezoAxisConfig>())
                    {
                        Piezos.Add(new PiezoVM()
                        {
                            Name = piezoConfig.AxisID,
                            Position = new LengthVM(0, LengthUnit.Micrometer),
                            Min = new LengthVM(piezoConfig.PositionMin, LengthUnit.Micrometer),
                            Max = new LengthVM(piezoConfig.PositionMax, LengthUnit.Micrometer)
                        });
                    }
                }
                // Initialize position
                var resultPos = _axesSupervisor.GetCurrentPosition()?.Result;
                if (resultPos != null && resultPos is AnaPosition position)
                {
                    UpdatePosition(position);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"AxesVM Init Failed");
            }
        }

        #region Private methods

        private void Position_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => GotoPoint.NotifyCanExecuteChanged()));
        }

        private void Status_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Console.WriteLine(e.PropertyName)
            if ((e.PropertyName == nameof(Status.IsMoving)) && (!Status.IsMoving))
                IsGoingToParticularPosition = false;
            UpdateAllCanExecutes();
        }

        public void AddMessagesOrExceptionToErrorsList<T>(Response<T> response, Exception e)
        {
            // If there is at least one message
            if ((response != null) && response.Messages.Any())
            {
                foreach (var message in response.Messages)
                {
                    if (!string.IsNullOrEmpty(message.UserContent))
                        _globalStatusSupervisor.SendUIMessage(message);
                }
            }
            else
            {
                if (e != null)
                    _globalStatusSupervisor.SendUIMessage(new Message(MessageLevel.Error, e.Message));
            }
        }

        private XYZTopZBottomPosition CreateSteppedDestinationFor(AxesMoveTypes moveType)
        {
            double x = Position.X;
            double y = Position.Y;
            double top = Position.ZTop;
            double bottom = Position.ZBottom;

            Length stepSize = GetStepSize(moveType);

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

                case AxesMoveTypes.ZTopPlus:
                    top += stepSize.Millimeters;
                    break;

                case AxesMoveTypes.ZTopMinus:
                    top -= stepSize.Millimeters;
                    break;

                case AxesMoveTypes.ZBottomPlus:
                    bottom += stepSize.Millimeters;
                    break;

                case AxesMoveTypes.ZBottomMinus:
                    bottom -= stepSize.Millimeters;
                    break;
            }

            var destination = new XYZTopZBottomPosition(new WaferReferential(), x, y, top, bottom);
            return destination;
        }

        private XYZTopZBottomPosition GetDestinationPositionFor(AxesMoveTypes moveType)
        {
            // set default values
            double x = double.NaN;
            double y = double.NaN;
            double top = double.NaN;
            double bottom = double.NaN;

            switch (moveType)
            {
                case AxesMoveTypes.XPlus:
                    x = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.X).PositionMax.Millimeters;
                    break;

                case AxesMoveTypes.XMinus:
                    x = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.X).PositionMin.Millimeters;
                    break;

                case AxesMoveTypes.YPlus:
                    y = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.Y).PositionMax.Millimeters;
                    break;

                case AxesMoveTypes.YMinus:
                    y = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.Y).PositionMin.Millimeters;
                    break;

                case AxesMoveTypes.ZTopPlus:
                    top = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop).PositionMax.Millimeters;
                    break;

                case AxesMoveTypes.ZTopMinus:
                    top = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop).PositionMin.Millimeters;
                    break;

                case AxesMoveTypes.ZBottomPlus:
                    bottom = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom).PositionMax.Millimeters;
                    break;

                case AxesMoveTypes.ZBottomMinus:
                    bottom = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom).PositionMin.Millimeters;
                    break;

                default:
                    break;
            }
            var destination = new XYZTopZBottomPosition(new MotorReferential(), x, y, top, bottom);
            return destination;
        }

        #endregion Private methods

        #region Properties

        public List<PiezoVM> Piezos { get; set; }

        public WaferMapResult WaferMap
        {
            get
            {
                return _waferMap;
            }
            set
            {
                _waferMap = value;
                if (!(_waferMap is null))
                    ServiceLocator.ReferentialSupervisor.SetSettings(new DieReferentialSettings(_waferMap.RotationAngle, _waferMap.DieDimensions, _waferMap.DieGridTopLeft, _waferMap.DiesPresence));

                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentDieUserIndex));
            }
        }

        // Position in WaferReferential
        public Position Position
        {
            get
            {
                if (_position is null)
                    _position = new Position(new WaferReferential(), 0, 0, 0, 0, AxisSpeed.Normal);
                return _position;
            }
        }

        // Current Die User index
        public DieIndex CurrentDieUserIndex
        {
            get
            {
                if (_currentDieUserIndex is null)
                    _currentDieUserIndex = new DieIndex(0, 0);
                //Console.WriteLine($"CurrentDieIndex Column : {_currentDieUserIndex.Column} Row : {_currentDieUserIndex.Row}");
                return _currentDieUserIndex;
            }
            set
            {
                if (_currentDieUserIndex == value)
                    return;
                _currentDieUserIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentDieIndex));
            }
        }

        public DieIndex CurrentDieIndex
        {
            get
            {
                return ConvertDieUserIndexToDieIndex(CurrentDieUserIndex);
            }
        }

        public Position PositionOnDie
        {
            get
            {
                if (_positionOnDie is null)
                    _positionOnDie = new Position(new DieReferential(), 0, 0, 0, 0, AxisSpeed.Normal);

                return _positionOnDie;
            }
        }

        public void UpdatePosition(AnaPosition result)
        {
            // Conversion to Wafer referencial
            var positionOnWafer = result;
            Position.UpdatePosition(positionOnWafer);

            OnPropertyChanged(nameof(Position));

            if (!(WaferMap is null))
            {
                PositionBase diePosition;
                // We update the die position
                try
                {
                    diePosition = ServiceLocator.ReferentialSupervisor.ConvertTo(ServiceLocator.AxesSupervisor.AxesVM.Position.ToAxesPosition(), ReferentialTag.Die)?.Result;
                    var diePositionXYZ = diePosition.ToXYZTopZBottomPosition();

                    _positionOnDie = new Position(diePositionXYZ.Referential, diePositionXYZ.X, diePositionXYZ.Y, diePositionXYZ.ZTop, diePositionXYZ.ZBottom, AxisSpeed.Fast);

                    var currentDieIndexServer = new DieIndex(((DieReferential)diePosition.Referential).DieColumn, ((DieReferential)diePosition.Referential).DieLine);
                    CurrentDieUserIndex = ConvertDieIndexToDieUserIndex(currentDieIndexServer);

                    OnPropertyChanged(nameof(PositionOnDie));
                }
                catch (Exception)
                {
                }
            }

            Length epsilon = 0.001.Millimeters();

            // Check the limits
            CanMoveDictionary[AxesMoveTypes.XPlus] = Position.X < (AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.X).PositionMax - epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.XMinus] = Position.X > (AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.X).PositionMin + epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.YPlus] = Position.Y < (AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.Y).PositionMax - epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.YMinus] = Position.Y > (AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.Y).PositionMin + epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.ZTopPlus] = Position.ZTop < (AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop).PositionMax - epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.ZTopMinus] = Position.ZTop > (AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop).PositionMin + epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.ZBottomPlus] = Position.ZBottom < (AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom).PositionMax - epsilon).Millimeters;
            CanMoveDictionary[AxesMoveTypes.ZBottomMinus] = Position.ZBottom > (AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom).PositionMin + epsilon).Millimeters;

            //Console.WriteLine($"Position Updated : {Position.X} {Position.Y}");
            OnPropertyChanged(nameof(CanMoveDictionary));

            // Update piezo position
            foreach (var piezoPosition in result.ZPiezoPositions)
            {
                var piezo = Piezos?.FirstOrDefault(x => x.Name == piezoPosition.AxisID);
                if (!(piezo is null))
                    piezo.Position.Value = piezoPosition.Position;
            }
        }

        private void KeyboardMouseHook_KeyEvent(object sender, KeyboardMouseHook.KeyGlobalEventArgs e)
        {
            if (e.CurrentKey == Key.F11)
            {
                IsKeyboardInputMode = true;
                return;
            }

            // If the axes are locked, we ignore the keys
            if ((!IsKeyboardInputMode) || IsLocked)
                return;

            AxesMoveTypes? curMoveType = null;

            switch (e.CurrentKey)
            {
                case Key.Left:
                    curMoveType = AxesMoveTypes.XMinus;
                    break;

                case Key.Right:
                    curMoveType = AxesMoveTypes.XPlus;
                    break;

                case Key.Up:
                    curMoveType = AxesMoveTypes.YPlus;
                    break;

                case Key.Down:
                    curMoveType = AxesMoveTypes.YMinus;
                    break;

                case Key.PageUp:
                    {
                        curMoveType = AxesMoveTypes.ZTopPlus;
                        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                            curMoveType = AxesMoveTypes.ZBottomPlus;
                    }
                    break;

                case Key.PageDown:
                    {
                        curMoveType = AxesMoveTypes.ZTopMinus;
                        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                            curMoveType = AxesMoveTypes.ZBottomMinus;
                    }
                    break;
            }

            if (curMoveType != null)
            {
                if (e.IsKeyDown)
                    MoveStepStart.Execute(curMoveType);
                else // Key up
                    MoveStepStop.Execute(curMoveType);
            }
        }

        private void KeyboardMouseHook_MouseDownEvent(object sender, EventArgs e)
        {
            // when a mouse button is clicked, we leave the keyboard mode
            IsKeyboardInputMode = false;
        }

        public Status Status => _status;

        public void UpdateStatus(AxesState state)
        {
            Console.WriteLine("Are axes Moving :" + state.OneAxisIsMoving);
            Console.WriteLine("Axes UpdateStatus");
            _status.IsEnabled = state.AllAxisEnabled;
            _status.IsMoving = state.OneAxisIsMoving;
            _status.IsLanded = state.Landed;

            // If the max/min position is reached
            if (!_status.IsMoving && _continuousMoveStarted)
                _continuousMoveStarted = false;

            Console.WriteLine($"Status IsMoving changed : {_status.IsMoving} ");

            OnPropertyChanged(nameof(IsReadyToStartMove));
        }

        public bool IsGoingToParticularPosition
        {
            get
            {
                return _isGoingToParticularPosition;
            }
            set
            {
                if (_isGoingToParticularPosition == value)
                {
                    return;
                }
                _isGoingToParticularPosition = value;
                OnPropertyChanged();
            }
        }

        public AxesConfig AxesConfiguration
        {
            get
            {
                if (_axesConfiguration == null)
                {
                    _axesConfiguration = _axesSupervisor.GetAxesConfiguration()?.Result;
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

        public AxisSpeed SelectedAxisSpeed
        {
            get { return _selectedAxisSpeed; }
            set
            {
                _selectedAxisSpeed = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<AxisSpeed> SpeedAxisValues
        {
            get
            {
                return Enum.GetValues(typeof(AxisSpeed))
                    .Cast<AxisSpeed>();
            }
        }

        public AxisConfig ConfigurationAxisX => AxesConfiguration?.AxisConfigs?.FirstOrDefault<AxisConfig>(a => a.MovingDirection == MovingDirection.X);
        public AxisConfig ConfigurationAxisY => AxesConfiguration?.AxisConfigs?.FirstOrDefault<AxisConfig>(a => a.MovingDirection == MovingDirection.Y);
        public AxisConfig ConfigurationAxisZTop => AxesConfiguration?.AxisConfigs?.FirstOrDefault<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop);
        public AxisConfig ConfigurationAxisZBottom => AxesConfiguration?.AxisConfigs?.FirstOrDefault<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom);

        public bool IsReadyToStartMove => _status.IsEnabled && !_isWaitingMove && !_status.IsMoving && !_status.IsLanded;

        protected bool IsWaitingMove
        {
            get => _isWaitingMove; set { if (_isWaitingMove != value) { _isWaitingMove = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsReadyToStartMove)); } }
        }

        public bool IsSpeedLimitedWhenUnclamped
        {
            get => _isSpeedLimitedWhenUnclamped;
            set
            {
                if (_isSpeedLimitedWhenUnclamped != value)
                {
                    _isSpeedLimitedWhenUnclamped = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<AxesMoveTypes, bool> _canMoveDictionary;

        public Dictionary<AxesMoveTypes, bool> CanMoveDictionary
        {
            get => _canMoveDictionary; set { if (_canMoveDictionary != value) { _canMoveDictionary = value; OnPropertyChanged(); } }
        }

        private bool _isKeyboardInputMode = false;

        public bool IsKeyboardInputMode
        {
            get => _isKeyboardInputMode;
            set
            {
                if (_isKeyboardInputMode != value)
                {
                    _isKeyboardInputMode = value;
                    if (_isKeyboardInputMode)
                    {
                        ServiceLocator.KeyboardMouseHook.HandleKeyBoard();
                        // We subscribe to the global mouse down event
                        ServiceLocator.KeyboardMouseHook.StartCaptureMouseDown();
                        ServiceLocator.KeyboardMouseHook.MouseDownEvent += KeyboardMouseHook_MouseDownEvent;
                    }
                    else
                    {
                        ServiceLocator.KeyboardMouseHook.UnhandleKeyBoard();
                        // We unsubscribe to the global mouse down event
                        ServiceLocator.KeyboardMouseHook.StopCaptureMouseDown();
                        ServiceLocator.KeyboardMouseHook.MouseDownEvent -= KeyboardMouseHook_MouseDownEvent;
                    }
                    OnPropertyChanged();
                }
            }
        }

        // When the axes are locked, the user can not move it
        private bool _isLocked = false;

        public bool IsLocked
        {
            get => _isLocked; set { if (_isLocked != value) { _isLocked = value; UpdateAllCanExecutes(); OnPropertyChanged(); } }
        }

        #endregion Properties

        #region RelayCommands

        public AutoRelayCommand<Position> GotoPoint
        {
            get
            {
                return _gotoPoint ?? (_gotoPoint = new AutoRelayCommand<Position>(
                (positionTarget) =>
                {
                    GotoPosition(positionTarget);
                },
                (positionTarget) => !Status.IsMoving && !Status.IsLanded));
            }
        }

        private void GotoPosition(Position positionTarget)
        {
            Response<bool> response = null;
            try
            {
                var destination = positionTarget.ToAxesPosition();

                response = _axesSupervisor.GotoPosition(destination, SelectedAxisSpeed);
            }
            catch (Exception e)
            {
                AddMessagesOrExceptionToErrorsList(response, e);
            }
        }

        public AutoRelayCommand<AxesMoveTypes> MoveStepStart
        {
            get
            {
                return _moveStepStart ?? (_moveStepStart = new AutoRelayCommand<AxesMoveTypes>(
                (moveType) =>
                {
                    Response<bool> response = null;
                    try
                    {
                        IsWaitingMove = true;
                        SelectedAxisSpeed = AxisSpeed.Normal;

                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            SelectedAxisSpeed = AxisSpeed.Slow;
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                            SelectedAxisSpeed = AxisSpeed.Fast;

                        _continuousMoveCTSource = new CancellationTokenSource();
                        var task = Task.Run(() => TaskStartMove(moveType), _continuousMoveCTSource.Token); // Pass same token to Task.Run.
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                (moveType) => IsReadyToStartMove && !IsLocked));
            }
        }

        private void TaskStartMove(AxesMoveTypes moveType)
        {
            Console.WriteLine("We start the Task move");

            // Were we already canceled?
            if (_continuousMoveCTSource.IsCancellationRequested)
                return;
            DateTime startWaitingTime = DateTime.Now;
            while ((DateTime.Now - startWaitingTime).TotalMilliseconds < _delayBeforeContinuousMove)
            {
                Thread.Sleep(10);
                if (_continuousMoveCTSource.IsCancellationRequested)
                    return;
            }
            if (_continuousMoveCTSource.IsCancellationRequested)
                return;
            Console.WriteLine("The delay is expired we start the continuous move");

            if (!IsWaitingMove)
                return;

            IsWaitingMove = false;

            Response<bool> response = null;
            // Delay is expired we start the continuous move
            try
            {
                var destination = GetDestinationPositionFor(moveType);
                response = _axesSupervisor.GotoPosition(destination, SelectedAxisSpeed);

                if (response.Result)
                    _continuousMoveStarted = true;
            }
            catch (Exception e)
            {
                AddMessagesOrExceptionToErrorsList(response, e);
            }
        }

        public AutoRelayCommand<AxesMoveTypes> MoveStepStop
        {
            get
            {
                return _moveStepStop ?? (_moveStepStop = new AutoRelayCommand<AxesMoveTypes>(
                (moveType) =>
                {
                    Response<bool> response = null;
                    try
                    {
                        if ((!IsWaitingMove) && (!_continuousMoveStarted))
                            return;

                        _continuousMoveCTSource.Cancel();

                        if (_continuousMoveStarted)
                        {
                            // We stop all moves
                            Console.WriteLine("We stop the continuous move");
                            response = _axesSupervisor.StopAllMoves();
                            _continuousMoveStarted = false;
                        }
                        else
                        {
                            // We move 1 step
                            Console.WriteLine("We move one step");
                            Console.WriteLine($"Position before move : {Position.X} : {Position.Y} : {Position.ZTop} : {Position.ZBottom}");

                            var stepAhead = CreateSteppedDestinationFor(moveType);

                            // For the step movements we select always the speed normal
                            SelectedAxisSpeed = AxisSpeed.Normal;
                            response = _axesSupervisor.GotoPosition(stepAhead, SelectedAxisSpeed);
                        }

                        IsWaitingMove = false;
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                }, (moveType) => true));
            }
        }

        private Length GetStepSize(AxesMoveTypes moveType)
        {
            // X or Y direction
            if ((moveType == AxesMoveTypes.XMinus) || (moveType == AxesMoveTypes.XPlus) || (moveType == AxesMoveTypes.YMinus) || (moveType == AxesMoveTypes.YPlus))
            {
                switch (SelectedAxisSpeed)
                {
                    case AxisSpeed.Slow:
                        return ServiceLocator.CamerasSupervisor.Objective.SmallStepSizeXY;

                    case AxisSpeed.Normal:
                        return ServiceLocator.CamerasSupervisor.Objective.NormalStepSizeXY;

                    case AxisSpeed.Fast:
                        return ServiceLocator.CamerasSupervisor.Objective.BigStepSizeXY;

                    case AxisSpeed.Measure:
                        return ServiceLocator.CamerasSupervisor.Objective.NormalStepSizeXY;
                }
                return ServiceLocator.CamerasSupervisor.Objective.NormalStepSizeXY;
            }
            else // ZTop or ZBottom
            {
                switch (SelectedAxisSpeed)
                {
                    case AxisSpeed.Slow:
                        return ServiceLocator.CamerasSupervisor.Objective.SmallStepSizeZ;

                    case AxisSpeed.Normal:
                        return ServiceLocator.CamerasSupervisor.Objective.NormalStepSizeZ;

                    case AxisSpeed.Fast:
                        return ServiceLocator.CamerasSupervisor.Objective.BigStepSizeZ;

                    case AxisSpeed.Measure:
                        return ServiceLocator.CamerasSupervisor.Objective.NormalStepSizeZ;
                }
                return ServiceLocator.CamerasSupervisor.Objective.NormalStepSizeZ;
            }
        }

        public AutoRelayCommand<double> MoveX
        {
            get
            {
                return _moveX ?? (_moveX = new AutoRelayCommand<double>(
                (newPositionX) =>
                {
                    DoMoveX(newPositionX);
                },
                (newPositionX) => IsReadyToStartMove && !IsLocked));
            }
        }

        private void DoMoveX(double newPositionX)
        {
            Response<bool> response = null;
            try
            {
                if (newPositionX > AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.X).PositionMax.Millimeters)
                    newPositionX = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.X).PositionMax.Millimeters;
                if (newPositionX < AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.X).PositionMin.Millimeters)
                    newPositionX = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.X).PositionMin.Millimeters;

                var destination = new XYZTopZBottomPosition(new WaferReferential(), newPositionX, Position.Y, Position.ZTop, Position.ZBottom);
                response = _axesSupervisor.GotoPosition(destination, SelectedAxisSpeed);
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionOnDie));
                OnPropertyChanged(nameof(CurrentDieUserIndex));
            }
            catch (Exception e)
            {
                AddMessagesOrExceptionToErrorsList(response, e);
            }
        }

        public AutoRelayCommand<double> MoveY
        {
            get
            {
                return _moveY ?? (_moveY = new AutoRelayCommand<double>(
                (newPositionY) =>
                {
                    DoMoveY(newPositionY);
                },
                (newPositionX) => IsReadyToStartMove && !IsLocked));
            }
        }

        private void DoMoveY(double newPositionY)
        {
            Response<bool> response = null;
            try
            {
                if (newPositionY > AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.Y).PositionMax.Millimeters)
                    newPositionY = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.Y).PositionMax.Millimeters;
                if (newPositionY < AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.Y).PositionMin.Millimeters)
                    newPositionY = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.Y).PositionMin.Millimeters;

                var destination = new XYZTopZBottomPosition(new WaferReferential(), Position.X, newPositionY, Position.ZTop, Position.ZBottom);
                response = _axesSupervisor.GotoPosition(destination, SelectedAxisSpeed);
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionOnDie));
                OnPropertyChanged(nameof(CurrentDieUserIndex));
            }
            catch (Exception e)
            {
                AddMessagesOrExceptionToErrorsList(response, e);
            }
        }

        public AutoRelayCommand<double> MoveXOnDie
        {
            get
            {
                return _moveXOnDie ?? (_moveXOnDie = new AutoRelayCommand<double>(
                (newPositionX) =>
                {
                    XYZTopZBottomPosition waferPosition;

                    try
                    {
                        var currentDieIndexServer = ConvertDieUserIndexToDieIndex(CurrentDieUserIndex);

                        waferPosition = ServiceLocator.ReferentialSupervisor.ConvertTo(new XYZTopZBottomPosition(new DieReferential(currentDieIndexServer.Column, currentDieIndexServer.Row), newPositionX, PositionOnDie.Y, double.NaN, double.NaN), ReferentialTag.Wafer)?.Result.ToXYZTopZBottomPosition(); ;
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    DoMoveX(waferPosition.X);
                },
                (newPositionX) => IsReadyToStartMove && !IsLocked));
            }
        }

        public AutoRelayCommand<double> MoveYOnDie
        {
            get
            {
                return _moveYOnDie ?? (_moveYOnDie = new AutoRelayCommand<double>(
                (newPositionY) =>
                {
                    XYZTopZBottomPosition waferPosition;

                    try
                    {
                        var currentDieIndexServer = ConvertDieUserIndexToDieIndex(CurrentDieUserIndex);
                        waferPosition = ServiceLocator.ReferentialSupervisor.ConvertTo(new XYZTopZBottomPosition(new DieReferential(currentDieIndexServer.Column, currentDieIndexServer.Row), PositionOnDie.X, newPositionY, double.NaN, double.NaN), ReferentialTag.Wafer)?.Result.ToXYZTopZBottomPosition(); ;
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    DoMoveY(waferPosition.Y);
                },
                (newPositionX) => IsReadyToStartMove && !IsLocked));
            }
        }

        // dieIndex is a user index
        public void MoveToDiePosition(DieIndex dieUserIndex, double x, double y)
        {
            XYZTopZBottomPosition waferPosition;

            try
            {
                var dieIndex = ConvertDieUserIndexToDieIndex(dieUserIndex);
                waferPosition = ServiceLocator.ReferentialSupervisor.ConvertTo(new XYZTopZBottomPosition(new DieReferential(dieIndex.Column, dieIndex.Row), x, y, double.NaN, double.NaN), ReferentialTag.Wafer)?.Result.ToXYZTopZBottomPosition(); ;
            }
            catch (Exception)
            {
                return;
            }

            _axesSupervisor.GotoPosition(waferPosition, SelectedAxisSpeed);
        }

        public AutoRelayCommand<double> MoveZTop
        {
            get
            {
                return _moveZTop ?? (_moveZTop = new AutoRelayCommand<double>(
                (newPositionZTop) =>
                {
                    Response<bool> response = null;
                    try
                    {
                        if (newPositionZTop > AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop).PositionMax.Millimeters)
                            newPositionZTop = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop).PositionMax.Millimeters;
                        if (newPositionZTop < AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop).PositionMin.Millimeters)
                            newPositionZTop = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZTop).PositionMin.Millimeters;
                        var destination = new XYZTopZBottomPosition(new WaferReferential(), Position.X, Position.Y, newPositionZTop, Position.ZBottom);
                        response = _axesSupervisor.GotoPosition(destination, SelectedAxisSpeed);
                        OnPropertyChanged(nameof(Position));
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                (newPositionZTop) => IsReadyToStartMove && !IsLocked));
            }
        }

        public AutoRelayCommand<double> MoveZBottom
        {
            get
            {
                return _moveZBottom ?? (_moveZBottom = new AutoRelayCommand<double>(
                (newPositionZBottom) =>
                {
                    Response<bool> response = null;
                    try
                    {
                        if (newPositionZBottom > AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom).PositionMax.Millimeters)
                            newPositionZBottom = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom).PositionMax.Millimeters;
                        if (newPositionZBottom < AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom).PositionMin.Millimeters)
                            newPositionZBottom = AxesConfiguration.AxisConfigs.First<AxisConfig>(a => a.MovingDirection == MovingDirection.ZBottom).PositionMin.Millimeters;
                        var destination = new XYZTopZBottomPosition(new WaferReferential(), Position.X, Position.Y, Position.ZTop, newPositionZBottom);
                        response = _axesSupervisor.GotoPosition(destination, SelectedAxisSpeed);
                        OnPropertyChanged(nameof(Position));
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                (newPositionZBottom) => IsReadyToStartMove && !IsLocked));
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
                        response = _axesSupervisor.StopAllMoves();
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => true));
            }
        }

        public AutoRelayCommand GoToHome
        {
            get
            {
                return _gotoHome ?? (_gotoHome = new AutoRelayCommand(
                () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        response = _axesSupervisor.GoToHome(SelectedAxisSpeed);
                        IsGoingToParticularPosition = true;
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => IsReadyToStartMove && !IsLocked));
            }
        }

        public AutoRelayCommand GoToPark
        {
            get
            {
                return _gotoPark ?? (_gotoPark = new AutoRelayCommand(
                () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        var waferDiameter = ServiceLocator.ChuckSupervisor.ChuckVM?.SelectedWaferCategory?.DimentionalCharacteristic?.Diameter;
                        if (waferDiameter == null)
                        {
                            throw new Exception("The wafer diameter cannot be found. Check that the dimensional characteristics are correctly defined.");
                        }
                        response = _axesSupervisor.GoToPark(waferDiameter, SelectedAxisSpeed);
                        IsGoingToParticularPosition = true;
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => IsReadyToStartMove && !IsLocked));
            }
        }

        public AutoRelayCommand GoToChuckCenter
        {
            get
            {
                return _gotoChuckCenter ?? (_gotoChuckCenter = new AutoRelayCommand(
                () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        var waferDiameter = ServiceLocator.ChuckSupervisor.ChuckVM?.SelectedWaferCategory?.DimentionalCharacteristic?.Diameter;
                        if (waferDiameter == null)
                        {
                            throw new Exception("The wafer diameter cannot be found. Check that the dimensional characteristics are correctly defined.");
                        }
                        response = _axesSupervisor.GoToChuckCenter(waferDiameter, SelectedAxisSpeed);
                        IsGoingToParticularPosition = true;
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => IsReadyToStartMove && !IsLocked));
            }
        }

        public AutoRelayCommand GoToManualLoad
        {
            get
            {
                return _gotoManualLoad ?? (_gotoManualLoad = new AutoRelayCommand(
                () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        var waferDiameter = ServiceLocator.ChuckSupervisor.ChuckVM?.SelectedWaferCategory?.DimentionalCharacteristic?.Diameter;
                        if(waferDiameter == null)
                        {
                            throw new Exception("The wafer diameter cannot be found. Check that the dimensional characteristics are correctly defined.");
                        }
                        response = _axesSupervisor.GoToManualLoad(waferDiameter, SelectedAxisSpeed);                        
                        IsGoingToParticularPosition = true;
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => IsReadyToStartMove && !IsLocked));
            }
        }

        public AutoRelayCommand<SpecificPositions> GotoSpecificPosition
        {
            get
            {
                return _gotoGotoSpecificPosition ?? (_gotoGotoSpecificPosition = new AutoRelayCommand<SpecificPositions>(
                (specificPosition) =>
                {
                    Response<bool> response = null;
                    try
                    {
                        var waferDiameter = ServiceLocator.ChuckSupervisor.ChuckVM?.SelectedWaferCategory?.DimentionalCharacteristic?.Diameter;
                        if (waferDiameter == null)
                        {
                            throw new Exception("The wafer diameter cannot be found. Check that the dimensional characteristics are correctly defined.");
                        }
                        response = _axesSupervisor.GotoSpecificPosition(specificPosition, waferDiameter, SelectedAxisSpeed);
                        IsGoingToParticularPosition = true;
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                (specificPosition) => IsReadyToStartMove && !IsLocked));
            }
        }

        public AutoRelayCommand<OpticalReferenceDefinition> GotoRefPos
        {
            get
            {
                return _gotoRefPos ?? (_gotoRefPos = new AutoRelayCommand<OpticalReferenceDefinition>(
                (reference) =>
                {
                    Response<bool> response = null;
                    try
                    {
                        var position = GetPositionOnOpticalReference(reference);
                        _axesSupervisor.GotoPosition(position, SelectedAxisSpeed);
                        IsGoingToParticularPosition = true;
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                (reference) => IsReadyToStartMove && !IsLocked));
            }
        }

        public AutoRelayCommand ChangeLandStatus
        {
            get
            {
                return _changeLandStatus ?? (_changeLandStatus = new AutoRelayCommand(
                () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        if (Status.IsLanded)
                        {
                            response = _axesSupervisor.Land();
                        }
                        else
                        {
                            response = _axesSupervisor.StopLanding();
                        }
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => !Status.IsMoving));
            }
        }

        public AutoRelayCommand GetAxesConfiguration
        {
            get
            {
                return _getAxesConfiguration ?? (_getAxesConfiguration = new AutoRelayCommand(
              () =>
              {
                  AxesConfiguration = _axesSupervisor.GetAxesConfiguration()?.Result;
              },
              () => true));
            }
        }

        public AutoRelayCommand<Increment> MoveIncremental
        {
            get
            {
                return _moveIncremental ?? (_moveIncremental = new AutoRelayCommand<Increment>(
                (increment) =>
                {
                    Response<bool> response = null;

                    try
                    {
                        var move = new XYZTopZBottomMove(increment.StepX, increment.StepY, increment.StepZTop, increment.StepZBottom);
                        response = _axesSupervisor.MoveIncremental(move, SelectedAxisSpeed);
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                (increment) => !Status.IsMoving && !Status.IsLanded));
            }
        }

        #endregion RelayCommands

        public DieIndex ConvertDieIndexToDieUserIndex(DieIndex dieIndex)
        {
            if (WaferMap is null)
                return null;
            return dieIndex.ToDieReference(WaferMap.DieReference);
        }

        public DieIndex ConvertDieUserIndexToDieIndex(DieIndex dieIndex)
        {
            if (WaferMap is null)
                return null;
            return dieIndex.FromDieReference(WaferMap.DieReference);
        }

        protected override void OnDeactivated()
        {
            ServiceLocator.KeyboardMouseHook.KeyUpDownEvent -= KeyboardMouseHook_KeyEvent;
            ServiceLocator.KeyboardMouseHook.MouseDownEvent -= KeyboardMouseHook_MouseDownEvent;

            Position.PropertyChanged -= Position_PropertyChanged;
            Status.PropertyChanged -= Status_PropertyChanged;
            ServiceLocator.GlobalStatusSupervisor.OnNewMessage -= GlobalStatusSupervisor_OnNewMessage;
            base.OnDeactivated();
        }

        public XYZTopZBottomPosition GetPositionOnOpticalReference(OpticalReferenceDefinition opticalRef)
        {
            if (opticalRef == null
                || opticalRef.PositionX == null
                || opticalRef.PositionY == null
                || opticalRef.PositionZ == null
                || opticalRef.PositionZLower == null
                || opticalRef.PositionObjectiveID == null)
            {
                throw new Exception("Impossible to compute the position on optical reference, it's not well defined.");
            }

            // Optical reference are stored in StageReferential
            var position = new XYZTopZBottomPosition(
                new StageReferential(),
                opticalRef.PositionX.Millimeters,
                opticalRef.PositionY.Millimeters,
                opticalRef.PositionZ.Millimeters,
                opticalRef.PositionZLower.Millimeters);

            try
            {
                correctZPosition(opticalRef, position);
                return position;
            }
            catch
            {
                // if position cannot be corrected, go to optical ref with current Z positions
                var currentPos = ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result.ToXYZTopZBottomPosition();
                if (currentPos != null)
                {
                    position.ZTop = currentPos.ZTop;
                    position.ZBottom = currentPos.ZBottom;
                }
                return position;
            }
        }

        private static void correctZPosition(OpticalReferenceDefinition opticalRef, XYZTopZBottomPosition position)
        {
            var calibrationSupervisor = ServiceLocator.CalibrationSupervisor;
            var cameraSupervisor = ServiceLocator.CamerasSupervisor;

            var opticalRefObjectiveConfig = cameraSupervisor.Objectives.Find(_ => _.DeviceID == opticalRef.PositionObjectiveID);
            var opticalRefObjectiveCalibration = calibrationSupervisor.GetObjectiveCalibration(opticalRefObjectiveConfig.DeviceID);

            var currentObjectiveConfig = cameraSupervisor.Objective;
            var objectiveCalibration = calibrationSupervisor.GetObjectiveCalibration(currentObjectiveConfig.DeviceID);
            var currentObjectivePosition = cameraSupervisor.Camera.Configuration.ModulePosition;

            if (currentObjectiveConfig.DeviceID != opticalRefObjectiveConfig.DeviceID)
            {
                if (currentObjectivePosition == ModulePositions.Up)
                {
                    Length previousZOffset = opticalRefObjectiveCalibration.ZOffsetWithMainObjective;
                    position.ZTop += (previousZOffset - objectiveCalibration.ZOffsetWithMainObjective).Millimeters;
                }
                else if (currentObjectivePosition == ModulePositions.Down)
                {
                    Length previousZOffset = opticalRefObjectiveCalibration.ZOffsetWithMainObjective;
                    position.ZBottom += (previousZOffset - objectiveCalibration.ZOffsetWithMainObjective).Millimeters;
                }
            }
        }
    }
}
