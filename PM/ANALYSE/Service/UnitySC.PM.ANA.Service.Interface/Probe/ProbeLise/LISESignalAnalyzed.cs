using System.Collections.Generic;
using System.Linq;

namespace UnitySC.PM.ANA.Service.Interface.ProbeLise
{
    public class LISESignalAnalyzed : IProbeSignal
    {
        public LISESignalAnalyzed()
        {
            StdDev = new List<double>();
            Means = new List<double>();
            RawValues = new List<double>();
            ReferencePeaks = new List<Peak>();
            SelectedPeaks = new List<Peak>();
            DiscardedPeaks = new List<Peak>();
            SaturationValue = double.NaN;
            StepX = double.NaN;

            SignalStatus = SignalAnalysisStatus.InvalidAnalyzedSignal;
        }

        public LISESignalAnalyzed(ProbeLiseSignal signal)
        {
            StdDev = signal.StdDev;
            Means = signal.Means;
            RawValues = signal.RawValues.ToList();
            ReferencePeaks = new List<Peak>();
            foreach (var peak in signal.ReferencePeaks)
            {
                ReferencePeaks.Add(new Peak(peak.X, peak.Y));
            }
            SelectedPeaks = new List<Peak>();
            foreach (var peak in signal.SelectedPeaks)
            {
                SelectedPeaks.Add(new Peak(peak.X, peak.Y));
            }
            DiscardedPeaks = new List<Peak>();
            foreach (var peak in signal.DiscardedPeaks)
            {
                DiscardedPeaks.Add(new Peak(peak.X, peak.Y));
            }
            SaturationValue = signal.SaturationValue;
            StepX = signal.StepX;

            if (RawValues.Count == 0)
            {
                SignalStatus = SignalAnalysisStatus.InvalidRawSignal;
            } else
            {
                SignalStatus = SignalAnalysisStatus.Valid;
            }
        }

        public LISESignalAnalyzed Clone()
        {
            var signalAnalyzedCopied = new LISESignalAnalyzed
            {
                SaturationValue = SaturationValue,
                StepX = StepX,
                SignalStatus = SignalStatus,
                RawValues = RawValues.Select(item => item).ToList(),
                Means = Means.Select(item => item).ToList(),
                StdDev = StdDev.Select(item => item).ToList(),
                ReferencePeaks = ReferencePeaks.Select(item => item).ToList(),
                SelectedPeaks = SelectedPeaks.Select(item => item).ToList()
            };

            return signalAnalyzedCopied;
        }

        public enum SignalAnalysisStatus
        {
            InvalidRawSignal,
            InvalidAnalyzedSignal,
            Valid
        }

        public List<double> RawValues { get; set; }
        public List<double> Means { get; set; }
        public List<double> StdDev { get; set; }
        public double SaturationValue { get; set; }
        public double StepX { get; set; }
        public List<Peak> ReferencePeaks { get; set; }
        public List<Peak> SelectedPeaks { get; set; }
        public List<Peak> DiscardedPeaks { get; set; }
        public SignalAnalysisStatus SignalStatus { get; set; }
        public string ProbeID { get; set; }
    }
}
