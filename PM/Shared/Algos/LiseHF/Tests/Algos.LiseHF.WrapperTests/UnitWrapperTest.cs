using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using UnitySCPMSharedAlgosLiseHFWrapper;

namespace Algos.LiseHF.WrapperTests
{
    [TestClass]
    public class UnitWrapperTest
    {
        private readonly string _sTEST_DATA_PATH = "\\..\\..\\Data\\";

        [TestMethod]
        public void TestComputeSingleLayer()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            string cwd = Directory.GetCurrentDirectory();

            string filetest = "Wafer center TSV center2 18-2-23 8ms 300av.TXT";
            
            // get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string FileInputSummarized = $"{projectDirectory}{_sTEST_DATA_PATH}{filetest}";

            List<double> wave = new List<double>(4096);
            List<double> spectrum = new List<double>(4096);
            List<double> darkcal = new List<double>(4096);
            List<double> refcal = new List<double>(4096);

            using (var sr = new StreamReader(FileInputSummarized))
            {
                while (sr.Peek() >= 0)
                {
                    // Read here
                    var line = (sr.ReadLine()).Trim();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (!line.StartsWith("#"))
                        { 
                            var cols = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                            if (cols.Length == 4)
                            {
                                if( double.TryParse(cols[0], out double dwav)) wave.Add(dwav);
                                if (double.TryParse(cols[1], out double dsig)) spectrum.Add(dsig);
                                if (double.TryParse(cols[2], out double ddark)) darkcal.Add(ddark);
                                if (double.TryParse(cols[3], out double dref)) refcal.Add(dref);
                            }
                        
                        }
                    }
                }

                Assert.IsTrue(wave.Count >= 4094);
                Assert.IsTrue(wave.Count == spectrum.Count);
                Assert.IsTrue(wave.Count == darkcal.Count);
                Assert.IsTrue(wave.Count == refcal.Count);
            }

            LiseHFAlgoInputs inputs = new LiseHFAlgoInputs();
            inputs.TSVDiameter_um = 5.0;
            inputs.Threshold_signal_pct = 0.01;
            inputs.Threshold_peak_pct = 0.95;
            inputs.OpMode = LiseHFMode.GridSearch;
            inputs.Wavelength_nm = wave;
            inputs.Spectrum = new LiseHFRawSignal(spectrum, 8, 1);
            inputs.DarkSpectrum = new LiseHFRawSignal(darkcal, 8, 1);
            inputs.RefSpectrum = new LiseHFRawSignal(refcal, 8, 1);
            inputs.DepthLayers = new LiseHFLayers(30.0, 2.0, 1.0);


            var ret = Olovia_Algos.Compute(inputs);

            Assert.IsNotNull(ret);
            Assert.IsTrue(ret.IsSuccess);

            Assert.IsTrue(ret.FFTDone);
            Assert.IsNotNull(ret.Outputs.FTTx);
            Assert.IsNotNull(ret.Outputs.FTTy);
            Assert.AreEqual(0.0, ret.Outputs.FTTx[0], 0.0001, "z0");
            Assert.AreEqual(4922.067767, ret.Outputs.FTTy[0], 0.0001, "ft0");
            Assert.AreEqual(82.213399, ret.Outputs.FTTx[807], 0.0001, "z807");
            Assert.AreEqual(0.547493, ret.Outputs.FTTy[807], 0.0001, "ft807");

            Assert.IsTrue(ret.AnalysisDone);
            Assert.AreEqual(1, ret.Outputs.MeasuredDepths.Count, "wrong peak detection");
            Assert.AreEqual(103.10670, ret.Outputs.PeaksY[0], 0.0001, "incorrect peak amplitude");
            Assert.AreEqual(29.815468, ret.Outputs.PeaksX[0], 0.0001, "incorrect optical depth ");
            Assert.AreEqual(29.712324, ret.Outputs.MeasuredDepths[0], 0.0001, "incorrect corrected geometrical depth ");

        }
    }
}
