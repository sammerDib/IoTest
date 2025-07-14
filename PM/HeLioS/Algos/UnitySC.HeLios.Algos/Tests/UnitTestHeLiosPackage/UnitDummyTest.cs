using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Runtime.InteropServices;

using UnitySCSharedAlgosCppWrapper;
using UnitySC.HeLios.Algos.Package;

namespace UnitTestHeLiosPackage
{
    // Temporary Dummy unit testing for example
    //  - test access to Hélios package 

    [TestClass]
    public class UnitDummyTest
    {
        [TestMethod]
        public void TestPackageLutInterpolation()
        {
            var obj = new HLSDummyClass();
            LutInterpolation LinInterp = new LutInterpolation();
            int NbPoints = 16;
            obj.Lut = LinInterp;
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

            obj.Lut.CopyArraysFrom(NbPoints, (ulong)xPin.AddrOfPinnedObject(), (ulong)yPin.AddrOfPinnedObject());

            xPin.Free();
            yPin.Free();

            double[] xExpect = new double[] { -3.0, -1.25, 1.2, 3.65, 5.5 };
            double[] yExpect = new double[] { -15.625, -2.1875, 1.95, 49.2125, 125 }; // value outside array are bounded to ymin and ymax
            double yVal = 0.0; double xVal = 0.0; double detlaTolerance = 0.000001;
            Console.WriteLine($"---- Search Y from x values");
            for (int i = 0; i < xExpect.Length; i++)
            {
                yVal = LinInterp.Y(xExpect[i]);
                Console.WriteLine($"x[{i}] = {xExpect[i]} => y = {yVal}");
                Assert.AreEqual(yExpect[i], yVal, detlaTolerance, $"for x[{i}]");

            }

            Console.WriteLine($"---- Search X from y values");
            xExpect[0] = -2.5; xExpect[4] = 5.0; // value outside array are bounded to xmin and xmax
            for (int i = 0; i < xExpect.Length; i++)
            {
                xVal = LinInterp.X(yExpect[i]);
                Console.WriteLine($"y[{i}] = {yExpect[i]} => x = {xVal}");
                Assert.AreEqual(xExpect[i], xVal, detlaTolerance, $"for y[{i}]");
            }

            xVal = obj.X(yExpect[2]);
            Assert.AreEqual(xExpect[2], xVal, detlaTolerance, $"from Hélios classes");

            LutInterpolation LinInterp2 = LinInterp;
            xVal = 4.2;
            double expected = LinInterp.Y(xVal);
            Assert.AreEqual(expected, LinInterp2.Y(xVal), detlaTolerance, $"copy contrsutor failure");


            LutInterpolation LinInterp3 = new LutInterpolation();
            LinInterp3.CopyArraysFrom(x, y);
            Console.WriteLine($"---- Search Y from x values and managed arrays");
            for (int i = 0; i < xExpect.Length; i++)
            {
                xVal = LinInterp3.X(yExpect[i]);
                Console.WriteLine($"y[{i}] = {yExpect[i]} => x = {xVal}");
                Assert.AreEqual(xExpect[i], xVal, detlaTolerance, $"Lin3 array @y[{i}]");
            }
        }
    }
}
