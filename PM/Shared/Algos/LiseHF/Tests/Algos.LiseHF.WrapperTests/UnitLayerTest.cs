using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCPMSharedAlgosLiseHFWrapper;

namespace Algos.LiseHF.WrapperTests
{
    [TestClass]
    public class UnitLayerTest
    {

        [TestMethod]
        public void TestMethodEmptyConstructor()
        {
            var EmptyLayer = new LiseHFLayers();

            Assert.IsNotNull(EmptyLayer.Depths_um);
            Assert.IsNotNull(EmptyLayer.DepthsToleranceSearch_um);
            Assert.IsNotNull(EmptyLayer.DepthsRefractiveIndex);

            Assert.AreEqual(0,EmptyLayer.Depths_um.Count);
            Assert.AreEqual(0, EmptyLayer.DepthsToleranceSearch_um.Count);
            Assert.AreEqual(0, EmptyLayer.DepthsRefractiveIndex.Count);

            Assert.AreEqual(0u, EmptyLayer.GetLayerCount());
            unsafe
            {
                Assert.AreEqual(0ul, (ulong)(EmptyLayer.GetNativeOpticalDepths()), "Should be null");
                Assert.AreEqual(0ul, (ulong)(EmptyLayer.GetNativeOpticalTolerancesSearch()), "Should be null");
                Assert.AreEqual(0ul, (ulong)(EmptyLayer.GetNativeRefIndex()), "Should be null");
            }
        }

        [TestMethod]
        public void TestMethodSingleLayer()
        {
            double depth = 50.2;
            double tolerance = 1.35;
            const double DefaultRefIndex = 3.8;
            var SingleLayer = new LiseHFLayers(depth, tolerance);

            Assert.IsNotNull(SingleLayer.Depths_um);
            Assert.IsNotNull(SingleLayer.DepthsToleranceSearch_um);
            Assert.IsNotNull(SingleLayer.DepthsRefractiveIndex);

            Assert.AreEqual(depth, SingleLayer.Depths_um[0]);
            Assert.AreEqual(tolerance, SingleLayer.DepthsToleranceSearch_um[0]);
            Assert.AreEqual(DefaultRefIndex, SingleLayer.DepthsRefractiveIndex[0]);

            Assert.AreEqual(1u, SingleLayer.GetLayerCount());
            unsafe
            {

                double* ptrD = SingleLayer.GetNativeOpticalDepths();
                double* ptrT = SingleLayer.GetNativeOpticalTolerancesSearch();
                double* ptrR = SingleLayer.GetNativeRefIndex();

                Assert.AreNotEqual(0ul, (ulong)ptrD, "Should not be null");
                Assert.AreNotEqual(0ul, (ulong)ptrT, "Should not be null");
                Assert.AreNotEqual(0ul, (ulong)ptrR, "Should not be null");

                Assert.AreEqual(depth * ptrR[0], ptrD[0], "different depth vs native optical Depth");
                Assert.AreEqual(tolerance * ptrR[0], ptrT[0], "different tolerance vs native Depth tolerance");
                Assert.AreEqual(DefaultRefIndex, ptrR[0], "different native Refractive Index");

            }
        }

        [TestMethod]
        public void TestMethodSingleLayerWithRI()
        {
            double depth = 103.25;
            double tolerance = 3.85;
            double RefIndex = 1.00023;
            var SingleLayer = new LiseHFLayers(depth, tolerance, RefIndex);

            Assert.IsNotNull(SingleLayer.Depths_um);
            Assert.IsNotNull(SingleLayer.DepthsToleranceSearch_um);
            Assert.IsNotNull(SingleLayer.DepthsRefractiveIndex);

            Assert.AreEqual(depth, SingleLayer.Depths_um[0]);
            Assert.AreEqual(tolerance, SingleLayer.DepthsToleranceSearch_um[0]);
            Assert.AreEqual(RefIndex, SingleLayer.DepthsRefractiveIndex[0]);

            Assert.AreEqual(1u, SingleLayer.GetLayerCount());
            unsafe
            {

                double* ptrD = SingleLayer.GetNativeOpticalDepths();
                double* ptrT = SingleLayer.GetNativeOpticalTolerancesSearch();
                double* ptrR = SingleLayer.GetNativeRefIndex();

                Assert.AreNotEqual(0ul, (ulong)ptrD, "Should not be null");
                Assert.AreNotEqual(0ul, (ulong)ptrT, "Should not be null");
                Assert.AreNotEqual(0ul, (ulong)ptrR, "Should not be null");

                Assert.AreEqual(depth * ptrR[0], ptrD[0], "different native optical Depth");
                Assert.AreEqual(tolerance * ptrR[0], ptrT[0], "different native optical tolerance Depth");
                Assert.AreEqual(RefIndex, ptrR[0], "different native Refractive Index");

            }
        }

        [TestMethod]
        public void TestMethodMultiLayer()
        {
            var depths = new List<double>() { 50.23, 4.5, 10.23 };
            var tols = new List<double>() { 5.2, 1.2, 0.1235 };
            var refIndx = new List<double>() { 3.883, 1.00025, 4.23};
            var multiLayer = new LiseHFLayers();
    
            Assert.IsNotNull(multiLayer.Depths_um);
            Assert.IsNotNull(multiLayer.DepthsToleranceSearch_um);
            Assert.IsNotNull(multiLayer.DepthsRefractiveIndex);

            for (int i = 0; i < depths.Count; i++)
            {
                multiLayer.AddNewDepthLayer(depths[i], tols[i], refIndx[i]);
                Assert.AreEqual(depths[i], multiLayer.Depths_um[i], $"Bad Depth[{i}]");
                Assert.AreEqual(tols[i], multiLayer.DepthsToleranceSearch_um[i], $"Bad tolerance[{i}]");
                Assert.AreEqual(refIndx[i], multiLayer.DepthsRefractiveIndex[i], $"Bad RI[{i}]");
            }

            // native not compute
            Assert.AreEqual(0u, multiLayer.GetLayerCount());
            unsafe
            {
                Assert.AreEqual(0ul, (ulong)(multiLayer.GetNativeOpticalDepths()), "Should be null");
                Assert.AreEqual(0ul, (ulong)(multiLayer.GetNativeOpticalTolerancesSearch()), "Should be null");
                Assert.AreEqual(0ul, (ulong)(multiLayer.GetNativeRefIndex()), "Should be null");
            }

            //Apply Compute
            multiLayer.ComputeNative();
            Assert.AreEqual((uint)depths.Count, multiLayer.GetLayerCount());
            unsafe
            {
                Assert.AreNotEqual(0ul, (ulong)(multiLayer.GetNativeOpticalDepths()), "Should not be null");
                Assert.AreNotEqual(0ul, (ulong)(multiLayer.GetNativeOpticalTolerancesSearch()), "Should not be null");
                Assert.AreNotEqual(0ul, (ulong)(multiLayer.GetNativeRefIndex()), "Should not be null");


                double* ptrD = multiLayer.GetNativeOpticalDepths();
                double* ptrT = multiLayer.GetNativeOpticalTolerancesSearch();
                double* ptrR = multiLayer.GetNativeRefIndex();

                for (int i = 0; i < depths.Count; i++)
                {
                    Assert.AreEqual(depths[i] * refIndx[i], ptrD[i], $"Bad Native Optical Depth[{i}]");
                    Assert.AreEqual(tols[i] * refIndx[i], ptrT[i], $"Bad Native tolerance[{i}]");
                    Assert.AreEqual(refIndx[i], multiLayer.DepthsRefractiveIndex[i], $"Bad Native RI[{i}]");
                }
           }
        }
    }
}
