using System;
using System.IO;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.Thickness
{
    public class LiseAirGapFlow : FlowComponent<AirGapLiseInput, AirGapLiseResult, AirGapLiseConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        public LiseAirGapFlow(AirGapLiseInput input) : base(input, "LiseAirGapFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
        }

        protected override void Process()
        {
            LiseAutofocusParameters autofocusLiseParams = null;
            try
            {
                autofocusLiseParams = HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, Input.LiseData.ProbeID);
            }
            catch (Exception ex)
            {
                Logger.Information(ex.Message);
            }

            if (double.IsNaN(Input.LiseData.Gain) && autofocusLiseParams == null)
            {
                throw new Exception($"Autofocus LISE parameters are missing and Gain is not defined");
            }

            double gain = double.IsNaN(Input.LiseData.Gain) ? Math.Round((autofocusLiseParams.MinGain + autofocusLiseParams.MaxGain) / 2, 2) : Input.LiseData.Gain;

            var probeLise = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.LiseData.ProbeID);

            var acquisitionParams = new LiseAcquisitionParams(gain, Input.LiseData.NbAveraging);
            var probeLiseConfig = HardwareUtils.GetProbeLiseConfigFromID(_hardwareManager, Input.LiseData.ProbeID);
            var analysisParams = new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef, probeLiseConfig.PeakInfluence);

            var probeResult = LiseMeasurement.DoAirGap(probeLise, acquisitionParams, analysisParams);
            Result.AirGap = probeResult.AirGap;
            Result.Quality = probeResult.Quality;

            if (Configuration.IsAnyReportEnabled())
            {
                var filecsv = Path.Combine(ReportFolder, $"raw_{Input.LiseData.ProbeID}_G{gain.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}_airGap.csv");
                LiseSignalReport.WriteRawSignalInCSVFormat(acquisitionParams.RawSignalAcquired, filecsv, LogHeader);
            }

            if(Result.AirGap == null)
            {
                throw new Exception($"The AirGap measure fails : {Result.AirGap}");
            }

            Logger.Information($"{LogHeader} The AirGap measure successful : {Result.AirGap}");
        }
    }
}
