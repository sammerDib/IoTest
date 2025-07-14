using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCPMSharedAlgosLiseHFWrapper;

namespace Algos.LiseHF.WrapperTests
{
    [TestClass]
    public class UnitInputsTest
    {
        [TestMethod]
        public void TestInputEmptyConstructor()
        {
            var inputs = new LiseHFAlgoInputs();

            Assert.IsNull(inputs.Wavelength_nm);
            Assert.IsNull(inputs.Spectrum);
            Assert.IsNull(inputs.DarkSpectrum);
            Assert.IsNull(inputs.RefSpectrum);

            Assert.IsNull(inputs.DepthLayers);

            Assert.AreEqual(0.0, inputs.TSVDiameter_um);
            Assert.AreEqual(LiseHFMode.GridSearch, inputs.OpMode);
            Assert.AreEqual(0.0, inputs.Threshold_signal_pct);
            Assert.AreEqual(0.0, inputs.Threshold_peak_pct);

            Assert.AreEqual(false, inputs.PeakDetectionOnRight);
            Assert.AreEqual(false, inputs.NewPeakDetection);
        }

        [TestMethod]
        public void TestValidityInputs()
        {
            var rand = new Random();
            var inputs = new LiseHFAlgoInputs();

            string errMessage = "";
            Assert.IsFalse(inputs.CheckValidity(ref errMessage));
            Assert.IsFalse(string.IsNullOrEmpty(errMessage));

            List<double> dummyvals = new List<double>(4094) { 1.0, 1.2, 2.2 };
            inputs.Wavelength_nm = dummyvals;
            inputs.Spectrum = new LiseHFRawSignal(dummyvals, 8, 1);
            inputs.DarkSpectrum = new LiseHFRawSignal(dummyvals, 8, 1);
            inputs.RefSpectrum = new LiseHFRawSignal(dummyvals, 8, 1);
            inputs.DepthLayers = new LiseHFLayers(10, 1);
            inputs.TSVDiameter_um = 5.0;
            errMessage = "";
            Assert.IsFalse(inputs.CheckValidity(ref errMessage));
            Assert.IsFalse(string.IsNullOrEmpty(errMessage));

            for(int i = dummyvals.Count-1; i< 4094; i++) { 
                dummyvals.Add( rand.NextDouble() * 5000.0); 
            }

            Assert.IsFalse(inputs.Spectrum.HasNativeBeenComputed());
            Assert.IsFalse(inputs.DarkSpectrum.HasNativeBeenComputed());
            Assert.IsFalse(inputs.RefSpectrum.HasNativeBeenComputed());

            inputs.Spectrum.ComputeNative();
            inputs.DarkSpectrum.ComputeNative();
            inputs.RefSpectrum.ComputeNative();

            errMessage = "";
            Assert.IsTrue(inputs.CheckValidity(ref errMessage));
            Assert.IsTrue(string.IsNullOrEmpty(errMessage));

            inputs.TSVDiameter_um = 0.0;
            errMessage = "";
            Assert.IsFalse(inputs.CheckValidity(ref errMessage));
            Assert.IsFalse(string.IsNullOrEmpty(errMessage));

            inputs.TSVDiameter_um = 15.0;
            errMessage = "";
            Assert.IsTrue(inputs.CheckValidity(ref errMessage));
            Assert.IsTrue(string.IsNullOrEmpty(errMessage));

            inputs.Threshold_signal_pct = 123.0;
            errMessage = "";
            Assert.IsFalse(inputs.CheckValidity(ref errMessage));
            Assert.IsFalse(string.IsNullOrEmpty(errMessage));

            inputs.Threshold_signal_pct = -1.0;
            errMessage = "";
            Assert.IsFalse(inputs.CheckValidity(ref errMessage));
            Assert.IsFalse(string.IsNullOrEmpty(errMessage));

            inputs.Threshold_signal_pct = 0.5;
            errMessage = "";
            Assert.IsTrue(inputs.CheckValidity(ref errMessage));
            Assert.IsTrue(string.IsNullOrEmpty(errMessage));

            inputs.Threshold_peak_pct = 123.0;
            errMessage = "";
            Assert.IsFalse(inputs.CheckValidity(ref errMessage));
            Assert.IsFalse(string.IsNullOrEmpty(errMessage));

            inputs.Threshold_peak_pct = -1.0;
            errMessage = "";
            Assert.IsFalse(inputs.CheckValidity(ref errMessage));
            Assert.IsFalse(string.IsNullOrEmpty(errMessage));

            inputs.Threshold_peak_pct = 0.025;
            errMessage = "";
            Assert.IsTrue(inputs.CheckValidity(ref errMessage));
            Assert.IsTrue(string.IsNullOrEmpty(errMessage));

            Assert.IsFalse(inputs.PeakDetectionOnRight);
            Assert.IsFalse(inputs.NewPeakDetection);


        }
    }
}
