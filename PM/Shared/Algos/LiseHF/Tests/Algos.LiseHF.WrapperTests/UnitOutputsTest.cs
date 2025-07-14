using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCPMSharedAlgosLiseHFWrapper;

namespace Algos.LiseHF.WrapperTests
{
    [TestClass]
    public class UnitOutputsTest
    {
        [TestMethod]
        public void TestMethodReturns()
        {
            var ret = new LiseHFAlgoReturns(true, true, true, null, null);

            Assert.IsTrue(ret.IsSuccess);
            Assert.IsTrue(ret.FFTDone);
            Assert.IsTrue(ret.AnalysisDone);
            Assert.IsNull(ret.ErrorMessage);
            Assert.IsNull(ret.Outputs);

            ret.FFTDone = false;
            Assert.IsFalse(ret.FFTDone);

            string dummyerrmsg = "Dummy error";
            ret.ErrorMessage = dummyerrmsg;
            Assert.AreEqual(dummyerrmsg, ret.ErrorMessage);

            LiseHFAlgoOutputs outp2 = new LiseHFAlgoOutputs();
            outp2.Quality = 0.55;

            string dummyerrmsg2 = "Dummy error at construct";
            var ret2 = new LiseHFAlgoReturns(false, true, false, dummyerrmsg2, outp2);

            Assert.IsFalse(ret2.IsSuccess);
            Assert.IsTrue(ret2.FFTDone);
            Assert.IsFalse(ret2.AnalysisDone);
            Assert.IsNotNull(ret2.ErrorMessage);
            Assert.AreEqual(dummyerrmsg2, ret2.ErrorMessage);
            Assert.AreEqual(0.55, ret2.Outputs.Quality);

            outp2.SaturationPercentage = 100.0;
            var ret3 = new LiseHFAlgoReturns(true, false, false, "ABc D", outp2);
            Assert.IsTrue(ret3.IsSuccess);
            Assert.IsFalse(ret3.FFTDone);
            Assert.IsFalse(ret3.AnalysisDone);
            Assert.IsNotNull(ret3.ErrorMessage);
            Assert.AreEqual("ABc D", ret3.ErrorMessage);
            Assert.AreEqual(0.55, ret3.Outputs.Quality);
            Assert.AreEqual(100.0, ret3.Outputs.SaturationPercentage);
        }
    }
}
