using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCPMSharedAlgosLiseHFWrapper;

namespace Algos.LiseHF.WrapperTests
{
    [TestClass]
    public class UnitSignalsTest
    {
        [TestMethod]
        public void TestMethodDefaultConstructor()
        {
            var Signal = new LiseHFRawSignal();

            Assert.IsNull(Signal.RawSignal);
            Assert.AreEqual(0, Signal.IntegrationTime_ms);
            Assert.AreEqual(0, Signal.Attenuation_ID);
            Assert.AreEqual(0u, Signal.GetRawSignalLength());
            unsafe
            {
                Assert.AreEqual(0ul, (ulong)(Signal.GetNativeRawSignal()), "Should be null");
            }
        }

        [TestMethod]
        public void TestMethodConstructor()
        {
            List<double> tst_list1 = new List<double>() { 1000.1, 2000.2, 6553.5 };
            int tst_IntegTime = 12;
            int tst_attid = 2;
            var Signal = new LiseHFRawSignal(tst_list1, tst_IntegTime, tst_attid);

            Assert.IsNotNull(Signal.RawSignal, "Signal null");

            Assert.AreEqual(tst_list1[0], Signal.RawSignal[0], "different rawsig ind = 0");
            Assert.AreEqual(tst_list1[1], Signal.RawSignal[1], "different rawsig ind = 1");
            Assert.AreEqual(tst_list1[2], Signal.RawSignal[2], "different rawsig ind = 2");
           
            Assert.AreEqual(tst_IntegTime, Signal.IntegrationTime_ms);
            Assert.AreEqual(tst_attid, Signal.Attenuation_ID);
            Assert.AreEqual((uint)tst_list1.Count, Signal.GetRawSignalLength());
            unsafe
            {
                double* ptr = Signal.GetNativeRawSignal();
                Assert.AreNotEqual(0ul, (ulong)ptr, "Should not be null");
                Assert.AreEqual(tst_list1[0], ptr[0], "different native rawsig ind = 0");
                Assert.AreEqual(tst_list1[1], ptr[1], "different native rawsig ind = 1");
                Assert.AreEqual(tst_list1[2], ptr[2], "different native rawsig ind = 2");
            }

            Assert.AreEqual(0.1, Signal.CalcSaturationPct(), 0.0000001);
        }

        [TestMethod]
        public void TestMethodConstructorAndModification()
        {
            List<double> tst_list1 = new List<double>() { 1.1, 2.2, 3.3 };
            int tst_IntegTime = 325;
            int tst_attid = 2;
            var Signal = new LiseHFRawSignal(tst_list1, tst_IntegTime, tst_attid);

            Assert.IsNotNull(Signal.RawSignal, "Signal null");
            Assert.AreEqual(tst_IntegTime, Signal.IntegrationTime_ms);
            Assert.AreEqual(tst_attid, Signal.Attenuation_ID);
            Assert.AreEqual((uint)tst_list1.Count, Signal.GetRawSignalLength());

            Assert.AreEqual(tst_list1[0], Signal.RawSignal[0], "different rawsig ind = 0");
            Assert.AreEqual(tst_list1[1], Signal.RawSignal[1], "different rawsig ind = 1");
            Assert.AreEqual(tst_list1[2], Signal.RawSignal[2], "different rawsig ind = 2");
            ulong oldPtr = 0ul;
            unsafe
            {
                double* ptr = Signal.GetNativeRawSignal();
                oldPtr = (ulong)ptr;
                Assert.AreNotEqual(0ul, (ulong)ptr, "Should not be null");
                Assert.AreEqual(tst_list1[0], ptr[0], "different native rawsig ind = 0");
                Assert.AreEqual(tst_list1[1], ptr[1], "different native rawsig ind = 1");
                Assert.AreEqual(tst_list1[2], ptr[2], "different native rawsig ind = 2");
            }
            Assert.IsTrue(Signal.HasNativeBeenComputed());
            Assert.AreEqual(3.3 / 65535.0, Signal.CalcSaturationPct(), 0.0000001);

            // Modification
            tst_list1.Add(4.4);
            double oldInd1Value = tst_list1[1];
            tst_list1[1] = 20.02;

            // Check that nothing has changed except RawSignal directly
            Assert.AreEqual(4u, Signal.GetRawSignalLength());
            Assert.AreEqual(tst_list1[0], Signal.RawSignal[0], "different Modified rawsig ind = 0");
            Assert.AreEqual(tst_list1[1], Signal.RawSignal[1], "different Modified rawsig ind = 1");
            Assert.AreEqual(tst_list1[2], Signal.RawSignal[2], "different Modified rawsig ind = 2");
            Assert.AreEqual(tst_list1[3], Signal.RawSignal[3], "different Modified rawsig ind = 3");

            Assert.IsFalse(Signal.HasNativeBeenComputed());
            Assert.AreEqual(3.3 / 65535.0, Signal.CalcSaturationPct(), 0.0000001);
            unsafe
            {
                double* ptr = Signal.GetNativeRawSignal();
                Assert.AreNotEqual(0ul, (ulong)ptr, "Should not be null");

                Assert.AreEqual(oldPtr, (ulong)ptr, "No change on native pointer");

                Assert.AreEqual(tst_list1[0], ptr[0], "different native Modified rawsig ind = 0");
                Assert.AreEqual(oldInd1Value, ptr[1], "different native Modified rawsig ind = 1");
                Assert.AreEqual(tst_list1[2], ptr[2], "different native Modified  rawsig ind = 2");
            }

            //Modify Native
            Signal.ComputeNative();
            Assert.IsTrue(Signal.HasNativeBeenComputed());
            Assert.AreEqual(20.02 / 65535.0, Signal.CalcSaturationPct(), 0.0000001);


            Assert.AreEqual((uint)tst_list1.Count, Signal.GetRawSignalLength());
            Assert.IsNotNull(Signal.RawSignal, "Signal after compute null");
            Assert.AreEqual(tst_IntegTime, Signal.IntegrationTime_ms);
            Assert.AreEqual(tst_attid, Signal.Attenuation_ID);

            unsafe
            {
                double* ptr = Signal.GetNativeRawSignal();
                Assert.AreNotEqual(0ul, (ulong)ptr, "Should not be null");

                // not relevant since same pointer could be reallocated since it is clear jsut before realloc
                //Assert.AreNotEqual(oldPtr, (ulong)ptr, "change on new native pointer");

                Assert.AreEqual(tst_list1[0], ptr[0], "different new native Modified rawsig ind = 0");
                Assert.AreEqual(tst_list1[1], ptr[1], "different new native Modified rawsig ind = 1");
                Assert.AreEqual(tst_list1[2], ptr[2], "different new native Modified  rawsig ind = 2");
                Assert.AreEqual(tst_list1[3], ptr[3], "different new native Modified  rawsig ind = 3");
            }

        }
    }
}
