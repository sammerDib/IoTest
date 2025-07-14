using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.ANA.Hardware.Shared.Algos;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;

namespace UnitySC.PM.ANA.Service.Core.Test.QualityScoreComputation
{
    [TestClass]
    public class AnalyzedSignalQualityTest
    {
        private const int LiseSignalLength = 30000;
        private const int RefPeakArbitraryPosition = 1400;
        private const int FirstPeakArbitraryPosition = 5000;
        private const float GeometricToNanometerRatio = 1;
        private const float SaturationValue = 6;

        private LISESignalAnalyzed _validSignalAnalyzed;

        [TestInitialize]
        public void Init()
        {
            const double maxAmplitude = 5;

            var refPeak = new Peak(RefPeakArbitraryPosition, maxAmplitude);
            var firstPeak = new Peak(FirstPeakArbitraryPosition, maxAmplitude);
            var rawSignal = LiseTestUtils.CreateLiseSignalFromPeaks(new List<Peak> { refPeak, firstPeak }, GeometricToNanometerRatio, LiseSignalLength);
            _validSignalAnalyzed = LiseTestUtils.CreateLISESignalAnalyzed(rawSignal, new List<Peak> { refPeak }, new List<Peak> { firstPeak }, SaturationValue);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_with_one_peak_of_interest_is_good_when_signal_is_good()
        {
            // Given
            var rawSignal = new ProbeLiseSignal() { RawValues = LiseTestUtils.CreateLiseRawSignalFromCSV("autofocusLISE_signal_at_gain_1.5_with_optimal_signal_AFLise.csv"), SaturationValue = 6.9F, StepX = 0.382375F * 1000 };
            var analyzer = new LiseSignalAnalyzer();
            LiseSignalAnalysisParams analysisParams = new LiseSignalAnalysisParams(1000, 100, 0);
            var signalAnalyzed = analyzer.AnalyzeRawSignal(rawSignal, analysisParams);
            var config = new ProbeLiseConfig() { SaturationValue = 6.7F, DiscriminationDistanceInTheAir = 40.Micrometers() };

            // When
            var peakOfInterestIndex = 0;
            double qualityScore = QualityScore.ComputeQualityScore(signalAnalyzed, peakOfInterestIndex, config);

            // Then
            Assert.IsTrue(qualityScore > 0.95);
            Assert.IsTrue(qualityScore < 1);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_with_one_peak_of_interest_is_better_when_peaks_are_higher()
        {
            // Given
            var rawSignal = new ProbeLiseSignal() { RawValues = LiseTestUtils.CreateLiseRawSignalFromCSV("autofocusLISE_signal_at_gain_1.5_with_optimal_signal_AFLise.csv"), SaturationValue = 6.9F, StepX = 0.382375F * 1000 };
            var rawSignalWithLowerPeaks = new ProbeLiseSignal() { RawValues = LiseTestUtils.CreateLiseRawSignalFromCSV("autofocusLISE_signal_at_gain_1.1_with_low_peaks_AFLise.csv"), SaturationValue = 6.9F, StepX = 0.382375F * 1000 };
            var analyzer = new LiseSignalAnalyzer();
            LiseSignalAnalysisParams analysisParams = new LiseSignalAnalysisParams(1000, 100, 0);
            var signalAnalyzed = analyzer.AnalyzeRawSignal(rawSignal, analysisParams);
            var signalAnalyzedWithLowerPeaks = analyzer.AnalyzeRawSignal(rawSignalWithLowerPeaks, analysisParams);
            var config = new ProbeLiseConfig() { SaturationValue = 6.7F, DiscriminationDistanceInTheAir = 40.Micrometers() };

            // When
            var peakOfInterestIndex = 0;
            double qualityScore = QualityScore.ComputeQualityScore(signalAnalyzed, peakOfInterestIndex, config);
            double qualityScoreOfSignalWithLowerPeaks = QualityScore.ComputeQualityScore(signalAnalyzedWithLowerPeaks, peakOfInterestIndex, config);

            // Then
            Assert.IsTrue(qualityScore > qualityScoreOfSignalWithLowerPeaks);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_with_one_peak_of_interest_is_better_when_peaks_are_not_saturated()
        {
            // Given
            var rawSignal = new ProbeLiseSignal() { RawValues = LiseTestUtils.CreateLiseRawSignalFromCSV("autofocusLISE_signal_at_gain_1.5_with_optimal_signal_AFLise.csv"), SaturationValue = 6.9F, StepX = 0.16F * 1000 };
            var rawSignalWithSaturatedPeaks = new ProbeLiseSignal() { RawValues = LiseTestUtils.CreateLiseRawSignalFromCSV("autofocusLISE_signal_at_gain_1.7_with_saturated_peak_AFLise.csv"), SaturationValue = 6.9F, StepX = 0.16F * 1000 };
            var analyzer = new LiseSignalAnalyzer();
            LiseSignalAnalysisParams analysisParams = new LiseSignalAnalysisParams(1000, 100, 0);
            var signalAnalyzed = analyzer.AnalyzeRawSignal(rawSignal, analysisParams);
            var signalAnalyzedWithSaturatedPeaks = analyzer.AnalyzeRawSignal(rawSignalWithSaturatedPeaks, analysisParams);
            var config = new ProbeLiseConfig() { SaturationValue = 6.7F, DiscriminationDistanceInTheAir = 40.Micrometers() };

            // When
            var peakOfInterestIndex = 0;
            double qualityScore = QualityScore.ComputeQualityScore(signalAnalyzed, peakOfInterestIndex, config);
            double qualityScoreOfSignalWithSaturatedPeaks = QualityScore.ComputeQualityScore(signalAnalyzedWithSaturatedPeaks, peakOfInterestIndex, config);

            // Then
            Assert.IsTrue(qualityScore > qualityScoreOfSignalWithSaturatedPeaks);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_with_one_peak_of_interest_is_better_when_peaks_are_not_too_close()
        {
            // Given
            var rawSignal = new ProbeLiseSignal() { RawValues = LiseTestUtils.CreateLiseRawSignalFromCSV("autofocusLISE_signal_at_gain_1.5_with_optimal_signal_AFLise.csv"), SaturationValue = 6.9F, StepX = 0.382375F * 1000 };
            var rawSignalWithTooClosePeaks = new ProbeLiseSignal() { RawValues = LiseTestUtils.CreateLiseRawSignalFromCSV("autofocusLISE_signal_at_gain_1.5_with_peaks_too_close_AFLise.csv"), SaturationValue = 6.9F, StepX = 0.382375F * 1000 };
            var analyzer = new LiseSignalAnalyzer();
            LiseSignalAnalysisParams analysisParams = new LiseSignalAnalysisParams(1000, 100, 0);
            var signalAnalyzed = analyzer.AnalyzeRawSignal(rawSignal, analysisParams);
            var signalAnalyzedWithTooClosePeaks = analyzer.AnalyzeRawSignal(rawSignalWithTooClosePeaks, analysisParams);
            var config = new ProbeLiseConfig() { SaturationValue = 6.7F, DiscriminationDistanceInTheAir = 40.Micrometers() };

            // When
            var peakOfInterestIndex = 0;
            double qualityScore = QualityScore.ComputeQualityScore(signalAnalyzed, peakOfInterestIndex, config);
            double qualityScoreOfSignalWithTooClosePeaks = QualityScore.ComputeQualityScore(signalAnalyzedWithTooClosePeaks, peakOfInterestIndex, config);

            // Then
            Assert.IsTrue(qualityScore > qualityScoreOfSignalWithTooClosePeaks);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_equals_zero_when_signal_contains_nothing()
        {
            // Given : Analyzed signal without peak
            var signalAnalyzed = new LISESignalAnalyzed();

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signalAnalyzed);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_equals_zero_when_signal_means_is_empty()
        {
            // Given
            var signalAnalyzed = _validSignalAnalyzed;
            signalAnalyzed.Means = new List<double>();

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signalAnalyzed);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_equals_zero_when_signal_stddev_is_empty()
        {
            // Given
            var signalAnalyzed = _validSignalAnalyzed;
            signalAnalyzed.StdDev = new List<double>();

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signalAnalyzed);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_equals_zero_when_signal_is_empty()
        {
            // Given
            var signalAnalyzed = _validSignalAnalyzed;
            signalAnalyzed.RawValues = new List<double>();

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signalAnalyzed);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_equals_zero_when_no_reference_peaks_are_detected()
        {
            // Given : Analyzed
            var signalAnalyzed = _validSignalAnalyzed;
            signalAnalyzed.ReferencePeaks = new List<Peak>();

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signalAnalyzed);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_equals_zero_when_no_selected_peaks_are_detected()
        {
            // Given
            var signalAnalyzed = _validSignalAnalyzed;
            signalAnalyzed.SelectedPeaks = new List<Peak>();

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signalAnalyzed);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_is_positive_less_than_1_when_peaks_are_negative()
        {
            // Given : Analyzed signal with peaks
            const double maxAmplitude = -1;
            var refPeak = new Peak(RefPeakArbitraryPosition, maxAmplitude);
            var firstPeak = new Peak(FirstPeakArbitraryPosition, maxAmplitude);
            var rawSignal = LiseTestUtils.CreateLiseSignalFromPeaks(new List<Peak> { refPeak, firstPeak }, GeometricToNanometerRatio, LiseSignalLength);
            var signalAnalyzed = LiseTestUtils.CreateLISESignalAnalyzed(rawSignal, new List<Peak> { refPeak }, new List<Peak> { firstPeak }, SaturationValue);

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signalAnalyzed);

            // Then : Quality score is higher than 0.9 without exceeding the maximum value of 1
            Assert.IsTrue(qualityScore > 0);
            Assert.IsTrue(qualityScore < 1);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_is_positive_less_than_1_when_peaks_are_positive()
        {
            // Given : Analyzed signal with peaks
            const double maxAmplitude = 1;
            var refPeak = new Peak(RefPeakArbitraryPosition, maxAmplitude);
            var firstPeak = new Peak(FirstPeakArbitraryPosition, maxAmplitude);
            var rawSignal = LiseTestUtils.CreateLiseSignalFromPeaks(new List<Peak> { refPeak, firstPeak }, GeometricToNanometerRatio, LiseSignalLength);
            var signalAnalyzed = LiseTestUtils.CreateLISESignalAnalyzed(rawSignal, new List<Peak> { refPeak }, new List<Peak> { firstPeak }, SaturationValue);

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signalAnalyzed);

            // Then : Quality score is higher than 0.9 without exceeding the maximum value of 1
            Assert.IsTrue(qualityScore > 0);
            Assert.IsTrue(qualityScore < 1);
        }

        [TestMethod]
        public void Quality_score_of_analyzed_signal_increase_when_peaks_are_higher()
        {
            // Given : Process autofocus on the raw signal with higher and higher amplitude peaks
            var signalAnalyzedWithHigherAndHigherPeaksAmplitude = new List<LISESignalAnalyzed>();
            for (int i = 5; i > 0; i--)
            {
                double amplitude = SaturationValue - i;
                var refPeak = new Peak(RefPeakArbitraryPosition, amplitude);
                var firstPeak = new Peak(FirstPeakArbitraryPosition, amplitude);
                var rawSignal = LiseTestUtils.CreateLiseSignalFromPeaks(new List<Peak> { refPeak, firstPeak }, GeometricToNanometerRatio, LiseSignalLength);
                var signalAnalyzed = LiseTestUtils.CreateLISESignalAnalyzed(rawSignal, new List<Peak> { refPeak }, new List<Peak> { firstPeak }, SaturationValue);
                signalAnalyzedWithHigherAndHigherPeaksAmplitude.Add(signalAnalyzed);
            }

            // When : Compute quality score
            var qualityOfSignalAnalyzedWithHigherAndHigherPeaksAmplitude = new List<double>();
            foreach (var signal in signalAnalyzedWithHigherAndHigherPeaksAmplitude)
            {
                double qualityScore = QualityScore.ComputeQualityScoreOfAnalyzedSignal(signal);
                qualityOfSignalAnalyzedWithHigherAndHigherPeaksAmplitude.Add(qualityScore);
            }

            // Then : Quality score increase when peaks are higher
            double previousQuality = 0;
            foreach (double quality in qualityOfSignalAnalyzedWithHigherAndHigherPeaksAmplitude)
            {
                Assert.IsTrue(previousQuality < quality);
                previousQuality = quality;
            }
        }
    }
}
