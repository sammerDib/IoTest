using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Devices.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class DummyAxes : IAxes
    {
        private static string NOT_A_XYZZ_POSITION = $"Received unsupported position which is not a XYZTopZBottomPosition";
        private readonly ILogger _logger;
        private XYZTopZBottomPosition _currentStagePosition;
        private IAxesController _axesController;
        private List<IAxis> _axesList;
        private double? _targetPositionX, _targetPositionY, _targetPositionZTop, _targetPositionZBottom;
        private AxesState _currentState;
        private double _currentSpeed;
        private AxisSpeed _currentAxisSpeed;
        private Thread _threadMove;
        private bool _movementStopped = false;
        protected Task HandleTaskWaitMotionEnd;
        private const int SleepForMoveToStart = 250; // ms
        private const int TimeoutWaitMotionEnd = 10000; // ms

        public event StateChangedEventHandler OnStatusChanged;

        public AxesConfig AxesConfiguration { get; set; }
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public DeviceState State { get; set; }
        public DeviceFamily Family { get; set; }

        #region Fields
        #endregion

        #region Constructors
        public DummyAxes(AxesConfig config)
        {
            AxesConfiguration = config;
            AxesLogger.CreateLogger("Debug");
            AxesLogger.Log.Information("******************************** STAGEDUMMY STARTUP ********************************");
            _logger = AxesLogger.Log;
            _axesList = new List<Axis>();
        }
        #endregion

        #region Properties
        #endregion

        #region Public methods

        public void Init(AxesConfig config, List<Message> initErrors)
        {
            _logger.Information(FormatMessage("Stage Init Called"));

            _currentStagePosition = new XYZTopZBottomPosition(new MotorReferential(), 0, 0, 0, 0);
            _currentState = new AxesState(true, false, false, false);
            AxesConfiguration = config;

            // Get all axes controllers from config
            List<AxesController> axesControllerList = new List<AxesController>();
            foreach (var axesControllerConfig in AxesConfiguration.AxesControllerList)
            {
                axesControllerList.Add(AxesControllerFactory.CreateController(axesControllerConfig));
            }
            if (string.IsNullOrEmpty(AxesConfiguration.AxesControllerToUse))
                initErrors.Add(new Message(MessageLevel.Error, "The axes controller to use must be specified in the configuration", "Axes"));

            // Select controller
            _axesController = Axes.GetControllerFromName(axesControllerList, config.AxesControllerToUse);


            var threadReportState = new Thread(ReportStateTask)
            {
                Name = "ThreadReportState",
                Priority = ThreadPriority.BelowNormal
            };
            threadReportState.Start();
        }
        private void StartMoveThread()
        {
            if (_currentState.OneAxisIsMoving)
                return;
            _movementStopped = false;
            _currentState.OneAxisIsMoving = true;
            _threadMove = new Thread(MoveTask);
            _threadMove.Name = "ThreadMove";
            _threadMove.Priority = ThreadPriority.BelowNormal;
            _threadMove.Start();

            HandleTaskWaitMotionEnd = new Task(() => { TaskWaitMotionEnd(); }, TaskCreationOptions.LongRunning);
            HandleTaskWaitMotionEnd.Start();
        }
        public void MoveIncremental(PositionBase position, AxisSpeed speed)
        {
            if (position is XYZTopZBottomPosition pos)
            {
                double xStep = pos.X;
                double yStep = pos.Y;
                double zTopStep = pos.ZTop;
                double zBottomStep = pos.ZBottom;

                lock (this)
                {
                    _logger.Information(FormatMessage($"MoveIncremental Speed: {speed} xStepMillimeters: {xStep}  yStepMillimeters: {yStep} zTopStepMillimeters: {zTopStep} zBottomStepMillimeters: {zBottomStep}"));

                    _currentStagePosition.X += xStep;
                    _currentStagePosition.Y += yStep;
                    _currentStagePosition.ZTop += zTopStep;
                    _currentStagePosition.ZBottom += zBottomStep;
                }
            }
        }

        public void StopAllMoves()
        {
            //lock (this)
            {
                _movementStopped = true;
                _logger.Information(FormatMessage("Stop all moves"));
            }
        }

        public void WaitMotionEnd(int timeout)
        {
            lock (this)
            {
                bool Success = SpinWait.SpinUntil(() =>
                {
                    return _currentState.OneAxisIsMoving == false;
                }, timeout);

                _logger.Information(FormatMessage("Wait motion end"));
            }
        }
        private void ReportStateTask()
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IStageServiceCallbackProxy>();
            var lastState = new AxesState(true, false, false, false);
            var lastPosition = new XYZTopZBottomPosition(new MotorReferential(), 0, 0, 0, 0);

            while (true)
            {
                // This is just an optimisation not to send all the time the same coordinates
                if (lastPosition != _currentStagePosition)
                {
                    //Console.WriteLine("[DUMMY] Report Stage pos changed x=" + _currentStagePosition.XPosition);
                    axesServiceCallback.PositionChanged(_currentStagePosition);
                    _currentStagePosition = (XYZTopZBottomPosition)lastPosition.Clone();
                }

                // This is just an optimisation not to send all the time the same coordinates
                if (lastState != _currentState)
                {
                    //Console.WriteLine("[DUMMY] Report Stage state changed " + _currentState.OneAxisIsMoving);
                    axesServiceCallback.StateChanged(_currentState);
                    _currentState.CopyTo(lastState);
                }

                Thread.Sleep(20);
            }
        }



        private void MoveTask()
        {
            int nbMoves = 0;

            double? stepX, stepY, stepZTop, stepZBottom;

            try
            {
                stepX = (_targetPositionX - _currentStagePosition.X) / 100;
                stepY = (_targetPositionY - _currentStagePosition.Y) / 100;
                stepZTop = (_targetPositionZTop - _currentStagePosition.ZTop) / 100;
                stepZBottom = (_targetPositionZBottom - _currentStagePosition.ZBottom) / 100;

                // if there is no move to do
                if ((stepX.IsNullOrEmpty() || (stepX == 0)) && ((stepY.IsNullOrEmpty()) || (stepY == 0)) && ((stepZTop.IsNullOrEmpty()) || (stepZTop == 0)) && ((stepZBottom.IsNullOrEmpty()) || (stepZBottom == 0)))
                    return;

                // We do 100 Moves 
                while ((nbMoves < 100) && (!_movementStopped))
                {
                    if ((!stepX.IsNullOrEmpty()) && (stepX != 0))
                        _currentStagePosition.X += (double)stepX;
                    if ((!stepY.IsNullOrEmpty()) && (stepY != 0))
                        _currentStagePosition.Y += (double)stepY;
                    if ((!stepZTop.IsNullOrEmpty()) && (stepZTop != 0))
                        _currentStagePosition.ZTop += (double)stepZTop;
                    if ((!stepZBottom.IsNullOrEmpty()) && (stepZBottom != 0))
                        _currentStagePosition.ZBottomPosition += (double)stepZBottom;
                    Thread.Sleep(GetSleepFromSpeed(_currentSpeed));
                    nbMoves += 1;
                }

                if (nbMoves == 100)
                {
                    // We ensure that we reach the target
                    if (!_targetPositionX.IsNullOrEmpty())
                        _currentStagePosition.X = (double)_targetPositionX;
                    if (!_targetPositionY.IsNullOrEmpty())
                        _currentStagePosition.Y = (double)_targetPositionY;
                    if (!_targetPositionZTop.IsNullOrEmpty())
                        _currentStagePosition.ZTop = (double)_targetPositionZTop;
                    if (!_targetPositionZBottom.IsNullOrEmpty())
                        _currentStagePosition.ZBottom = (double)_targetPositionZBottom;
                }


                // DEBUG test 
                //var stageServiceCallback = ClassLocator.Default.GetInstance<IStageServiceCallbackProxy>();
                //stageServiceCallback.DeviceError(new Message(MessageLevel.Fatal,"Erreur fatale", "FatalError"));


            }
            catch (Exception)
            {
            }
            finally
            {
                _currentState.OneAxisIsMoving = false;
                _logger.Information(FormatMessage("Move Terminated"));
            }
        }

        private bool IsTargetReached()
        {
            if (!_targetPositionX.IsNullOrEmpty())
                if (_currentStagePosition.X != (double)_targetPositionX)
                    return false;
            if (!_targetPositionY.IsNullOrEmpty())
                if (_currentStagePosition.Y != (double)_targetPositionY)
                    return false;

            if (!_targetPositionZTop.IsNullOrEmpty())
                if (_currentStagePosition.ZTop != (double)_targetPositionZTop)
                    return false;

            if (!_targetPositionZBottom.IsNullOrEmpty())
                if (_currentStagePosition.ZBottom != (double)_targetPositionZBottom)
                    return false;

            return true;
        }
        public PositionBase GetPos()
        {
            _logger.Information(FormatMessage($"GetPos xCoord: {_currentStagePosition.X}  yCoord: {_currentStagePosition.Y}"));
            return _currentStagePosition;
        }

        public AxesState GetState()
        {
            _logger.Information(FormatMessage($"GetState enabled: {_currentState.AllAxisEnabled}  OneAxisIsMoving: {_currentState.OneAxisIsMoving}"));
            return _currentState;
        }

        public void StopLanding()
        {
            _currentState.Landed = false;
            _logger.Information(FormatMessage("StopLanding"));
        }
        public void Land()
        {
            _currentState.Landed = true;
            _logger.Information(FormatMessage("Land"));
        }
        public void ClampWafer(List<string> valvesToUse)
        {
            _currentState.WaferClamped = true;
            _logger.Information(FormatMessage("ClampWafer"));
        }

        public void ReleaseWafer(List<string> valvesToUse)
        {
            _currentState.WaferClamped = false;
            _logger.Information(FormatMessage("ReleaseWafer"));
        }

        public void GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom)
        {
            lock (this)
            {
                CheckAxisLimits(_axisX, moveX.Position);
                CheckAxisLimits(_axisY, moveY.Position);
                CheckAxisLimits(_axisZTop, moveZTop.Position);
                CheckAxisLimits(_axisZBottom, moveZBottom.Position);

                _logger.Information(FormatMessage($"MoveToPoint SpeedX: {moveX.Speed}, SpeedY: {moveY.Speed}, SpeedZTop: {moveZTop.Speed},SpeedZBottom:{moveZBottom.Speed}  xCoord: {moveX.Position}  yCoord: {moveY.Position} zTopCoord: {moveZTop.Position} zBottomCoord: {moveZBottom.Position}"));

                if (double.IsNaN(moveX.Position) == false)
                    _targetPositionX = moveX.Position;
                if (double.IsNaN(moveY.Position) == false)
                    _targetPositionY = moveY.Position;
                if (double.IsNaN(moveZTop.Position) == false)
                    _targetPositionZTop = moveZTop.Position;
                if (double.IsNaN(moveZBottom.Position) == false)
                    _targetPositionZBottom = moveZBottom.Position;

                if (double.IsNaN(moveX.Position) && double.IsNaN(moveY.Position) && double.IsNaN(moveZTop.Position) && double.IsNaN(moveZBottom.Position))
                    throw (new Exception("Function GotoPoint called without coordinates"));

                // DEBUG To detect
                //throw (new Exception("Function GotoPoint test failed"));
                _currentSpeed = moveX.Speed;
                StartMoveThread();
            }
        }

        public void GotoManualLoadPos(AxisSpeed speed)
        {
            double xCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.X).PositionManualLoad;
            double yCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.Y).PositionManualLoad;
            double zTopCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.ZTop).PositionManualLoad;
            double zBottomCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.ZBottom).PositionManualLoad;

            _logger.Information(FormatMessage($"GotoManualLoadPos xCoord: {xCoord}  yCoord: {yCoord} zTopCoord: {zTopCoord} zBottomCoord: {zBottomCoord} with speed {speed}"));
            GotoPosition(new XYZTopZBottomPosition(new MotorReferential(), xCoord, yCoord, zTopCoord, zBottomCoord), speed);
        }

        public void GotoParkPos(AxisSpeed speed)
        {
            double xCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.X).PositionPark;
            double yCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.Y).PositionPark;
            double zTopCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.ZTop).PositionPark;
            double zBottomCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.ZBottom).PositionPark;

            _logger.Information(FormatMessage($"GotoParkPos xCoord: {xCoord}  yCoord: {yCoord} zTopCoord: {zTopCoord} zBottomCoord: {zBottomCoord} with speed {speed}"));
            GotoPosition(new XYZTopZBottomPosition(new MotorReferential(), xCoord, yCoord, zTopCoord, zBottomCoord), speed);
        }

        public void GotoHomePos(AxisSpeed speed)
        {
            double xCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.X).PositionHome;
            double yCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.Y).PositionHome;
            double zTopCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.ZTop).PositionHome;
            double zBottomCoord = AxesConfiguration.CurrentStageController.GetAxis(MovingDirection.ZBottom).PositionHome;

            _logger.Information(FormatMessage($"GoToHomePos xCoord: {xCoord}  yCoord: {yCoord} zTopCoord: {zTopCoord} zBottomCoord: {zBottomCoord} with speed {speed}"));
            GotoPosition(new XYZTopZBottomPosition(new MotorReferential(), xCoord, yCoord, zTopCoord, zBottomCoord), speed);

            // DEBUG
            //throw (new Exception("Function GotoHomePos test failed"));
        }
        public void GotoPosition(PositionBase position, AxisSpeed speed)
        {
            if (position is XYZTopZBottomPosition pos)
            {
                double xCoord = pos.X;
                double yCoord = pos.Y;
                double zTopCoord = pos.ZTop;
                double zBottomCoord = pos.ZBottom;

                lock (this)
                {
                    CheckAxisLimits(_axisX, xCoord);
                    CheckAxisLimits(_axisY, yCoord);
                    CheckAxisLimits(_axisZTop, zTopCoord);
                    CheckAxisLimits(_axisZBottom, zBottomCoord);

                    _logger.Information(FormatMessage($"MoveToPoint Speed: {speed}  xCoord: {xCoord}  yCoord: {yCoord} zTopCoord: {zTopCoord} zBottomCoord: {zBottomCoord}"));

                    if (double.IsNaN(xCoord) == false)
                        _targetPositionX = xCoord;
                    if (double.IsNaN(yCoord) == false)
                        _targetPositionY = yCoord;
                    if (double.IsNaN(zTopCoord) == false)
                        _targetPositionZTop = zTopCoord;
                    if (double.IsNaN(zBottomCoord) == false)
                        _targetPositionZBottom = zBottomCoord;

                    if (double.IsNaN(xCoord) && double.IsNaN(yCoord) && double.IsNaN(zTopCoord) && double.IsNaN(zBottomCoord))
                        throw (new Exception("Function GotoPoint called without coordinates"));

                    // DEBUG To detect
                    //throw (new Exception("Function GotoPoint test failed"));
                    _currentAxisSpeed = speed;
                    ConvertAxisSpeedToInt();
                    StartMoveThread();
                }
            }
            else
            {
                _logger.Error(NOT_A_XYZZ_POSITION);
            }
        }

        public void CheckAxisLimits(IAxis axis, double wantedPosition)
        {
            // Limitation des valeurs min et max
            if (wantedPosition > axis.AxisConfiguration.PositionMax)
                throw (new Exception("CheckAxisLimits " + wantedPosition.ToString("0.000") + "mm out of axis maximum limit " + axis.AxisConfiguration.PositionMax.ToString("0.000") + "mm"));
            else if (wantedPosition < axis.AxisConfiguration.PositionMin)
                throw (new Exception("CheckAxisLimits " + wantedPosition.ToString("0.000") + "mm out of axis minimum limit " + axis.AxisConfiguration.PositionMin.ToString("0.000") + "mm"));
        }
        #endregion

        #region Private methods
        
        private void NotifyEndMove(bool targetReached)
        {
            var stageServiceCallback = ClassLocator.Default.GetInstance<IStageServiceCallbackProxy>();
            stageServiceCallback.EndMove(targetReached);
        }

        private void TaskWaitMotionEnd()
        {
            try
            {
                WaitMotionEnd(TimeoutWaitMotionEnd);
            }
            catch (Exception ex)
            {
                string errorMsg = FormatMessage($" TaskWaitMotionEnd Failed - {ex.Message}");
                _logger?.Error(errorMsg);
                throw;
            }
            finally
            {
                var targetReached = IsTargetReached();
                NotifyEndMove(targetReached);
            }
        }

        private void ConvertAxisSpeedToInt()
        {
            switch (_currentAxisSpeed)
            {
                case AxisSpeed.Slow:
                    _currentSpeed = 50;
                    break;
                case AxisSpeed.Normal:
                    _currentSpeed = 150;
                    break;
                case AxisSpeed.Fast:
                    _currentSpeed = 200;
                    break;
                case AxisSpeed.Measure:
                    _currentSpeed = 250;
                    break;
                default:
                    _currentSpeed = 250;
                    break;
            }


        }
        private int GetSleepFromSpeed(double speed)
        {

            if (speed <= 100)
                return 500;
            else if (speed > 100 && speed <= 150)
                return 50;
            else if (speed > 150)
                return 20;
            else
                return 1000;

        }
        private string FormatMessage(string message)
        {
            return $"[Stage Dummy]{message}";
        }

        public void GotoToPointX(double xCoord)
        {
            throw new NotImplementedException();
        }

        public void SendCommandToStage(string commandToApply)
        {
            throw new NotImplementedException();
        }
        public List<string> GetReceivedMessages()
        {
            throw new NotImplementedException();
        }

        public void DisconnectAxes()
        {
            throw new NotImplementedException();
        }

        public int GetAxesPosition()
        {
            throw new NotImplementedException();
        }

        public int GetChuckPosition()
        {
            throw new NotImplementedException();
        }

        public void Init(List<Message> initErrors)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Static methods
        #endregion
    }
}
