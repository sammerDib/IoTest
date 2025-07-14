using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public class ProbeDualLise : ProbeBase, IProbeDualLise
    {
        public IProbeLise ProbeLiseUp { get; protected set; }
        public IProbeLise ProbeLiseDown { get; protected set; }

        public ProbeStatus Status { get; set; }


        private readonly ProbeDualLiseConfig _configDouble;
        protected readonly ProbeLiseConfig ConfigUp;
        protected readonly ProbeLiseConfig ConfigDown;

        public ProbeDualLise(ProbeDualLiseConfig configDouble, ProbeLiseConfig configUp, ProbeLiseConfig configDown, IProbeLiseWrapper wrapper = null, ILogger logger = null) : base(configDouble,logger)
        {
            _configDouble = configDouble;
            ConfigUp = configUp;
            ConfigDown = configDown;
        }

        public override void Init()
        {
            var logFacto =ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();
            var loggerProbeTop = logFacto.CreateHardwareLogger(_configDouble.LogLevel.ToString(), Family.ToString(), $"{DeviceID} {ConfigUp.ModulePosition}");
            var loggerProbeBot = logFacto.CreateHardwareLogger(_configDouble.LogLevel.ToString(), Family.ToString(), $"{DeviceID} {ConfigDown.ModulePosition}");

            Logger.Debug("Init device");
            ProbeLiseUp = new ProbeLise(ConfigUp, loggerProbeTop);
            ProbeLiseDown = new ProbeLise(ConfigDown, loggerProbeBot);

            ProbeLiseUp.Init();
            ProbeLiseDown.Init();

            Status = ProbeStatus.Initialized;
        }

        public override void Shutdown()
        {
            // TODO Assert we can call multiple times Shutdown and Dll close functions
            ProbeLiseUp?.Shutdown();
            ProbeLiseDown?.Shutdown();
        }

        public override IProbeSignal DoSingleAcquisition(IProbeInputParams inputParameters)
        {
            if (!(inputParameters is IDualLiseInputParams input))
            {
                throw new Exception("Invalid parameters.");
            }

            if (input.CurrentProbeModule.Equals(ModulePositions.Up))
            {
                return ProbeLiseUp.DoSingleAcquisition(inputParameters);
            }
            else
            {
                return ProbeLiseDown.DoSingleAcquisition(inputParameters);
            }
        }

        public override IEnumerable<IProbeSignal> DoMultipleAcquisitions(IProbeInputParams inputParameters)
        {
            if (!(inputParameters is DualLiseInputParams input))
            {
                throw new Exception("Invalid parameters.");
            }

            int nbAverage;
            if (input.CurrentProbeModule.Equals(ModulePositions.Up))
            {
                nbAverage = input.ProbeUpParams.NbMeasuresAverage;
            }
            else
            {
                nbAverage = input.ProbeDownParams.NbMeasuresAverage;
            }

            for (int i=0; i< nbAverage; ++i)
            {
                yield return DoSingleAcquisition(inputParameters);
            }
        }

        public override void StartContinuousAcquisition(IProbeInputParams inputParameters)
        {
            var input = inputParameters as IDualLiseInputParams;
            if (input == null)
            {
                throw new Exception("Invalid parameters.");
            }

            if (input.CurrentProbeModule.Equals(ModulePositions.Up))
            {
                ProbeLiseUp.StartContinuousAcquisition(input.ProbeUpParams);
            }
            else
            {
                ProbeLiseDown.StartContinuousAcquisition(input.ProbeDownParams);
            }
            Status = ProbeStatus.Acquisition;
        }

        public override void StopContinuousAcquisition()
        {
            if (ProbeLiseUp.Status == ProbeStatus.Acquisition)
            {
                ProbeLiseUp.StopContinuousAcquisition();
            }
            if (ProbeLiseDown.Status == ProbeStatus.Acquisition)
            {
                ProbeLiseDown.StopContinuousAcquisition();
            }
            Status = ProbeStatus.Initialized;
        }

        public void NotifyCalibrationResultsAvailable(ProbeDualLiseCalibResult probeCalibResults)
        {
            probeCalibResults.ProbeID = Configuration.DeviceID;
            Logger?.Information("NotifyCalibrationResultsAvailable - Begin of function");
            var probeServiceCallback = ClassLocator.Default.GetInstance<IProbeServiceCallbackProxy>();
            probeServiceCallback.ProbeCalibrationResultsCallback(probeCalibResults);
        }

        public override IProbeResult DoMeasure(IProbeInputParams inputParameters)
        {
            if (!(inputParameters is DualLiseInputParams))
            {
                throw new ArgumentException("inputParameters is not valid");
            }

            var input = inputParameters as DualLiseInputParams;
            var result = new LiseResult();

            ProbeDualLiseCalibResult probeLiseCalibResult = CalibrationManager.GetCalibration(true, DeviceID, inputParameters, null, null) as ProbeDualLiseCalibResult;

            bool isLayerUp = true;
            var layerUp = new List<ProbeSampleLayer>();
            var layerDown = new List<ProbeSampleLayer>();
            foreach (var layer in input.ProbeSample.Layers)
            {
                if (layer.RefractionIndex == 0)
                {
                    isLayerUp = false;
                }
                else if (isLayerUp)
                {
                    layerUp.Add(layer);
                }
                else
                {
                    layerDown.Add(layer);
                }
            }

            switch (Configuration)
            {
                case ProbeDualLiseConfig config when config.IsSimulated:
                    result = new SingleLiseResult();
                    foreach (var layer in input.ProbeSample.Layers)
                    {
                        result.LayersThickness.Add(new ProbeThicknessMeasure(layer.Thickness, 1));
                    }
                    break;

                case ProbeDualLiseConfig config when !config.IsSimulated:
                    var acquisitionUpParams = new LiseAcquisitionParams(input.ProbeUpParams.Gain, HighPrecisionMeasurement, new ProbeSample(layerUp, input.ProbeSample.Name, input.ProbeSample.Info));
                    var acquisitionDownParams = new LiseAcquisitionParams(input.ProbeDownParams.Gain, HighPrecisionMeasurement, new ProbeSample(layerDown, input.ProbeSample.Name, input.ProbeSample.Info));
                    var acquisitionParams = new DualLiseAcquisitionParams(acquisitionUpParams, acquisitionDownParams);

                    var configUp = config.ProbeUp as ProbeLiseConfig;
                    var configDown = config.ProbeDown as ProbeLiseConfig;
                    var analysisUpParams = new LiseSignalAnalysisParams(configUp.Lag, configUp.DetectionCoef, configUp.PeakInfluence);
                    var analysisDownParams = new LiseSignalAnalysisParams(configDown.Lag, configDown.DetectionCoef, configDown.PeakInfluence);

                    var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

                    result = LiseMeasurement.DoUnknownLayerMeasure(this, probeLiseCalibResult.GlobalDistance, acquisitionParams, unknownLayer);
                    break;
            }

            return result;
        }
    }
}
