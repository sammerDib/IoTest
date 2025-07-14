using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.Units;
using UnitySC.PM.ANA.Hardware.Shared.Algos;
using UnitySC.PM.ANA.Service.Interface;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;

namespace UnitySC.PM.ANA.Hardware.UnitTests.Shared.Algos
{
    [TestClass]
    public class LiseSignalAnalysisTest
    {
        private LiseSignalAnalysisParams _algoParams;
        private LiseSignalAnalysisAccordingSampleParams _algoWithSampleParams;

        [TestInitialize]
        public void Init()
        {
            _algoParams = new LiseSignalAnalysisParams(1000, 9, 0);
            _algoWithSampleParams = new LiseSignalAnalysisAccordingSampleParams(_algoParams, 1.Micrometers());
        }

        [TestMethod]
        public void Analyze_raw_signal_nominal_case()
        {
            // Given
            int secondPeakPos = FirstPeakArbitraryPosition + 750;
            var rawSignal = CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition, FirstPeakArbitraryPosition, secondPeakPos }, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(rawSignal, _algoParams);

            // Then : All peaks are detected and global analysis succeeds
            Assert.AreEqual(1, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(2, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);

            Assert.AreEqual(RefPeakArbitraryPosition, signalAnalyzed.ReferencePeaks[0].X, 1);
            Assert.AreEqual(FirstPeakArbitraryPosition, signalAnalyzed.SelectedPeaks[0].X, 1);
            Assert.AreEqual(secondPeakPos, signalAnalyzed.SelectedPeaks[1].X, 1);

            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzed.SignalStatus);
        }

        [TestMethod]
        public void Analyze_raw_signal_according_sample_nominal_case()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample);

            // Then : All peaks are detected and global analysis succeeds
            Assert.AreEqual(1, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(3, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);

            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzed.SignalStatus);
        }

        [TestMethod]
        public void Analyzed_signal_contains_same_constant_values_​​as_raw_signal()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);
            rawSignal.StepX = 1.6F * 1000;
            rawSignal.SaturationValue = 8;

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(rawSignal, _algoParams);
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample);

            // Then : All peaks are detected and global analysis succeeds
            Assert.AreEqual(rawSignal.SaturationValue, signalAnalyzed.SaturationValue);
            Assert.AreEqual(rawSignal.StepX, signalAnalyzed.StepX);

            Assert.AreEqual(rawSignal.SaturationValue, signalAnalyzedAccordingSample.SaturationValue);
            Assert.AreEqual(rawSignal.StepX, signalAnalyzedAccordingSample.StepX);
        }

        [TestMethod]
        public void Analyzed_signal_according_sample_contains_same_constant_values_​​as_raw_signal()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);
            rawSignal.StepX = 1.6F * 1000;
            rawSignal.SaturationValue = 8;

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample);

            // Then : All peaks are detected and global analysis succeeds
            Assert.AreEqual(rawSignal.SaturationValue, signalAnalyzedAccordingSample.SaturationValue);
            Assert.AreEqual(rawSignal.StepX, signalAnalyzedAccordingSample.StepX);
        }

        [TestMethod]
        public void Analyzed_signal_contains_same_number_of_values_​​as_raw_signal()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(rawSignal, _algoParams);
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample);

            // Then : All peaks are detected and global analysis succeeds
            Assert.AreEqual(LiseSignalLength, signalAnalyzed.RawValues.Count);
            Assert.AreEqual(LiseSignalLength, signalAnalyzed.Means.Count);
            Assert.AreEqual(LiseSignalLength, signalAnalyzed.StdDev.Count);

            Assert.AreEqual(LiseSignalLength, signalAnalyzedAccordingSample.RawValues.Count);
            Assert.AreEqual(LiseSignalLength, signalAnalyzedAccordingSample.Means.Count);
            Assert.AreEqual(LiseSignalLength, signalAnalyzedAccordingSample.StdDev.Count);
        }

        [TestMethod]
        public void Analyzed_signal_according_sample_contains_same_number_of_values_​​as_raw_signal()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample);

            // Then : All peaks are detected and global analysis succeeds
            Assert.AreEqual(LiseSignalLength, signalAnalyzedAccordingSample.RawValues.Count);
            Assert.AreEqual(LiseSignalLength, signalAnalyzedAccordingSample.Means.Count);
            Assert.AreEqual(LiseSignalLength, signalAnalyzedAccordingSample.StdDev.Count);
        }

        [TestMethod]
        public void Analyzed_signal_contains_only_commun_peaks_between_comings_and_goings_signal()
        {
            // Given : Goings signal differs from comings signal
            int secondPeakPos = FirstPeakArbitraryPosition + 500;
            int thirdPeakPos = secondPeakPos + 500;
            var peaksPositionInFirstPartOfSignal = new List<int> { RefPeakArbitraryPosition, FirstPeakArbitraryPosition, secondPeakPos, thirdPeakPos };
            var peaksPositionInSecondPartOfSignal = new List<int> { RefPeakArbitraryPosition, thirdPeakPos };
            var invalidRawSignal = CreateLiseSignalFromItsSubparts(peaksPositionInFirstPartOfSignal, peaksPositionInSecondPartOfSignal, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(invalidRawSignal, _algoParams);

            // Then : Analysis is valid and selected peaks are only commune peak between comings and goings signal
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzed.SignalStatus);
            Assert.AreEqual(1, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(1, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);
            Assert.AreEqual(thirdPeakPos, signalAnalyzed.SelectedPeaks[0].X, 1);
        }

        [TestMethod]
        public void Analyzed_signal_according_sample_contains_only_commun_peaks_between_comings_and_goings_signal()
        {
            // Given : Goings signal differs from comings signal
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var validRawSignal = CreateLiseSignalFromSamples(sample);
            var peaksPositionInFirstPartOfSignal = new List<int> { RefPeakArbitraryPosition, FirstPeakArbitraryPosition - 500, (int)validRawSignal.SelectedPeaks[0].X, (int)validRawSignal.SelectedPeaks[1].X, (int)validRawSignal.SelectedPeaks[2].X };
            var peaksPositionInSecondPartOfSignal = new List<int> { RefPeakArbitraryPosition, FirstPeakArbitraryPosition + 500, (int)validRawSignal.SelectedPeaks[0].X, (int)validRawSignal.SelectedPeaks[1].X, (int)validRawSignal.SelectedPeaks[2].X };
            var invalidRawSignal = CreateLiseSignalFromItsSubparts(peaksPositionInFirstPartOfSignal, peaksPositionInSecondPartOfSignal, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(invalidRawSignal, _algoWithSampleParams, sample);

            // Then : Analysis is valid and selected peaks are only commune peak between comings and goings signal
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(3, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
            Assert.AreEqual(validRawSignal.SelectedPeaks[0].X, signalAnalyzedAccordingSample.SelectedPeaks[0].X, 1);
            Assert.AreEqual(validRawSignal.SelectedPeaks[1].X, signalAnalyzedAccordingSample.SelectedPeaks[1].X, 1);
            Assert.AreEqual(validRawSignal.SelectedPeaks[2].X, signalAnalyzedAccordingSample.SelectedPeaks[2].X, 1);
        }

        [TestMethod]
        public void Analysis_fails_when_the_raw_signal_is_null()
        {
            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(null, _algoParams);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.InvalidRawSignal, signalAnalyzed.SignalStatus);

            Assert.AreEqual(0, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.RawValues.Count);
            Assert.AreEqual(0, signalAnalyzed.Means.Count);
            Assert.AreEqual(0, signalAnalyzed.StdDev.Count);
            Assert.AreEqual(double.NaN, signalAnalyzed.StepX);
            Assert.AreEqual(double.NaN, signalAnalyzed.SaturationValue);
        }

        [TestMethod]
        public void Analysis_according_sample_fails_when_the_raw_signal_is_null()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { }, "name", "info");

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(null, _algoWithSampleParams, sample);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.InvalidRawSignal, signalAnalyzedAccordingSample.SignalStatus);

            Assert.AreEqual(0, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.RawValues.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.Means.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.StdDev.Count);
            Assert.AreEqual(double.NaN, signalAnalyzedAccordingSample.StepX);
            Assert.AreEqual(double.NaN, signalAnalyzedAccordingSample.SaturationValue);
        }

        [TestMethod]
        public void Analysis_fails_when_the_raw_signal_is_empty()
        {
            // Given
            var emptySignal = CreateNullLiseSignal();

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(emptySignal, _algoParams);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.InvalidRawSignal, signalAnalyzed.SignalStatus);

            Assert.AreEqual(0, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);
            Assert.AreEqual(emptySignal.RawValues.Count, signalAnalyzed.RawValues.Count);
            Assert.AreEqual(emptySignal.Means.Count, signalAnalyzed.Means.Count);
            Assert.AreEqual(emptySignal.StdDev.Count, signalAnalyzed.StdDev.Count);
            Assert.AreEqual(emptySignal.StepX, signalAnalyzed.StepX);
            Assert.AreEqual(emptySignal.SaturationValue, signalAnalyzed.SaturationValue);
        }

        [TestMethod]
        public void Analysis_according_sample_fails_when_the_raw_signal_is_empty()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { }, "name", "info");
            var emptySignal = CreateNullLiseSignal();

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(emptySignal, _algoWithSampleParams, sample);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.InvalidRawSignal, signalAnalyzedAccordingSample.SignalStatus);

            Assert.AreEqual(0, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
            Assert.AreEqual(emptySignal.RawValues.Count, signalAnalyzedAccordingSample.RawValues.Count);
            Assert.AreEqual(emptySignal.Means.Count, signalAnalyzedAccordingSample.Means.Count);
            Assert.AreEqual(emptySignal.StdDev.Count, signalAnalyzedAccordingSample.StdDev.Count);
            Assert.AreEqual(emptySignal.StepX, signalAnalyzedAccordingSample.StepX);
            Assert.AreEqual(emptySignal.SaturationValue, signalAnalyzedAccordingSample.SaturationValue);
        }

        [TestMethod]
        public void Analysis_fails_when_the_raw_signal_has_no_peak()
        {
            // Given
            var signalWithoutPeak = CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(signalWithoutPeak, _algoParams);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.InvalidAnalyzedSignal, signalAnalyzed.SignalStatus);
            Assert.AreEqual(0, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_fails_when_the_raw_signal_has_no_peak()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { }, "name", "info");
            var signalWithoutPeak = CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(signalWithoutPeak, _algoWithSampleParams, sample);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.InvalidAnalyzedSignal, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_success_without_peak_selected_when_the_sample_is_empty()
        {
            // Given
            var emptySample = new ProbeSample(new List<ProbeSampleLayer>() { }, "name", "info");
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, emptySample);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(3, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_success_without_peak_selected_when_the_sample_does_not_match()
        {
            // Given
            var sample1 = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer750 }, "name", "info");
            var sample2 = new ProbeSample(new List<ProbeSampleLayer>() { Layer200, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample1);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample2);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(3, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_success_when_the_raw_signal_has_only_ref_peak_and_no_interest_peak()
        {
            // Given
            var signalWithOnlyRefPeak = CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(signalWithOnlyRefPeak, _algoParams);

            // Then : Ref peak is detected but global analysis fails
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzed.SignalStatus);
            Assert.AreEqual(1, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_success_when_the_raw_signal_has_only_ref_peak_and_no_interest_peak()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer200 }, "name", "info");
            var signalWithOnlyRefPeak = CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(signalWithOnlyRefPeak, _algoWithSampleParams, sample);

            // Then : Ref peak is detected but global analysis fails
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_fails_when_the_raw_signal_has_only_interest_peak_and_no_ref_peak()
        {
            // Given
            var signalWithOnlyInterestPeak = CreateLiseSignalFromPeakPositions(new List<int> { FirstPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(signalWithOnlyInterestPeak, _algoParams);

            // Then : Interest peak is detected but global analysis fails
            Assert.AreNotEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzed.SignalStatus);
            Assert.AreEqual(0, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(1, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_fails_when_the_raw_signal_has_only_interest_peak_and_no_ref_peak()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer200 }, "name", "info");
            var signalWithOnlyInterestPeak = CreateLiseSignalFromPeakPositions(new List<int> { FirstPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(signalWithOnlyInterestPeak, _algoWithSampleParams, sample);

            // Then : Interest peak is detected but global analysis fails
            Assert.AreNotEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_fails_when_signal_contains_more_than_one_potential_ref_peak()
        {
            // Given
            int secondRefPeakPos = RefPeakArbitraryPosition - 100;
            var signalWithTwoPotentialRefPeak = CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition, FirstPeakArbitraryPosition, secondRefPeakPos }, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = signalAnalyzer.AnalyzeRawSignal(signalWithTwoPotentialRefPeak, _algoParams);

            // Then
            Assert.AreNotEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzed.SignalStatus);
            Assert.AreEqual(2, signalAnalyzed.ReferencePeaks.Count);
            Assert.AreEqual(1, signalAnalyzed.SelectedPeaks.Count);
            Assert.AreEqual(0, signalAnalyzed.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_fails_when_signal_contains_more_than_one_potential_ref_peak()
        {
            // Given
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer200 }, "name", "info");
            int secondRefPeakPos = RefPeakArbitraryPosition - 100;
            var signalWithTwoPotentialRefPeak = CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition, FirstPeakArbitraryPosition, secondRefPeakPos }, GeometricToMicrometerRatio, LiseSignalLength);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(signalWithTwoPotentialRefPeak, _algoWithSampleParams, sample);

            // Then
            Assert.AreNotEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(2, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_succeeds_and_ignores_interfering_peaks()
        {
            // Given: The interference peaks cannot coincide with virtual layers of expected thickness
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);
            var peaks = new List<Peak>
            {
                new Peak(RefPeakArbitraryPosition, 6),
                new Peak((int)rawSignal.SelectedPeaks[0].X - 200, 6),
                new Peak((int)rawSignal.SelectedPeaks[0].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[0].X + 50, 6),
                new Peak((int)rawSignal.SelectedPeaks[1].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[1].X + 50, 6),
                new Peak((int)rawSignal.SelectedPeaks[2].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[2].X + 500, 6)
            };
            var rawSignalWithInterferences = CreateLiseSignalFromPeaks(peaks, rawSignal.StepX / 1000, rawSignal.RawValues.Count);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalWithoutInterferencesAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample);
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignalWithInterferences, _algoWithSampleParams, sample);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(3, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(4, signalAnalyzedAccordingSample.DiscardedPeaks.Count);

            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[0].X, signalAnalyzedAccordingSample.SelectedPeaks[0].X, 1);
            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[1].X, signalAnalyzedAccordingSample.SelectedPeaks[1].X, 1);
            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[2].X, signalAnalyzedAccordingSample.SelectedPeaks[2].X, 1);
        }

        [TestMethod]
        public void Analysis_according_sample_succeeds_and_ignores_reflective_interfering_peaks()
        {
            // Given: The interference peaks coincide with layers of expected thickness
            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer200 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);
            var peaks = new List<Peak>
            {
                new Peak(RefPeakArbitraryPosition, 6),
                new Peak((int)rawSignal.SelectedPeaks[0].X - 400, 4),
                new Peak((int)rawSignal.SelectedPeaks[0].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[1].X - 400, 4),
                new Peak((int)rawSignal.SelectedPeaks[1].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[2].X - 400, 4),
                new Peak((int)rawSignal.SelectedPeaks[2].X, 6)
            };

            var rawSignalWithInterferences = CreateLiseSignalFromPeaks(peaks, rawSignal.StepX / 1000, rawSignal.RawValues.Count);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalWithoutInterferencesAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample);
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignalWithInterferences, _algoWithSampleParams, sample);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(3, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(3, signalAnalyzedAccordingSample.DiscardedPeaks.Count);

            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[0].X, signalAnalyzedAccordingSample.SelectedPeaks[0].X, 1);
            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[1].X, signalAnalyzedAccordingSample.SelectedPeaks[1].X, 1);
            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[2].X, signalAnalyzedAccordingSample.SelectedPeaks[2].X, 1);
        }

        [TestMethod]
        public void Analysis_according_sample_suceeds_and_ignore_layer_with_zero_refractive_index()
        {
            // Given
            var unknownLayer = new ProbeSampleLayer(750.Micrometers(), Tolerance, 0);

            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750 }, "name", "info");
            var sampleWithUnknowLayer = new ProbeSample(new List<ProbeSampleLayer>() { unknownLayer }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sampleWithUnknowLayer);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(2, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_suceeds_and_ignore_all_layers_after_one_layer_with_zero_refractive_index()
        {
            // Given
            var unknownLayer = new ProbeSampleLayer(750.Micrometers(), Tolerance, 0);

            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer750, Layer750, Layer750, Layer750 }, "name", "info");
            var sampleWithUnknowLayer = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, unknownLayer, Layer750, unknownLayer, Layer750 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sampleWithUnknowLayer);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(2, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(4, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_suceeds_and_ignore_layer_with_unknown_refractive_index()
        {
            // Given
            var unknownLayer = new ProbeSampleLayer(750.Micrometers(), Tolerance, double.NaN);

            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750 }, "name", "info");
            var sampleWithUnknowLayer = new ProbeSample(new List<ProbeSampleLayer>() { unknownLayer }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sampleWithUnknowLayer);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(0, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(2, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_suceeds_and_ignore_all_layers_after_one_layer_with_unknown_refractive_index()
        {
            // Given
            var unknownLayer = new ProbeSampleLayer(750.Micrometers(), Tolerance, double.NaN);

            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, Layer750, Layer750, Layer750, Layer750 }, "name", "info");
            var sampleWithUnknowLayer = new ProbeSample(new List<ProbeSampleLayer>() { Layer750, unknownLayer, Layer750, unknownLayer, Layer750 }, "name", "info");
            var rawSignal = CreateLiseSignalFromSamples(sample);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sampleWithUnknowLayer);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(2, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(4, signalAnalyzedAccordingSample.DiscardedPeaks.Count);
        }

        [TestMethod]
        public void Analysis_according_sample_succeeds_and_ignores_layers_after_layers_with_unknown_refractive_index_and_reflective_interfering_peaks()
        {
            // Given: The interference peaks coincide with layers of expected thickness
            var unknownLayer = new ProbeSampleLayer(200.Micrometers(), Tolerance, double.NaN);

            var sample = new ProbeSample(new List<ProbeSampleLayer>() { Layer25, Layer200, Layer180, Layer750, Layer200, Layer200, Layer25 }, "name", "info");
            var sampleWithUnknowLayer = new ProbeSample(new List<ProbeSampleLayer>() { Layer25, Layer200, Layer180, Layer750, unknownLayer, Layer200, Layer25 }, "name", "info");

            var rawSignal = CreateLiseSignalFromSamples(sample);
            var peaks = new List<Peak>
            {
                new Peak(RefPeakArbitraryPosition, 6),
                new Peak((int)rawSignal.SelectedPeaks[0].X - 10, 4),
                new Peak((int)rawSignal.SelectedPeaks[0].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[1].X - 10, 4),
                new Peak((int)rawSignal.SelectedPeaks[1].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[2].X - 10, 4),
                new Peak((int)rawSignal.SelectedPeaks[2].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[3].X - 10, 4),
                new Peak((int)rawSignal.SelectedPeaks[3].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[4].X - 10, 4),
                new Peak((int)rawSignal.SelectedPeaks[4].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[5].X - 10, 4),
                new Peak((int)rawSignal.SelectedPeaks[5].X, 6),
                new Peak((int)rawSignal.SelectedPeaks[6].X - 10, 4),
                new Peak((int)rawSignal.SelectedPeaks[6].X, 6),
            };

            var rawSignalWithInterferences = CreateLiseSignalFromPeaks(peaks, rawSignal.StepX / 1000, rawSignal.RawValues.Count);

            // When
            var signalAnalyzer = new LiseSignalAnalyzer();
            var signalWithoutInterferencesAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignal, _algoWithSampleParams, sample);
            var signalAnalyzedAccordingSample = signalAnalyzer.AnalyzeRawSignalAccordingSample(rawSignalWithInterferences, _algoWithSampleParams, sampleWithUnknowLayer);

            // Then
            Assert.AreEqual(LISESignalAnalyzed.SignalAnalysisStatus.Valid, signalAnalyzedAccordingSample.SignalStatus);
            Assert.AreEqual(1, signalAnalyzedAccordingSample.ReferencePeaks.Count);
            Assert.AreEqual(5, signalAnalyzedAccordingSample.SelectedPeaks.Count);
            Assert.AreEqual(9, signalAnalyzedAccordingSample.DiscardedPeaks.Count);

            Assert.IsTrue(signalAnalyzedAccordingSample.SelectedPeaks.Count <= signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks.Count);

            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[0].X, signalAnalyzedAccordingSample.SelectedPeaks[0].X, 1);
            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[1].X, signalAnalyzedAccordingSample.SelectedPeaks[1].X, 1);
            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[2].X, signalAnalyzedAccordingSample.SelectedPeaks[2].X, 1);
            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[3].X, signalAnalyzedAccordingSample.SelectedPeaks[3].X, 1);
            Assert.AreEqual(signalWithoutInterferencesAnalyzedAccordingSample.SelectedPeaks[4].X, signalAnalyzedAccordingSample.SelectedPeaks[4].X, 1);
        }

        [TestMethod]
        public void Analyze_raw_signal_clone_performs_a_deep_copy()
        {
            // Given : Analyzed signal with peaks

            float saturationValue = 7;
            double stepX = 1;
            var signalStatus = LISESignalAnalyzed.SignalAnalysisStatus.Valid;
            var refPeak = new Peak(RefPeakArbitraryPosition, 6);
            var firstPeak = new Peak(FirstPeakArbitraryPosition, 6);
            var rawSignal = CreateLiseSignalFromPeaks(new List<Peak> { refPeak, firstPeak }, GeometricToMicrometerRatio, LiseSignalLength);
            var signalAnalyzed = CreateLISESignalAnalyzed(rawSignal, new List<Peak> { refPeak }, new List<Peak> { firstPeak }, saturationValue, stepX, signalStatus);

            // When : Copy this analyzed signal

            var signalCopied = signalAnalyzed.Clone();

            // Then : The copied object is independent of its clone

            signalCopied.RawValues = new List<double>();
            signalCopied.Means = new List<double>();
            signalCopied.StdDev = new List<double>();
            signalCopied.SelectedPeaks.Clear();
            signalCopied.ReferencePeaks.Clear();
            signalCopied.SaturationValue = 0;
            signalCopied.StepX = 0;
            signalCopied.SignalStatus = LISESignalAnalyzed.SignalAnalysisStatus.InvalidAnalyzedSignal;

            Assert.AreNotEqual(signalAnalyzed.RawValues.Count, signalCopied.RawValues.Count);
            Assert.AreNotEqual(signalAnalyzed.Means.Count, signalCopied.Means.Count);
            Assert.AreNotEqual(signalAnalyzed.StdDev.Count, signalCopied.StdDev.Count);
            Assert.AreNotEqual(signalAnalyzed.SelectedPeaks.Count, signalCopied.SelectedPeaks.Count);
            Assert.AreNotEqual(signalAnalyzed.ReferencePeaks.Count, signalCopied.ReferencePeaks.Count);
            Assert.AreNotEqual(signalAnalyzed.SaturationValue, signalCopied.SaturationValue);
            Assert.AreNotEqual(signalAnalyzed.StepX, signalCopied.StepX);
            Assert.AreNotEqual(signalAnalyzed.SignalStatus, signalCopied.SignalStatus);
        }
    }
}
