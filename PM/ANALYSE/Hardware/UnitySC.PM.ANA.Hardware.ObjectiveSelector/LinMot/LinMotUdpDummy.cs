using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.ObjectiveSelector
{
    public class LinMotUdpDummy : IObjectiveSelector
    {
        private readonly string _ipAddressAxis;
        private readonly Length _positionMin;
        private readonly Length _positionMax;
        private readonly float _speed;
        private readonly float _accel;
        private readonly float _decel;
        private ILogger _logger;

        private ObjectiveConfig _currentObjective;
        private const int TimeoutWaitEndMoveMs = 10000; // ms
        private const int PeriodWaitEndMoveMs = 100; // ms
        private double _currentPos;
        private ObjectiveConfig _targetObjective;
        private bool _moving;
        private Thread _threadMove;
        protected Task HandleTaskWaitMotionEnd;

        #region Properties

        public ObjectivesSelectorConfigBase Config { get; set; }
        public string DeviceID { get; set; }
        public ModulePositions Position { get; set; }

        #endregion Properties

        #region Constructor

        public LinMotUdpDummy(LineMotObjectivesSelectorConfig objectivesSelectorConfig,ILogger logger)
        {
            _logger = logger;
            Config = objectivesSelectorConfig;

            _ipAddressAxis = objectivesSelectorConfig.IpAddressAxis;
            _positionMin = objectivesSelectorConfig.PositionMin;
            _positionMax = objectivesSelectorConfig.PositionMax;
            DeviceID = objectivesSelectorConfig.DeviceID;

            _speed = objectivesSelectorConfig.Speed;
            _accel = objectivesSelectorConfig.Accel;
            _decel = objectivesSelectorConfig.Decel;
        }

        #endregion Constructor

        #region Public methods

        public void Init()
        {
            _logger.Information("Init LinMotUdp as dummy");
            _moving = false;
            _currentObjective = Config.Objectives[0];
            return;
        }

        private void CheckMoveParams(Length wantedPosition, float speed, float accel, float decel)
        {
            // Check axis limits
            if (wantedPosition > _positionMax)
                throw (new Exception("CheckMoveParams - " + wantedPosition.ToString("0.000") + "mm out of axis maximum limit " + _positionMax.Millimeters.ToString("0.000") + "mm"));
            else if (wantedPosition < _positionMin)
                throw (new Exception("CheckMoveParams - " + wantedPosition.ToString("0.000") + "mm out of axis minimum limit " + _positionMin.Millimeters.ToString("0.000") + "mm"));

            if (speed > _speed)
                throw (new Exception("CheckMoveParams - Excessive speed detected: " + speed.ToString("0.000") + "mm.s-1"));

            if (accel > _accel)
                throw (new Exception("CheckMoveParams - Excessive acceleration detected: " + accel.ToString("0.000") + "mm.s-2"));

            if (decel > _decel)
                throw (new Exception("CheckMoveParams - Excessive deceleration detected: " + decel.ToString("0.000") + "mm.s-2"));
        }

        public ObjectiveConfig GetObjectiveInUse()
        {
            //_logger?.Debug(FormatMessage($"GetCurrentObjective - Name: {_currentObjective.Name} Id: {_currentObjective.DeviceID}"));
            return _currentObjective;
        }

        public void SetObjective(ObjectiveConfig wantedObjective)
        {
            try
            {
                _logger?.Information(FormatMessage($"Entering in SetPos with requested objective: {wantedObjective.Name} position ({wantedObjective.Coord.ToString()})"));
                CheckMoveParams(wantedObjective.Coord, _speed, _accel, _decel);

                _targetObjective = wantedObjective;
                StartMoveThread();
            }
            catch (Exception ex)
            {
                _logger?.Error(FormatMessage($" SetPos Failed with objective: {_currentObjective.Name} position value {_currentObjective.Coord.ToString("0.00")} for axis {_ipAddressAxis} - {ex.Message}"));
            }
        }

        #endregion Public methods

        #region Private methods

        private string FormatMessage(string message)
        {
            return $"[{DeviceID}]{message}";
        }

        private void NotifyNewObjectiveInUse(ObjectiveResult newObjectiveInUse)
        {
            var probeServiceCallback = ClassLocator.Default.GetInstance<IProbeServiceCallbackProxy>();
            probeServiceCallback.ProbeNewObjectiveInUseCallback(newObjectiveInUse);
        }

        private void StartMoveThread()
        {
            if (_moving)
                return;
            _moving = true;
            _threadMove = new Thread(MoveTask);
            _threadMove.Name = "ThreadMove";
            _threadMove.Priority = ThreadPriority.BelowNormal;
            _threadMove.Start();

            HandleTaskWaitMotionEnd = new Task(() => { TaskWaitNewObjectiveSet(); }, TaskCreationOptions.LongRunning);
            HandleTaskWaitMotionEnd.Start();
        }

        private void MoveTask()
        {
            int nbMoves = 0;
            double? step;

            try
            {
                step = (_targetObjective.Coord.Millimeters - _currentPos) / 100;

                // if there is no move to do
                if (step.IsNullOrNaN() || (step == 0))
                    return;

                // We do 100 Moves
                while (nbMoves < 100)
                {
                    if ((!step.IsNullOrNaN()) && (step != 0))
                        _currentPos += (double)step;
                    Thread.Sleep(10);
                    nbMoves += 1;
                }

                if (nbMoves == 100)
                {
                    // We ensure that we reach the target
                    if (!((double)_targetObjective.Coord.Millimeters).IsNaN())
                        _currentPos = _targetObjective.Coord.Millimeters;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                _moving = false;
                _currentObjective = _targetObjective;
                _logger?.Information(FormatMessage("Move Terminated"));
            }
        }

        private bool IsTargetReached()
        {
            if (!((double)_targetObjective.Coord.Millimeters).IsNaN())
                if (_currentPos != _targetObjective.Coord.Millimeters)
                    return false;
            return true;
        }

        private void TaskWaitNewObjectiveSet()
        {
            var stopWatch = new Stopwatch();
            var targetReached = false;

            try
            {
                stopWatch.Start();
                while (_moving)
                {
                    Thread.Sleep(PeriodWaitEndMoveMs);
                    if (stopWatch.ElapsedMilliseconds > TimeoutWaitEndMoveMs)
                        throw (new Exception($"Timeout: {stopWatch.ElapsedMilliseconds} ms elapsed > {TimeoutWaitEndMoveMs}"));
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(FormatMessage($" TaskWaitNewObjectiveSet Failed - {ex.Message}"));
                targetReached = false;
                throw;
            }
            finally
            {
                targetReached = IsTargetReached();
                if (!targetReached)
                    _logger?.Error(FormatMessage($" TaskWaitEndMove - Target not reached: {_targetObjective.Coord.ToString("0.00")} for axis {_ipAddressAxis}"));

                NotifyNewObjectiveInUse(new ObjectiveResult(_targetObjective.DeviceID, targetReached));
            }
        }

        public void WaitMotionEnd()
        {
            Thread.Sleep(100);
            return;
        }

        public void Disconnect()
        {
        }

        #endregion Private methods
    }
}
