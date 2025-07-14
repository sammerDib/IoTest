using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Enum;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class DummyAxes : DeviceBase, IAxes
    {
        public override DeviceFamily Family => DeviceFamily.Axes;
        private IGlobalStatusServer _globalStatusServiceCallback;
        private AnaPosition _currentAxesPosition;
        private AnaPosition _targetAxesPosition;
        private AxesState _currentState;
        private double _axisSpeed;
        private Thread _threadMove;
        private bool _movementStopped = false;
        private IReferentialManager _referentialManager;
        protected Task HandleTaskWaitMotionEnd;
        private const int TimeoutWaitMotionEnd = 10000; // ms
        private const double PositionEpsilon_mm = 0.200; //200 µm
        private AxesConfig _axesConfiguration;
        public AxesConfig AxesConfiguration { get => _axesConfiguration; set => _axesConfiguration = value; }

        private List<IAxis> _axesList = new List<IAxis>();
        public List<IAxis> Axes => _axesList;

        public List<IAxesController> AxesControllers { get; set; }
        private IMessenger _messenger;

        #region Constructors

        public DummyAxes(AxesConfig config, IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager)
            : base(globalStatusServer, logger)
        {
            base.Logger.Information("******************************** AXES DUMMY STARTUP ********************************");
            _axesConfiguration = config;
            Name = config.Name;
            DeviceID = config.DeviceID;
            _globalStatusServiceCallback = globalStatusServer;
            _referentialManager = referentialManager;
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            // The logger level must be retrieved from the configuration
            try
            {
                // Create all axes from config
                foreach (var axisConfig in config.AxisConfigs)
                {
                    base.Logger.Information($"Create axis '{axisConfig.Name}' with AxisID='{axisConfig.AxisID}' and ControllerID='{axisConfig.ControllerID}'.");

                    // Create axis
                    var newAxis = AxisFactory.CreateAxis(axisConfig, logger);

                    if (_axesList.Exists(axe => axe.AxisID == newAxis.AxisID))
                    {
                        throw new Exception($"Axis with ID {newAxis.AxisID} already exists. Double Axis ID found in configuration");
                    }

                    // Add to axesList
                    _axesList.Add(newAxis);
                }
            }
            catch (Exception ex)
            {
                string msgErr = "Axes creation failed. Check configuration !!!";
                base.Logger.Information(msgErr);
                throw new Exception(msgErr, ex);
            }
        }

        #endregion Constructors

        #region Public methods

        public void Init(List<Message> initErrors)
        {
            Logger.Information("Init Axes as dummy");
            Logger.Information("Stage Init Called");

            InitializePositions();

            _currentState = new AxesState(true, false, false);

            var threadReportState = new Thread(ReportStateTask)
            {
                Name = "ThreadReportState",
                Priority = ThreadPriority.BelowNormal
            };
            threadReportState.Start();
        }

        private void InitializePositions()
        {
            _currentAxesPosition = new AnaPosition(new MotorReferential())
            {
                X = 0,
                Y = 0,
                ZTop = 0,
                ZBottom = 0,
            };

            foreach (var piezoAxis in _axesList.Where(axis => axis.AxisConfiguration.MovingDirection == MovingDirection.ZPiezo))
            {
                var piezoPosition = new ZPiezoPosition(new MotorReferential(), piezoAxis.AxisID, 0.Micrometers());
                _currentAxesPosition.ZPiezoPositions.Add(piezoPosition);
            }

            _targetAxesPosition = (AnaPosition)_currentAxesPosition.Clone();
        }

        private void StartMoveThread()
        {
            if (_currentState.OneAxisIsMoving)
            {
                return;
            }
            _movementStopped = false;
            _currentState.OneAxisIsMoving = true;
            _threadMove = new Thread(MoveTask);
            _threadMove.Name = "ThreadMove";
            _threadMove.Priority = ThreadPriority.BelowNormal;
            _threadMove.Start();

            HandleTaskWaitMotionEnd = new Task(() => { TaskWaitMotionEnd(); }, TaskCreationOptions.LongRunning);
            HandleTaskWaitMotionEnd.Start();
        }

        public void MoveIncremental(IncrementalMoveBase movement, AxisSpeed speed)
        {
            if (movement is XYZTopZBottomMove move)
            {
                double xStep = move.X;
                double yStep = move.Y;
                double zTopStep = move.ZTop;
                double zBottomStep = move.ZBottom;
                Logger.Information($"MoveIncremental Speed: {speed} xStepMillimeters: {xStep}  yStepMillimeters: {yStep} zTopStepMillimeters: {zTopStep} zBottomStepMillimeters: {zBottomStep}");

                _targetAxesPosition.X = _currentAxesPosition.X + xStep;
                _targetAxesPosition.Y = _currentAxesPosition.Y + yStep;
                _targetAxesPosition.ZTop = _currentAxesPosition.ZTop + zTopStep;
                _targetAxesPosition.ZBottom = _currentAxesPosition.ZBottom + zBottomStep;

                CheckAxisLimits();

                StartMoveThread();
            }
        }

        private void CheckAxisLimits()
        {
            foreach (var axis in _axesList)
            {
                switch (axis.AxisID)
                {
                    case "X":
                        if (_targetAxesPosition.X > axis.AxisConfiguration.PositionMax.Millimeters + PositionEpsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {_targetAxesPosition.X:0.00} mm out of axis maximum limit {axis.AxisConfiguration.PositionMax.Millimeters:0.000} mm");
                            _targetAxesPosition.X = axis.AxisConfiguration.PositionMax.Millimeters;
                        }
                        if (_targetAxesPosition.X < axis.AxisConfiguration.PositionMin.Millimeters - PositionEpsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {_targetAxesPosition.X:0.00} mm out of axis minimum limit {axis.AxisConfiguration.PositionMin.Millimeters:0.000} mm");
                            _targetAxesPosition.X = axis.AxisConfiguration.PositionMin.Millimeters;
                        }
                        break;

                    case "Y":
                        if (_targetAxesPosition.Y > axis.AxisConfiguration.PositionMax.Millimeters + PositionEpsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {_targetAxesPosition.Y:0.00} mm out of axis maximum limit {axis.AxisConfiguration.PositionMax.Millimeters:0.000} mm");
                            _targetAxesPosition.Y = axis.AxisConfiguration.PositionMax.Millimeters;
                        }
                        if (_targetAxesPosition.Y < axis.AxisConfiguration.PositionMin.Millimeters - PositionEpsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {_targetAxesPosition.Y:0.00} mm out of axis minimum limit {axis.AxisConfiguration.PositionMin.Millimeters:0.000} mm");
                            _targetAxesPosition.Y = axis.AxisConfiguration.PositionMin.Millimeters;
                        }
                        break;

                    case "ZTop":
                        if (_targetAxesPosition.ZTop > axis.AxisConfiguration.PositionMax.Millimeters + PositionEpsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {_targetAxesPosition.ZTop:0.00} mm out of axis maximum limit {axis.AxisConfiguration.PositionMax.Millimeters:0.000} mm");
                            _targetAxesPosition.ZTop = axis.AxisConfiguration.PositionMax.Millimeters;
                        }
                        if (_targetAxesPosition.ZTop < axis.AxisConfiguration.PositionMin.Millimeters - PositionEpsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {_targetAxesPosition.ZTop:0.00} mm out of axis minimum limit {axis.AxisConfiguration.PositionMin.Millimeters:0.000} mm");
                            _targetAxesPosition.ZTop = axis.AxisConfiguration.PositionMin.Millimeters;
                        }
                        break;

                    case "ZBottom":
                        if (_targetAxesPosition.ZBottom > axis.AxisConfiguration.PositionMax.Millimeters + PositionEpsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {_targetAxesPosition.ZBottom:0.00} mm out of axis maximum limit {axis.AxisConfiguration.PositionMax.Millimeters:0.000} mm");
                            _targetAxesPosition.ZBottom = axis.AxisConfiguration.PositionMax.Millimeters;
                        }
                        if (_targetAxesPosition.ZBottom < axis.AxisConfiguration.PositionMin.Millimeters - PositionEpsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {_targetAxesPosition.ZBottom:0.00} mm out of axis minimum limit {axis.AxisConfiguration.PositionMin.Millimeters:0.000} mm");
                            _targetAxesPosition.ZBottom = axis.AxisConfiguration.PositionMin.Millimeters;
                        }
                        break;
                }
            }
        }

        public void StopAllMoves()
        {
            _movementStopped = true;
            Logger.Information("Stop all moves");
        }

        public void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            lock (this)
            {
                bool Success = SpinWait.SpinUntil(() =>
                {
                    return _currentState.OneAxisIsMoving == false;
                }, timeout);

                Logger.Information("Wait motion end");
            }
        }

        private void ReportStateTask()
        {
            var stageServiceCallback = ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            var lastState = new AxesState(true, false, false);
            var lastPosition = new AnaPosition(new MotorReferential(), 0, 0, 0, 0, new List<ZPiezoPosition>());

            while (true)
            {
                // This is just an optimisation not to send all the time the same coordinates
                if (lastPosition != _currentAxesPosition)
                {
                    stageServiceCallback.PositionChanged(_currentAxesPosition);
                    _messenger.Send<AxesPositionChangedMessage>(new AxesPositionChangedMessage() { Position = _currentAxesPosition });
                    lastPosition = (AnaPosition)_currentAxesPosition.Clone();
                }

                // This is just an optimisation not to send all the time the same coordinates
                if (lastState != _currentState)
                {
                    stageServiceCallback.StateChanged(_currentState);
                    _currentState.CopyTo(lastState);
                }

                Thread.Sleep(20);
            }
        }
        
        private void MoveTask()
        {
            int currentMoveNumber = 1;
            double diffX = _targetAxesPosition.X - _currentAxesPosition.X;
            double diffY = _targetAxesPosition.Y - _currentAxesPosition.Y;

            double distance = Math.Sqrt(diffX * diffX + diffY * diffY);
            
            double step = _axisSpeed / 20;

            int nbMoves = (int)Math.Floor(distance / step);

            try
            {
                double stepX = (_targetAxesPosition.X - _currentAxesPosition.X) / nbMoves;
                double stepY = (_targetAxesPosition.Y - _currentAxesPosition.Y) / nbMoves;
                double stepZTop = (_targetAxesPosition.ZTop - _currentAxesPosition.ZTop) / nbMoves;
                double stepZBottom = (_targetAxesPosition.ZBottom - _currentAxesPosition.ZBottom) / nbMoves;
                var stepsZPiezo = new Dictionary<string, double>(); // <axisID, stepZPiezo>

                foreach (var targetPiezoPosition in _targetAxesPosition.ZPiezoPositions)
                {
                    string axisID = targetPiezoPosition.AxisID;
                    double currentPiezoPosition = _currentAxesPosition.GetPiezoPosition(axisID).Position;
                    double stepZPiezo = (targetPiezoPosition.Position - currentPiezoPosition) / nbMoves;
                    stepsZPiezo.Add(targetPiezoPosition.AxisID, stepZPiezo);
                }

                // Early return if there is no move to do
                if (IsTargetReached())
                {
                    Logger.Information("Already on target.");
                    return;
                }

                // Moving loop: increment _currentAxesPosition based on the step values and converge to the _targetAxesPosition
                while ((currentMoveNumber <= nbMoves) && (!_movementStopped))
                {
                    if (stepX != 0)
                    {
                        _currentAxesPosition.X += stepX;
                    }

                    if (stepY != 0)
                    {
                        _currentAxesPosition.Y += stepY;
                    }

                    if (stepZTop != 0)
                    {
                        _currentAxesPosition.ZTop += stepZTop;
                    }

                    if (stepZBottom != 0)
                    {
                        _currentAxesPosition.ZBottom += stepZBottom;
                    }

                    foreach (var stepZPiezo in stepsZPiezo)
                    {
                        if (stepZPiezo.Value != 0)
                        {
                            var referential = _currentAxesPosition.Referential;
                            string axisID = stepZPiezo.Key;
                            double updatedPosition = _currentAxesPosition.GetPiezoPosition(axisID).Position + stepZPiezo.Value;

                            _currentAxesPosition.AddOrUpdateZPiezoPosition(new ZPiezoPosition(referential, axisID, updatedPosition.Micrometers()));
                        }
                    }

                    Thread.Sleep(50);
                    currentMoveNumber++;
                }
                
                _currentAxesPosition = (AnaPosition)_targetAxesPosition.Clone();

                if (!_movementStopped)
                {

                    var axis = Axes.FirstOrDefault<IAxis>(a => a.AxisID == "X");
                    if (axis != null)
                    {
                        axis.CurrentPos = _currentAxesPosition.X.Millimeters();
                    }

                    axis = Axes.FirstOrDefault<IAxis>(a => a.AxisID == "Y");
                    if (axis != null)
                    {
                        axis.CurrentPos = _currentAxesPosition.Y.Millimeters();
                    }

                    axis = Axes.FirstOrDefault<IAxis>(a => a.AxisID == "ZTop");
                    if (axis != null)
                    {
                        axis.CurrentPos = _currentAxesPosition.ZTop.Millimeters();
                    }

                    axis = Axes.FirstOrDefault<IAxis>(a => a.AxisID == "ZBottom");
                    if (axis != null)
                    {
                        axis.CurrentPos = _currentAxesPosition.ZBottom.Millimeters();
                    }

                    foreach (var ZPiezoaxis in _currentAxesPosition.ZPiezoPositions)
                    {
                        axis = Axes.FirstOrDefault<IAxis>(a => a.AxisID == ZPiezoaxis.AxisID);
                        if (axis != null)
                        {
                            axis.CurrentPos = ZPiezoaxis.PiezoPosition;
                        }
                    }
                }

                Logger.Information($"Target position reached: {_currentAxesPosition}");
            }
            catch (Exception)
            {
            }
            finally
            {
                _currentState.OneAxisIsMoving = false;
                Logger.Information("Move terminated.");
            }
        }

        protected List<IAxis> GetAllAxes_ParkPos()
        {
            return _axesList.Where(axis => !double.IsNaN(axis.AxisConfiguration.PositionPark.Millimeters)).ToList();
        }

        protected List<IAxis> GetAllAxes_ManualLoad()
        {
            return _axesList.Where(axis => !double.IsNaN(axis.AxisConfiguration.PositionManualLoad.Millimeters)).ToList();
        }

        protected List<IAxis> GetAllAxes_HomePos()
        {
            return _axesList.Where(axis => !double.IsNaN(axis.AxisConfiguration.PositionHome.Millimeters)).ToList();
        }

        private IAxis GetAxisFromID(string axisID)
        {
            var axis = _axesList.FirstOrDefault(a => a.AxisID == axisID);
            if (axis is null)
            {
                Logger.Error($"Axis with ID {axisID} not found.");
            }

            return axis;
        }

        private bool IsTargetReached()
        {
            return _currentAxesPosition == _targetAxesPosition;
        }

        public PositionBase GetPos()
        {
            Logger.Verbose($"GetPos called: {_currentAxesPosition}");
            return _currentAxesPosition.Clone() as PositionBase;
        }

        public TimestampedPosition GetAxisPosWithTimestamp(IAxis axis)
        {
            Logger.Verbose($"GetAxisPosWithTimestamp called: {_currentAxesPosition}");
            var position = 0;
            var time = DateTime.UtcNow;

            return new TimestampedPosition(position.Millimeters(), time);
        }

        public AxesState GetState()
        {
            return _currentState;
        }

        public void StopLanding()
        {
            _currentState.Landed = false;
            Logger.Information("StopLanding");
        }

        public bool IsAtPosition(PositionBase position)
        {           
            return CheckIfPositionReached(position);
        }

        public bool CheckIfPositionReached(PositionBase position)
        {
            var curentPosition = GetPos();
            var tolerance = 0.01.Millimeters();
            if (!position.Near(curentPosition, tolerance))
            {
                Logger.Verbose($"Position check failed. Expected position: {position.ToString()}, Current position: {curentPosition.ToString()}, Tolerance: {tolerance}");
                return false;
            }
            else
            {
                Logger.Debug($"Position check succeeded. Position {position.ToString()} is within tolerance: {tolerance} from {curentPosition.ToString()}");
            }
            return true;
        }

        public void Land()
        {
            _currentState.Landed = true;
            Logger.Information("Land");
        }

        public void GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom)
        {
            Logger.Information("GotoPointCustomSpeedAccel called");
            lock (this)
            {
                foreach (var axis in _axesList)
                {
                    switch (axis.AxisID)
                    {
                        case "X":
                            {
                                CheckAxisLimits(axis, moveX?.Position);
                                break;
                            }
                        case "Y":
                            {
                                CheckAxisLimits(axis, moveY?.Position);
                                break;
                            }
                        case "ZTop":
                            {
                                CheckAxisLimits(axis, moveZTop?.Position);
                                break;
                            }
                        case "ZBottom":
                            {
                                CheckAxisLimits(axis, moveZBottom?.Position);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }

                Logger.Information($"MoveToPoint SpeedX: {moveX?.Speed}, SpeedY: {moveY?.Speed}, SpeedZTop: {moveZTop?.Speed},SpeedZBottom:{moveZBottom?.Speed}  xCoord: {moveX?.Position}  yCoord: {moveY?.Position} zTopCoord: {moveZTop?.Position} zBottomCoord: {moveZBottom?.Position}");

                if (moveX?.Position != null && double.IsNaN(moveX.Position) == false)
                {
                    _targetAxesPosition.X = moveX.Position;
                }

                if (moveY?.Position != null && double.IsNaN(moveY.Position) == false)
                {
                    _targetAxesPosition.Y = moveY.Position;
                }

                if (moveZTop?.Position != null && double.IsNaN(moveZTop.Position) == false)
                {
                    _targetAxesPosition.ZTop = moveZTop.Position;
                }

                if (moveZBottom?.Position != null && double.IsNaN(moveZBottom.Position) == false)
                {
                    _targetAxesPosition.ZBottom = moveZBottom.Position;
                }

                if ((moveX?.Position is null || double.IsNaN(moveX.Position))
                    && (moveY?.Position is null || double.IsNaN(moveY.Position))
                    && (moveZTop?.Position is null || double.IsNaN(moveZTop.Position))
                    && (moveZBottom?.Position is null || double.IsNaN(moveZBottom.Position)))
                {
                    throw (new Exception("Function GotoPoint called without coordinates"));
                }

                _axisSpeed = moveX?.Speed ?? 200;
                StartMoveThread();
            }
        }

        private PositionBase GetPositionFromConfiguration(PredefinedPosition positionExpected)
        {
            // TODO DATAFLOW : Je n'ai compris les modifs dans ce fichier (FDS).
            double xCoord = double.NaN;
            double yCoord = double.NaN;
            double zTopCoord = double.NaN;
            double zBottomCoord = double.NaN;
            List<ZPiezoPosition> zPiezoList = new List<ZPiezoPosition>();

            List<IAxis> axesList_ParkPos;
            switch (positionExpected)
            {
                case PredefinedPosition.Park:
                    axesList_ParkPos = GetAllAxes_ParkPos();
                    break;

                case PredefinedPosition.ManualLoading:
                    axesList_ParkPos = GetAllAxes_ManualLoad();
                    break;

                case PredefinedPosition.Home:
                    axesList_ParkPos = GetAllAxes_HomePos();
                    break;

                default:
                    throw new Exception("Predefined position unknown");
            }
            foreach (var axis in axesList_ParkPos)
            {
                if (axis.AxisConfiguration.MovingDirection == MovingDirection.X)
                {
                    xCoord = GetPositionValueFromConfig(positionExpected, axis.AxisConfiguration);
                }

                if (axis.AxisConfiguration.MovingDirection == MovingDirection.Y)
                {
                    yCoord = GetPositionValueFromConfig(positionExpected, axis.AxisConfiguration);
                }

                if (axis.AxisConfiguration.MovingDirection == MovingDirection.ZTop)
                {
                    zTopCoord = GetPositionValueFromConfig(positionExpected, axis.AxisConfiguration);
                }

                if (axis.AxisConfiguration.MovingDirection == MovingDirection.ZBottom)
                {
                    zBottomCoord = GetPositionValueFromConfig(positionExpected, axis.AxisConfiguration);
                }

                if (axis.AxisConfiguration.MovingDirection == MovingDirection.ZPiezo)
                {
                    double zPiezoCoord = GetPositionValueFromConfig(positionExpected, axis.AxisConfiguration);
                    ZPiezoPosition newZPiezo = new ZPiezoPosition(new MotorReferential(), axis.AxisID, new Length(zPiezoCoord, LengthUnit.Micrometer));
                    newZPiezo.Position = newZPiezo.PiezoPosition.Micrometers;
                    zPiezoList.Add(newZPiezo);
                }
            }
            if (double.IsNaN(xCoord))
            {
                throw new Exception("Axis x is unknow in configuration");
            }
            else 
            if (double.IsNaN(yCoord))
            {
                return new XPosition(new MotorReferential(), xCoord);
            }
            else 
            if (double.IsNaN(zTopCoord))
            {
                return new XYPosition(new MotorReferential(), xCoord, yCoord);
            }
            else 
            if (zPiezoList.Count == 0)
            {
                return new XYZTopZBottomPosition(new MotorReferential(), xCoord, yCoord, zTopCoord, zBottomCoord);
            }
            else
            {
                return new AnaPosition(new MotorReferential(), xCoord, yCoord, zTopCoord, zBottomCoord, zPiezoList);
            }
        }

        private static double GetPositionValueFromConfig(PredefinedPosition positionExpected, AxisConfig config)
        {
            double coord = double.NaN;
            if (config != null)
            {
                switch (positionExpected)
                {
                    case PredefinedPosition.Park:
                        coord = config.PositionPark.Millimeters;
                        break;

                    case PredefinedPosition.ManualLoading:
                        coord = config.PositionManualLoad.Millimeters;
                        break;

                    case PredefinedPosition.Home:
                        coord = config.PositionHome.Millimeters;
                        break;

                    default:
                        throw new Exception("Unknown predefined position in GetPositionValueFromConfig()");
                }
            }

            return coord;
        }

        public void GotoHomePos(AxisSpeed speed)
        {
            PositionBase position = GetPositionFromConfiguration(PredefinedPosition.Home);
            Logger.Information($"GotoHomePos Position: {position}  with speed {speed}");
            GotoPosition(position, speed);
        }

        // FIXME: positionBase content is not used
        public void GotoPosition(PositionBase position, AxisSpeed speed)
        {
            Logger.Information("GotoPosition called");
            XYZTopZBottomPosition xyzPosition;

            switch (position)
            {
                case XYZTopZBottomPosition xyZTopZBottomPosition:
                    {
                        xyzPosition = xyZTopZBottomPosition;
                        break;
                    }
                case XYPosition xyPosition:
                    {
                        var currPosition = ConvertTo(GetPos(), xyPosition.Referential.Tag) as XYZTopZBottomPosition;
                        xyzPosition = new XYZTopZBottomPosition(
                            xyPosition.Referential,
                            xyPosition.X,
                            xyPosition.Y,
                            currPosition.ZTop,
                            currPosition.ZBottom);
                        break;
                    }
                case ZPiezoPosition zPiezoPosition:
                    {
                        var currPosition = ConvertTo(GetPos(), zPiezoPosition.Referential.Tag) as XYZTopZBottomPosition;
                        xyzPosition = new XYZTopZBottomPosition(
                            zPiezoPosition.Referential,
                            currPosition.X,
                            currPosition.Y,
                            currPosition.ZTop,
                            currPosition.ZBottom);
                        break;
                    }
                default:
                    throw new ArgumentException("Unhandled position of type '" + position.GetType().Name +
                                                "'. Only XYPosition, XYZTopZBottomPosition and ZPizezoPosition are supported");
            }

            var targetPosition = ConvertTo(xyzPosition, ReferentialTag.Motor);
            var currentPosition = GetPos() as XYZTopZBottomPosition;
            var currentAnaPosition = new AnaPosition(new MotorReferential(), currentPosition.X, currentPosition.Y, currentPosition.ZTop, currentPosition.ZBottom, ((AnaPosition)GetPos()).ZPiezoPositions);
            switch (targetPosition)
            {
                case AnaPosition anaTargetPosition:
                    currentAnaPosition.Merge(anaTargetPosition);
                    break;

                case XYZTopZBottomPosition xyZTopZBottomTargetPosition:
                    currentAnaPosition.Merge(xyZTopZBottomTargetPosition);
                    break;

                case XYPosition xyTargetPosition:
                    currentAnaPosition.Merge(xyTargetPosition);
                    break;

                case OneAxisPosition oneAxisTargetPosition:
                    var axis = GetAxisFromID(oneAxisTargetPosition.AxisID);
                    var targetPositionBaseTmp = new AnaPosition(oneAxisTargetPosition.Referential) { X = double.NaN, Y = double.NaN, ZTop = double.NaN, ZBottom = double.NaN };
                    switch (axis.AxisConfiguration.MovingDirection)
                    {
                        case MovingDirection.X:
                            targetPositionBaseTmp.X = oneAxisTargetPosition.Position;
                            break;
                        case MovingDirection.Y:
                            targetPositionBaseTmp.Y = oneAxisTargetPosition.Position;
                            break;
                        case MovingDirection.ZTop:
                            targetPositionBaseTmp.ZTop = oneAxisTargetPosition.Position;
                            break;
                        case MovingDirection.ZBottom:
                            targetPositionBaseTmp.ZBottom = oneAxisTargetPosition.Position;
                            break;
                        case MovingDirection.ZPiezo:
                            targetPositionBaseTmp.AddOrUpdateZPiezoPosition((ZPiezoPosition)oneAxisTargetPosition);
                            break;
                        default:
                            throw new Exception(
                                $"Unknown axis moving direction {axis.AxisConfiguration.MovingDirection}");
                    }
                    currentAnaPosition.Merge(targetPositionBaseTmp);
                    break;

                default:
                    throw new Exception("Position not supported.");
            }

            CheckPositionIsWithinAxesBounds(currentAnaPosition);
            _targetAxesPosition = (AnaPosition)currentAnaPosition.Clone();
            _axisSpeed = GetAxisSpeedValueFromEnum(speed);

            StartMoveThread();
        }

        private void CheckPositionIsWithinAxesBounds(AnaPosition anaPosition)
        {
            // X, Y, ZTop and ZBottom axes
            foreach (var axis in _axesList.Where(axis => axis.AxisConfiguration.MovingDirection != MovingDirection.ZPiezo))
            {
                switch (axis.AxisConfiguration.MovingDirection)
                {
                    case MovingDirection.X:
                        CheckAxisLimits(axis, anaPosition.X);
                        break;
                    case MovingDirection.Y:
                        CheckAxisLimits(axis, anaPosition.Y);
                        break;
                    case MovingDirection.ZTop:
                        CheckAxisLimits(axis, anaPosition.ZTop);
                        break;
                    case MovingDirection.ZBottom:
                        CheckAxisLimits(axis, anaPosition.ZBottom);
                        break;
                    default: break;
                }
            }

            // Piezo axes
            foreach (var piezoAxis in _axesList.Where(axis => axis.AxisConfiguration.MovingDirection == MovingDirection.ZPiezo))
            {
                var piezoPosition = anaPosition.ZPiezoPositions.FirstOrDefault(p => p.AxisID == piezoAxis.AxisID);
                if (piezoPosition is null)
                {
                    throw new Exception("Piezo position not found.");
                }

                CheckAxisLimits(piezoAxis, piezoPosition.PiezoPosition.Micrometers);
            }
        }

        public void CheckAxisLimits(IAxis axis, double? wantedPosition)
        {
            if (wantedPosition == null || !wantedPosition.HasValue)
            {
                return;
            }

            double position = wantedPosition.Value;

            double limitMax;
            double limitMin;
            string unit = String.Empty;
            // Limitation des valeurs min et max
            switch (axis.AxisConfiguration)
            {
                case PiezoAxisConfig piezoAxisConfig:
                    limitMax = piezoAxisConfig.PositionMax.Micrometers;
                    limitMin = piezoAxisConfig.PositionMin.Micrometers;
                    unit = " um";
                    break;

                default:
                    limitMax = axis.AxisConfiguration.PositionMax.Millimeters;
                    limitMin = axis.AxisConfiguration.PositionMin.Millimeters;
                    unit = " mm";
                    break;
            }
            if (wantedPosition > limitMax)
            {
                throw (new Exception("CheckAxisLimits " + position.ToString("0.000") + unit + " out of axis maximum limit " + limitMax.ToString("0.000") + unit));
            }

            if (wantedPosition < limitMin)
            {
                throw (new Exception("CheckAxisLimits " + position.ToString("0.000") + unit + " out of axis minimum limit " + limitMin.ToString("0.000") + unit));
            }
        }

        protected PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            return _referentialManager.ConvertTo(positionToConvert, referentialTo);
        }

        #endregion Public methods

        #region Private methods

        private void NotifyEndMove(bool targetReached)
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            axesServiceCallback.EndMove(targetReached);
        }

        private void TaskWaitMotionEnd()
        {
            try
            {
                WaitMotionEnd(TimeoutWaitMotionEnd);
            }
            catch (Exception ex)
            {
                Logger?.Error(ex,"TaskWaitMotionEnd Failed");
                throw;
            }
            finally
            {
                bool targetReached = IsTargetReached();
                NotifyEndMove(targetReached);
            }
        }

        private double GetAxisSpeedValueFromEnum(AxisSpeed axisSpeed)
        {
            switch (axisSpeed)
            {
                case AxisSpeed.Slow: return 50;
                case AxisSpeed.Normal: return 150;
                case AxisSpeed.Fast: return 200;
                case AxisSpeed.Measure:
                default: return 250;
            }
        }


        public void GotoToPointX(double xCoord)
        {
            throw new NotImplementedException();
        }

        public void LinearMotion(PositionBase position, AxisSpeed speed)
        {
            Logger.Information("LinearMotion called");
            if (position is XYZTopZBottomPosition pos)
            {
                double xCoord = pos.X;
                double yCoord = pos.Y;
                double zTopCoord = pos.ZTop;
                double zBottomCoord = pos.ZBottom;

                lock (this)
                {
                    foreach (var axis in _axesList)
                    {
                        switch (axis.AxisID)
                        {
                            case "X":
                                CheckAxisLimits(axis, xCoord);
                                break;
                            case "Y":
                                CheckAxisLimits(axis, yCoord);
                                break;
                            case "ZTop":
                                CheckAxisLimits(axis, zTopCoord);
                                break;
                            case "ZBottom":
                                CheckAxisLimits(axis, zBottomCoord);
                                break;
                            default:
                                break;
                        }
                    }

                    Logger.Information($"LinearMotion Speed: {speed}  xCoord: {xCoord}  yCoord: {yCoord} zTopCoord: {zTopCoord} zBottomCoord: {zBottomCoord}");

                    if (double.IsNaN(xCoord) == false)
                    {
                        _targetAxesPosition.X = xCoord;
                    }

                    if (double.IsNaN(yCoord) == false)
                    {
                        _targetAxesPosition.Y = yCoord;
                    }

                    if (double.IsNaN(zTopCoord) == false)
                    {
                        _targetAxesPosition.ZTop = zTopCoord;
                    }

                    if (double.IsNaN(zBottomCoord) == false)
                    {
                        _targetAxesPosition.ZBottom = zBottomCoord;
                    }

                    if (double.IsNaN(xCoord) && double.IsNaN(yCoord) && double.IsNaN(zTopCoord) && double.IsNaN(zBottomCoord))
                    {
                        throw (new Exception("Function LinearMotion called without coordinates"));
                    }

                    _axisSpeed = GetAxisSpeedValueFromEnum(speed);
                    StartMoveThread();
                }
            }
            else
            {
                Logger.Error("Received unsupported position which is not a XYZTopZBottomPosition");
            }
        }

        public void Move(params PMAxisMove[] moves)
        {
            throw new NotImplementedException();
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            throw new NotImplementedException();
        }

        public void Home(AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public void StopAllMotion()
        {
            throw new NotImplementedException();
        }

        public PositionBase GetPosition()
        {
            throw new NotImplementedException();
        }

        public void ResetAxis()
        {
            Thread.Sleep(10000);
            Logger.Information("Reset axis is finished");
        }

        public void InitZTopFocus()
        {
            Logger.Information("Init ZTop started");
            Thread.Sleep(50);
            Logger.Information("Init ZTop finished");
        }

        public void InitZBottomFocus()
        {
            Logger.Information("Init ZBottom started");
            Thread.Sleep(50);
            Logger.Information("Init ZBottom finished");
        }

        public void AcknowledgeResetAxis()
        {
            Thread.Sleep(500);
        }

        #endregion Private methods
    }
}
