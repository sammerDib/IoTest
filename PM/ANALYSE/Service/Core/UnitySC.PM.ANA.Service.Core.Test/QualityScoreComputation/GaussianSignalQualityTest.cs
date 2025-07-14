using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.Shared;

namespace UnitySC.PM.ANA.Service.Core.Test.QualityScoreComputation
{
    [TestClass]
    public class GaussianSignalQualityTest
    {
        [TestMethod]
        public void Quality_score_of_gaussian_signal_equals_zero_when_signal_is_empty()
        {
            // Given : Empty signal
            var signal = new List<double>();

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfGaussianSignal(signal);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_gaussian_signal_equals_zero_when_signal_contains_only_one_value()
        {
            // Given : Signal with one value
            var signal = new List<double>() { 1 };

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfGaussianSignal(signal);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_gaussian_signal_equals_zero_when_signal_contains_only_zeros()
        {
            // Given : Signal with zeros
            double zeroAmplitude = 0;
            var signal = new List<double>();
            for (int i = 0; i < 100; i++)
            {
                signal.Add(zeroAmplitude);
            }

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfGaussianSignal(signal);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_gaussian_signal_equals_zero_when_signal_contains_the_same_value()
        {
            // Given : Signal without peak
            double minAmplitude = 1;
            var signal = new List<double>();
            for (int i = 0; i < 100; i++)
            {
                signal.Add(minAmplitude);
            }

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfGaussianSignal(signal);

            // Then : Quality score equals zero
            Assert.AreEqual(0, qualityScore);
        }

        [TestMethod]
        public void Quality_score_of_gaussian_signal_is_positive_less_than_one_when_signal_is_negative()
        {
            // Given
            const double minAmplitude = -5;
            const double maxAmplitude = -1.5;
            var signal = new List<double>();
            for (int i = 0; i < 100; i++)
            {
                signal.Add(minAmplitude);
            }
            signal[50] = maxAmplitude;

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfGaussianSignal(signal);

            // Then : Quality score is higher than 0.9 without exceeding the maximum value of 1
            Assert.IsTrue(qualityScore > 0);
            Assert.IsTrue(qualityScore < 1);
        }

        [TestMethod]
        public void Quality_score_of_gaussian_signal_is_positive_less_than_one_when_signal_is_positive()
        {
            // Given
            const double minAmplitude = 1.5;
            const double maxAmplitude = 100;
            var signal = new List<double>();
            for (int i = 0; i < 100; i++)
            {
                signal.Add(minAmplitude);
            }
            signal[50] = maxAmplitude;

            // When : Compute quality score
            double qualityScore = QualityScore.ComputeQualityScoreOfGaussianSignal(signal);

            // Then : Quality score is higher than 0.9 without exceeding the maximum value of 1
            Assert.IsTrue(qualityScore > 0);
            Assert.IsTrue(qualityScore < 1);
        }

        [TestMethod]
        public void Quality_score_of_gaussian_signal_increase_when_gaussian_is_higher()
        {
            // Given : Process autofocus on the raw signal with higher and higher amplitude peaks
            const double minAmplitude = 1.5;
            var signalWithHigherAndHigherGaussianAmplitude = new List<List<double>>();
            for (double amplitude = 10; amplitude < 50; amplitude += 10)
            {
                double maxAmplitude = minAmplitude + amplitude;
                var signal = new List<double>();
                for (int i = 0; i < 100; i++)
                {
                    signal.Add(minAmplitude);
                }

                signal[50] = maxAmplitude;
                signalWithHigherAndHigherGaussianAmplitude.Add(signal);
            }

            // When : Compute quality score
            var qualityOfSignalWithHigherAndHigherGaussianAmplitude = new List<double>();
            foreach (var signal in signalWithHigherAndHigherGaussianAmplitude)
            {
                double qualityScore = QualityScore.ComputeQualityScoreOfGaussianSignal(signal);
                qualityOfSignalWithHigherAndHigherGaussianAmplitude.Add(qualityScore);
            }

            // Then : Quality score increase when peaks are higher
            double previousQuality = 0;
            foreach (double quality in qualityOfSignalWithHigherAndHigherGaussianAmplitude)
            {
                Assert.IsTrue(previousQuality < quality);
                previousQuality = quality;
            }
        }
    }
}
