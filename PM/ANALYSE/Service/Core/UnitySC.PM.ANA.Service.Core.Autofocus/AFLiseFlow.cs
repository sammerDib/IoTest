using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.Autofocus
{
    /// <summary>
    /// Compute the Z position for an objective to be on focus.
    /// Current objective of given lise probe is involved here.
    /// </summary>
    public class AFLiseFlow : FlowComponent<AFLiseInput, AFLiseResult, AFLiseConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        public AFLiseFlow(AFLiseInput input) : base(input, "AFLiseFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            FdcProvider = ClassLocator.Default.GetInstance<AFLiseFlowFDCProvider>();
        }

        protected override void Process()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Result.QualityScore = 0;
            var initialPos = HardwareUtils.GetAxesPosition(_hardwareManager.Axes);

            SearchFocusPosition();

            MoveToFocusPosition(Result.ZPosition);

            if (Configuration.IsAnyReportEnabled())
            {
                ReportAnalyzedSignalAtFocusPosition();
            }

            ResetUninvolvedZAxeAfterAutofocus(initialPos);

            stopWatch.Stop();
            Logger.Information($"{LogHeader} Autofocus was successful at Z position {Result.ZPosition} mm. Found in {stopWatch.Elapsed}");
        }

        private void SearchFocusPosition()
        {
            double gain = GetLiseGain();
            ObjectiveConfig currentObjective = _hardwareManager.GetObjectiveInUseByProbe(Input.ProbeID);
            ObjectiveCalibration objectiveCalibration = HardwareUtils.GetObjectiveParameters(_calibrationManager, currentObjective.DeviceID);
            ScanRangeWithStep focusPositionRange = GetFocusPositionRange(currentObjective, objectiveCalibration);
            LiseAutofocusParameters afLiseParams = objectiveCalibration.AutoFocus.Lise;
            LiseAcquisitionParams acquisitionParams = new LiseAcquisitionParams(gain, LowPrecisionMeasurement);

            // Try to find focus position from the current position
            Length ZCurrentPosition = HardwareUtils.GetAxesPosition(_hardwareManager.Axes).ZTop.Millimeters();
            Logger.Debug($"{LogHeader} Try to find a valid signal at z position (mm) : {ZCurrentPosition}.");

            bool focusPositionIsFound = ComputeFocusPositionFromGivenPosition(afLiseParams, acquisitionParams, focusPositionRange, ZCurrentPosition.Millimeters);
            if (focusPositionIsFound)
            {
                // If we found a focus position, we need to re-analyze signal from this position to be as repeatable as possible
                // TODO : Aims to be a temporary fix, the source of the problem seems to come from signal analysis
                focusPositionIsFound = ComputeFocusPositionFromGivenPosition(afLiseParams, acquisitionParams, focusPositionRange, Result.ZPosition);
                if (focusPositionIsFound)
                {
                    return;
                }
            }

            // If no valid signal found, try to find a valid signal all over the scan range
            Logger.Debug($"{LogHeader} Try to find a valid signal in the z positions range (mm) [{focusPositionRange.Min},{focusPositionRange.Max}].");

            for (double currentPos = focusPositionRange.Max; currentPos > focusPositionRange.Min; currentPos -= focusPositionRange.Step)
            {
                focusPositionIsFound = ComputeFocusPositionFromGivenPosition(afLiseParams, acquisitionParams, focusPositionRange, currentPos);
                if (focusPositionIsFound)
                {
                    // If we found a focus position, we need to re-analyze signal from this position to be as repeatable as possible
                    // TODO : Aims to be a temporary fix, the source of the problem seems to come from signal analysis
                    focusPositionIsFound = ComputeFocusPositionFromGivenPosition(afLiseParams, acquisitionParams, focusPositionRange, Result.ZPosition);
                    if (focusPositionIsFound)
                    {
                        return;
                    }
                }
            }

            throw new Exception($"No z position in the available range [{focusPositionRange.Min},{focusPositionRange.Max}] allows to obtain a correct signal with a gain of {gain}.");
        }

        private bool ComputeFocusPositionFromGivenPosition(LiseAutofocusParameters afLiseParams, LiseAcquisitionParams acquisitionParams, ScanRangeWithStep focusPositionRange, double zPos)
        {
            var probeLise = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.ProbeID);

            MoveZAxes(zPos, probeLise.Configuration.ModulePosition);

            var rawSignal = AcquireRawSignal(probeLise, acquisitionParams);
            var analyzedSignal = new LISESignalAnalyzed(rawSignal);

            if (Configuration.IsAnyReportEnabled())
            {
                ReportAnalyzedSignal(analyzedSignal, acquisitionParams.ProbeGain);
            }

            if (analyzedSignal.SignalStatus == LISESignalAnalyzed.SignalAnalysisStatus.Valid)
            {
                var focusPosition = CalculateZFocusPositionFromSignal(analyzedSignal, afLiseParams, probeLise.Configuration.ModulePosition);

                if (FocusPositionIsInValidRange(focusPositionRange, focusPosition))
                {
                    int peakOfInterestIndex = 0;
                    var config = probeLise.Configuration as ProbeLiseConfig;
                    Result.QualityScore = QualityScore.ComputeQualityScore(analyzedSignal, peakOfInterestIndex, config);
                    Result.ZPosition = focusPosition;
                    return true;
                }
            }

            return false;
        }

        private bool FocusPositionIsInValidRange(ScanRangeWithStep positionRange, double focusPosition)
        {
            bool focusPositionIsValid = focusPosition >= positionRange.Min && focusPosition <= positionRange.Max;
            return focusPositionIsValid;
        }

        private double GetLiseGain()
        {
            var autofocusLiseParams = HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, Input.ProbeID);

            if (double.IsNaN(Input.Gain) && autofocusLiseParams == null)
            {
                throw new Exception($"Autofocus LISE parameters are missing and Gain is not defined");
            }

            double gain = double.IsNaN(Input.Gain) ? Math.Round((autofocusLiseParams.MinGain + autofocusLiseParams.MaxGain) / 2, 2) : Input.Gain;

            return gain;
        }

        private ScanRangeWithStep GetFocusPositionRange(ObjectiveConfig currentObjective, ObjectiveCalibration objectiveCalibration)
        {
            double zStep = currentObjective.DepthOfField.Millimeters * 10;

            if (Input.ZPosScanRange is null)
            {
                AutofocusParameters autofocusParams = objectiveCalibration.AutoFocus;

                Length ZMin = new Length(objectiveCalibration.AutoFocus.ZFocusPosition.Millimeters - (Configuration.AutoFocusScanRange.Millimeters / 2.0), LengthUnit.Millimeter);
                Length ZMax = new Length(objectiveCalibration.AutoFocus.ZFocusPosition.Millimeters + (Configuration.AutoFocusScanRange.Millimeters / 2.0), LengthUnit.Millimeter);

                // Optical reference elevation is set to 0 for bottom probe
                if (!(objectiveCalibration?.OpticalReferenceElevationFromStandardWafer is null))
                {
                    ZMin -= objectiveCalibration.OpticalReferenceElevationFromStandardWafer;
                    ZMax -= objectiveCalibration.OpticalReferenceElevationFromStandardWafer;
                }
                return new ScanRangeWithStep(ZMin.Millimeters, ZMax.Millimeters, zStep);
            }
            else
            {
                return new ScanRangeWithStep(Input.ZPosScanRange.Min, Input.ZPosScanRange.Max, zStep);
            }
        }

        private void MoveZAxes(double zPos, ModulePositions probeLisePosition)
        {
            var initialPos = HardwareUtils.GetAxesPosition(_hardwareManager.Axes);

            if (probeLisePosition == ModulePositions.Up)
            {
                bool isZTopNotAlreadyInPlace = Math.Round(initialPos.ZTop, 1) != Math.Round(zPos, 1);
                bool isZBottomToClose = initialPos.ZBottom >= -Configuration.MinDistanceToAvoidInterference;

                var zBottomPos = isZBottomToClose ? -Configuration.MinDistanceToAvoidInterference : initialPos.ZBottom;

                if (isZTopNotAlreadyInPlace || isZBottomToClose)
                {
                    var pos = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, zPos, zBottomPos);
                    HardwareUtils.MoveAxesTo(_hardwareManager.Axes, pos, Configuration.Speed);
                }
            }
            else if (probeLisePosition == ModulePositions.Down)
            {
                bool isZBottomNotAlreadyInPlace = Math.Round(initialPos.ZBottom, 1) != Math.Round(zPos, 1);
                bool isZTopToClose = initialPos.ZTop <= Configuration.MinDistanceToAvoidInterference;

                var zTopPos = isZTopToClose ? Configuration.MinDistanceToAvoidInterference : initialPos.ZTop;

                if (isZBottomNotAlreadyInPlace || isZTopToClose)
                {
                    var pos = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, zTopPos, zPos);
                    HardwareUtils.MoveAxesTo(_hardwareManager.Axes, pos, Configuration.Speed);
                }
            }
        }

        private void ResetUninvolvedZAxeAfterAutofocus(XYZTopZBottomPosition initialPos)
        {
            var actualPos = HardwareUtils.GetAxesPosition(_hardwareManager.Axes);
            var probeLise = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.ProbeID);

            if (probeLise.Configuration.ModulePosition == ModulePositions.Up)
            {
                bool isZBottomNotAlreadyInPlace = Math.Round(actualPos.ZBottom, 1) != Math.Round(initialPos.ZBottom, 1);

                if (isZBottomNotAlreadyInPlace)
                {
                    var pos = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, double.NaN, initialPos.ZBottom);
                    HardwareUtils.MoveAxesTo(_hardwareManager.Axes, pos, Configuration.Speed);
                }
            }
            else if (probeLise.Configuration.ModulePosition == ModulePositions.Down)
            {
                bool isZTopNotAlreadyInPlace = Math.Round(actualPos.ZTop, 1) != Math.Round(initialPos.ZTop, 1);

                if (isZTopNotAlreadyInPlace)
                {
                    var pos = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, initialPos.ZTop, double.NaN);
                    HardwareUtils.MoveAxesTo(_hardwareManager.Axes, pos, Configuration.Speed);
                }
            }
        }

        private double CalculateZFocusPositionFromSignal(LISESignalAnalyzed analyzedSignal, LiseAutofocusParameters autofocusLiseParams, ModulePositions probeLisePosition)
        {
            var displacementOnZAxis = CalculateDisplacementOnZAxisToReachGivenAirGap(analyzedSignal, autofocusLiseParams.AirGap);

            if (probeLisePosition == ModulePositions.Up)
            {
                double currentZTop = ((XYZTopZBottomPosition)_hardwareManager.Axes.GetPos()).ZTop;
                double zTopAutofocusPosition = currentZTop - displacementOnZAxis;
                return zTopAutofocusPosition;
            }
            else if (probeLisePosition == ModulePositions.Down)
            {
                double currentZBottom = ((XYZTopZBottomPosition)_hardwareManager.Axes.GetPos()).ZBottom;
                double zBottomAutofocusPosition = currentZBottom + displacementOnZAxis;
                return zBottomAutofocusPosition;
            }

            return double.NaN;
        }

        private double CalculateDisplacementOnZAxisToReachGivenAirGap(LISESignalAnalyzed analyzedSignal, Length airGapToReach)
        {
            if (analyzedSignal.ReferencePeaks.Count == 0 || analyzedSignal.SelectedPeaks.Count == 0)
            {
                Logger.Debug($"{LogHeader} Can't calculate the current air gap from the current analyzed signal data.");
                return double.NaN;
            }

            var referencePeak = analyzedSignal.ReferencePeaks.ElementAt(0);
            var peakOfInterest = analyzedSignal.SelectedPeaks.ElementAt(0);

            // Theorical way by ze book
            double currentAirGapOnSignal_um = (peakOfInterest.X - referencePeak.X) * (analyzedSignal.StepX / 1000.0); // in micrometer (µm)
            double displacementOnZAxis_mm = (currentAirGapOnSignal_um - airGapToReach.Micrometers) / 1000.0; // in millimetre (mm)
            return displacementOnZAxis_mm;
        }

        private int MoveToFocusPosition(double focusPosition)
        {
            if (double.IsNaN(focusPosition))
            {
                Logger.Error($"{LogHeader} Can't move to the focus position: {focusPosition}.");
                return -1;
            }

            var probeLise = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.ProbeID);

            if (probeLise.Configuration.ModulePosition == ModulePositions.Up)
            {
                var zTopPosition = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, focusPosition, double.NaN);
                HardwareUtils.MoveAxesTo(_hardwareManager.Axes, zTopPosition, Configuration.Speed);
            }
            else if (probeLise.Configuration.ModulePosition == ModulePositions.Down)
            {
                var zBottomPosition = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, double.NaN, focusPosition);
                HardwareUtils.MoveAxesTo(_hardwareManager.Axes, zBottomPosition, Configuration.Speed);
            }

            return 0;
        }

        private void ReportAnalyzedSignalAtFocusPosition()
        {
            var gain = GetLiseGain();
            var probeLise = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.ProbeID);
            var acquisitionParams = new LiseAcquisitionParams(gain, LowPrecisionMeasurement);
            var rawSignal = AcquireRawSignal(probeLise, acquisitionParams);
            var analyzedSignalAtFocus = new LISESignalAnalyzed(rawSignal);

            var autofocusLiseParams = HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, Input.ProbeID);
            double autofocusAirGapOnSignal = autofocusLiseParams.AirGap.Micrometers / (analyzedSignalAtFocus.StepX / 1000); // conversion of air gap μm to x coordinate

            string filename = $"autofocusLISE_signal_at_focus_position_{Result.ZPosition}.csv";

            int rawFocusPosition = (int)analyzedSignalAtFocus.ReferencePeaks[0].X + (int)autofocusAirGapOnSignal;
            LiseSignalReport.WriteSignalAtfocusPositionInCSVFormat(analyzedSignalAtFocus, rawFocusPosition, Path.Combine(ReportFolder, filename));
        }

        private void ReportAnalyzedSignal(LISESignalAnalyzed analyzedSignal, double gain)
        {
            string status = analyzedSignal.SignalStatus == LISESignalAnalyzed.SignalAnalysisStatus.Valid ? "valid" : "invalid";
            string filename = $"autofocusLISE_signal_at_gain_{gain}_{status}.csv";

            var probeLiseConfig = HardwareUtils.GetProbeLiseConfigFromID(_hardwareManager, Input.ProbeID);
            var analysisParams = new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef, probeLiseConfig.PeakInfluence);

            LiseSignalReport.WriteSignalAnalysisInCSVFormat(analyzedSignal, analysisParams, Path.Combine(ReportFolder, filename));
        }
    }
}
