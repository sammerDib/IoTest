using System;
using System.Collections.Generic;
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
    public class LiseMultipleThicknessMeasurementFlow : FlowComponent<MultipleMeasuresLiseInput, MultipleMeasuresLiseResult, MultipleMeasuresLiseConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        public LiseMultipleThicknessMeasurementFlow(MultipleMeasuresLiseInput input) : base(input, "LiseMultipleThicknessMeasurementFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
        }

        protected override void Process()
        {
            var probeLise = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.MeasureLise.ProbeID);
            var autofocusLiseParams = HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, Input.MeasureLise.ProbeID);

            if (double.IsNaN(Input.MeasureLise.Gain) && autofocusLiseParams == null)
            {
                throw new Exception($"Autofocus LISE parameters are missing and Gain is not defined");
            }

            double gain = double.IsNaN(Input.MeasureLise.Gain) ? Math.Round((autofocusLiseParams.MinGain + autofocusLiseParams.MaxGain) / 2, 2) : Input.MeasureLise.Gain;

            var rawSignals = Configuration.IsAnyReportEnabled() ? new List<IEnumerable<ProbeLiseSignal>>() : null; 

            var acquisitionParams = new LiseAcquisitionParams(gain, Input.MeasureLise.NbAveraging, Input.MeasureLise.Sample);
            var probeLiseConfig = HardwareUtils.GetProbeLiseConfigFromID(_hardwareManager, Input.MeasureLise.ProbeID);
            var analysisParams = new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef, probeLiseConfig.PeakInfluence);
            var probeResults = LiseMeasurement.DoMultipleMeasures(probeLise, acquisitionParams, analysisParams, Input.NbMeasures, rawSignals);
            var globalQuality = 0.0;
            foreach (var probeResult in probeResults)
            {
                Result.ProbeThicknessMeasures.Add(probeResult.LayersThickness);
                globalQuality += probeResult.Quality;
            }
            Result.Quality = globalQuality / probeResults.Count;

            if (Configuration.IsAnyReportEnabled())
            {
                int i = 1;
                foreach (var probeResult in rawSignals)
                {
                    var filecsv = Path.Combine(ReportFolder, $"raw_{Input.MeasureLise.ProbeID}_G{gain.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}_Multi{i}.csv");
                    LiseSignalReport.WriteRawSignalInCSVFormat(acquisitionParams.RawSignalAcquired, filecsv, LogHeader);
                    i++;
                }
            }

            foreach (var probeResult in probeResults)
            {
                if (!LiseMeasurement.AreMandatoryMeasuresValid(probeResult.LayersThickness))
                {
                    throw new Exception($"Some mandatory layer thicknesses could not be measured or with insufficient precision.");
                }
            }
        }
    }
}
