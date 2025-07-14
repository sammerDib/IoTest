using System;
using System.IO;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.Thickness
{
    public class LiseThicknessMeasurementFlow : FlowComponent<MeasureLiseInput, MeasureLiseResult, MeasureLiseConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        public LiseThicknessMeasurementFlow(MeasureLiseInput input) : base(input, "LiseThicknessMeasurementFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
        }

        protected override void Process()
        {
            var probeLise = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.LiseData.ProbeID);
            var autofocusLiseParams = HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, Input.LiseData.ProbeID);

            if (double.IsNaN(Input.LiseData.Gain) && autofocusLiseParams == null)
            {
                throw new Exception($"Autofocus LISE parameters are missing and Gain is not defined");
            }
            double gain = double.IsNaN(Input.LiseData.Gain) ? Math.Round((autofocusLiseParams.MinGain + autofocusLiseParams.MaxGain) / 2, 2) : Input.LiseData.Gain;

            var acquisitionParams = new LiseAcquisitionParams(gain, Input.LiseData.NbAveraging, Input.LiseData.Sample);
            var probeLiseConfig = HardwareUtils.GetProbeLiseConfigFromID(_hardwareManager, Input.LiseData.ProbeID);
            var analysisParams = new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef, probeLiseConfig.PeakInfluence);
            var probeResults = LiseMeasurement.DoMeasure(probeLise, acquisitionParams, analysisParams);
            Result.LayersThickness = probeResults.LayersThickness;
            Result.AirGap = probeResults.AirGap;
            Result.Timestamp = probeResults.Timestamp;
            Result.Name = Input.LiseData.Sample.Name;
            Result.Quality = probeResults.Quality;

            if (Configuration.IsAnyReportEnabled())
            {
                var filecsv = Path.Combine(ReportFolder, $"raw_{Input.LiseData.ProbeID}_G{gain.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}_Single.csv");
                LiseSignalReport.WriteRawSignalInCSVFormat(acquisitionParams.RawSignalAcquired, filecsv, LogHeader);
            }

            if (!LiseMeasurement.AreMandatoryMeasuresValid(Result.LayersThickness))
            {
                throw new Exception($"Some mandatory layer thicknesses could not be measured or with insufficient precision.");
            }
        }
    }
}
