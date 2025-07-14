using System;
using System.IO;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.Thickness
{
    public class
    DualLiseThicknessMeasurementFlow : FlowComponent<MeasureDualLiseInput, MeasureDualLiseResult, MeasureDualLiseConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        public DualLiseThicknessMeasurementFlow(MeasureDualLiseInput input) : base(input, "DualLiseThicknessMeasurementFlow")
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

            var acquisitionUpParams = new LiseAcquisitionParams(liseUpGain, Input.MeasureLiseUp.LiseData.NbAveraging, Input.MeasureLiseUp.LiseData.Sample);
            var acquisitionDownParams = new LiseAcquisitionParams(liseDownGain, Input.MeasureLiseDown.LiseData.NbAveraging, Input.MeasureLiseDown.LiseData.Sample);
            var acquisitionParams = new DualLiseAcquisitionParams(acquisitionUpParams, acquisitionDownParams);

            var probeResults = LiseMeasurement.DoUnknownLayerMeasure(probeDualLise, Input.DualLiseCalibration.GlobalDistance, acquisitionParams, Input.UnknownLayer);
            Result.LayersThickness = probeResults.LayersThickness;
            Result.AirGapUp = probeResults.AirGapUp;
            Result.AirGapDown = probeResults.AirGapDown;
            Result.Timestamp = probeResults.Timestamp;
            Result.Quality = probeResults.Quality;


            if (Configuration.IsAnyReportEnabled())
            {
                var filecsvup = Path.Combine(ReportFolder, $"raw_TOP_G{liseUpGain.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}_Dual.csv");
                LiseSignalReport.WriteRawSignalInCSVFormat(acquisitionUpParams.RawSignalAcquired, filecsvup, LogHeader);
                var filecsvdown = Path.Combine(ReportFolder, $"raw_BOTTOM_G{liseUpGain.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}_Dual.csv");
                LiseSignalReport.WriteRawSignalInCSVFormat(acquisitionDownParams.RawSignalAcquired, filecsvdown, LogHeader);
            }

            if (!LiseMeasurement.AreMandatoryMeasuresValid(probeResults.LayersThickness))
            {
                throw new Exception($"Some mandatory layer thicknesses could not be measured or with insufficient precision.");
            }
        }
    }
}
