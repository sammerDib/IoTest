using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Profile1D;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySCSharedAlgosCppWrapper;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.Profile1D
{
    public class Profile1DFlow : FlowComponent<Profile1DInput, Profile1DFlowResult, Profile1DConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        private Stopwatch _stopWatch = new Stopwatch();

        private IProbeLise _probe;

        private IAxes _axes;

        private Dictionary<long, Length> _probeSignals = new Dictionary<long, Length>();

        private ConcurrentDictionary<long, XYPosition> _positions = new ConcurrentDictionary<long, XYPosition>();

        private Profile2d _map = new Profile2d();

        public Profile1DFlow(Profile1DInput input) : base(input, "Profile1DFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
        }

        protected override void Process()
        {
            double speed = Input.Speed.MillimetersPerSecond;

            _probe = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.LiseData.ProbeID);

            var startPosition = Input.StartPosition;
            var endPosition = Input.EndPosition;

            // Moving to initial position
            _hardwareManager.Axes.GotoPosition(startPosition, AxisSpeed.Fast);
            _hardwareManager.Axes.WaitMotionEnd(Configuration.MotionTimeout);

            _axes = _hardwareManager.Axes;
            var autofocusLiseParams =
                HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, _probe.DeviceID);
            double gain = Math.Round((autofocusLiseParams.MinGain + autofocusLiseParams.MaxGain) / 2, 2);
            var acquisitionParams = new LiseAcquisitionParams(gain, HighPrecisionMeasurement);

            var probeLiseConfig = HardwareUtils.GetProbeLiseConfigFromID(_hardwareManager, Input.LiseData.ProbeID);
            var analysisParams = new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef,
                probeLiseConfig.PeakInfluence);


            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            _stopWatch.Start();
            var signalAcquisition =
                Task.Run(() => AcquireContinuousSignal(acquisitionParams, analysisParams, cancellationToken),
                    cancellationToken);
            var positionAcquisition = Task.Run(() => AcquireContinuousPosition(cancellationToken), cancellationToken);

            //_hardwareManager.Axes.GotoPosition(endPosition, speed);
            var moveX = new AxisMove(endPosition.X, speed, 200);
            var moveY = new AxisMove(endPosition.Y, speed, 200);
            _hardwareManager.Axes.GotoPointCustomSpeedAccel(moveX, moveY, null, null);
            _hardwareManager.Axes.WaitMotionEnd(Configuration.MotionTimeout);
            cancellationTokenSource.Cancel();

            try
            {
                Task.WhenAll(signalAcquisition, positionAcquisition).Wait();
            }
            catch (AggregateException exs) when (exs.InnerExceptions.All(ex => ex is TaskCanceledException))
            {
                // Skip - Nothing to do
            }

            _stopWatch.Stop();


            _stopWatch.Restart();
            Logger.Information(
                $"Number of signals : {_probeSignals.Count} // Number of positions : {_positions.Count}");
            ProcessData();
            Result.Profile = _map;
            Result.Status = new FlowStatus(FlowState.Success);
            if (Configuration.IsAnyReportEnabled())
            {
                SignalReport.WriteSignalInCSVFormat("X (µm)", "Z (µm)", _map.Select(d => d.X).ToList(),
                    _map.Select(d => d.Y).ToList(),
                    Path.Combine(ReportFolder, $"profile_1D_height_function_of_xy_shift.csv"));
            }
        }

        private void AcquireContinuousSignal(LiseAcquisitionParams acquisitionParams,
            LiseSignalAnalysisParams analysisParams, CancellationToken cancellationToken)
        {
            while (true)
            {
                long signalTimestamp = _stopWatch.ElapsedTicks;
                var probeResult = LiseMeasurement.DoAirGap(_probe, acquisitionParams, analysisParams);
                _probeSignals.Add(signalTimestamp, probeResult.AirGap);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private void AcquireContinuousPosition(CancellationToken cancellationToken)
        {
            while (true)
            {
                long positionTimestamp = _stopWatch.ElapsedTicks;
                var position = _axes.GetPos() as XYPosition;
                _positions.TryAdd(positionTimestamp, position);
                cancellationToken.ThrowIfCancellationRequested();
                Task.Delay(50).Wait(); // Too many positions compared to lise signals, need to reduce the acquisition rate
            }
        }

        private void ProcessData()
        {
            int total = _probeSignals.Count;
            int current = 0;
            foreach (var timestampedSignal in _probeSignals)
            {
                long timestamp = timestampedSignal.Key;
                var value = timestampedSignal.Value;
                (double? x, double? y) = ComputePositionByLinearRegression(timestamp);
                if (x.HasValue && y.HasValue)
                {
                    try
                    {
                        var point = new Point2d(
                            ComputeDistanceFromStart(x.Value, y.Value), 
                            value.Micrometers
                        );
                        _map.Add(point);
                    }
                    catch (ArgumentException)
                    {
                        // Skip
                    }
                }

                current++;
                double progress = Math.Round(100 * (double)current / total, 2);
                Logger.Debug($"ProcessData Progress : {progress}%");
            }

            double avg = _map.Average(point => point.Y);
            foreach (var point in _map)
            {
                point.Y -= avg;
                point.Y *= -1;
            }
        }

        private double ComputeDistanceFromStart(double x, double y)
        {
            double startX = Input.StartPosition.X;
            double startY = Input.StartPosition.Y;
            return Math.Sqrt(Math.Pow((x - startX), 2) + Math.Pow((y - startY), 2));
        }

        private (double? x, double? y) ComputePositionByLinearRegression(long reference)
        {
            var sortedTimestamps = _positions.Keys.OrderBy(timestamp => timestamp).ToList();

            int index = sortedTimestamps.BinarySearch(reference);

            if (index < 0)
            {
                index = ~index;
            }

            int lowerIndex = index - 1;
            int upperIndex = index;
            
            // If we don't have neither upper timestamps nor lower timestamps, return null
            if (lowerIndex < 0 && upperIndex >= sortedTimestamps.Count)
            {
                return (null, null);
            }
            
            // If we don't have lower timestamps, return the lowest of the upper timestamps
            if (lowerIndex < 0)
            {
                var position = _positions[sortedTimestamps[0]];
                return (position.X, position.Y);
            }
            
            // If we don't have upper timestamps, return the uppest of the lower timestamps
            if (upperIndex >= sortedTimestamps.Count)
            {
                var position = _positions[sortedTimestamps[sortedTimestamps.Count]];
                return (position.X, position.Y);
            }
            
            // Otherwise, we do a linear regression between the closest ones
            long closestLowerTimestamp = sortedTimestamps[lowerIndex];
            long closestUpperTimestamp = sortedTimestamps[upperIndex];

            double ratio = ((double)(reference - closestLowerTimestamp)) /
                           (closestUpperTimestamp - closestLowerTimestamp);
            var positionLower = _positions[closestLowerTimestamp];
            var positionUpper = _positions[closestUpperTimestamp];
            double x = positionLower.X + (positionUpper.X - positionLower.X) * ratio;
            double y = positionLower.Y + (positionUpper.Y - positionLower.Y) * ratio;
            return (x, y);
        }
    }
}
