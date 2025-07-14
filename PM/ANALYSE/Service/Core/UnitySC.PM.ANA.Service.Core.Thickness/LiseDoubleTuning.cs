using System;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Thickness
{
    public class LiseDoubleTuning
    {
        private ILogger<LiseDoubleTuning> _logger;
        private AnaHardwareManager _hardwareManager;
        private IAxesService _axesService;
        private ProbeDualLise _probeLiseDouble;

        private double _zTopUsedForCalibration;
        private readonly double _zBottomUsedForCalibration;
        private bool _calibrationAvailable;

        public LiseDoubleTuning(string probeLiseDoubleID)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<LiseDoubleTuning>>();
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _axesService = ClassLocator.Default.GetInstance<IAxesService>();
            _probeLiseDouble = _hardwareManager.Probes[probeLiseDoubleID] as ProbeDualLise;

            _calibrationAvailable = false;
        }

        public bool Calibration(DualLiseCalibParams calibParams, DualLiseInputParams inputParams)
        {
            switch (_probeLiseDouble.Configuration)
            {
                case ProbeDualLiseConfig plc when plc.IsSimulated:
                    return true;

                case ProbeDualLiseConfig plc when !plc.IsSimulated:
                    // save the position used for calibration because the calibration is no longer valid if this position changes
                    RecordCalibrationPosition(calibParams.ZTopUsedForCalib, calibParams.ZBottomUsedForCalib);
                    _calibrationAvailable = RunCalibration(calibParams, inputParams);
                    return CalibrationIsYetValid();

                default:
                    return false;
            }
        }

        private bool CalibrationIsYetValid()
        {
            var position = (XYZTopZBottomPosition)_axesService.GetCurrentPosition()?.Result;
            if (position == null)
            {
                return false;
            }

            return _calibrationAvailable && position.ZTop.Near(_zTopUsedForCalibration, 0.001) == true && position.ZBottom.Near(_zBottomUsedForCalibration, 0.001) == true;
        }

        private ProbeDualLiseCalibResult GetCalibrationResults()
        {
            ProbeDualLiseCalibResult probeCalibResults = new ProbeDualLiseCalibResult();
            /*
            probeCalibResults.CalibValue = _probeLiseDouble.ProbeLiseDll.FPGetParamDouble(ProbeLiseParams.FPID_D_CALIBRATE_TOTAL_TH);
            probeCalibResults.UpperAirGap = _probeLiseDouble.ProbeLiseDll.FPGetParamDouble(ProbeLiseParams.FPID_D_CALIBRATE_UPPER_AG);
            probeCalibResults.LowerAirGap = _probeLiseDouble.ProbeLiseDll.FPGetParamDouble(ProbeLiseParams.FPID_D_CALIBRATE_LOWER_AG);
            probeCalibResults.ZTopUsedForCalib = _zTopUsedForCalibration;
            probeCalibResults.ZBottomUsedForCalib = _zBottomUsedForCalibration;
            probeCalibResults.RefThicknessUsed = _probeLiseDouble.ProbeLiseDll.FPGetParamDouble(ProbeLiseParams.FPID_D_CALIBRATE_TH_USED);
            */
            return probeCalibResults;
        }

        private void RecordCalibrationPosition(double zTopUsedForCalibration, double zBottomUsedForCalibration)
        {
            /*
            _zTopUsedForCalibration = zTopUsedForCalibration;
            var zTopValue = new double[] { zTopUsedForCalibration };
            _probeLiseDouble.ProbeLiseDll.FPSetParam(zTopValue, ProbeLiseParams.FPID_D_ZVALUE_TOTAL_TH);

            _zBottomUsedForCalibration = zBottomUsedForCalibration;
            // Z bottom record is not yet available in the Lise dll.
            */
        }

        private bool RunCalibration(DualLiseCalibParams calibParams, DualLiseInputParams inputParams)
        {
            bool succeed = false;
            try
            {
                InitCalibration(calibParams, inputParams);
                SetCalibrationParams(calibParams, inputParams);
                succeed = true;
            }
            catch (Exception e)
            {
                _logger.Error($"[Calibration] Run calibration failed : {e.Message}.");
            }
            _probeLiseDouble.NotifyCalibrationResultsAvailable(GetCalibrationResults());

            return succeed;
        }

        private void InitCalibration(DualLiseCalibParams calibParams, DualLiseInputParams inputParams)
        {
            /*
            _probeLiseDouble.ConfigureProbeForAcquisition(inputParams);
            _probeLiseDouble.SetAirGapThreshold(inputParams);
            _probeLiseDouble.ProbeLiseDll.FPSetParam(calibParams.CalibrationMode, ProbeLiseParams.FPID_I_CALIBRATION_MODE);
            _probeLiseDouble.ProbeLiseDll.FPSetParam(calibParams.NbRepeatCalib, ProbeLiseParams.FPID_I_REPEATS_CALIBRATION);
            */
        }

        private void SetCalibrationParams(DualLiseCalibParams calibParams, DualLiseInputParams inputParams)
        {
            /*
            var calibReference = calibParams.ProbeCalibrationReference;
            double[] calibrationParams = new double[6] {
               calibReference.RefThickness,
               calibReference.RefTolerance,
               calibReference.RefRefrIndex,
               inputParams.GainUp,
               inputParams.GainDown,
               inputParams.QualityThresholdUp
            };

            _probeLiseDouble.ProbeLiseDll.FPSetParam(calibrationParams, ProbeLiseParams.FPID_D_CALIBRATE_TOTAL_TH);
            */
        }
    }
}
