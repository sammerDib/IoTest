using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgosLibrary;
using System.Collections.Generic;

namespace UnitySC.Algorithms.Wrapper.Tests
{
    [TestClass]
    public class SpikeDetectorTest
    {
        [TestInitialize]
        public void Init() { }

        [TestMethod]
        public void Spike_Detector_wrapper_return_correct_managed_values()
        {
            // Given : An input signal

            int lag = 3;
            double threshold = 10;
            double influence = 0;

            double baseSignalMean = 0.8;
            double peakValue1 = 5.99;
            double peakValue2 = 4.25;
            List<double> input = new List<double>();
            for (int i = 0; i < lag; i++)
            {
                input.Add(baseSignalMean);
            }
            input.Add(peakValue1);
            for (int i = 0; i < 5; i++)
            {
                input.Add(baseSignalMean);
            }
            input.Add(peakValue2);

            // When : Run managed spike detector
            var result = AlgosLibrary.SpikeDetector.AnalyzeSignal(input.ToArray(), lag, threshold, influence);

            // Then : the managed return structure contains correct values
            Assert.AreEqual(2, result.Spikes.Length);
            Assert.AreEqual(peakValue1, result.Spikes[0].Value, 0.01);
            Assert.AreEqual(peakValue2, result.Spikes[1].Value, 0.01);

            Assert.AreEqual(input.Count, result.Means.Length);
            Assert.AreEqual(input.Count, result.Stddev.Length);
            for (int i = 0; i < lag - 1; i++)
            {
                Assert.AreEqual(0.0, result.Means[i], 0.01);
                Assert.AreEqual(0.0, result.Stddev[i], 0.01);
            }
            for (int i = lag - 1; i < result.Means.Length; i++)
            {
                Assert.AreEqual(baseSignalMean, result.Means[i], 0.01);
                Assert.AreEqual(0.0, result.Stddev[i], 0.01);
            }
        }
    }
}
