using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.CalibFlow
{
    public class DualLiseCalibration : IProbeCalibration
    {
        private ProbeDualLise _probeDualLise;

        private double _calibrationZTopPosition;
        private double _calibrationZBottomPosition;


        private ILogger _logger;


        private readonly AnaHardwareManager _hardwareManager;

        public IProbe Probe { get; set; }
        public bool IsCalibrationValid { get; set; }

        public Length CalibrationGlobalThickness { get; set; }
        public DualLiseCalibration(ProbeDualLise probeDualLise, ILogger logger)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _probeDualLise = probeDualLise;
            _calibrationZTopPosition = 0;
            _calibrationZBottomPosition = 0;
            CalibrationGlobalThickness = 0.Nanometers();
            IsCalibrationValid = false;
            _logger = logger;
        }

        public bool StartCalibration(IProbeCalibParams probeCalibParams, IProbeInputParams inputParameters)
        {
            if (!(inputParameters is DualLiseInputParams))
                throw new ArgumentException("inputParameters is not valid");

            var input = inputParameters as DualLiseInputParams;

            var calibRef = (DualLiseCalibParams)probeCalibParams;
            var calibRefXPos = calibRef.ProbeCalibrationReference.PositionX.Millimeters;
            var calibRefYPos = calibRef.ProbeCalibrationReference.PositionY.Millimeters;

            switch (_probeDualLise.Configuration)
            {
                case ProbeDualLiseConfig config when config.IsSimulated:
                    CalibrationGlobalThickness = calibRef.ProbeCalibrationReference.RefThickness;
                    _calibrationZTopPosition = calibRef.ZTopUsedForCalib;
                    _calibrationZBottomPosition = calibRef.ZBottomUsedForCalib;
                    IsCalibrationValid = true;
                    return true;

                case ProbeDualLiseConfig config when !config.IsSimulated:
                    var calibrationPosition = new XYPosition(new StageReferential(), calibRefXPos, calibRefYPos);

                    var calibrationLayer = new ProbeSampleLayer(calibRef.ProbeCalibrationReference.RefThickness, new LengthTolerance(calibRef.ProbeCalibrationReference.RefTolerance.Micrometers, LengthToleranceUnit.Micrometer), calibRef.ProbeCalibrationReference.RefRefrIndex);
                    var calibrationSample = new ProbeSample(new List<ProbeSampleLayer>() { calibrationLayer }, "CalibrationSample", "calibration");

                    var calibrationUpParams = new LiseAcquisitionParams(input.ProbeUpParams.Gain, HighPrecisionMeasurement, calibrationSample);
                    var calibrationDownParams = new LiseAcquisitionParams(input.ProbeDownParams.Gain, HighPrecisionMeasurement, calibrationSample);
                    var calibrationParams = new DualLiseAcquisitionParams(calibrationUpParams, calibrationDownParams);

                    var configUp = config.ProbeUp as ProbeLiseConfig;
                    var configDown = config.ProbeDown as ProbeLiseConfig;
                    var analysisUpParams = new LiseSignalAnalysisParams(configUp.Lag, configUp.DetectionCoef, configUp.PeakInfluence);
                    var analysisDownParams = new LiseSignalAnalysisParams(configDown.Lag, configDown.DetectionCoef, configDown.PeakInfluence);

                    var result = UpdateCalibration(_probeDualLise, calibrationPosition, calibrationParams, analysisUpParams, analysisDownParams);
                    return result;
            }

            return false;


        }

        public void CancelCalibration()
        {
            throw new NotImplementedException();
        }

        private bool UpdateCalibration(IProbeDualLise dualLise, XYPosition calibrationPosition, DualLiseAcquisitionParams calibrationParams, LiseSignalAnalysisParams analysisUpParams, LiseSignalAnalysisParams analysisDownParams)
        {
            if (!CheckCalibrationValidity())
            {
                try
                {
                    var initialPosition = (XYZTopZBottomPosition)_hardwareManager.Axes.GetPos();

                    HardwareUtils.MoveAxesTo(_hardwareManager.Axes, calibrationPosition);

                    var topCalibrationResult = LiseMeasurement.CalibrateLise(dualLise.ProbeLiseUp, calibrationParams.LiseUpParams, analysisUpParams);
                    var airGapUpResult = topCalibrationResult.Item1;
                    var airGapUp = airGapUpResult.AirGap;
                    var layersThicknessUp = topCalibrationResult.Item2;

                    var bottomCalibrationResult = LiseMeasurement.CalibrateLise(dualLise.ProbeLiseDown, calibrationParams.LiseDownParams, analysisDownParams);
                    var airGapDownResult = bottomCalibrationResult.Item1;
                    var airGapDown = airGapDownResult.AirGap;
                    var layersThicknessDown = bottomCalibrationResult.Item2;

                    _calibrationZTopPosition = initialPosition.ZTop;
                    _calibrationZBottomPosition = initialPosition.ZBottom;
                    CalibrationGlobalThickness = LiseMeasurement.CalibrateDualLise(calibrationParams, airGapUp, airGapDown, layersThicknessUp, layersThicknessDown);
                    IsCalibrationValid = true;
                }
                catch (InvalidCastException)
                {
                    _logger.Error("{LogHeader} Update calibration failed  : Axes return a current position which cannot be cast in the type XYZTopZBottomPosition.");
                    return false;
                }
                catch (Exception e)
                {
                    _logger.Error("{LogHeader} Update calibration failed : " + e.Message);
                    return false;
                }
            }

            return CheckCalibrationValidity();
        }

        private bool CheckCalibrationValidity()
        {
            if (!IsCalibrationValid)
            {
                return false;
            }

            double tolerance = 0.1;
            var position = (XYZTopZBottomPosition)_hardwareManager.Axes.GetPos();
            bool axesPositionIsYetValid = position.ZTop.Near(_calibrationZTopPosition, tolerance) && position.ZBottom.Near(_calibrationZBottomPosition, tolerance);
            return axesPositionIsYetValid;
        }
    }
}
