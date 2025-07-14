using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class PIE709Controller : PiezoController
    {
        public PIE709ControllerConfig Configuration;
        public PIE709ControllerApiWrapper Api;

        public override List<IAxis> AxesList
        {
            get => ControlledAxesList;
            set
            {
                CheckAxisTypesInListAreValid(value);
                CheckAxisListHasAtMostASingleAxis(value);

                ControlledAxesList = value;
            }
        }

        private void CheckAxisListHasAtMostASingleAxis(List<IAxis> axesList)
        {
            if (axesList.Count > 1) throw new Exception($"PI controller E-709 can only handle a single axe.");
        }

        public PiezoAxis Axis
        {
            get
            {
                CheckAxisTypesInListAreValid(AxesList);
                CheckAxisListHasAtMostASingleAxis(AxesList);

                return (PiezoAxis)AxesList.First();
            }
        }

        public PIE709Controller(PIE709ControllerConfig configuration, PIE709ControllerApiWrapper api = null) : base(configuration)
        {
            Configuration = configuration;
            Api = api ?? new PIE709ControllerApiWrapper(configuration);
        }

        /// <summary>
        /// Single axis piezo controller from PI, model E-709.
        /// </summary>
        public PIE709Controller(PIE709ControllerConfig configuration, IGlobalStatusServer globalStatusServer, ILogger logger, PIE709ControllerApiWrapper api = null) : base(configuration, globalStatusServer, logger)
        {
            Configuration = configuration;
            Api = api ?? new PIE709ControllerApiWrapper(configuration);
        }

        public override void InitializationAllAxes(List<Message> initErrors)
        {
            try
            {
                InitializeSingleAxis();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InitializeSingleAxis()
        {
            InitAxisName();

            if (!IsAutoZeroDone())
            {
                Logger.Warning($"Piezo controller {Name} AutoZERO is NOT Set.");
                PerformAutoZero();
            }

            SetServoMode(ServoMode.On);
            SetWaveGeneratorStartingMode(WaveGeneratorStartingMode.Off);
            Park();
            StopTimerPositionPolling();
            StartTimerPositionPolling();
        }

        /// <summary>
        /// Initialize the axis name.
        /// </summary>
        private void InitAxisName()
        {
            var axisNames = Api.WrappedGetMethod("qSAI").Split('\n').Where(name => !string.IsNullOrEmpty(name)).ToList();
            if (axisNames.Count != 1) throw new Exception("The PI E-709 controller can handle only one axis, please check the controller configuration.");

            Api.AxisName = axisNames.First();
        }

        public bool IsAutoZeroDone() => responseAsBool(Api.WrappedGetMethod<int>("qATZ"));
        
        public void PerformAutoZero()
        {
            Logger.Information($"-+- Piezo controller {Name} Performing AutoZERO....");

            Api.WrappedSetMethod("ATZ", (double)0.0);

            WaitMotionEnd(40000);
            Logger.Information($"-+- Piezo controller {Name} AutoZERO Done.");
        }

        public override void RefreshAxisState(IAxis _)
        {
            Axis.Enabled = Configuration.IsEnabled && IsConnected;
            Axis.Moving = IsMoving();
        }

        public override void Dispose()
        {
            StopTimerPositionPolling();
            StopLogTriggerInSignal();
        }

        /******************
         *   CONNECTION   *
         ******************/
        public override int ConnectionId => Api.ConnectionID;
        public override bool IsConnected => Api.ConnectionID >= 0;

        public override void Connect()
        {
            Logger.Information($"Connect to piezo controller {Name} on port {Configuration.Port} with baudrate {Configuration.BaudRate}.");
            Api.ConnectRS232(Configuration.Port, Configuration.BaudRate);

            if (Api.ConnectionID == -1)
            {
                string message = $"Error: the controller {Name} is unreachable or not responding.";
                Logger.Error(message);
                throw new Exception(message);
            }

            Logger.Information($"Connexion to piezo controller {Name} established.");
        }

        public override void Disconnect()
        {
            Dispose();
            Api.CloseConnection(Api.ConnectionID);
        }

        /************
         *   MISC   *
         ************/

        public string GetHelp() => Api.WrappedGetMethod("qHLP", stringBufferCapacity: 1024 * 10);

        public string GetSupportedParameters() => Api.WrappedGetMethod("qHPA", stringBufferCapacity: 1024 * 10);

        /**************
         *   RIGHTS   *
         **************/

        public void SetNormalRigths() => Api.CCL(CommandLevel.Normal, "");

        public void SetAdminRigths() => Api.CCL(CommandLevel.Admin, "ADVANCED");

        /******************
         *   PROPERTIES   *
         ******************/

        public override string GetAxisName()
        {
            if (string.IsNullOrEmpty(Api.AxisName)) InitAxisName();
            return Api.AxisName;
        }

        public ServoMode GetServoMode() => responseAsEnum<ServoMode>(Api.WrappedGetMethod<int>("qSVO"));

        public void SetServoMode(ServoMode servoMode)
        {
            Api.WrappedSetMethod("SVO", (int)servoMode);
        }

        public override void SetSpeedAxis(List<IAxis> axes, List<AxisSpeed> speeds)
        {
            if (axes.Count != 1 || speeds.Count != 1) throw new Exception($"Piezo controller E-709 can handle only one axis.");

            double speed = GetSpeedValueFromAxisConfig(speeds.First());
            SetSpeed(speed);
        }

        /// <summary>
        /// Set the axis speed, expressed in mm/s. Acceleration is not supported.
        /// </summary>
        public override void SetSpeedAccelAxis(List<IAxis> axes, List<double> speeds, List<double> accels)
        {
            if (axes.Count != 1 || speeds.Count != 1) throw new Exception($"Piezo controller E-709 can handle only one axis.");
            SetSpeed(speeds.First());
        }

        /// <summary>
        /// Get the piezo's motion speed, expressed in mm/s.
        /// </summary>
        public override double GetSpeed() => Api.WrappedGetMethod<double>("qVEL") / 1000; // convert speed from µm/s to mm/s

        /// <summary>
        /// Set the piezo's motion speed, expressed in mm/s.
        /// </summary>
        public override void SetSpeed(double speed)
        {
            if (GetServoMode() == ServoMode.Off)
            {
                SetServoMode(ServoMode.On);
            }
            Api.WrappedSetMethod("VEL", speed * 1000); // convert speed from mm/s to µm/s
        }

        public CommandMode GetCommandMode() => responseAsEnum<CommandMode>((int)Api.WrappedGetMethod<double>("qSPA", PIE709Param.CommandModeParameter));

        /******************
         *  POSITIONNING  *
         ******************/
        private Length _minPosition;

        public override Length GetPositionMin()
        {
            if (_minPosition is null)
            {
                _minPosition = Api.WrappedGetMethod<double>("qTMN").Micrometers();
            }
            double minPosition_um = Math.Max(_minPosition.Micrometers, Axis.PiezoAxisConfig.PositionMin.Micrometers);
            return minPosition_um.Micrometers();
        }

        private Length _maxPosition;

        public override Length GetPositionMax()
        {
            if (_maxPosition is null)
            {
                _maxPosition = Api.WrappedGetMethod<double>("qTMX").Micrometers();
            }
            double maxPosition_um = Math.Min(_maxPosition.Micrometers, Axis.PiezoAxisConfig.PositionMax.Micrometers);
            return maxPosition_um.Micrometers();
        }

        public override void RefreshCurrentPos(List<IAxis> _ = null)
        {
            Axis.CurrentPos = GetCurrentPosition();
        }

        public override TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis)
        {
            var position = GetCurrentPosition().Millimeters;
            var highResolutionDateTime = StartTime.AddTicks(StopWatch.Elapsed.Ticks);
            return new TimestampedPosition(position.Millimeters(), highResolutionDateTime);
        }

        public override Length GetCurrentPosition()
        {
            Length currentPosition = Api.WrappedGetMethod<double>("qPOS").Micrometers();
            Axis.CurrentPos = currentPosition; // RefreshCurrentPos

            return currentPosition;
        }

        public override void SetPosAxis(List<double> targetPositions, List<IAxis> axes, List<AxisSpeed> speeds)
        {
            if (targetPositions.Count != 1 || axes.Count != 1 || speeds.Count != 1) throw new Exception($"Piezo controller E-709 can handle only one axis.");

            var targetPosition = targetPositions.First().Millimeters();
            double speed = GetSpeedValueFromAxisConfig(speeds.First());

            MoveTo(targetPosition, speed);
        }

        public override void SetPosAxisWithSpeedAndAccel(List<double> targetPositions, List<IAxis> axes, List<double> speeds, List<double> accels)
        {
            if (targetPositions.Count != 1 || axes.Count != 1 || speeds.Count != 1) throw new Exception($"Piezo controller E-709 can handle only one axis.");

            var targetPosition = targetPositions.First().Millimeters();
            double speed = speeds.First();

            MoveTo(targetPosition, speed);
        }

        public void MoveTo(Length targetPosition, double speed = double.NaN)
        {
            speed = double.IsNaN(speed) ? Axis.PiezoAxisConfig.SpeedNormal : speed;
            if (GetSpeed() != speed) SetSpeed(speed);

            double validPosition_um = Math.Max(GetPositionMin().Micrometers, Math.Min(targetPosition.Micrometers, GetPositionMax().Micrometers));
            Api.WrappedSetMethod("MOV", validPosition_um);
        }

        /// <summary>
        /// Check if the axis is moving.
        /// </summary>
        /// <returns>True if the axis is moving, false otherwise.</returns>
        private bool IsMoving() => responseAsBool(Api.WrappedGetMethod<int>("IsMoving"));

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true) => WaitMotionEnd(timeout_ms, waitStabilization);

        public virtual void WaitMotionEnd(int timeout_ms, bool waitStabilizationbool = true, bool logPositionStatus = false)
        {
            try
            {
                // Wait until the controller stops moving
                bool hasStoppedWithinTimeout = SpinWait.SpinUntil(() =>
                {
                    bool isStopped = !IsMoving();
                    if (logPositionStatus && !isStopped)
                    {
                        Logger.Debug($"Piezo is moving. Current position = {GetCurrentPosition()}");
                    }
                    return isStopped;
                }, timeout_ms);

                if (!hasStoppedWithinTimeout)
                {
                    throw new TimeoutException($"Piezo WaitMotionEnd exits on timeout ({timeout_ms} ms).");
                }
            }
            finally
            {
                RefreshCurrentPos();

                var currentPosition_um = Axis.CurrentPos;
                var piezoPosition = new ZPiezoPosition(new MotorReferential(), Axis.AxisID, currentPosition_um);
                RaisePositionChangedEvent(piezoPosition);
            }
        }

        /// <summary>
        /// Move the piezo to the position specified in axis config (ParkPosition).
        /// </summary>
        public void Park()
        {
            var parkPosition = Axis.PiezoAxisConfig.PositionPark;
            MoveTo(parkPosition);
            WaitMotionEnd(20000);
        }

        private System.Timers.Timer _timerPositionPolling;

        private void StartTimerPositionPolling()
        {
            _timerPositionPolling = new System.Timers.Timer(50) { Enabled = true }; // 50 ms period
            _timerPositionPolling.Elapsed += RaiseEventIfPositionChanged;
            _timerPositionPolling.Start();
        }

        private void StopTimerPositionPolling()
        {
            if (_timerPositionPolling != null && _timerPositionPolling.Enabled)
            {
                _timerPositionPolling.Elapsed -= RaiseEventIfPositionChanged;
            }
            _timerPositionPolling?.Stop();
            _timerPositionPolling?.Dispose();
        }

        private void RaiseEventIfPositionChanged(object sender, ElapsedEventArgs e)
        {
            var previousPosition = Axis.CurrentPos;
            var currentPosition = GetCurrentPosition();

            if (!previousPosition.Near(currentPosition, 0.1.Micrometers()))
            {
                RaisePositionChangedEvent(new ZPiezoPosition(new MotorReferential(), Axis.AxisID, currentPosition));
            }
        }

        /******************
         *   TRIGGER IN   *
         ******************/

        public TriggerInSignal GetTriggerInSignal() => responseAsEnum<TriggerInSignal>(Api.WrappedGetMethod<int>("qDIO"));

        public TriggerInType GetTriggerInType() => responseAsEnum<TriggerInType>(int.Parse(Api.WrappedGetMethod("qCTI", TriggerInParam.TriggerType)));

        public void SetTriggerInType(TriggerInType triggerInType) => Api.WrappedSetMethod("CTI", ((int)triggerInType).ToString(), TriggerInParam.TriggerType);

        public TriggerPolarity GetTriggerInPolarity() => responseAsEnum<TriggerPolarity>(int.Parse(Api.WrappedGetMethod("qCTI", TriggerInParam.Polarity)));

        public void SetTriggerInPolarity(TriggerPolarity triggerPolarity) => Api.WrappedSetMethod("CTI", ((int)triggerPolarity).ToString(), TriggerInParam.Polarity);

        public TriggerInUsage GetTriggerInUsage() => responseAsEnum<TriggerInUsage>((int)Api.WrappedGetMethod<double>("qSPA", PIE709Param.TriggerInUsage));

        public void SetTriggerInUsage(TriggerInUsage triggerInUsage) => Api.WrappedSetMethod("SPA", (double)triggerInUsage, PIE709Param.TriggerInUsage);

        public TriggerInState GetTriggerInState() => responseAsEnum<TriggerInState>(Api.WrappedGetMethod<int>("qTRI"));

        public void SetTriggerInState(TriggerInState triggerInState) => Api.WrappedSetMethod("TRI", (int)triggerInState);

        public override void ConfigureTriggerIn(Length initialPosition, Length motionStep)
        {
            ConfigureTriggerIn(TriggerInType.Edge, motionStep, waveGeneratorOffset: initialPosition);
        }

        public override void DisableTriggerIn()
        {
            SetServoMode(ServoMode.On);
            SetWaveGeneratorStartingMode(WaveGeneratorStartingMode.Off);
        }

        /// <summary>
        /// Configure a (linear) motion triggered by the digital trigger IN line.
        /// Configure the trigger IN according to the 'triggerInType' mode.
        /// </summary>
        /// <param name="triggerInType"></param>
        /// <param name="motionStep">The amplitude of the motion.</param>
        /// <param name="waveTableId"></param>
        /// <param name="waveGeneratorOffset"></param>
        private void ConfigureTriggerIn(TriggerInType triggerInType, Length motionStep, int waveTableId = 2, Length waveGeneratorOffset = null)
        {
            SetAdminRigths();
            SetTriggerInUsage(TriggerInUsage.WaveGenerator);
            SetWaveGeneratorOffset(waveGeneratorOffset ?? 0.Micrometers());
            SetCyclesCount(0);

            // Only define a new wave when necessary, i.e. when the new amplitude value (motionStep) really differs from the previous one (more than 1 nano).
            if (Math.Abs(motionStep.Micrometers - GetWaveAmplitude(waveTableId)) > 0.001)
            {
                switch (triggerInType)
                {
                    case TriggerInType.Edge: DefineLinearWave(waveTableId, motionStep.Micrometers); break;
                    case TriggerInType.Level: DefineLinearWave(waveTableId, motionStep.Micrometers); break;
                    default: throw new Exception($"Trigger IN type '{triggerInType}' unknowned.");
                }
            }
            else
            {
                Logger.Debug($"=== Wave {waveTableId} already defined with an amplitude of {motionStep}.");
            }

            SetGeneratorWaveTableId(waveTableId);
            SetWaveGeneratorStartingMode(WaveGeneratorStartingMode.StartWithDigitalTriggerInCumulative);
            SetTriggerInType(triggerInType);
            SetTriggerInState(TriggerInState.On);
        }

        /******************
         *   TRIGGER OUT  *
         ******************/

        public TriggerOutMode GetTriggerOutMode() => responseAsEnum<TriggerOutMode>((int)Api.WrappedGetMethod<double>("qCTO", TriggerOutParam.TriggerMode));

        public void SetTriggerOutMode(TriggerOutMode triggerOutMode) => Api.WrappedSetMethod<double>("CTO", (double)triggerOutMode, TriggerOutParam.TriggerMode);

        public double GetStartThreshold() => Api.WrappedGetMethod<double>("qCTO", TriggerOutParam.StartThreshold);

        public void SetStartThreshold(double startThreshold) => Api.WrappedSetMethod("CTO", (double)startThreshold, TriggerOutParam.StartThreshold);

        public double GetStopThreshold() => Api.WrappedGetMethod<double>("qCTO", TriggerOutParam.StopThreshold);

        public void SetStopThreshold(double stopThreshold) => Api.WrappedSetMethod("CTO", (double)stopThreshold, TriggerOutParam.StopThreshold);

        public double GetTriggerStep() => Api.WrappedGetMethod<double>("qCTO", TriggerOutParam.TriggerStep);

        public void SetTriggerStep(double triggerStep) => Api.WrappedSetMethod("CTO", (double)triggerStep, TriggerOutParam.TriggerStep);

        public override void ConfigureTriggerPositionDistance(double startThreshold, double stopThreshold, double triggerStep)
        {
            SetTriggerOutMode(TriggerOutMode.PositionDistance);
            SetStartThreshold(startThreshold);
            SetStopThreshold(stopThreshold);
            SetTriggerStep(triggerStep);
        }

        /******************
         * WAVE GENERATOR *
         ******************/

        public bool IsGeneratorRunning() => responseAsBool(Api.WrappedGetMethod<int>("IsGeneratorRunning"));

        /// <summary>
        /// <para>Write in controller's non-volatile flash memory a linear wave of an amplitude value given in parameter.</para>
        /// /!\ Since the non-volatile flash memory of the controller has a finite
        /// number of erase-write cycles (~10000), please write waveforms only when necessary.
        /// </summary>
        /// <param name="waveTableId"></param>
        /// <param name="amplitude">Wave amplitude in µm.</param>
        /// <param name="pointsCount">Total number of wave points.
        /// Wave generator output a point every 0.1ms so that 10000 wave points define a wave with a 1s duration.
        /// Maxuimum value is 16306 points.</param>
        /// <param name="speedUpDownPointsCount">Number of points used for acceleration/deceleration phase definition.</param>
        /// <param name="firstPointOffset">Offset before the first point definition.</param>
        /// <param name="amplitudeOffset">Offset of the wave amplitude.</param>
        public void DefineLinearWave(int waveTableId, double amplitude, int pointsCount = 1, int speedUpDownPointsCount = 0, int firstPointOffset = 0, double amplitudeOffset = 0)
        {
            // Params
            int writeOption = 0; // 0 = clear, 1 = add, 2 = append

            Api.WAV_LIN(
                waveTableId,
                firstPointOffset,
                pointsCount,
                writeOption,
                speedUpDownPointsCount,
                amplitude,
                amplitudeOffset
            );
        }

        public double GetWaveGeneratorOffset() => Api.WrappedGetMethod<double>("qWOS");

        public void SetWaveGeneratorOffset(Length waveGeneratorOffset) => Api.WrappedSetMethod("WOS", waveGeneratorOffset.Micrometers);

        /// <summary>
        /// Returns wave amplitude in micrometers.
        /// </summary>
        /// <param name="waveTableId"></param>
        /// <returns></returns>
        public double GetWaveAmplitude(int waveTableId)
        {
            double[] wave = GetWave(waveTableId);

            switch (wave?.Length)
            {
                case 0: return 0;
                case 1: return wave.First();
                case int length when wave.Length >= 2: return wave.Last() - wave.First(); // Assuming that the function is strictly increasing
                default: return 0;
            }
        }

        public double[] GetWave(int waveTableId)
        {
            int wavePointsCount = GetWavePointsCount(waveTableId);

            if (wavePointsCount >= 1)
            {
                double[] wave = new double[wavePointsCount];
                Api.qGWD_SYNC(waveTableId, 1, wavePointsCount, wave);
                return wave;
            }
            return null;
        }

        public bool HasADefinedWave(int waveTableId)
        {
            if (waveTableId >= GetGeneratorWaveTablesCount())
            {
                throw new ArgumentException("Invalid waveId");
            }

            return GetWavePointsCount(waveTableId) > 0;
        }

        public int GetWavePointsCount(int waveTableId)
        {
            double[] wavePointsCount = new double[1];
            Api.qWAV(waveTableId, WaveParam.WavePointsCount, wavePointsCount);
            return (int)wavePointsCount[0];
        }

        public int GetGeneratorWaveTablesCount() => (int)Api.WrappedGetMethod<double>("qSPA", PIE709Param.WaveTablesCount);

        public int GetGeneratorWaveTableId() => Api.WrappedGetMethod<int>("qWSL");

        public void SetGeneratorWaveTableId(int generatorWaveTableId) => Api.WrappedSetMethod("WSL", generatorWaveTableId);

        public WaveGeneratorStartingMode GetWaveGeneratorStartingMode() => responseAsEnum<WaveGeneratorStartingMode>(Api.WrappedGetMethod<int>("qWGO"));

        public void SetWaveGeneratorStartingMode(WaveGeneratorStartingMode waveGeneratorStartingMode) => Api.WrappedSetMethod("WGO", (int)waveGeneratorStartingMode);

        // Generator's number of output cycles
        public int GetCyclesCount() => Api.WrappedGetMethod<int>("qWGC");

        public void SetCyclesCount(int cyclesCount) => Api.WrappedSetMethod("WGC", cyclesCount);

        // Generator's table rate
        public int GetTableRate() => Api.WrappedGetMethod<int>("qWTR");

        /******************
         *      UTILS     *
         ******************/

        private bool isNotAZeroOneValue(int value) => isNotAValidValue(value, (int)Bool.True, (int)Bool.False);

        private bool isNotAValidValue(double value, params double[] allowedValues) => !allowedValues.Contains(value);

        private int[] valuesOf<T>() => (int[])Enum.GetValues(typeof(T));

        private bool responseAsBool(int responseValue)
        {
            if (isNotAZeroOneValue(responseValue))
            {
                string errorMessage = "Error: incorrect or unknown response value.";
                throw new Exception(errorMessage);
            }

            return responseValue == (int)Bool.True;
        }

        private T responseAsEnum<T>(int responseValue)
        {
            if (isNotAValidValue(responseValue, valuesOf<T>().Select(Convert.ToDouble).ToArray()))
            {
                string errorMessage = "Error: incorrect or unknown response value.";
                throw new Exception(errorMessage);
            }

            return (T)Enum.ToObject(typeof(T), responseValue);
        }

        private double GetSpeedValueFromAxisConfig(AxisSpeed speedEnum)
        {
            switch (speedEnum)
            {
                case AxisSpeed.Slow: return Axis.PiezoAxisConfig.SpeedSlow;
                case AxisSpeed.Normal: return Axis.PiezoAxisConfig.SpeedNormal;
                case AxisSpeed.Fast: return Axis.PiezoAxisConfig.SpeedFast;
                case AxisSpeed.Measure: return Axis.PiezoAxisConfig.SpeedMeasure;
                default: throw new Exception($"Unknown speed enum value '{speedEnum}'");
            }
        }

        private System.Timers.Timer _timerLogTriggerInSignal;

        public void StartLogTriggerInSignal(int period_ms)
        {
            _timerLogTriggerInSignal = new System.Timers.Timer(period_ms) { Enabled = true };
            _timerLogTriggerInSignal.Elapsed += LogTriggerInSignal;
            _timerLogTriggerInSignal.Start();
        }

        public void StopLogTriggerInSignal()
        {
            _timerLogTriggerInSignal?.Stop();
            if (_timerLogTriggerInSignal != null && _timerLogTriggerInSignal.Enabled)
            {
                _timerLogTriggerInSignal.Elapsed -= LogTriggerInSignal;
            }
            _timerLogTriggerInSignal?.Dispose();
        }

        private void LogTriggerInSignal(object sender, ElapsedEventArgs e)
        {
            Logger.Debug($"-------------- TriggerInSignal = {GetTriggerInSignal()}.");
        }

        public override void InitControllerAxes(List<Message> initErrors)
        {
            //Nothing
        }

        public override void InitZTopFocus()
        {
            //Nothing
        }

        public override void InitZBottomFocus()
        {
            //Nothing
        }
    }
}
