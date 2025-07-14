using System;
using System.Collections.Generic;
using System.Linq;

using UnitySCSharedAlgosOpenCVWrapper;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;

namespace UnitySC.PM.ANA.Hardware.Shared.Algos
{
    public class LiseSignalAnalyzer
    {
        private const int RefPeakMinRange = 0;
        private const int RefPeakMaxRange = 1500;
        private const int MatchLimitBetweenPeak = 100;

        public struct SubLiseSignalAnalyzed
        {
            public double[] Signal { get; set; }
            public double[] Means { get; set; }
            public double[] StdDev { get; set; }

            public List<Peak> Peaks;
        }

        public struct LiseSignalSubParted
        {
            public int LagAtStart; // the removed start of the unusable LISE signal
            public SubLiseSignalAnalyzed GoingsPart;
            public SubLiseSignalAnalyzed ComingsPart;
        }

        public virtual LISESignalAnalyzed AnalyzeRawSignal(ProbeLiseSignal probeRawSignal, LiseSignalAnalysisParams analysisParameters)
        {
            if (probeRawSignal == null)
            {
                var defaultAnalyzedSignal = new LISESignalAnalyzed();
                defaultAnalyzedSignal.SignalStatus = LISESignalAnalyzed.SignalAnalysisStatus.InvalidRawSignal;
                return defaultAnalyzedSignal;
            }

            if (probeRawSignal.RawValues.Count == 0)
            {
                var defaultAnalyzedSignal = new LISESignalAnalyzed(probeRawSignal);
                defaultAnalyzedSignal.SignalStatus = LISESignalAnalyzed.SignalAnalysisStatus.InvalidRawSignal;
                return defaultAnalyzedSignal;
            }

            var subPartedSignal = AnalyzeSubPartsOfSignal(probeRawSignal, analysisParameters);
            var analyzedSignal = AnalyzeSignalFromItsSubparts(probeRawSignal, subPartedSignal, analysisParameters);

            if (analyzedSignal.ReferencePeaks.Count != 1)
            {
                analyzedSignal.SignalStatus = LISESignalAnalyzed.SignalAnalysisStatus.InvalidAnalyzedSignal;
                return analyzedSignal;
            }

            analyzedSignal.SignalStatus = LISESignalAnalyzed.SignalAnalysisStatus.Valid;
            return analyzedSignal;
        }

        public virtual LISESignalAnalyzed AnalyzeRawSignalAccordingSample(ProbeLiseSignal probeRawSignal, LiseSignalAnalysisAccordingSampleParams analysisParameters, ProbeSample sample)
        {
            var analyzedSignal = AnalyzeRawSignal(probeRawSignal, analysisParameters.AnalysisParams);

            var potentialCombinationsOfPeaks = determineAllCombinaisonsOfPeaksThatFitWithLayers(analyzedSignal, sample.Layers, analysisParameters.AcceptanceTreeshold);

            var fittedPeaks = determineBestPeaksCombinaison(analyzedSignal, sample.Layers, potentialCombinationsOfPeaks);

            List<Peak> selectedPeaks = new List<Peak>();
            List<Peak> discardedPeaks = new List<Peak>();

            for (int i = 0; i < fittedPeaks.Count; i++)
            {
                if (fittedPeaks[i])
                {
                    selectedPeaks.Add(analyzedSignal.SelectedPeaks[i]);
                }
                else
                {
                    discardedPeaks.Add(analyzedSignal.SelectedPeaks[i]);
                }
            }

            analyzedSignal.SelectedPeaks = selectedPeaks;
            analyzedSignal.DiscardedPeaks = discardedPeaks;

            return analyzedSignal;
        }

        private List<List<bool>> determineAllCombinaisonsOfPeaksThatFitWithLayers(LISESignalAnalyzed analyzedSignal, List<ProbeSampleLayer> layers, Length acceptanceTreeshold)
        {
            var potentialCombinationsOfFittedPeaks = new List<List<bool>>();

            for (int initialPeak = 0; initialPeak < analyzedSignal.SelectedPeaks.Count() - layers.Count; initialPeak++)
            {
                var currentPeakID = initialPeak;

                var fittedPeaks = Enumerable.Repeat(false, analyzedSignal.SelectedPeaks.Count()).ToList();

                for (int layerID = 0; layerID < layers.Count; layerID++)
                {
                    if (layers[layerID].RefractionIndex == 0 || double.IsNaN(layers[layerID].RefractionIndex))
                    {
                        break;
                    }

                    bool layerAlreadyMeasured = false;

                    for (int outputPeakID = currentPeakID + 1; outputPeakID < analyzedSignal.SelectedPeaks.Count; outputPeakID++)
                    {
                        if (layerAlreadyMeasured)
                        {
                            break;
                        }

                        var entryPeak = analyzedSignal.SelectedPeaks[currentPeakID];
                        var outputPeak = analyzedSignal.SelectedPeaks[outputPeakID];

                        double opticalDist = outputPeak.X - entryPeak.X;
                        double geometricDist = opticalDist / layers[layerID].RefractionIndex;
                        var thicknessMeasured = (geometricDist * analyzedSignal.StepX).Nanometers();

                        var diffBetweenActualAndExpected = Math.Abs(thicknessMeasured.Millimeters - layers[layerID].Thickness.Millimeters);
                        bool peaksCorrespondToTheExpectedLayer = diffBetweenActualAndExpected <= acceptanceTreeshold.Millimeters;
                        if (peaksCorrespondToTheExpectedLayer)
                        {
                            layerAlreadyMeasured = true;
                            fittedPeaks[currentPeakID] = true;
                            fittedPeaks[outputPeakID] = true;
                            currentPeakID = outputPeakID;
                        }
                    }
                }
                potentialCombinationsOfFittedPeaks.Add(fittedPeaks);
            }
            return potentialCombinationsOfFittedPeaks;
        }

        private List<bool> determineBestPeaksCombinaison(LISESignalAnalyzed analyzedSignal, List<ProbeSampleLayer> layers, List<List<bool>> potentialCombinationsOfPeaks)
        {
            var fittedPeaks = Enumerable.Repeat(false, analyzedSignal.SelectedPeaks.Count()).ToList();
            if(analyzedSignal == null || analyzedSignal.RawValues.Count == 0)
            {
                return fittedPeaks;
            }

            double maxAmplitudeSum = analyzedSignal.RawValues.Min();
            foreach (var combinationsOfPeaks in potentialCombinationsOfPeaks)
            {
                var expectedLayerNumber = 0;
                foreach (var layer in layers)
                {
                    if (layer.RefractionIndex == 0 || double.IsNaN(layer.RefractionIndex))
                    {
                        break;
                    }
                    expectedLayerNumber++;
                }

                var fittedPeakNumber = combinationsOfPeaks.Where(c => c).Count();

                if (fittedPeakNumber - 1 != expectedLayerNumber)
                {
                    continue;
                }

                double currentAmplitudeSum = 0;
                for (int i = 0; i < combinationsOfPeaks.Count; i++)
                {
                    if (combinationsOfPeaks[i])
                    {
                        currentAmplitudeSum += analyzedSignal.SelectedPeaks[i].Y;
                    }
                }
                if (currentAmplitudeSum > maxAmplitudeSum)
                {
                    fittedPeaks = combinationsOfPeaks;
                    maxAmplitudeSum = currentAmplitudeSum;
                }
            }
            return fittedPeaks;
        }

        public LiseSignalSubParted AnalyzeSubPartsOfSignal(ProbeLiseSignal probeRawSignal, LiseSignalAnalysisParams analysisParams)
        {
            var fullRawSignal = probeRawSignal.RawValues;

            int subSignalGoSize = fullRawSignal.Count / 2;
            var subPartGoOfSignal = fullRawSignal.GetRange(0, subSignalGoSize);

            int subSignalComeSize = fullRawSignal.Count - subSignalGoSize;
            var subPartComeOfSignal = fullRawSignal.GetRange(subSignalGoSize, subSignalComeSize);

            // Remove the start of the unusable LISE signal
            int lagAtStartOfSignal = 0;
            while (subPartGoOfSignal.Count() > 0 && subPartGoOfSignal.ElementAt(0) > 0)
            {
                subPartGoOfSignal.RemoveAt(0);
                lagAtStartOfSignal++;
            }

            // Reverse the "return part" of the signal to have the correct indexes
            subPartComeOfSignal.Reverse();

            // Signal analysis
            var goingsSignalAnalyzed = SpikeDetector.AnalyzeSignal(subPartGoOfSignal, analysisParams.Lag, analysisParams.DetectionCoef, analysisParams.Influence);
            var comingsSignalAnalyzed = SpikeDetector.AnalyzeSignal(subPartComeOfSignal, analysisParams.Lag, analysisParams.DetectionCoef, analysisParams.Influence);

            // Remove negatives peaks (LISE probe artifact between going and coming signal)
            var goingsSignalAnalyzedSpikes = goingsSignalAnalyzed.Spikes.Where(x => x.Type.Equals(SpikeType.Peak));
            var comingsSignalAnalyzedSpikes = comingsSignalAnalyzed.Spikes.Where(x => x.Type.Equals(SpikeType.Peak));

            var peaksOnGoSignal = new List<Peak>();
            foreach (Spike spike in goingsSignalAnalyzedSpikes)
            {
                peaksOnGoSignal.Add(new Peak(spike.Index, spike.Value));
            }

            var peaksOnComeSignal = new List<Peak>();
            foreach (Spike spike in comingsSignalAnalyzedSpikes)
            {
                peaksOnComeSignal.Add(new Peak(spike.Index, spike.Value));
            }

            return new LiseSignalSubParted
            {
                LagAtStart = lagAtStartOfSignal,
                GoingsPart = new SubLiseSignalAnalyzed
                {
                    Signal = subPartGoOfSignal.ToArray(),
                    Means = goingsSignalAnalyzed.Means,
                    StdDev = goingsSignalAnalyzed.Stddev,
                    Peaks = peaksOnGoSignal
                },
                ComingsPart = new SubLiseSignalAnalyzed
                {
                    Signal = subPartComeOfSignal.ToArray(),
                    Means = comingsSignalAnalyzed.Means,
                    StdDev = comingsSignalAnalyzed.Stddev,
                    Peaks = peaksOnComeSignal
                }
            };
        }

        private LISESignalAnalyzed AnalyzeSignalFromItsSubparts(ProbeLiseSignal rawSignal, LiseSignalSubParted subPartedSignal, LiseSignalAnalysisParams analysisParams)
        {
            var analyzedSignal = new LISESignalAnalyzed(rawSignal);

            var goingsSignalAnalyzed = subPartedSignal.GoingsPart;
            var comingsSignalAnalyzed = subPartedSignal.ComingsPart;

            // Find corresponding peaks and average of goings and comings signals for each peak
            var significantPeaks = new List<Peak>();
            for (int i = 0; i < goingsSignalAnalyzed.Peaks.Count(); i++)
            {
                var peakOfInterest = goingsSignalAnalyzed.Peaks.ElementAt(i);
                var potentialCorrespondingPeaks = comingsSignalAnalyzed.Peaks.Where(_ => Math.Abs(peakOfInterest.X - _.X) <= MatchLimitBetweenPeak).ToList();
                if (potentialCorrespondingPeaks.Count == 0)
                {
                    continue;
                }

                var averagedPeak = AverageCorrespondingPeaks(peakOfInterest, potentialCorrespondingPeaks);
                significantPeaks.Add(averagedPeak);
            }

            // Search for the reference peak among all peaks
            var potentialReferencePeaks = new List<Peak>();
            var potentialSelectedPeaks = new List<Peak>();

            foreach (var peak in significantPeaks)
            {
                bool isPotentialReferencePeak = peak.X > RefPeakMinRange && peak.X < RefPeakMaxRange;
                if (isPotentialReferencePeak)
                {
                    potentialReferencePeaks.Add(new Peak(subPartedSignal.LagAtStart + peak.X, peak.Y));
                }
                else
                {
                    potentialSelectedPeaks.Add(new Peak(subPartedSignal.LagAtStart + peak.X, peak.Y));
                }
            }

            var means = goingsSignalAnalyzed.Means.Concat(comingsSignalAnalyzed.Means.Reverse()).ToArray();
            var stddev = goingsSignalAnalyzed.StdDev.Concat(comingsSignalAnalyzed.StdDev.Reverse()).ToArray();
            analyzedSignal.Means = new double[subPartedSignal.LagAtStart].Concat(means).ToList();
            analyzedSignal.StdDev = new double[subPartedSignal.LagAtStart].Concat(stddev).ToList();
            analyzedSignal.ReferencePeaks = potentialReferencePeaks;
            analyzedSignal.SelectedPeaks = potentialSelectedPeaks;

            return analyzedSignal;
        }

        private Peak AverageCorrespondingPeaks(Peak peakOfInterest, List<Peak> potentialCorrespondingPeaks)
        {
            var averagedPeak = new Peak((peakOfInterest.X + potentialCorrespondingPeaks[0].X) / 2, (peakOfInterest.Y + potentialCorrespondingPeaks[0].Y) / 2);

            if (potentialCorrespondingPeaks.Count > 1)
            {
                foreach (var peak in potentialCorrespondingPeaks)
                {
                    bool isBetterCorrespondance = Math.Abs(peakOfInterest.X - (peakOfInterest.X + peak.X) / 2) < Math.Abs(peakOfInterest.X - averagedPeak.X);
                    if (isBetterCorrespondance)
                    {
                        averagedPeak.X = (peakOfInterest.X + peak.X) / 2;
                        averagedPeak.Y = (peakOfInterest.Y + peak.Y) / 2;
                    }
                }
            }
            return averagedPeak;
        }
    }
}
