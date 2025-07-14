using System;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.CalibFlow
{
    public class
        DualLiseCalibrationFlow : FlowComponent<CalibrationDualLiseInput, CalibrationDualLiseFlowResult, MeasureDualLiseConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        private double _zTop;
        private double _zBottom;
        private Length _globalThickness;
        private Length _airGapUp;
        private Length _airGapDown;
        private DateTime _timestamp;
        private double _quality;

        public DualLiseCalibrationFlow(CalibrationDualLiseInput input) : base(input, "DualLiseCalibrationFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();

            _hardwareManager.Probes.TryGetValue(Input.MeasureLiseUp.LiseData.ProbeID, out var probeLiseUp);
            if (probeLiseUp == null)
            {
                throw new InvalidCastException("Provided probe up ID is not a probe Lise.");
            }

            _hardwareManager.Probes.TryGetValue(Input.MeasureLiseDown.LiseData.ProbeID, out var probeLiseDown);
            if (probeLiseDown == null)
            {
                throw new InvalidCastException("Provided probe down ID is not a probe Lise.");
            }

            _zTop = 0;
            _zBottom = 0;
            _globalThickness = 0.Nanometers();
            _airGapDown = 0.Nanometers();
            _airGapUp = 0.Nanometers();
            _quality = 0;
        }

        protected override void Process()
        {
            var probeDualLise = (IProbeDualLise)HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.ProbeID);

            var autofocusLiseUpParams = HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, Input.MeasureLiseUp.LiseData.ProbeID);
            var autofocusLiseDownParams = HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, Input.MeasureLiseDown.LiseData.ProbeID);

            if (double.IsNaN(Input.MeasureLiseUp.LiseData.Gain) && autofocusLiseUpParams == null)
            {
                throw new Exception($"Autofocus LISE UP parameters are missing and Gain is not defined");
            }
            if (double.IsNaN(Input.MeasureLiseDown.LiseData.Gain) && autofocusLiseDownParams == null)
            {
                throw new Exception($"Autofocus LISE DOWN parameters are missing and Gain is not defined");
            }

            double liseUpGain = double.IsNaN(Input.MeasureLiseUp.LiseData.Gain) ? Math.Round((autofocusLiseUpParams.MinGain + autofocusLiseUpParams.MaxGain) / 2, 2) : Input.MeasureLiseUp.LiseData.Gain;
            double liseDownGain = double.IsNaN(Input.MeasureLiseDown.LiseData.Gain) ? Math.Round((autofocusLiseDownParams.MinGain + autofocusLiseDownParams.MaxGain) / 2, 2) : Input.MeasureLiseDown.LiseData.Gain;

            var calibrationUpParams = new LiseAcquisitionParams(liseUpGain, HighPrecisionMeasurement, Input.CalibrationSample);
            var calibrationDownParams = new LiseAcquisitionParams(liseDownGain, HighPrecisionMeasurement, Input.CalibrationSample);
            var calibrationParams = new DualLiseAcquisitionParams(calibrationUpParams, calibrationDownParams);

            var probeLiseUpConfig = HardwareUtils.GetProbeLiseConfigFromID(_hardwareManager, Input.MeasureLiseUp.LiseData.ProbeID);
            var probeLiseDownConfig = HardwareUtils.GetProbeLiseConfigFromID(_hardwareManager, Input.MeasureLiseDown.LiseData.ProbeID);

            var analysisUpParams = new LiseSignalAnalysisParams(probeLiseUpConfig.Lag, probeLiseUpConfig.DetectionCoef, probeLiseUpConfig.PeakInfluence);
            var analysisDownParams = new LiseSignalAnalysisParams(probeLiseDownConfig.Lag, probeLiseDownConfig.DetectionCoef, probeLiseDownConfig.PeakInfluence);

            bool calibrationIsValid = UpdateCalibration(probeDualLise, calibrationParams, analysisUpParams, analysisDownParams);
            if (!calibrationIsValid)
            {
                throw new Exception($"Calibration of dual Lise measure failed.");
            }

            Result.CalibResult=new ProbeDualLiseCalibResult()
            {
                AirGapUp = _airGapUp,
                AirGapDown = _airGapDown,
                ZTopUsedForCalib = _zTop,
                ZBottomUsedForCalib = _zBottom,
                GlobalDistance = _globalThickness
            };

            Result.Timestamp = _timestamp;
            Result.Quality = _quality;
        }

        public bool CheckCalibrationValidity()
        {
            if (_quality == 0)
            {
                return false;
            }

            double tolerance = 0.1;
            var position = (XYZTopZBottomPosition)_hardwareManager.Axes.GetPos();
            bool axesPositionIsYetValid = position.ZTop.Near(_zTop, tolerance) && position.ZBottom.Near(_zBottom, tolerance);
            return axesPositionIsYetValid;
        }

        private bool UpdateCalibration(IProbeDualLise dualLise, DualLiseAcquisitionParams calibrationParams, LiseSignalAnalysisParams analysisUpParams, LiseSignalAnalysisParams analysisDownParams)
        {
            if (!CheckCalibrationValidity())
            {
                XYZTopZBottomPosition initialPosition = _hardwareManager.Axes.GetPos() as XYZTopZBottomPosition;
                if (initialPosition == null)
                {
                    Logger.Error("{LogHeader} Update calibration failed  : Axes return a current position which cannot be cast in the type XYZTopZBottomPosition.");
                    return false;
                }

                try
                {
                    var calibPosition=new XYZTopZBottomPosition(new StageReferential(), Input.CalibrationPosition.X, Input.CalibrationPosition.Y,double.NaN,double.NaN);
                    HardwareUtils.MoveAxesTo(_hardwareManager.Axes, calibPosition);

                    var topCalibrationResult = LiseMeasurement.CalibrateLise(dualLise.ProbeLiseUp, calibrationParams.LiseUpParams, analysisUpParams);
                    var airGapUp = topCalibrationResult.Item1;
                    var layersThicknessUp = topCalibrationResult.Item2;
                    var airGapUpQuality = airGapUp.Quality;
                    var layersThicknessUpQuality = 0.0;
                    foreach (var layerThickness in layersThicknessUp)
                    {
                        layersThicknessUpQuality += layerThickness.Quality;
                    }
                    layersThicknessUpQuality = layersThicknessUpQuality / layersThicknessUp.Count;

                    var bottomCalibrationResult = LiseMeasurement.CalibrateLise(dualLise.ProbeLiseDown, calibrationParams.LiseDownParams, analysisDownParams);
                    var airGapDown = bottomCalibrationResult.Item1;
                    var layersThicknessDown = bottomCalibrationResult.Item2;
                    var airGapDownQuality = airGapDown.Quality;
                    var layersThicknessDownQuality = 0.0;
                    foreach (var layerThickness in layersThicknessDown)
                    {
                        layersThicknessDownQuality += layerThickness.Quality;
                    }
                    layersThicknessDownQuality = layersThicknessDownQuality / layersThicknessDown.Count;

                    _zTop = initialPosition.ZTop;
                    _zBottom = initialPosition.ZBottom;
                    _globalThickness = LiseMeasurement.CalibrateDualLise(calibrationParams, airGapUp.AirGap, airGapDown.AirGap, layersThicknessUp, layersThicknessDown);
                    _airGapUp = airGapUp.AirGap;
                    _airGapDown = airGapDown.AirGap;
                    _timestamp = DateTime.UtcNow;
                    _quality = (airGapUpQuality + layersThicknessUpQuality + airGapDownQuality + layersThicknessDownQuality) / 4;
                }
                catch (Exception e)
                {
                    Logger.Error("{LogHeader} Update calibration failed : " + e.Message);
                    return false;
                }
                finally
                {
                    var currentPosition = _hardwareManager.Axes.GetPos() as XYZTopZBottomPosition;
                    if (currentPosition != null && (currentPosition.X != initialPosition.X || currentPosition.Y != initialPosition.Y))
                    {
                        var initialPositionXY = new XYPosition(initialPosition.Referential, initialPosition.X, initialPosition.Y);
                        HardwareUtils.MoveAxesTo(_hardwareManager.Axes, initialPositionXY);
                    }
                }
            }

            return CheckCalibrationValidity();
        }
    }
}
