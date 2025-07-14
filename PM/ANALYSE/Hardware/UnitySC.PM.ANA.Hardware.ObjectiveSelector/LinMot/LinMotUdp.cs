using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.ObjectiveSelector
{
    public class LinMotUdp : IObjectiveSelector
    {
        #region Fields

        private bool _initialized = false;
        private Task _pollTask;
        private CancellationTokenSource _cancelationToken;
        private readonly LinUDP.ACI _communicationApi;
        private readonly string _ipAddressController;
        private readonly string _ipAddressAxis;
        private readonly string _portNumberAxis;
        private readonly Length _positionMin;
        private readonly Length _positionMax;
        private readonly float _speed; // m.s-1
        private readonly float _accel; // m.s-2
        private readonly float _decel; // m.s-2
        private const float SecurityThresholdSpeed = 4F; // m.s-1
        private const float SecurityThresholdAccel = 0.5F; // m.s-2
        private const float SecurityThresholdDeccel = 0.5F; // m.s-2
        private const int TimeoutMs = 1000; // ms
        private const int TimeoutWaitEndMoveMs = 4000; // ms
        private const int PeriodWaitEndMoveMs = 100; // ms
        private const int PeriodWaitMotionEndedMs = 50; // ms
        private const int PollingPeriodMs = 1000; // ms
        private bool _isConnectionActive;
        private ILogger _logger;
        private ObjectiveConfig _currentObjective;
        protected Task HandleTaskWaitEndMove;

        #endregion Fields

        #region Properties

        public event ErrorDelegate ErrorEvent;

        public ObjectivesSelectorConfigBase Config { get; set; }
        public string DeviceID { get; set; }
        public ModulePositions Position { get; set; }

        #endregion Properties

        #region Public constructor

        public LinMotUdp(LineMotObjectivesSelectorConfig objectivesSelectorConfig,ILogger logger)
        {
            Config = objectivesSelectorConfig;
            _ipAddressController = objectivesSelectorConfig.EthernetIP;
            _ipAddressAxis = objectivesSelectorConfig.IpAddressAxis;
            _portNumberAxis = objectivesSelectorConfig.PortNumberAxis;
            _positionMin = objectivesSelectorConfig.PositionMin;
            _positionMax = objectivesSelectorConfig.PositionMax;
            DeviceID = objectivesSelectorConfig.DeviceID;

            _speed = objectivesSelectorConfig.Speed;
            _accel = objectivesSelectorConfig.Accel;
            _decel = objectivesSelectorConfig.Decel;

            _communicationApi = new LinUDP.ACI();

            _logger = logger;
        }

        #endregion Public constructor

        #region Public methods

        public void Disconnect()
        {
            if (_isConnectionActive)
            {
                _communicationApi.CloseConnection();
                _isConnectionActive = false;
            }
            _initialized = false;
        }

        public void Init()
        {
            try
            {
                if (_initialized)
                    return;

                InitTargetsList();
                Connect();
                TryInitAxis();

                _cancelationToken = new CancellationTokenSource();
                _pollTask = new Task(() => { TaskErrorsPolling(); }, TaskCreationOptions.LongRunning);
                _pollTask.Start();

                if (Config.Objectives[0] != null)
                    SetObjective(Config.Objectives[0]);

                _initialized = true;
            }
            catch (Exception ex)
            {
                string message = FormatMessage($"InitCommunication - {ex.Message}");
                _logger?.Error(message);
                Disconnect();
                throw (new Exception(message));
            }
        }

        public ObjectiveConfig GetObjectiveInUse()
        {
            return _currentObjective;
        }

        public void SetObjective(ObjectiveConfig newObjectiveToUse)
        {
            _cancelationToken?.Cancel();

            try
            {
                _currentObjective = newObjectiveToUse;
                lock (_communicationApi)
                {
                    _logger?.Information(FormatMessage($"Entering in SetPos with requested position ({newObjectiveToUse.Coord.ToString()})"));
                    CheckMoveParams(newObjectiveToUse.Coord, _speed, _accel, _decel);
                    if (!_communicationApi.LMmt_MoveAbs(_ipAddressAxis, (float)newObjectiveToUse.Coord.Millimeters, _speed, _accel, _decel))
                        throw (new Exception(FormatMessage($" SetPos Failed with position value {newObjectiveToUse.Coord.ToString("0.00")} for axis {_ipAddressAxis}")));

                    HandleTaskWaitEndMove = new Task(() => { TaskWaitNewObjectiveSet(newObjectiveToUse); }, TaskCreationOptions.LongRunning);
                    HandleTaskWaitEndMove.Start();
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(FormatMessage($" SetPos Failed with position value {newObjectiveToUse.Coord.ToString("0.00")} for axis {_ipAddressAxis} - {ex.Message}"));
                throw;
            }
        }

        private void TaskWaitNewObjectiveSet(ObjectiveConfig newObjectiveToUse)
        {
            var stopWatch = new Stopwatch();
            bool targetReached = false;

            try
            {
                bool moving = true;
                stopWatch.Start();
                while (moving)
                {
                    if (_cancelationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    Thread.Sleep(PeriodWaitEndMoveMs);
                    lock (_communicationApi)
                    {
                        moving = _communicationApi.isMotionActive(_ipAddressAxis);
                    }

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
                lock (_communicationApi)
                {
                    targetReached = _communicationApi.isInTargetPosition(_ipAddressAxis);
                }
                if (!targetReached && !_cancelationToken.IsCancellationRequested)
                    _logger?.Error(FormatMessage($" TaskWaitEndMove - Target not reached: {newObjectiveToUse.Coord.ToString("0.00")} for axis {_ipAddressAxis}"));

                NotifyNewObjectiveInUse(new ObjectiveResult(newObjectiveToUse.DeviceID, targetReached));
            }
        }

        public void WaitMotionEnd()
        {
            try
            {
                if (!SpinWait.SpinUntil(() =>
                {
                    Thread.Sleep(PeriodWaitMotionEndedMs);
                    lock (_communicationApi)
                    {
                        return !_communicationApi.isMotionActive(_ipAddressAxis);
                    }
                }, TimeoutWaitEndMoveMs))
                {
                    string message = FormatMessage("WaitMotionEnd Timeout");
                    _logger.Error(message);
                    throw (new Exception(message));
                }
            }
            catch (Exception Ex)
            {
                throw (new Exception("LinMotUdp WaitMotionEnd - Exception: " + Ex.Message));
            }
        }

        #endregion Public methods

        #region Private methods

        protected void NotifyNewObjectiveInUse(ObjectiveResult newObjectiveInUse)
        {
            var probeServiceCallback = ClassLocator.Default.GetInstance<IProbeServiceCallbackProxy>();
            probeServiceCallback.ProbeNewObjectiveInUseCallback(newObjectiveInUse);
        }

        private void InitTargetsList()
        {
            lock (_communicationApi)
            {
                _communicationApi.CreateTargetAddressList();
                _communicationApi.ClearTargetAddressList();
                _communicationApi.SetTargetAddressList(_ipAddressAxis, _portNumberAxis);
            }
        }

        private void Connect()
        {
            lock (_communicationApi)
            {
                _isConnectionActive = _communicationApi.ActivateConnection(_ipAddressController, "");
                if (!_isConnectionActive)
                    throw (new Exception(FormatMessage($"Connection to {_ipAddressController} not active")));

                if (!_communicationApi.isConnected(_ipAddressAxis))
                    throw (new Exception(FormatMessage($"{_ipAddressController} not connected")));
            }
        }

        public bool Homing()
        {
            var success = false;
            lock (_communicationApi)
            {
                if (!_isConnectionActive)
                    return false;

                if (_communicationApi.isHomed(_ipAddressAxis))
                    return true;
                success = _communicationApi.Homing(_ipAddressAxis);
            }
            return success;
        }

        private void TryInitAxis()
        {
            var success = false;
            for (int nbTry = 0; nbTry < 2 && !success; nbTry++)
            {
                success = InitAxis();
                _logger?.Information(FormatMessage($"InitCommunication - AxisInitialization try: {nbTry} success: {success}"));
            }
            if (!success)
                throw (new Exception(FormatMessage($"AxisInitialization failed")));
        }

        private bool InitAxis()
        {
            lock (_communicationApi)
            {
                if (!_isConnectionActive)
                {
                    _logger?.Error(FormatMessage($"InitAxis - Connection is not active"));
                    return false;
                }

                SpinWait.SpinUntil(() => _communicationApi.isResponseUpToDate(_ipAddressAxis), TimeoutMs);

                if (!_communicationApi.isConnected(_ipAddressAxis))
                {
                    _logger?.Error(FormatMessage($"InitAxis - {_ipAddressAxis} Not connected"));
                    return false;
                }

                if (!ResetAxisErrors())
                {
                    _logger?.Error(FormatMessage($"InitAxis - Function ResetAxisErrors failed"));
                    return false;
                }

                if (!AxisEnable())
                {
                    _logger?.Error(FormatMessage($"InitAxis - Function AxisEnable failed"));
                    return false;
                }

                if (!Homing())
                {
                    _logger?.Error(FormatMessage($"InitAxis - LinUdp is in error Homing error"));
                    return false;
                }
            }

            return true;
        }

        private bool ResetAxisErrors()
        {
            lock (_communicationApi)
            {
                if (_communicationApi.isErrorSM(_ipAddressAxis))
                {
                    _communicationApi.AckErrors(_ipAddressAxis);
                    if (!SpinWait.SpinUntil(() => !_communicationApi.isErrorSM(_ipAddressAxis) && _communicationApi.isResponseUpToDate(_ipAddressAxis), TimeoutMs))
                    {
                        _logger?.Error(FormatMessage($"LinUdp is in error"));
                        return false;
                    }
                }
            }
            return true;
        }

        private bool AxisEnable()
        {
            lock (_communicationApi)
            {
                if (_communicationApi.isSwitchOnActive(_ipAddressAxis) && _communicationApi.isOperationEnabledSM(_ipAddressAxis))
                    return true;

                if (!SpinWait.SpinUntil(() =>
                {
                    return _communicationApi.isReadyToSwitchOnSM(_ipAddressAxis);
                }, TimeoutMs))
                {
                    _logger?.Error(FormatMessage($"AxisEnable - LinUdp is in error SwitchOn timeout"));
                }

                _communicationApi.SwitchOn(_ipAddressAxis);
                if (!SpinWait.SpinUntil(() => _communicationApi.isSwitchOnActive(_ipAddressAxis) && _communicationApi.isResponseUpToDate(_ipAddressAxis), 10000))
                {
                    _logger?.Error(FormatMessage($"AxisEnable - LinUdp is in error SwitchOn timeout"));
                    return false;
                }

                if (!SpinWait.SpinUntil(() => { return _communicationApi.isOperationEnabledSM(_ipAddressAxis) && _communicationApi.isResponseUpToDate(_ipAddressAxis); }, TimeoutMs))
                {
                    _logger?.Error(FormatMessage($"AxisEnable - LinUdp is in error OperationEnable timeout"));
                    return false;
                }
            }

            return true;
        }

        private void CheckMoveParams(Length wantedPosition, float speed, float accel, float decel)
        {
            try
            {
                if (wantedPosition > _positionMax)
                    throw (new Exception(wantedPosition.ToString("0.000") + "mm out of axis maximum limit " + _positionMax.Millimeters.ToString("0.000") + "mm"));
                else if (wantedPosition < _positionMin)
                    throw (new Exception(wantedPosition.ToString("0.000") + "mm out of axis minimum limit " + _positionMin.Millimeters.ToString("0.000") + "mm"));

                if (speed > SecurityThresholdSpeed)
                    throw (new Exception("Excessive speed detected: " + speed.ToString("0.000") + "mm.s-1"));

                if (accel > SecurityThresholdAccel)
                    throw (new Exception("Excessive acceleration detected: " + accel.ToString("0.000") + "mm.s-2"));

                if (decel > SecurityThresholdDeccel)
                    throw (new Exception("Excessive deceleration detected: " + decel.ToString("0.000") + "mm.s-2"));
            }
            catch (Exception ex)
            {
                string message = FormatMessage($"CheckMoveParams - Exception: {ex.Message}");
                _logger?.Error(message);
                throw (new Exception(message));
            }
        }

        private void TaskErrorsPolling()
        {
            while (_cancelationToken.IsCancellationRequested == false)
            {
                try
                {
                    lock (_communicationApi)
                    {
                        var dllError = _communicationApi.getDLLError();
                        if (String.IsNullOrEmpty(dllError) == false)
                            throw (new Exception($"TaskErrorsPolling - Dll error: {dllError}"));

                        if (_communicationApi.isError(_ipAddressAxis))
                            throw (new Exception($"TaskErrorsPolling - Axis which IP is {_ipAddressAxis} is in error"));

                        if (_communicationApi.isFatalError(_ipAddressAxis))
                            throw (new Exception($"TaskErrorsPolling - Axis which IP is {_ipAddressAxis} is in fatal error"));

                        if (_communicationApi.isSetupErrorSM(_ipAddressAxis))
                            throw (new Exception($"TaskErrorsPolling - Axis which IP is {_ipAddressAxis} is in setup error"));

                        if (_communicationApi.isErrorSM(_ipAddressAxis))
                            throw (new Exception($"TaskErrorsPolling - Axis which IP is {_ipAddressAxis} is in SM error"));
                    }
                    Thread.Sleep(PollingPeriodMs);
                }
                catch (Exception)
                {
                    lock (_communicationApi)
                    {
                        _communicationApi.AckErrors(_ipAddressAxis);
                    }
                    var errorMsg = FormatMessage($"TaskErrorsPolling : Error");
                    _logger?.Error(errorMsg);
                    ErrorEvent?.Invoke(new Message(MessageLevel.Error, errorMsg, "", DeviceID));
                }
            }
        }

        private string FormatMessage(string message)
        {
            return $"[{DeviceID}]{message}";
        }

        #endregion Private methods
    }
}
