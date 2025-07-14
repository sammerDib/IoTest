using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using System.Runtime.InteropServices;

using UnitySCSharedAlgosCppWrapper;

namespace SharedCpp.WrapperTests
{

    [TestClass]
    public class UnitTestInterpolations
    {
        [ClassInitialize]
        public static void DisableOpenMPclass(TestContext context)
        {
            Utils.DisableOpenMP();
        }

        [TestMethod]
        public void TestOpenMPActivate()
        {
            int NbPoints = 16 * 512;
            int nIter = 5;

            double Timems_NOomp = Utils.testopenmp(NbPoints, false);
            Timems_NOomp = 0.0;
            for (int i = 0; i < nIter; i++)
                Timems_NOomp += Utils.testopenmp(NbPoints, false);
            Timems_NOomp /= nIter;
            //Console.WriteLine($"Open MP Disabled=> Done in {Timems_NOomp} ms");

            double Timems_omp = Utils.testopenmp(NbPoints, true);
            Timems_omp = 0.0;
            for (int i = 0; i < nIter; i++)
                Timems_omp += Utils.testopenmp(NbPoints, true);
            Timems_omp /= nIter;
            //Console.WriteLine($"Open MP Enable => Done in {Timems_omp} ms");

            // ON CI depending on machine it could be worse so do not test perf on CI
            //Assert.IsTrue(Timems_NOomp >= (1.50 * Timems_omp), "Open MP enabled should be performed at least 50% faster");
        }

        [TestMethod]
        public void TestLutInterpolation()
        {
            LutInterpolation LinInterp = new LutInterpolation();
            int NbPoints = 16;
            var x = new double[NbPoints];
            var y = new double[NbPoints];

            x[0] = -2.5;
            y[0] = x[0] * x[0] * x[0];
            for (int i = 1; i < NbPoints; i++)
            {
                x[i] = x[i - 1] + 0.5;
                y[i] = x[i] * x[i] * x[i];
            }

            GCHandle xPin = GCHandle.Alloc(x, GCHandleType.Pinned);
            GCHandle yPin = GCHandle.Alloc(y, GCHandleType.Pinned);

            LinInterp.CopyArraysFrom(NbPoints, (ulong)xPin.AddrOfPinnedObject(), (ulong)yPin.AddrOfPinnedObject());

            xPin.Free();
            yPin.Free();

            double[] xExpect = new double[] { -3.0, -1.25, 1.2, 3.65, 5.5 };
            double[] yExpect = new double[] { -15.625, -2.1875, 1.95, 49.2125, 125 }; // value outside array are bounded to ymin and ymax
            double yVal = 0.0; double xVal = 0.0; double detlaTolerance = 0.000001;
            //Console.WriteLine($"---- Search Y from x values");
            for (int i = 0; i < xExpect.Length; i++)
            {
                yVal = LinInterp.Y(xExpect[i]);
                //Console.WriteLine($"x[{i}] = {xExpect[i]} => y = {yVal}");
                Assert.AreEqual(yExpect[i], yVal, detlaTolerance, $"for x[{i}]");

            }

            //Console.WriteLine($"---- Search X from y values");
            xExpect[0] = -2.5; xExpect[4] = 5.0; // value outside array are bounded to xmin and xmax
            for (int i = 0; i < xExpect.Length; i++)
            {
                xVal = LinInterp.X(yExpect[i]);
                //Console.WriteLine($"y[{i}] = {yExpect[i]} => x = {xVal}");
                Assert.AreEqual(xExpect[i], xVal, detlaTolerance, $"for y[{i}]");
            }

            LutInterpolation LinInterp2 = LinInterp;
            xVal = 4.2;
            double expected = LinInterp.Y(xVal);
            Assert.AreEqual(expected, LinInterp2.Y(xVal), detlaTolerance, $"copy contructor failure");


            //LutInterpolation LinInterp3 = new LutInterpolation();
            //LinInterp3.CopyArraysFrom(x, y);
            ////Console.WriteLine($"---- Search Y from x values and managed arrays");
            //for (int i = 0; i < xExpect.Length; i++)
            //{
            //    xVal = LinInterp3.X(yExpect[i]);
            //    //Console.WriteLine($"y[{i}] = {yExpect[i]} => x = {xVal}");
            //    Assert.AreEqual(xExpect[i], xVal, detlaTolerance, $"Lin3 array @y[{i}]");
            //}
        }

        [TestMethod]
        public void TestInterpolationMBA()
        {
            Interpolator2D Interp = new Interpolator2D(InterpolateAlgoType.MBA);

            Assert.IsFalse(Interp.InitSettings(null), "null settings array");

            double[] badsettings = { 0.0, 3.0 };
            Assert.IsFalse(Interp.InitSettings(badsettings), "bad size settings array");

            double[] settingsInit = { -1.0, -1.0, 10.0, 10.0, 3.0, 3.0, 100.0 };
            Assert.IsTrue(Interp.InitSettings(settingsInit), "InitSettings with initializer");

            // loX, loY, HiX, HiY, GridX, GridY, Initializer
            double[] settings = { -1.0, -1.0, 10.0, 10.0, 3.0, 3.0 };
            Assert.IsTrue(Interp.InitSettings(settings), "InitSettings");

            Assert.IsFalse(Interp.ComputeData(), "Empty inputs cannot compute");

            double[] badx0 = { 0.0, 9.0 };
            double[] bady0 = { 0.0, 9.0, 6.0 };
            double[] badz0 = { 0.0 };
            Assert.IsFalse(Interp.SetInputsPoints(badx0, badx0, null), " Bad Set Inputs array 3");
            Assert.IsFalse(Interp.SetInputsPoints(badx0, null, badx0), " Bad Set Inputs array 2");
            Assert.IsFalse(Interp.SetInputsPoints(null, badx0, badx0), " Bad Set Inputs array 1");
            Assert.IsFalse(Interp.SetInputsPoints(badx0, bady0, badz0), " Bad Set Inputs array sizes - missing 1 Z values");
            Interp.ResetInputsPoints();
            Assert.IsTrue(Interp.SetInputsPoints(badx0, bady0, badx0), " Allowed if miniaml size X is reached");
            Interp.ResetInputsPoints();

            double[] x0 = { 0.0, 0.0, 3.0, 9.0, 9.0 };
            double[] y0 = { 0.0, 9.0, 3.0, 0.0, 9.0 };
            double[] z0 = { 100.0, 95.0, 105.0, 101.0, 98.0 };
            Assert.IsTrue(Interp.SetInputsPoints(x0, y0, z0), "SetInputsPoints");

            Assert.IsTrue(Interp.ComputeData(), "ComputeData");

            for (int i = 0; i < 5; i++)
            {
                double interpval = Interp.Interpolate(x0[i], y0[i]);
                if (z0[i] == 0.0)
                    z0[i] = 1e-8;
                double pctErr = Math.Abs(z0[i] - interpval) / Math.Abs(z0[i]);
                //Console.WriteLine($"z[{i}]= {z0[i]} -> {x0[i]}x{y0[i]} zinterp={interpval}, err%= {pctErr}");
                Assert.IsTrue(pctErr < 0.001);
            }

            //Console.WriteLine($"1 1> zinterp={Interp.Interpolate(1.0, 1.0)}");
            //Console.WriteLine($"8 8> zinterp={Interp.Interpolate(8.0, 8.0)}");
            //Console.WriteLine($"3.5 5.0> zinterp={Interp.Interpolate(3.5, 5.0)}");
            Assert.AreEqual(102.423334183746, Interp.Interpolate(1.0, 1.0), 0.0001);
            Assert.AreEqual(101.977786264771, Interp.Interpolate(8.0, 8.0), 0.0001);
            Assert.AreEqual(107.756479440569, Interp.Interpolate(3.5, 5.0), 0.0001);

            int gridW = 50;
            int gridH = gridW;
            double xStart = -0.5;
            double yStart = -0.5;
            double xEnd = 9.5;
            double yEnd = 9.5;
            double xStep = (xEnd - xStart) / (gridW - 1);
            double yStep = (yEnd - yStart) / (gridH - 1);
            double[] gridMap = new double[gridW * gridH];

            GCHandle pinGridMap = GCHandle.Alloc(gridMap, GCHandleType.Pinned);

            Assert.IsTrue(Interp.InterpolateGrid((ulong)pinGridMap.AddrOfPinnedObject(), gridW, gridH, xStart, xStep, yStart, yStep), "InterpolateGrid");

            pinGridMap.Free();

            //Console.WriteLine($"0> grid map={gridMap[0]}");
            //Console.WriteLine($"250> grid map={gridMap[250]}");
            //Console.WriteLine($"500> grid map={gridMap[500]}");
            //Console.WriteLine($"12 40> grid map={gridMap[50*40+12]}");

            Assert.AreEqual(97.2328533902716, gridMap[0], 0.0001);
            Assert.AreEqual(98.8999050431473, gridMap[250], 0.0001);
            Assert.AreEqual(97.9218655232545, gridMap[500], 0.0001);
            Assert.AreEqual(102.466791298046, gridMap[50 * 40 + 12], 0.0001);

        }

        [TestMethod]
        public void TestInterpolationIDW()
        {
            Interpolator2D Interp = new Interpolator2D(InterpolateAlgoType.IDW);

            Assert.IsFalse(Interp.InitSettings(null), "null settings array");

            double[] badsettings = { 0.0, 3.0 };
            Assert.IsFalse(Interp.InitSettings(badsettings), "bad size settings array");
            double[] badsettingsValue = { -1.0 };
            Assert.IsFalse(Interp.InitSettings(badsettings), "bad value setting");

            // POW
            double[] settings = { 3.0 };
            Assert.IsTrue(Interp.InitSettings(settings), "InitSettings");

            Assert.IsFalse(Interp.ComputeData(), "Empty inputs cannot compute");

            double[] badx0 = { 0.0, 9.0 };
            double[] bady0 = { 0.0, 9.0, 6.0 };
            double[] badz0 = { 0.0 };
            Assert.IsFalse(Interp.SetInputsPoints(badx0, badx0, null), " Bad Set Inputs array 3");
            Assert.IsFalse(Interp.SetInputsPoints(badx0, null, badx0), " Bad Set Inputs array 2");
            Assert.IsFalse(Interp.SetInputsPoints(null, badx0, badx0), " Bad Set Inputs array 1");
            Assert.IsFalse(Interp.SetInputsPoints(badx0, bady0, badz0), " Bad Set Inputs array sizes - missing 1 Z values");
            Interp.ResetInputsPoints();
            Assert.IsTrue(Interp.SetInputsPoints(badx0, bady0, badx0), " Allowed if miniaml size X is reached");
            Interp.ResetInputsPoints();

            double[] x0 = { 0.0, 0.0, 3.0, 9.0, 9.0 };
            double[] y0 = { 0.0, 9.0, 3.0, 0.0, 9.0 };
            double[] z0 = { 100.0, 95.0, 105.0, 101.0, 98.0 };
            Assert.IsTrue(Interp.SetInputsPoints(x0, y0, z0), "SetInputsPoints");

            Assert.IsTrue(Interp.ComputeData(), "ComputeData");

            for (int i = 0; i < 5; i++)
            {
                double interpval = Interp.Interpolate(x0[i], y0[i]);
                if (z0[i] == 0.0)
                    z0[i] = 1e-8;
                double pctErr = Math.Abs(z0[i] - interpval) / Math.Abs(z0[i]);
                //Console.WriteLine($"z[{i}]= {z0[i]} -> {x0[i]}x{y0[i]} zinterp={interpval}, err%= {pctErr}");
                Assert.IsTrue(pctErr < 0.001);
            }

            //Console.WriteLine($"1 1> zinterp={Interp.Interpolate(1.0, 1.0)}");
            //Console.WriteLine($"8 8> zinterp={Interp.Interpolate(8.0, 8.0)}");
            //Console.WriteLine($"3.5 5.0> zinterp={Interp.Interpolate(3.5, 5.0)}");
            Assert.AreEqual(100.526922302109, Interp.Interpolate(1.0, 1.0), 0.0001);
            Assert.AreEqual(98.058688597453, Interp.Interpolate(8.0, 8.0), 0.0001);
            Assert.AreEqual(104.078126774855, Interp.Interpolate(3.5, 5.0), 0.0001);

            int gridW = 50;
            int gridH = gridW;
            double xStart = -0.5;
            double yStart = -0.5;
            double xEnd = 9.5;
            double yEnd = 9.5;
            double xStep = (xEnd - xStart) / (gridW - 1);
            double yStep = (yEnd - yStart) / (gridH - 1);
            double[] gridMap = new double[gridW * gridH];

            GCHandle pinGridMap = GCHandle.Alloc(gridMap, GCHandleType.Pinned);

            Assert.IsTrue(Interp.InterpolateGrid((ulong)pinGridMap.AddrOfPinnedObject(), gridW, gridH, xStart, xStep, yStart, yStep), "InterpolateGrid");

            pinGridMap.Free();

            //Console.WriteLine($"0> grid map={gridMap[0]}");
            //Console.WriteLine($"250> grid map={gridMap[250]}");
            //Console.WriteLine($"500> grid map={gridMap[500]}");
            //Console.WriteLine($"12 40> grid map={gridMap[50 * 40 + 12]}");

            Assert.AreEqual(100.012594135231, gridMap[0], 0.0001);
            Assert.AreEqual(100.020696266837, gridMap[250], 0.0001);
            Assert.AreEqual(100.309385933403, gridMap[500], 0.0001);
            Assert.AreEqual(96.2713749095315, gridMap[50 * 40 + 12], 0.0001);

        }

        [TestMethod]
        public void TestInterpolationFNN_MonoChannel()
        {
            Interpolator2D Interp = new Interpolator2D(InterpolateAlgoType.fNN);

            Assert.IsFalse(Interp.InitSettings(null), "null settings array");

            // no argument - Initializer
            double[] settings = { 0.0 };
            Assert.IsTrue(Interp.InitSettings(settings), "InitSettings");

            Assert.IsFalse(Interp.ComputeData(), "Empty inputs cannot compute");

            double[] badx0 = { 0.0, 9.0 };
            double[] bady0 = { 0.0, 9.0, 6.0 };
            double[] badz0 = { 0.0 };
            Assert.IsFalse(Interp.SetInputsPoints(badx0, badx0, null), " Bad Set Inputs array 3");
            Assert.IsFalse(Interp.SetInputsPoints(badx0, null, badx0), " Bad Set Inputs array 2");
            Assert.IsFalse(Interp.SetInputsPoints(null, badx0, badx0), " Bad Set Inputs array 1");
            Assert.IsFalse(Interp.SetInputsPoints(badx0, bady0, badz0), " Bad Set Inputs array sizes - missing 1 Z values");
            Interp.ResetInputsPoints();
            Assert.IsTrue(Interp.SetInputsPoints(badx0, bady0, badx0), " Allowed if miniaml size X is reached");
            Interp.ResetInputsPoints();

            double[] x0 = { 0.0, 0.0, 3.0, 9.0, 9.0 };
            double[] y0 = { 0.0, 9.0, 3.0, 0.0, 9.0 };
            double[] z0 = { 100.0, 95.0, 105.0, 101.0, 98.0 };
            Assert.IsTrue(Interp.SetInputsPoints(x0, y0, z0), "SetInputsPoints");

            Assert.IsTrue(Interp.ComputeData(), "ComputeData");

            for (int i = 0; i < 5; i++)
            {
                double interpval = Interp.Interpolate(x0[i], y0[i]);
                if (z0[i] == 0.0)
                    z0[i] = 1e-8;
                double pctErr = Math.Abs(z0[i] - interpval) / Math.Abs(z0[i]);
                //Console.WriteLine($"z[{i}]= {z0[i]} -> {x0[i]}x{y0[i]} zinterp={interpval}, err%= {pctErr}");
                Assert.IsTrue(pctErr < 0.001);
            }

            //Console.WriteLine($"1 1> zinterp={Interp.Interpolate(1.0, 1.0)}");
            //Console.WriteLine($"8 8> zinterp={Interp.Interpolate(8.0, 8.0)}");
            //Console.WriteLine($"3.5 5.0> zinterp={Interp.Interpolate(3.5, 5.0)}");
            //Console.WriteLine($"8.5 0.0> zinterp={Interp.Interpolate(3.5, 5.0)}");
            //Console.WriteLine($"0.0 7.0> zinterp={Interp.Interpolate(3.5, 5.0)}");

            Assert.AreEqual(100.0, Interp.Interpolate(1.0, 1.0), 0.0001);
            Assert.AreEqual(98.0, Interp.Interpolate(8.0, 8.0), 0.0001);
            Assert.AreEqual(105.0, Interp.Interpolate(3.5, 5.0), 0.0001);
            Assert.AreEqual(101.0, Interp.Interpolate(8.5, .0), 0.0001);
            Assert.AreEqual(95.0, Interp.Interpolate(0.0, 7.0), 0.0001);

            int gridW = 50;
            int gridH = gridW;
            double xStart = -0.5;
            double yStart = -0.5;
            double xEnd = 9.5;
            double yEnd = 9.5;
            double xStep = (xEnd - xStart) / (gridW - 1);
            double yStep = (yEnd - yStart) / (gridH - 1);
            double[] gridMap = new double[gridW * gridH];

            GCHandle pinGridMap = GCHandle.Alloc(gridMap, GCHandleType.Pinned);

            Assert.IsTrue(Interp.InterpolateGrid((ulong)pinGridMap.AddrOfPinnedObject(), gridW, gridH, xStart, xStep, yStart, yStep), "InterpolateGrid");

            pinGridMap.Free();

            //Console.WriteLine($"0> grid map={gridMap[0]}");
            //Console.WriteLine($"250> grid map={gridMap[250]}");
            //Console.WriteLine($"500> grid map={gridMap[500]}");
            //Console.WriteLine($"12 40> grid map={gridMap[50 * 40 + 12]}");

            Assert.AreEqual(100.0, gridMap[0], 0.0001);
            Assert.AreEqual(100.0, gridMap[250], 0.0001);
            Assert.AreEqual(100.0, gridMap[500], 0.0001);
            Assert.AreEqual(95.0, gridMap[50 * 40 + 12], 0.0001);

        }

        [TestMethod]
        public void TestInterpolationFNN_DualChannel()
        {
            Interpolator2D Interp = new Interpolator2D(InterpolateAlgoType.fNN);

            Assert.IsFalse(Interp.InitSettings(null), "null settings array");

            // no argument - Initializer
            double[] settings = { 0.0 };
            Assert.IsTrue(Interp.InitSettings(settings), "InitSettings");

            Assert.IsFalse(Interp.ComputeData(), "Empty inputs cannot compute");

            double[] x0 = { 0.0, 0.0, 3.0, 9.0, 9.0 };
            double[] y0 = { 0.0, 9.0, 3.0, 0.0, 9.0 };
            double[] z0 = { 100.0, 95.0, 105.0, 101.0, 98.0 };
            double[] z1 = { -10.0, -9.5, -10.5, -10.1, -9.8 };
            Assert.IsTrue(Interp.SetInputsPoints(x0, y0, z0, z1), "SetInputsPoints Dual");

            Assert.IsTrue(Interp.ComputeData(), "ComputeData");

            for (int i = 0; i < 5; i++)
            {
                double interpval1 = 0.0;
                double interpval2 = 0.0;
                Interp.Interpolate(x0[i], y0[i], ref interpval1, ref interpval2);
                if (z0[i] == 0.0)
                    z0[i] = 1e-8;
                double pctErr = Math.Abs(z0[i] - interpval1) / Math.Abs(z0[i]);
                //Console.WriteLine($"z[{i}]= {z0[i]} -> {x0[i]}x{y0[i]} zinterp={interpval1}, err%= {pctErr}");
                Assert.IsTrue(pctErr < 0.001);

                double pctErr2 = Math.Abs(z1[i] - interpval2) / Math.Abs(z1[i]);
                //Console.WriteLine($"z[{i}]= {z1[i]} -> {x0[i]}x{y0[i]} zinterp={interpval2}, err%= {pctErr2}");
            }

            double ival1 = 0.0; double ival2 = 0.0;
            Interp.Interpolate(1.0, 1.0, ref ival1, ref ival2);
            Assert.AreEqual(100.0, ival1, 0.0001); Assert.AreEqual(-10.00, ival2, 0.0001);

            Interp.Interpolate(8.0, 8.0, ref ival1, ref ival2);
            Assert.AreEqual(98.0, ival1, 0.0001); Assert.AreEqual(-9.80, ival2, 0.0001);

            Interp.Interpolate(3.5, 5.0, ref ival1, ref ival2);
            Assert.AreEqual(105.0, ival1, 0.0001); Assert.AreEqual(-10.5, ival2, 0.0001);

            Interp.Interpolate(8.5, 0.0, ref ival1, ref ival2);
            Assert.AreEqual(101.0, ival1, 0.0001); Assert.AreEqual(-10.1, ival2, 0.0001);

            Interp.Interpolate(0.0, 7.0, ref ival1, ref ival2);
            Assert.AreEqual(95.0, ival1, 0.0001); Assert.AreEqual(-9.5, ival2, 0.0001);

            int gridW = 50;
            int gridH = gridW;
            double xStart = -0.5;
            double yStart = -0.5;
            double xEnd = 9.5;
            double yEnd = 9.5;
            double xStep = (xEnd - xStart) / (gridW - 1);
            double yStep = (yEnd - yStart) / (gridH - 1);
            double[] gridMap1 = new double[gridW * gridH];
            double[] gridMap2 = new double[gridW * gridH];
            GCHandle pinGridMap1 = GCHandle.Alloc(gridMap1, GCHandleType.Pinned);
            GCHandle pinGridMap2 = GCHandle.Alloc(gridMap2, GCHandleType.Pinned);

            Assert.IsTrue(Interp.InterpolateGrid((ulong)pinGridMap1.AddrOfPinnedObject(), (ulong)pinGridMap2.AddrOfPinnedObject(), gridW, gridH, xStart, xStep, yStart, yStep), "InterpolateGrid");

            pinGridMap2.Free();
            pinGridMap1.Free();

            //Console.WriteLine($"0> grid map={gridMap1[0]}");
            //Console.WriteLine($"250> grid map={gridMap1[250]}");
            //Console.WriteLine($"500> grid map={gridMap1[500]}");
            //Console.WriteLine($"12 40> grid map={gridMap1[50 * 40 + 12]}");

            Assert.AreEqual(100.0, gridMap1[0], 0.0001);
            Assert.AreEqual(-10.0, gridMap2[0], 0.0001);
            Assert.AreEqual(100.0, gridMap1[250], 0.0001);
            Assert.AreEqual(-10.0, gridMap2[250], 0.0001);
            Assert.AreEqual(100.0, gridMap1[500], 0.0001);
            Assert.AreEqual(-10.0, gridMap2[500], 0.0001);
            Assert.AreEqual(95.0, gridMap1[50 * 40 + 12], 0.0001);
            Assert.AreEqual(-9.5, gridMap2[50 * 40 + 12], 0.0001);
        }

        [TestMethod]
        public void TestInterpolationQuadNN_DualChannel()
        {
            Interpolator2D Interp = new Interpolator2D(InterpolateAlgoType.QuadNN);

            Assert.IsFalse(Interp.InitSettings(null), "null settings array");

            // no argument - Initializer
            double[] settings0 = { 0.0 };
            Assert.IsFalse(Interp.InitSettings(settings0), "InitSettings");

            double[] badsettings = { 0.0, 3.0 };
            Assert.IsFalse(Interp.InitSettings(badsettings), "bad size settings array");

            double[] settingsInit = { -1.0, -1.0, 10.0, 10.0 };
            Assert.IsTrue(Interp.InitSettings(settingsInit), "InitSettings without margin");

            //wafer Angle in degree, wafer Tx in mm, wafer Ty in mm, grid step in mm, Margin in mm
            double[] settings = { 0.5, 0.8, -0.924, 75.0, 0.006 };
            Assert.IsTrue(Interp.InitSettings(settings), "InitSettings");

            Assert.IsFalse(Interp.ComputeData(), "Empty inputs cannot compute");

            double[] x0 = { -75.0, 0.0, 75.0, -75.0, 0.0, 75.0, -75.0, 0.0, 75.0 };
            double[] y0 = { 75.0, 75.0, 75.0, 0.0, 0.0, 0.0, -75.0, -75.0, -75.0 };
            double cosA = Math.Cos(Math.PI * settings[0] / 180.0);
            double sinA = Math.Sin(Math.PI * settings[0] / 180.0);
            for (int i = 0; i < 9; ++i)
            {
                double x = x0[i]; double y = y0[i];
                x0[i] = settings[1] + (x - settings[1]) * cosA + (y - settings[2]) * sinA;
                y0[i] = settings[2] - (x - settings[1]) * sinA + (y - settings[2]) * cosA;
            }

            double[] z0 = { 100.0, 105.0, 103.0,
                             97.0, 101.0, 104.0,
                             95.0, 98.0,  99.0 };
            double[] z1 = { -10.0, -10.5, -10.3, -9.7, -10.1, -10.4, -9.5, -9.8, -9.9 }; // *-1.0/10.0
            Assert.IsTrue(Interp.SetInputsPoints(x0, y0, z0, z1), "SetInputsPoints Dual");

            Assert.IsTrue(Interp.ComputeData(), "ComputeData");

            for (int i = 0; i < 9; i++)
            {
                double interpval1 = 0.0;
                double interpval2 = 0.0;
                Interp.Interpolate(x0[i], y0[i], ref interpval1, ref interpval2);
                if (z0[i] == 0.0)
                    z0[i] = 1e-8;
                double pctErr = Math.Abs(z0[i] - interpval1) / Math.Abs(z0[i]);
                //Console.WriteLine($"z[{i}]= {z0[i]} -> {x0[i]}x{y0[i]} zinterp={interpval1}, err%= {pctErr}");
                Assert.IsTrue(pctErr < 0.001);

                double pctErr2 = Math.Abs(z1[i] - interpval2) / Math.Abs(z1[i]);
                //Console.WriteLine($"z[{i}]= {z1[i]} -> {x0[i]}x{y0[i]} zinterp={interpval2}, err%= {pctErr2}");
            }


            double ival1 = 0.0; double ival2 = 0.0;

            // QUadrant TL
            Interp.Interpolate(-0.5, 0.5, ref ival1, ref ival2); //Console.WriteLine($"v1 = {ival1}");
            Assert.AreEqual(100.998687450778, ival1, 0.0001); Assert.AreEqual(ival1 * -0.1, ival2, 0.0001);

            // QUadrant BL
            Interp.Interpolate(-0.5, -0.5, ref ival1, ref ival2); //Console.WriteLine($"v1 = {ival1}");
            Assert.AreEqual(100.952729997793, ival1, 0.0001); Assert.AreEqual(ival1 * -0.1, ival2, 0.0001);

            // QUadrant TR
            Interp.Interpolate(0.5, 0.5, ref ival1, ref ival2); //Console.WriteLine($"v1 = {ival1}");
            Assert.AreEqual(101.045811997519, ival1, 0.0001); Assert.AreEqual(ival1 * -0.1, ival2, 0.0001);

            // QUadrant BR
            Interp.Interpolate(0.5, -0.5, ref ival1, ref ival2); //Console.WriteLine($"v1 = {ival1}");
            Assert.AreEqual(100.999658392707, ival1, 0.0001); Assert.AreEqual(ival1 * -0.1, ival2, 0.0001);

            Interp.Interpolate(40.0, -45.0, ref ival1, ref ival2); //Console.WriteLine($"v1 = {ival1}");
            Assert.AreEqual(100.187881493239, ival1, 0.0001); Assert.AreEqual(ival1 * -0.1, ival2, 0.0001);

            int gridW = 50;
            int gridH = gridW;
            double xStart = -80.0;
            double yStart = -80.0;
            double xEnd = 80.0;
            double yEnd = 80.0;
            double xStep = (xEnd - xStart) / (gridW - 1);
            double yStep = (yEnd - yStart) / (gridH - 1);
            double[] gridMap1 = new double[gridW * gridH];
            double[] gridMap2 = new double[gridW * gridH];
            GCHandle pinGridMap1 = GCHandle.Alloc(gridMap1, GCHandleType.Pinned);
            GCHandle pinGridMap2 = GCHandle.Alloc(gridMap2, GCHandleType.Pinned);

            Assert.IsTrue(Interp.InterpolateGrid((ulong)pinGridMap1.AddrOfPinnedObject(), (ulong)pinGridMap2.AddrOfPinnedObject(), gridW, gridH, xStart, xStep, yStart, yStep), "InterpolateGrid");

            pinGridMap2.Free();
            pinGridMap1.Free();

            //Console.WriteLine($"0 = {gridMap1[0]}");
            //Console.WriteLine($"12 = {gridMap1[12]}");
            //Console.WriteLine($"394 = {gridMap1[394]}");
            //Console.WriteLine($"747 = {gridMap1[747]}");
            //Console.WriteLine($"1024 = {gridMap1[1024]}");
            //Console.WriteLine($"2372 = {gridMap1[2372]}");

            int rk = 0;
            rk = 0; Assert.AreEqual(95.0, gridMap1[rk], 0.0001); Assert.AreEqual(gridMap1[rk] * -0.1, gridMap2[rk], 0.0001);
            rk = 12; Assert.AreEqual(95.0, gridMap1[rk], 0.0001); Assert.AreEqual(gridMap1[rk] * -0.1, gridMap2[rk], 0.0001);
            rk = 394; Assert.AreEqual(100.011748291028, gridMap1[rk], 0.0001); Assert.AreEqual(gridMap1[rk] * -0.1, gridMap2[rk], 0.0001);
            rk = 747; Assert.AreEqual(101.721820242903, gridMap1[rk], 0.0001); Assert.AreEqual(gridMap1[rk] * -0.1, gridMap2[rk], 0.0001);
            rk = 1024; Assert.AreEqual(100.334711864536, gridMap1[rk], 0.0001); Assert.AreEqual(gridMap1[rk] * -0.1, gridMap2[rk], 0.0001);
            rk = 2372; Assert.AreEqual(104.329094757744, gridMap1[rk], 0.0001); Assert.AreEqual(gridMap1[rk] * -0.1, gridMap2[rk], 0.0001);

        }
    }
}
