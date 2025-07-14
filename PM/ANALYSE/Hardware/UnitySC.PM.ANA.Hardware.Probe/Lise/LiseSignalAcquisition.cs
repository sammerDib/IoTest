using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public static class LiseSignalAcquisition
    {
        public const int HighPrecisionMeasurement = 16;
        public const int LowPrecisionMeasurement = 1;

        public struct LiseAcquisitionParams
        {
            public LiseAcquisitionParams(double probeGain, int nbMeasuresAverage, ProbeSample sample = null)
            {
                Sample = sample;
                ProbeGain = probeGain;
                NbMeasuresAverage = nbMeasuresAverage;
                RawSignalAcquired = null;
            }

            public double ProbeGain;
            public ProbeSample Sample;
            public int NbMeasuresAverage;
            public ProbeLiseSignal RawSignalAcquired;

        }

        public struct DualLiseAcquisitionParams
        {
            public DualLiseAcquisitionParams(LiseAcquisitionParams liseUpAcquisitionParams, LiseAcquisitionParams liseDownAcquisitionParams)
            {
                LiseUpParams = liseUpAcquisitionParams;
                LiseDownParams = liseDownAcquisitionParams;
            }

            public LiseAcquisitionParams LiseUpParams;
            public LiseAcquisitionParams LiseDownParams;
        }

        public static ProbeLiseSignal AcquireRawSignal(IProbeLise probeLise, LiseAcquisitionParams acquisitionParams)
        {
            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            var probeInputParameters = new SingleLiseInputParams(acquisitionParams.Sample, acquisitionParams.ProbeGain, qualityThreshold, detectionThreshold, acquisitionParams.NbMeasuresAverage);

            var probeRawSignal = probeLise.DoSingleAcquisition(probeInputParameters) as ProbeLiseSignal;

            acquisitionParams.RawSignalAcquired = probeRawSignal;
            return probeRawSignal;
        }

        public static IEnumerable<ProbeLiseSignal> AcquireAverageRawSignal(IProbeLise probeLise, LiseAcquisitionParams acquisitionParams)
        {
            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            var probeInputParameters = new SingleLiseInputParams(acquisitionParams.Sample, acquisitionParams.ProbeGain, qualityThreshold, detectionThreshold, acquisitionParams.NbMeasuresAverage);

            var probeRawSignal = probeLise.DoMultipleAcquisitions(probeInputParameters).OfType<ProbeLiseSignal>();

            acquisitionParams.RawSignalAcquired = probeRawSignal.First();
            return probeRawSignal;
        }
    }
}
