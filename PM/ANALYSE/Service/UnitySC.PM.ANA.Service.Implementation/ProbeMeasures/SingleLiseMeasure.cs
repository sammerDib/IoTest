using System;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.Shared.Logger;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Implementation.ProbeMeasures
{
    public class SingleLiseMeasure : IProbeMeasure
    {
        private ILogger _logger;
        private ProbeLise _probeSingleLise;

        public SingleLiseMeasure(ProbeLise probeSingleLise, ILogger logger)
        {
            _probeSingleLise = probeSingleLise;
            _logger = logger;
        }

        public IProbeResult DoMeasure(IProbeInputParams inputParameters)
        {
            if (!(inputParameters is DualLiseInputParams))
            {
                throw new ArgumentException("inputParameters is not valid");
            }

            var input = inputParameters as SingleLiseInputParams;
            var result = new SingleLiseResult();

            switch (_probeSingleLise.Configuration)
            {
                case ProbeLiseConfig config when config.IsSimulated:
                    result = new SingleLiseResult();
                    foreach (var layer in input.ProbeSample.Layers)
                    {
                        result.LayersThickness.Add(new ProbeThicknessMeasure(layer.Thickness, 1));
                    }
                    break;

                case ProbeLiseConfig config when !config.IsSimulated:
                    var acquisitionParams = new LiseAcquisitionParams(input.Gain, HighPrecisionMeasurement, input.ProbeSample as ProbeSample);
                    var analysisParams = new LiseSignalAnalysisParams(config.Lag, config.DetectionCoef, config.PeakInfluence);
                    result = LiseMeasurement.DoMeasure((IProbeLise)_probeSingleLise, acquisitionParams, analysisParams);
                    break;
            }

            return result;
        }
    }
}
