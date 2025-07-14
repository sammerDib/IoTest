using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Probe.Configuration.ProbeLiseHF;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.Spectrometer;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.CalibFlow
{
    public class LiseHFIntegrationTimeCalibrationFlow : FlowComponent<LiseHFIntegrationTimeCalibrationInput,
        LiseHFIntegrationTimeCalibrationResults, LiseHFIntegrationTimeCalibrationConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly IReferentialManager _referentialManager;

        private List<double> _lastWaveLenghtSignal;
        private List<double> _lastSignal;

        public LiseHFIntegrationTimeCalibrationFlow(LiseHFIntegrationTimeCalibrationInput input) : base(input,
            "LiseHFIntegrationTimeCalibrationFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();

            Result = new LiseHFIntegrationTimeCalibrationResults();
        }

        protected override void Process()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var probeLiseHF = GetProbe();
            CheckCancellation();

            PrepareHardware(probeLiseHF);
            CheckCancellation();
            try
            {
                var processDate = DateTime.Now;
                int count = 0;
                foreach (string objectiveId in Input.ObjectiveIds)
                {
                    try
                    {
                        count++;
                        var liseHfRefCalibration = InitCalibResult(objectiveId);
                        liseHfRefCalibration.Date = processDate;

                        SetProgressMessage($"Calibration {objectiveId} - [{count}/{Input.ObjectiveIds.Count}] - START", Result);
                        CheckCancellation();
                        // Perform IntegrationTime calib with Standard Filter
                        AutoIntegrationTimeForObjectiveAndFilter(objectiveId, count, probeLiseHF, liseHfRefCalibration, false);
                        CheckCancellation();
                        // Perform IntegrationTime calib with Low Filter
                        AutoIntegrationTimeForObjectiveAndFilter(objectiveId, count, probeLiseHF, liseHfRefCalibration, true);

                    }
                    catch (TaskCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        var refCalibration = Result.CalibIntegrationTimes.LastOrDefault();
                        if (refCalibration != null)
                            refCalibration.Status = new FlowStatus(FlowState.Error, $"[{refCalibration.ObjectiveDeviceId}] Exception - {ex.Message}");
                        SetProgressMessage($"Calibration failed [{refCalibration.ObjectiveDeviceId}] - {ex.Message} ", Result);
                    }
                    finally
                    {
                        probeLiseHF.ProbeDevices.Shutter.CloseIris();
                    }
                }
            }
            catch (TaskCanceledException)
            {
                FinalizeUnfinishedResultsWithStatus(FlowState.Canceled, "Canceled");
                SetProgressMessage($"Calibration Cancelled", Result);
            }
            catch (Exception ex)
            {
                FinalizeUnfinishedResultsWithStatus(FlowState.Error, ex.Message);
                SetProgressMessage($"Calibration failed : {ex.Message}", Result);
            }
            stopwatch.Stop();
            Logger.Information($"{LogHeader} Calibration done in {stopwatch.Elapsed}");
        }

        /// <summary>
        /// Compute optimum integration time for a given objective and standar or low filter and set value of <paramref name="liseHFObjectiveIntegrationTimeCalibration"/>
        /// </summary>
        /// <param name="objectiveId">The ID of the objective to calibrate</param>
        /// <param name="count">The index of the current objective</param>
        /// <param name="probeLiseHF">The LiseHF to retrieve spectrometer data</param>
        /// <param name="liseHFObjectiveIntegrationTimeCalibration">Result of the calibration and variable</param>
        /// <param name="useStandardFilter">Define the filter to use</param>
        private void AutoIntegrationTimeForObjectiveAndFilter(string objectiveId, int count, ProbeLiseHF probeLiseHF, LiseHFObjectiveIntegrationTimeCalibration liseHFObjectiveIntegrationTimeCalibration, bool useLowFilter)
        {
            CheckCancellation();

            SelectObjective(objectiveId);
            CheckCancellation();


            // Move to Filter Slider to Standard 
            if (useLowFilter)
            {
                probeLiseHF.SetLowIllumSlider();
            }
            else
            {
                probeLiseHF.SetStandardSlider();
            }

            // Open Iris        
            if (!probeLiseHF.OpenShutterAndWait(2500))
            {
                liseHFObjectiveIntegrationTimeCalibration.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] Shutter Open Timeout");
                SetProgressMessage($"Calibration Done with Error for {objectiveId} - Shutter Open TimeOut", Result);
                return;
            }
            CheckCancellation();

#if DEBUG
            bool bPerformDoTest = false;
            if (bPerformDoTest)
            ///// TEST
            {
                DoTest(probeLiseHF.ProbeDevices.Spectrometer, objectiveId, useLowFilter ? "Low Illum Filter" : "Standard Filter");
            }
#endif

            // Compute Integration Time for Standard Slide
            (double timeValue, double countValue) = AutoIntegrationTime(probeLiseHF.ProbeDevices.Spectrometer, Configuration.MinTimeRoughScan, Configuration.MaxTimeRoughScan, objectiveId);
            // Close Iris        
            if (!probeLiseHF.CloseShutterAndWait(2500))
            {
                liseHFObjectiveIntegrationTimeCalibration.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] Shutter Close Timeout");
                SetProgressMessage($"Calibration Done with Error for {objectiveId} - Shutter Close TimeOut", Result);
                throw new Exception($"[{objectiveId}] Shutter Close Timeout");
            }

            CheckCancellation();

            if (useLowFilter)
            {
                liseHFObjectiveIntegrationTimeCalibration.LowIllumSignal = _lastSignal.ToList();
                liseHFObjectiveIntegrationTimeCalibration.Status = new FlowStatus(FlowState.Success, $"[{objectiveId}] - Low Illum Filter Done");
                liseHFObjectiveIntegrationTimeCalibration.LowIllumFilterIntegrationTime_ms = timeValue;
                liseHFObjectiveIntegrationTimeCalibration.LowIllumFilterBaseCount = countValue;
                SetProgressMessage($"Calibration {objectiveId} LOW Done [{count}/{Input.ObjectiveIds.Count}]", Result);
            }
            else
            {
                liseHFObjectiveIntegrationTimeCalibration.WaveSignal = _lastWaveLenghtSignal.ToList();
                liseHFObjectiveIntegrationTimeCalibration.StandardSignal = _lastSignal.ToList();
                liseHFObjectiveIntegrationTimeCalibration.Status = new FlowStatus(FlowState.Partial, $"[{objectiveId}] - Standard Filter Done");
                liseHFObjectiveIntegrationTimeCalibration.StandardFilterIntegrationTime_ms = timeValue;
                liseHFObjectiveIntegrationTimeCalibration.StandardFilterBaseCount = countValue;
                SetProgressMessage($"Calibration {objectiveId} STD Done [{count}/{Input.ObjectiveIds.Count}]", Result);
            }
            CheckCancellation();
        }

        /// <summary>
        /// Compute optimum integration time for a given couple (objective, filter)
        /// </summary>
        /// <param name="spectroLiseHF">The spectrometer to retrieve signal</param>
        /// <param name="min">The minimum integration time</param>
        /// <param name="max">The maximum integration time</param>
        /// <returns>A couple representing (integrationTime, count)</returns>
        private (double, double) AutoIntegrationTime(SpectrometerBase spectroLHF, double minStart_ms, double maxStart_ms, string objectiveId)
        {
            // we search the lower X to reach or aprproach Y target with minimal error
            //
            // integratTime_ms = X axis
            // Count ==  Y axis

            double target_Count = Configuration.TargetSpectroCount;
            double percentageFinalAcceptance = 0.1; //10% in specs to be set in configuration
            double toleranceCheck = percentageFinalAcceptance * target_Count;
            // check Start Range Viability
            // for Min 
            if ((target_Count + toleranceCheck) <= DoMeasure(spectroLHF, minStart_ms, 4))
            {
                // too hight minimum start
                throw new Exception($"integration time cannot be reached - too Hight start min - check attenuation filter");
            }

            // for Max 
            if ((target_Count - toleranceCheck) >= DoMeasure(spectroLHF, maxStart_ms, 2))
            {
                // Use Lower Target scan (this is the case low magnitude objective)
                Logger.Information($"Using LOW Target for [{objectiveId}]");
                target_Count = Configuration.TargetSpectroCountLow;
                toleranceCheck = percentageFinalAcceptance * target_Count;
                if ((target_Count + toleranceCheck) <= DoMeasure(spectroLHF, minStart_ms, 4))
                {
                    // too hight minimum start
                    throw new Exception($"integration time cannot be reached - too Hight start min - LOW Target - check attenuation filter");
                }

                if ((target_Count - toleranceCheck) >= DoMeasure(spectroLHF, maxStart_ms, 2))
                {
                    // too low maximum start
                    throw new Exception($"integration time cannot be reached - too low start max");
                }
            }

            double minIntTime_ms = minStart_ms;
            double maxIntTime_ms = maxStart_ms;
            double intgTime_ms = (minIntTime_ms + maxIntTime_ms) * 0.5;

            // RTi note :  in the case of a dichotomy search, I'll use an average of ~8 (configurable by config and independent of the range).
            int spectroAverage = Configuration.AverageRoughScan;
            double countBlindAcceptance = Configuration.CountBlindAcceptance;
            double stopAtMinimalinterval_ms = Configuration.StopAtMinimalInterval_ms;

            // first measure
            double maxRollingAverage_Count = DoMeasure(spectroLHF, intgTime_ms, spectroAverage);
            double targetError_Count = target_Count - maxRollingAverage_Count;
            if (Math.Abs(targetError_Count) <= countBlindAcceptance)
            {
                // the loto winner prize :)
                Logger.Information($"Done Winner [{objectiveId}] It= {intgTime_ms:F5} ms CNT={maxRollingAverage_Count:F1}");
                //Phase 2 - Fine Search
                return AutoIntegrationTime_FineSearch(spectroLHF, intgTime_ms, maxRollingAverage_Count, target_Count, objectiveId);
            }

            // Phase 1 - Rougth Search 
            Logger.Information($"[{objectiveId}] Rougth It Search");

            double previousTargetError_Count = targetError_Count;
            double previousIntTime_ms = intgTime_ms;
            double previousCount = maxRollingAverage_Count;

            int NbIter = 0;
            bool foundIntTime = false;
            do
            {
                CheckCancellation();
                ++NbIter;

                if (maxRollingAverage_Count < target_Count)
                {
                    minIntTime_ms = previousIntTime_ms;
                }
                else
                {
                    maxIntTime_ms = previousIntTime_ms;
                }

                // new search
                intgTime_ms = (minIntTime_ms + maxIntTime_ms) * 0.5;
                maxRollingAverage_Count = DoMeasure(spectroLHF, intgTime_ms, spectroAverage);
                targetError_Count = target_Count - maxRollingAverage_Count;

                Logger.Information($"[{NbIter:000}][{objectiveId}] It= {intgTime_ms:F5} ms CNT={maxRollingAverage_Count:F1}");

                if (Math.Abs(targetError_Count) <= countBlindAcceptance)
                {
                    foundIntTime = true;
                }
                else if ((maxIntTime_ms - minIntTime_ms) <= stopAtMinimalinterval_ms)
                {
                    if (Math.Abs(targetError_Count) > Math.Abs(previousTargetError_Count))
                    {
                        // recover previous best
                        targetError_Count = previousTargetError_Count;
                        intgTime_ms = previousIntTime_ms;
                        maxRollingAverage_Count = previousCount;

                        foundIntTime = true;
                    }
                }
                else
                {
                    // continue search
                    previousTargetError_Count = targetError_Count;
                    previousIntTime_ms = intgTime_ms;
                    previousCount = maxRollingAverage_Count;
                }

            }
            while (!foundIntTime);

            Logger.Information($"[{objectiveId}] Rougth FOUND It= {intgTime_ms:F5} ms CNT={maxRollingAverage_Count:F1} in [{NbIter}] iterations");


            if (Math.Abs(targetError_Count) > toleranceCheck)
            {
                throw new Exception($"integration time found outside Count acceptance range : ITime = {intgTime_ms} ms; Count max = {maxRollingAverage_Count}; Count Error = {targetError_Count}");
            }

            //Phase 2 - Fine Search
            return AutoIntegrationTime_FineSearch(spectroLHF, intgTime_ms, maxRollingAverage_Count, target_Count, objectiveId);
        }

        private (double, double) AutoIntegrationTime_FineSearch(SpectrometerBase spectroLHF, double roughTime_ms, double rough_Count, double target_Count, string objectiveId)
        {
            double intgTime_ms = roughTime_ms;

            double itFineHalfSpan_ms = Configuration.SearchSpanFineSpan * 0.5;
            double itFineStep_ms = Configuration.StepTimeFineScan;
            int spectroFineAverage = Configuration.AverageFineScan;
            double ItMin_ms = Math.Max(Configuration.MinTimeRoughScan, intgTime_ms - itFineHalfSpan_ms);
            double ItMax_ms = Math.Min(Configuration.MaxTimeRoughScan, intgTime_ms + itFineHalfSpan_ms);
            Logger.Information($"[{objectiveId}] Fine It Search [{ItMin_ms:F3}]->[{ItMax_ms:F3}]");

            double itFine_ms = ItMin_ms;
            List<(double x, double y)> cloudPoints = new List<(double x, double y)>((int)((ItMax_ms - ItMin_ms) / itFineStep_ms));
            int countSaturatedMax = 0;
            const int saturatedLimitCount = 3;
            const double spectroSatLimit = 65500.0;
            do
            {
                CheckCancellation();
                double max_FineCount = DoMeasure(spectroLHF, itFine_ms, spectroFineAverage);
                cloudPoints.Add((itFine_ms, max_FineCount));
                if (max_FineCount >= spectroSatLimit)
                    ++countSaturatedMax;

                Logger.Verbose($"[{objectiveId}] x[{itFine_ms:F3}] y[{max_FineCount:F3}]");

                itFine_ms += itFineStep_ms;

            }
            while( (itFine_ms <= ItMax_ms) && (countSaturatedMax < saturatedLimitCount) );

            try
            {
                // y = a * x + b
                var (a, b) = CalculateRegLinearCoefficients(cloudPoints);
                Logger.Verbose($"[{objectiveId}] reg lin found = y = ax+b; a[{a}] b[{b}]");
                // so for y = target_Count -->  x = (y-b)/a
                intgTime_ms = (target_Count - b) / a;
                rough_Count = target_Count;
            }
            catch (Exception e)
            {
                // on retourn le rought du coup...
                Logger.Warning($"[{objectiveId}] Linear Regression failure => using rought scan result");
            }

            //in order to save last signal after
            double maxFinalCount = DoMeasure(spectroLHF, intgTime_ms, spectroFineAverage);
            Logger.Information($"=> Done [{objectiveId}] It= {intgTime_ms:F5} ms CNT={rough_Count:F1} | {maxFinalCount:F1}  ");

            return (intgTime_ms, rough_Count);
        }

        public static (double Slope, double Intercept) CalculateRegLinearCoefficients(List<(double x, double y)> points)
        {
            if (points == null || points.Count < 2)
                throw new ArgumentException("At least two points are required for linear regression.");

            int n = points.Count;
            double sumX = 0.0; double sumY = 0.0; double sumXY = 0.0; double sumX2 = 0.0;
            foreach (var (x, y) in points)
            {
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double denominator = (n * sumX2 - sumX * sumX);
            if (denominator == 0.0)
                throw new InvalidOperationException("Denominator in slope calculation is zero, check input points.");

            double slope = (n * sumXY - sumX * sumY) / denominator;
            double intercept = (sumY - slope * sumX) / n;
            return (slope, intercept);
        }

        /// <summary>
        /// Retrieve signal from the spectrometer for a given integration time and average, and a rolling average window
        /// </summary>
        /// <param name="spectroLiseHF">The spectrometer to be used</param>
        /// <param name="integrationTime">The integration time to be applied</param>
        /// <param name="average">The average ratio to be used</param>
        /// <returns>The max of the rolling average windows for given integration time and average</returns>
        private double DoMeasure(SpectrometerBase spectroLiseHF, double integrationTime, int average)
        {
            int rollingAverageWindowSize = Configuration.RollingAverageWindowSize;

            var param = new SpectrometerParamBase(integrationTime, average);
            CheckCancellation();
            var spectroSignal = spectroLiseHF.DoMeasure(param, isSilent: true);

            _lastWaveLenghtSignal = spectroSignal.Wave;
            var rawValues = spectroSignal.RawValues;
           _lastSignal = rawValues;

            double maxRollingAverage = rawValues
                // Enumerate values with their index
                .Select((val, index) => new { val, index })
                // Only process data after the first rollingAverageWindowSize values
                .Where(x => x.index >= rollingAverageWindowSize - 1)
                .Select(x => rawValues
                    // Skip elements before the window start
                    .Skip(x.index - (rollingAverageWindowSize - 1))
                    // Take exactly rollingAverageWindowSize elements
                    .Take(rollingAverageWindowSize)
                    // Calculate Average on each window
                    .Average())
                // Retrieve the max of the averages
                .Max();
            return maxRollingAverage;
        }

        /// <summary>
        /// Finalize the Result object if process is interrupted (errors or canceled) with status param
        /// </summary>
        protected void FinalizeUnfinishedResultsWithStatus(FlowState state, string message)
        {
            //Set status of "in progress" calibration to canceled
            foreach (var calib in Result.CalibIntegrationTimes.Where(s => s.Status.State == FlowState.InProgress))
            {
                calib.Status = new FlowStatus(state, $"[{calib.ObjectiveDeviceId}] {message}");
            }

            //Add result canceled for all calib not treat
            foreach (var objectiveId in Input.ObjectiveIds.Except(Result.CalibIntegrationTimes.Select(x => x.ObjectiveDeviceId)))
            {
                Result.CalibIntegrationTimes.Add(new LiseHFObjectiveIntegrationTimeCalibration(objectiveId, new FlowStatus(state, $"[{objectiveId}] {message}")));
            }
        }


        /// <summary>
        /// Init Probe Lise HF and close shutter
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown exception when unable to initialise probe</exception>
        private ProbeLiseHF GetProbe()
        {
            var probeHW = HardwareUtils.GetProbeFromID<IProbe>(_hardwareManager, Input.ProbeId);
            if (probeHW == null)
            {
                throw new Exception($"Probe HW id=<{Input.ProbeId}> has not be found");
            }

            var probeLiseHF = probeHW as ProbeLiseHF;
            if (probeLiseHF == null)
            {
                throw new Exception($"HW Probe is not a Probe Lise HF");
            }

            if (probeLiseHF.Status == ProbeStatus.Uninitialized)
                throw new Exception($"Probe Lis HF is not Initialized");

            return probeLiseHF;
        }

        /// <summary>
        /// Close shutter of probe, move stage to REF SI chuck then turn off lights
        /// </summary>
        private void PrepareHardware(ProbeLiseHF probeLiseHF)
        {
            // Stop any continuous acquisition and close shutter
            probeLiseHF.StopContinuousAcquisition();
            probeLiseHF.ProbeDevices.Shutter.CloseIris();

            var chuckConfig = _hardwareManager.Chuck.Configuration as ANAChuckConfig;
            // Retrieve REFERENCE SILICON CALIBRATION
            var refSi = chuckConfig.ReferencesList.Find(x => x.ReferenceName == "REF SI");
            HardwareUtils.MoveAxesToOpticalReference(_hardwareManager, refSi, AxisSpeed.Fast);
            // Turn all the light off
            HardwareUtils.TurnOffAllLights(_hardwareManager, 1000);
        }

        /// <summary>
        /// Init the calib result and his status and add it to the result flow
        /// </summary>
        /// <param name="objectiveId"></param>
        /// <returns></returns>
        private LiseHFObjectiveIntegrationTimeCalibration InitCalibResult(string objectiveId)
        {
            var liseHfRefCalibration = new LiseHFObjectiveIntegrationTimeCalibration(objectiveId, new FlowStatus(FlowState.InProgress, $"[{objectiveId}] In progress"));
            Result.CalibIntegrationTimes.Add(liseHfRefCalibration);
            return liseHfRefCalibration;
        }

        /// <summary>
        /// Select objective and wait for stabilization
        /// </summary>
        /// <param name="objectiveId"></param>
        private void SelectObjective(string objectiveId)
        {
            var objectiveSelector = _hardwareManager.GetObjectiveSelectorOfObjective(objectiveId);
            HardwareUtils.SetNewObjective(objectiveSelector.DeviceID, objectiveId, applyObjectiveOffset: true, Logger, _referentialManager, _hardwareManager);
            Task.Delay(2000, CancellationToken).GetAwaiter().GetResult();

        }


#if DEBUG
        private void DoTest(SpectrometerBase spectroLiseHF, string objectiveID, string filter)
        {
            string csv = $"Integration Time data for objective {objectiveID} and filter {filter}\n";
            csv += "Integration Time;Spectro Value\n";
            for (double i = Configuration.MinTimeRoughScan; i < Configuration.MaxTimeRoughScan; i += Configuration.StepTimeRoughScan)
            {
                CheckCancellation();
                Logger.Information("TEST : Integration time = " + i);
                double value = DoMeasure(spectroLiseHF, i, 1);
                csv += i + ";" + value + "\n";
            }
            Logger.Information(csv);
        }
#endif

    }
}
