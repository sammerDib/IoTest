using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public class ProbeLise : ProbeBase, IProbeLise
    {
        public ProbeLise() : base(new ProbeLiseConfig(),null)
        {

        }
        public ProbeLiseDllProxy ProbeLiseDllProxy { get; set; }
        public ProbeStatus Status { get; set; }


        public ProbeLise(IProbeConfig config, ILogger logger=null) : base(config, logger)
        {
            ProbeLiseDllProxy = ProbeLiseDllProxy.Instance;
        }

        public override void Init()
        {
            Logger.Debug("Init probe");
            string configTextFilePath = ""; // Use of txt files in dll is disable here. To enable it, use "DoubleLISEED.txt" instead of empty string
            ProbeLiseDllProxy.InitProbeLiseDll(configTextFilePath, Configuration as ProbeLiseConfig);
        }

        public override void Shutdown()
        {
            ProbeLiseDllProxy?.Shutdown();
        }

        public override IProbeSignal DoSingleAcquisition(IProbeInputParams inputParams)
        {
            var signal = ProbeLiseDllProxy.DoSingleAcquisition(Configuration, inputParams);
            NotifySignalUpdated(signal.First());
            return signal.First();
        }

        public override IEnumerable<IProbeSignal> DoMultipleAcquisitions(IProbeInputParams inputParams)
        {
            var signals = ProbeLiseDllProxy.DoSingleAcquisition(Configuration, inputParams);
            NotifySignalUpdated(signals.First());
            return signals;
        }

        public override void StartContinuousAcquisition(IProbeInputParams inputParams)
        {
            ProbeLiseDllProxy.StartContinuousAcquisition(Configuration, inputParams, NotifySignalUpdated);
        }

        public override void StopContinuousAcquisition()
        {
            ProbeLiseDllProxy.StopContinuousAcquisition();
        }

        private void NotifySignalUpdated(ProbeLiseSignal newProbeSignal)
        {
            var probeServiceCallback = ClassLocator.Default.GetInstance<IProbeServiceCallbackProxy>();
            probeServiceCallback.ProbeRawMeasuresCallback(ShrinkProbeLiseSignal(ref newProbeSignal));
        }

        private ProbeLiseSignal ShrinkProbeLiseSignal(ref ProbeLiseSignal probeLiseSignal)
        {
            if (probeLiseSignal == null || probeLiseSignal.RawValues == null || probeLiseSignal.RawValues.Count == 0)
                return null;

            var newProbeSignal = (ProbeLiseSignal)probeLiseSignal.Clone();
            newProbeSignal.RawValues = newProbeSignal.RawValues.Take(newProbeSignal.RawValues.Count / 2).ToList();
            var lastXValue_um = newProbeSignal.RawValues.Count * 1000.0 / newProbeSignal.StepX; //StepX should be in Namometer usually
            newProbeSignal.ReferencePeaks = newProbeSignal.ReferencePeaks?.Where(pt => (pt.X <= lastXValue_um)).ToList();
            newProbeSignal.DiscardedPeaks = newProbeSignal.DiscardedPeaks?.Where(pt => (pt.X <= lastXValue_um)).ToList();
            newProbeSignal.SelectedPeaks = newProbeSignal.SelectedPeaks?.Where(pt => (pt.X <= lastXValue_um)).ToList();
            return newProbeSignal;
        }

        public override IProbeResult DoMeasure(IProbeInputParams inputParameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
