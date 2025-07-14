using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.Calibration
{
    public class LiseCalibration
    {
        private readonly ILogger _logger;
        private readonly AnaHardwareManager _hardwareManager;

        public LiseCalibration()
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _logger = ClassLocator.Default.GetInstance<ILogger<LiseCalibration>>();
        }

        /// <summary>
        /// Calibrate the gain value range and z-top position to avoid having an empty signal or a saturated signal
        /// </summary>
        ///
        /// <param name="sample">       - the probe sample </param>
        /// <param name="calibration">  - optimal calibration values to acquire signal in current condition </param>
        /// <param name="zMin">         - minimum z value allowing the precalibration to be carried out within a limited range </param>
        /// <param name="zMax">         - maximum z value allowing the precalibration to be carried out within a limited range </param>
        /// <param name="zSpeed">       - moving speed of the z axis </param>
        /// <param name="zStep">        - the step between two changes of position z (default: 0.5) </param>
        /// <param name="minGain">      - minimum gain value allowing the precalibration to be carried out within a limited range (default: 0) </param>
        /// <param name="maxGain">      - maximum gain value allowing the precalibration to be carried out within a limited range (default: 5.5) </param>
        /// <param name="gainStep">     - the step between two changes of gain (default: 0.1) </param>
        ///
        /// <returns> true if the pre-calibration is successful, false otherwise </returns>
        public bool Calibration(ProbeSample sample, ref LiseAutofocusCalibration calibration, string probeID, double zMin = 0,
                                   double zMax = 15, AxisSpeed zSpeed = AxisSpeed.Slow, double zStep = 0.5, double minGain = 0, double maxGain = 5.5,
                                   double gainStep = 0.1, Action<string> updateState = null, CancellationToken token = default)
        {
            var LISE = _hardwareManager.Probes[probeID];

            if (zMin > zMax)
            {
                _logger.Error($"[Calibration] Invalid parameters : zMin must be smaller than zMax.");
                return false;
            }

            // Try to find a valide gain at ZPosition =  zMin + ((zMax - zMin) / 2)
            var axes = _hardwareManager.Axes;
            calibration.ZPosition = zMin + ((zMax - zMin) / 2);
            if (LISE.Configuration.ModulePosition == ModulePositions.Up)
                axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, calibration.ZPosition, double.NaN), zSpeed);
            else // ProbeLiseBottom
                axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, double.NaN, calibration.ZPosition), zSpeed);

            bool gainCalibrationSuceeded = GainCalibration(sample, ref calibration, probeID, minGain, maxGain, gainStep, updateState, token);
            if (gainCalibrationSuceeded)
                return true;

            double zPos = (LISE.Configuration.ModulePosition == ModulePositions.Up) ? zMin : zMax;

            // Iterate on the position z as long as the pre-calibration fails on the current position and another position z can be tried
            while (!gainCalibrationSuceeded)
            {
                if (token.IsCancellationRequested)
                {
                    _logger.Information($"[Lise Calibration] Calibration Cancelled.");
                    return false;
                }

                calibration.ZPosition = zPos;

                if (LISE.Configuration.ModulePosition == ModulePositions.Up)
                {
                    if (zPos > zMax)
                    {
                        _logger.Information($"[Calibration] Calibration failed for all available z positions.");
                        return false;
                    }
                    axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, zPos, double.NaN), zSpeed);
                }
                else // ProbeLiseBottom
                {
                    if (zPos < zMin)
                    {
                        _logger.Information($"[Calibration] Calibration failed for all available z positions.");
                        return false;
                    }
                    axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, double.NaN, zPos), zSpeed);
                }

                gainCalibrationSuceeded = GainCalibration(sample, ref calibration, probeID, minGain, maxGain, gainStep, updateState, token);
                zPos = (LISE.Configuration.ModulePosition == ModulePositions.Up) ? zPos + zStep : zPos - zStep;
            }

            _logger.Information($"[Calibration] Calibration was successful.");
            return true;
        }

        /// <summary>
        /// Calibrate the gain value range at current z position
        /// </summary>
        ///
        /// <param name="sample">       - the probe sample </param>
        /// <param name="calibration">  - optimal calibration values to acquire signal in current condition </param>
        /// <param name="probeType">    - to define whether we want to calibrate the lise up or the lise bottom </param>
        /// <param name="minGain">      - minimum gain value allowing the precalibration to be carried out within a limited range (default: 0) </param>
        /// <param name="maxGain">      - maximum gain value allowing the precalibration to be carried out within a limited range (default: 5.5) </param>
        /// <param name="gainStep">     - the step between two changes of gain (default: 0.1) </param>
        ///
        /// <returns> true if the pre-calibration is successful, false otherwise </returns>
        private bool GainCalibration(ProbeSample sample, ref LiseAutofocusCalibration calibration, string probeID, double minGain = 0, double maxGain = 5.5,
                                   double gainStep = 0.1, Action<string> updateState = null, CancellationToken token = default)
        {
            var LISE = _hardwareManager.Probes[probeID] as IProbeLise;

            List<Tuple<double, double>> stddevAtGain = new List<Tuple<double, double>>();
            double medianGainValue = Math.Round((minGain + maxGain) / 2, 1);

            // Minimum gain calibration
            _logger.Debug($"[gainCalibration] Minimum gain calibration begins.");
            bool gainMinCalibrated = false;
            bool validPeaksHaveBeenDetected = false;
            double currentGainValue = medianGainValue;

            var numberOfSteps = Math.Abs(medianGainValue - maxGain) / gainStep;
            var stepNumber = 0;

            while (!gainMinCalibrated && stepNumber <= numberOfSteps)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                if (updateState != null)
                {
                    updateState($"{Math.Round(stepNumber / numberOfSteps, 3) * 100}%, Gain = {currentGainValue}");
                }

                _logger.Debug($"[gainCalibration] - current gain value : {currentGainValue}.");

                // init probe
                double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
                double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
                int nbMeasures = 1; // Measuring accuracy is not necessary here

                var probeInputParameters = new SingleLiseInputParams(sample, currentGainValue, qualityThreshold, detectionThreshold, nbMeasures);

                // analyze signal acquired from the LISE probe
                var acquisitionParams = new LiseAcquisitionParams(currentGainValue, nbMeasures);
                var rawSignal = AcquireRawSignal(LISE, acquisitionParams);
                var analyzedSignal = new LISESignalAnalyzed(rawSignal);

                double currentStddev = rawSignal.RawValues.StandardDeviation();
                stddevAtGain.Add(new Tuple<double, double>(currentGainValue, currentStddev));

                // The minimal gain is that for which peaks are no longer detected
                if (analyzedSignal.ReferencePeaks.Count() != 0)
                {
                    validPeaksHaveBeenDetected = true;
                }
                else if (validPeaksHaveBeenDetected)
                {
                    calibration.MinGain = currentGainValue + gainStep; //previous gain value
                    gainMinCalibrated = true;
                }

                currentGainValue = Math.Round(currentGainValue - gainStep, 1);
                stepNumber++;
            }

            if (!gainMinCalibrated)
            {
                _logger.Debug($"[gainCalibration] Minimum gain calibration failed.");
                return false;
            }

            _logger.Debug($"[gainCalibration] Minimum gain calibration was successful at : {calibration.MinGain}.");
            _logger.Debug($"[gainCalibration] Maximum gain calibration begins.");

            // Maximum gain calibration
            // The maximum gain is that for which the maximum standard deviation is obtained
            double stddevStep = 0.05; // minimum stddev difference between a maximum signal and a saturated signal
            stddevAtGain.Reverse();
            int maxStddevId = stddevAtGain.Select((value, index) => new { Value = value, Index = index })
                                        .Aggregate((a, b) => (a.Value.Item2 > b.Value.Item2) ? a : b)
                                        .Index;
            Tuple<double, double> maxStddev = stddevAtGain.ElementAt(maxStddevId);

            if (maxStddevId != stddevAtGain.Count - 1 && (maxStddev.Item2) - (stddevAtGain.ElementAt(maxStddevId + 1).Item2) >= stddevStep)
            {
                _logger.Debug($"[gainCalibration] Maximum gain calibration was successful at : {calibration.MaxGain}.");
                calibration.MaxGain = maxStddev.Item1;
                return true;
            }

            bool gainMaxCalibrated = false;
            currentGainValue = medianGainValue + gainStep;
            double previousStddev = maxStddev.Item2;

            stepNumber = 0;
            while (!gainMaxCalibrated && stepNumber <= numberOfSteps)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                if (updateState != null)
                {
                    updateState($"{Math.Round(stepNumber / numberOfSteps, 3) * 100}%, Gain = {currentGainValue}");
                }

                _logger.Debug($"[gainCalibration] - current gain value : {currentGainValue}.");

                if (currentGainValue > maxGain)
                {
                    calibration.MaxGain = maxGain;
                    _logger.Debug($"[gainCalibration] Maximum gain calibration failed.");
                    return true;
                }

                // init probe
                double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
                double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
                int nbMeasures = 1; // Measuring accuracy is not necessary here

                var probeInputParameters = new SingleLiseInputParams(sample, currentGainValue, qualityThreshold, detectionThreshold, nbMeasures);

                // analyze signal acquired from the LISE probe
                var acquisitionParams = new LiseAcquisitionParams(currentGainValue, nbMeasures);
                var rawSignal = AcquireRawSignal(LISE, acquisitionParams);
                var analyzedSignal = new LISESignalAnalyzed(rawSignal);

                double currentStddev = rawSignal.RawValues.StandardDeviation();
                if (previousStddev - currentStddev >= stddevStep)
                {
                    calibration.MaxGain = Math.Round(currentGainValue - gainStep, 1);
                    gainMaxCalibrated = true;
                    _logger.Debug($"[gainCalibration] Maximum gain calibration was successful at : {calibration.MaxGain}.");
                    return true;
                }
                previousStddev = currentStddev;
                currentGainValue = Math.Round(currentGainValue + gainStep, 1);
                stepNumber++;
            }

            _logger.Debug($"[gainCalibration] Maximum gain calibration failed.");
            return false;
        }
    }
}
