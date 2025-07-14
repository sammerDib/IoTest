using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.CalibFlow
{
    /// <summary>
    /// The ObjectiveCalibrationFlow is done for all objective type : INT, NIR, Top and bottom.
    /// For each one, the flow :
    /// Restore Image parameters (X/YPixelSize and X/YOffset)
    /// Measure Autofocus paramaters (ZFocusPosition)
    /// Measure Lise parameters (AirGap, MinGain, MaxGain, ZStartPosition)
    /// Measure AirGap difference between reference and wafer (OpticalReferenceElevationFromStandardWafer)
    /// </summary>
    public class ObjectiveCalibrationFlow : FlowComponent<ObjectiveCalibrationInput, ObjectiveCalibrationResult, ObjectiveCalibrationConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly ObjectiveConfig _currentObjectiveConfig;
        private readonly IProbe _probe;
        private readonly XYZTopZBottomPosition _focusPosition;

        public ObjectiveCalibrationFlow(ObjectiveCalibrationInput input) : base(input, "ObjectiveCalibrationFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _currentObjectiveConfig = _hardwareManager.GetObjectiveConfigs().SingleOrDefault(x => x.DeviceID == Input.ObjectiveId);
            _probe = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.ProbeId);
            _focusPosition = (XYZTopZBottomPosition)_hardwareManager.Axes.GetPos();
            Result = new ObjectiveCalibrationResult();
            Result.ObjectiveId = Input.ObjectiveId;
        }

        protected override void Process()
        {
            if (_probe is null)
                throw new Exception($"Provided Probe ID (" + Input.ProbeId + ") cannot be found in hardware");

            if (_currentObjectiveConfig is null)
                throw new Exception($"Provided Objective Id (" + Input.ObjectiveId + ") cannot be found in hardware configuration");

            if (_currentObjectiveConfig.IsMainObjective && !IsOpenChuck() && !IsThereAWaferClamped())
            {
                throw new Exception("A \"standard\" wafer should be clamp to continue the calibration.");
            }

            SetOrRestoreDefaultValues();

            _probe.StopContinuousAcquisition();

            // Objectives INT and VIS don't need LISE calibration
            if (_currentObjectiveConfig.ObjType == ObjectiveConfig.ObjectiveType.INT || _currentObjectiveConfig.ObjType == ObjectiveConfig.ObjectiveType.VIS)
            {
                Result.OpticalReferenceElevationFromStandardWafer = GetMainObjectiveOpticalElevation();
                Result.AutoFocus.Lise = null;
                return;
            }

            CheckCancellation();
            MoveAxesToAvoidInterference();

            CheckCancellation();
            MeasureLiseAirGap();
            SetProgressMessage($"InProgress: Air Gap {Result.AutoFocus.Lise.AirGap?.ToString("F3")}.\nStarting Lise Calibration.", Result);

            CheckCancellation();
            MeasureLiseCalibration();
            SetProgressMessage($"InProgress: MinGain {Result.AutoFocus.Lise.MinGain:F3}, MaxGain {Result.AutoFocus.Lise.MaxGain:F3}, ZStartPosition {Result.AutoFocus.Lise.ZStartPosition:F3}.\nStarting Reference elevation from wafer measure.", Result);

            CheckCancellation();
            MeasureOpticalReferenceElevation();
            SetProgressMessage($"InProgress: Reference elevation from wafer {Result.OpticalReferenceElevationFromStandardWafer?.ToString("F3")}.", Result);

            CheckCancellation();
            RestorePosition();
        }

        private void SetOrRestoreDefaultValues()
        {
            // ReUse previous calibration data
            if (Input?.PreviousCalibration?.Image != null)
            {
                Result.Image.PixelSizeX = Input.PreviousCalibration.Image.PixelSizeX;
                Result.Image.PixelSizeY = Input.PreviousCalibration.Image.PixelSizeY;
                Result.Image.XOffset = Input.PreviousCalibration.Image.XOffset;
                Result.Image.YOffset = Input.PreviousCalibration.Image.YOffset;
                Result.Image.CentricitiesRefPosition = Input.PreviousCalibration.Image.CentricitiesRefPosition;
                Result.OpticalReferenceElevationFromStandardWafer = Input.PreviousCalibration.OpticalReferenceElevationFromStandardWafer;
            }
            else
            {
                Result.Image.PixelSizeX = 0.Micrometers();
                Result.Image.PixelSizeY = 0.Micrometers();
                Result.Image.XOffset = 0.Micrometers();
                Result.Image.YOffset = 0.Micrometers();
            }

            Length zPosition = _probe.Configuration.ModulePosition == ModulePositions.Up ? _focusPosition.ZTop.Millimeters() : _focusPosition.ZBottom.Millimeters();
            Result.Magnification = _currentObjectiveConfig.Magnification;
            Result.AutoFocus.ZFocusPosition = zPosition;
        }

        private void MoveAxesToAvoidInterference()
        {
            List<AxisConfig> axisConfigs = _hardwareManager.Axes.AxesConfiguration.AxisConfigs;
            if (_probe.Configuration.ModulePosition == ModulePositions.Up)
            {
                // Move bottom lise to avoid interference
                Length ZBottomMaxPos = axisConfigs.First(cfg => cfg.MovingDirection == MovingDirection.ZBottom).PositionMin;
                var pos = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, double.NaN, ZBottomMaxPos.Millimeters);
                HardwareUtils.MoveAxesTo(_hardwareManager.Axes, pos);
            }
            else
            {
                // Move top lise to avoid interference
                Length ZTopMaxPos = axisConfigs.First(cfg => cfg.MovingDirection == MovingDirection.ZTop).PositionMax;
                var pos = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, ZTopMaxPos.Millimeters, double.NaN);
                HardwareUtils.MoveAxesTo(_hardwareManager.Axes, pos);
            }
        }

        private void MeasureLiseAirGap()
        {
            var nbAirGapOnReference = 10;
            var AirGarpSum = 0.Micrometers();
            for (int i = 0; i < nbAirGapOnReference; i++)
            {
                CheckCancellation();
                SetProgressMessage($"InProgress: Computing air gap mean : {i}/{nbAirGapOnReference}.", Result);
                var airGapResultOpticalReference = DoAirGap();
                AirGarpSum += airGapResultOpticalReference;
            }
            var airGapMean = AirGarpSum / nbAirGapOnReference;
            Result.AutoFocus.Lise.AirGap = airGapMean;
        }

        private void MeasureLiseCalibration()
        {
            // Compute AF data
            var liseCalibration = new LiseCalibration();
            var liseCalibrationResult = new LiseAutofocusCalibration();
            var probeSample = new ProbeSample(); // probeSample is useless in this case

            var flowConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>();
            var afLiseConfiguration = flowConfiguration?.Flows?.OfType<AFLiseConfiguration>().FirstOrDefault();
            if (afLiseConfiguration == null)
            {
                throw new Exception("Autofocus Lise Configuration is null");
            }

            var zMin = Result.AutoFocus.ZFocusPosition.Millimeters - (afLiseConfiguration.AutoFocusScanRange.Millimeters / 2.0);
            var zMax = Result.AutoFocus.ZFocusPosition.Millimeters + (afLiseConfiguration.AutoFocusScanRange.Millimeters / 2.0);

            var liseCalibrationSuccess = liseCalibration.Calibration(probeSample, ref liseCalibrationResult, _probe.DeviceID, zMin, zMax,
                                                                        AxisSpeed.Normal, _currentObjectiveConfig.DepthOfField.Millimeters,
                                                                        0, 5.5, 0.1, UpdateLiseCalibrationState, CancellationToken);
            if (!liseCalibrationSuccess)
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    throw new Exception("Lise calibration Cancelled");
                }
                else
                {
                    throw new Exception("Lise calibration Error");
                }
            }

            Result.AutoFocus.Lise.MinGain = liseCalibrationResult.MinGain;
            Result.AutoFocus.Lise.MaxGain = liseCalibrationResult.MaxGain;
            Result.AutoFocus.Lise.ZStartPosition = new Length(liseCalibrationResult.ZPosition, LengthUnit.Millimeter);
        }

        private void MeasureOpticalReferenceElevation()
        {
            if (_currentObjectiveConfig.IsMainObjective || _probe.Configuration.ModulePosition == ModulePositions.Down)
            {
                Result.OpticalReferenceElevationFromStandardWafer = FindOpticalRefElevation();
            }
            else
            {
                Result.OpticalReferenceElevationFromStandardWafer = GetMainObjectiveOpticalElevation();
            }
        }

        private Length GetMainObjectiveOpticalElevation()
        {
            var mainObjectiveOpticalElevation = Input.OpticalReferenceElevationFromStandardWafer;
            if (mainObjectiveOpticalElevation == null)
            {
                throw new Exception("Main objective optical reference elevation from wafer is missing.");
            }

            return mainObjectiveOpticalElevation;
        }

        private Length FindOpticalRefElevation()
        {
            var elevation = 0.Micrometers();
            int nbIter = 0;
            int maxIter = 3; // 3, why not?
            var waferCenter = new XYZTopZBottomPosition(new StageReferential(), 0, 0, _focusPosition.ZTop, _focusPosition.ZBottom);
            do
            {
                HardwareUtils.MoveAxesTo(_hardwareManager.Axes, waferCenter);
                Length airGapResultWaferCenter = DoAirGap();

                if (airGapResultWaferCenter.Micrometers > 0.0)
                {
                    elevation = (airGapResultWaferCenter - Result.AutoFocus.Lise.AirGap).ToUnit(LengthUnit.Micrometer);
                    if (_probe.Configuration.ModulePosition == ModulePositions.Up)
                    {
                        waferCenter.ZTop -= elevation.Millimeters;
                    }
                    else
                    {
                        waferCenter.ZBottom += elevation.Millimeters;
                    }

                    HardwareUtils.MoveAxesTo(_hardwareManager.Axes, waferCenter);
                    var airGapAtFocusPosition = DoAirGap();
                    var correction = airGapAtFocusPosition - Result.AutoFocus.Lise.AirGap;
                    elevation += correction - nbIter * _currentObjectiveConfig.BigStepSizeZ;
                    return elevation;
                }
                else
                {
                    if (_probe.Configuration.ModulePosition == ModulePositions.Up)
                    {
                        waferCenter.ZTop -= _currentObjectiveConfig.BigStepSizeZ.Millimeters;
                    }
                    else
                    {
                        waferCenter.ZBottom += _currentObjectiveConfig.BigStepSizeZ.Millimeters;
                    }
                }

                nbIter++;
            } while (elevation.Micrometers == 0 && nbIter < maxIter);

            throw new Exception("Optical reference elevation can't be measured.");
        }

        private Length DoAirGap()
        {
            var airGapInput = new AirGapLiseInput(Input.ProbeId, Input.Gain);
            var airGapFlow = new LiseAirGapFlow(airGapInput);
            airGapFlow.CancellationToken = CancellationToken;

            var airGapFlowResult = airGapFlow.Execute();
            if (airGapFlowResult.Status.State != FlowState.Success)
            {
                throw new Exception($"Air Gap Error {airGapFlowResult.Status.Message}");
            }

            return airGapFlowResult.AirGap;
        }

        private void UpdateLiseCalibrationState(string msg)
        {
            SetProgressMessage($"InProgress: Lise calibration {msg}.", Result);
        }

        private bool IsOpenChuck()
        {
            if(_hardwareManager.Chuck.Configuration is ANAChuckConfig anaChuckConfig)
                return anaChuckConfig.IsOpenChuck;
            else
                throw new Exception("Chuck configuration is not an ANAChuckConfig type");
        }

        private bool IsThereAWaferClamped()
        {
            return _hardwareManager.Chuck.GetState().WaferClampStates.Any(w => w.Value == true); // Is a wafer clamped at least ?
        }

        private void RestorePosition()
        {
            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, _focusPosition);
        }
    }
}
