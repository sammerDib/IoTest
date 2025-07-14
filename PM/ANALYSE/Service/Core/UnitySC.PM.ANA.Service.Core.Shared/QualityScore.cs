using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    public static class QualityScore
    {
        public static double ComputeQualityScoreOfGaussianSignal(List<double> signal)
        {
            if (signal.Count == 0)
            {
                return 0;
            }

            var min = signal.Min();
            var max = signal.Max();
            var dist = max - min;

            if (dist == 0)
            {
                return 0;
            }

            double confidence = 1.0 - (1 / dist);
            return confidence;
        }

        public static double ComputeQualityScoreOfAnalyzedSignal(LISESignalAnalyzed analyzedSignal)
        {
            if (analyzedSignal.RawValues == null || analyzedSignal.Means == null || analyzedSignal.StdDev == null || analyzedSignal.ReferencePeaks == null || analyzedSignal.SelectedPeaks == null)
            {
                return 0;
            }

            if (analyzedSignal.RawValues.Count == 0 || analyzedSignal.Means.Count == 0 || analyzedSignal.StdDev.Count == 0)
            {
                return 0;
            }

            if (analyzedSignal.ReferencePeaks.Count == 0 || analyzedSignal.SelectedPeaks.Count == 0)
            {
                return 0;
            }

            double minBaseSignal = analyzedSignal.Means.Average() - analyzedSignal.StdDev.Average();

            int peaksNb = analyzedSignal.ReferencePeaks.Count + analyzedSignal.SelectedPeaks.Count;

            double normalizationValue = analyzedSignal.SaturationValue - minBaseSignal;
            if (normalizationValue == 0)
            {
                return 0;
            }

            double peaksSignificance = 0;
            foreach (var refPeak in analyzedSignal.ReferencePeaks)
            {
                peaksSignificance += refPeak.Y - minBaseSignal;
            }
            foreach (var interestPeak in analyzedSignal.SelectedPeaks)
            {
                peaksSignificance += interestPeak.Y - minBaseSignal;
            }
            double peaksQualityNormalized = peaksSignificance / (peaksNb * normalizationValue);

            return peaksQualityNormalized;
        }

        public static double ComputeQualityScore(LISESignalAnalyzed analyzedSignal, int peakOfInterestIndex, ProbeLiseConfig config)
        {
            bool nullsignal = analyzedSignal.RawValues == null || analyzedSignal.Means == null || analyzedSignal.StdDev == null || analyzedSignal.ReferencePeaks == null || analyzedSignal.SelectedPeaks == null || analyzedSignal.RawValues.Count == 0 || analyzedSignal.Means.Count == 0 || analyzedSignal.StdDev.Count == 0;
            if (nullsignal)
            {
                return 0;
            }

            bool invalidSignal = analyzedSignal.ReferencePeaks.Count != 1 || analyzedSignal.SelectedPeaks.Count < (peakOfInterestIndex + 1);
            if (invalidSignal)
            {
                return 0;
            }

            var refPeak = analyzedSignal.ReferencePeaks[0];
            var selectedPeak = analyzedSignal.SelectedPeaks[peakOfInterestIndex];
            var peaksOfInterest = new List<Peak>() { refPeak, selectedPeak };

            var unsaturationRate = 1 - calculateSaturationRate(peaksOfInterest, config.SaturationValue);

            var dynamiqueRange = calculateDynamiqueRange(peaksOfInterest, analyzedSignal);

            var distantPeakRate = calculateDistantPeakRate(analyzedSignal, peakOfInterestIndex, config.DiscriminationDistanceInTheAir);

            var peaksQualityNormalized = 0.4 * unsaturationRate + 0.4 * distantPeakRate + 0.2 * dynamiqueRange;

            return peaksQualityNormalized;
        }

        public static double AggregateLayersQuality(List<ProbeThicknessMeasure> layersThickness)
        {
            var quality = 0.0;
            foreach (var layerThickness in layersThickness)
            {
                quality += layerThickness.Quality;
            }

            return quality / layersThickness.Count;
        }

        public static double ComputeQualityScore(LISESignalAnalyzed analyzedSignal, ProbeLiseConfig config)
        {
            bool nullsignal = analyzedSignal.RawValues == null || analyzedSignal.Means == null || analyzedSignal.StdDev == null || analyzedSignal.ReferencePeaks == null || analyzedSignal.SelectedPeaks == null || analyzedSignal.RawValues.Count == 0 || analyzedSignal.Means.Count == 0 || analyzedSignal.StdDev.Count == 0;
            if (nullsignal)
            {
                return 0;
            }

            bool invalidSignal = analyzedSignal.ReferencePeaks.Count != 1;
            if (invalidSignal)
            {
                return 0;
            }

            var peaksOfInterest = new List<Peak>() { analyzedSignal.ReferencePeaks[0] };
            peaksOfInterest.Concat(analyzedSignal.SelectedPeaks);

            var unsaturationRate = 1 - calculateSaturationRate(peaksOfInterest, config.SaturationValue);

            var dynamiqueRange = calculateDynamiqueRange(peaksOfInterest, analyzedSignal);

            var distantPeakRates = new List<double>();
            for (int i = 0; i < analyzedSignal.SelectedPeaks.Count; i++)
            {
                distantPeakRates.Add(calculateDistantPeakRate(analyzedSignal, i, config.DiscriminationDistanceInTheAir));
            }
            var distantPeakRate = distantPeakRates.Average();

            var peaksQualityNormalized = 0.4 * unsaturationRate + 0.4 * distantPeakRate + 0.2 * dynamiqueRange;

            return peaksQualityNormalized;
        }

        private static double calculateSaturationRate(List<Peak> peaksOfInterest, double saturationValue)
        {
            double numberOfInterestPeaks = peaksOfInterest.Count;
            double numberOfSaturatedPeaks = 0;

            foreach (var peak in peaksOfInterest)
            {
                if (peak.Y >= saturationValue)
                {
                    numberOfSaturatedPeaks++;
                }
            }

            return numberOfSaturatedPeaks / numberOfInterestPeaks;
        }

        private static double calculateDynamiqueRange(List<Peak> peaksOfInterest, LISESignalAnalyzed analyzedSignal)
        {
            var cumulativeAmplitude = 0.0;
            foreach (var peak in peaksOfInterest)
            {
                cumulativeAmplitude += peak.Y;
            }
            var meanAmplitude = cumulativeAmplitude / peaksOfInterest.Count;

            var meanNoise = analyzedSignal.Means.Average();
            var meanStdDev = analyzedSignal.StdDev.Average();
            var dynamiqueRange = meanAmplitude - meanNoise - meanStdDev;

            var maxSignalValue = analyzedSignal.SaturationValue;
            var minSignalValue = -5;
            var normalizedDynamiqueRange = dynamiqueRange / (maxSignalValue - minSignalValue);

            return normalizedDynamiqueRange;
        }

        private static double calculateDistantPeakRate(LISESignalAnalyzed analyzedSignal, int peakOfInterestIndex, Length discriminationDistanceInMicrometers)
        {
            var distanceToReachOnSignal = discriminationDistanceInMicrometers.Micrometers / (analyzedSignal.StepX / 1000); // conversion of dist μm to x coordinate

            if (peakOfInterestIndex - 1 >= 0)
            {
                var dist = analyzedSignal.SelectedPeaks[peakOfInterestIndex].X - analyzedSignal.SelectedPeaks[peakOfInterestIndex - 1].X;
                if (dist <= distanceToReachOnSignal)
                {
                    return 0;
                }
            }

            if (peakOfInterestIndex + 1 < analyzedSignal.SelectedPeaks.Count)
            {
                var dist = analyzedSignal.SelectedPeaks[peakOfInterestIndex + 1].X - analyzedSignal.SelectedPeaks[peakOfInterestIndex].X;
                if (dist <= distanceToReachOnSignal)
                {
                    return 0;
                }
            }

            return 1;
        }
    }
}
