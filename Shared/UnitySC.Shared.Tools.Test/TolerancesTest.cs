using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Tools.Test
{
    [TestClass]
    public class TolerancesTest
    {
        [TestMethod]
        public void IsInAbsoluteTolerance()
        {
            var tolerance = new Tolerance(10, ToleranceUnit.AbsoluteValue);
            Assert.IsTrue(tolerance.IsInTolerance(190,200));
            Assert.IsTrue(tolerance.IsInTolerance(210, 200));
            Assert.IsFalse(tolerance.IsInTolerance(189, 200));
            Assert.IsFalse(tolerance.IsInTolerance(211, 200));
        }

        [TestMethod]
        public void IsInPercentageTolerance()
        {
            var tolerance = new Tolerance(10, ToleranceUnit.Percentage);
            Assert.IsTrue(tolerance.IsInTolerance(180, 200));
            Assert.IsTrue(tolerance.IsInTolerance(220, 200));
            Assert.IsFalse(tolerance.IsInTolerance(179, 200));
            Assert.IsFalse(tolerance.IsInTolerance(221, 200));
        }

        [TestMethod]
        public void IsInAbsoluteLengthTolerance()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Micrometer);
            Assert.IsTrue(tolerance.IsInTolerance(190.Micrometers(), 200.Micrometers()));
            Assert.IsTrue(tolerance.IsInTolerance(210.Micrometers(), 200.Micrometers()));
            Assert.IsFalse(tolerance.IsInTolerance(189.Micrometers(), 200.Micrometers()));
            Assert.IsFalse(tolerance.IsInTolerance(211.Micrometers(), 200.Micrometers()));

            // With conversion
            Assert.IsTrue(tolerance.IsInTolerance(0.190.Millimeters(), 200000.Nanometers()));
            Assert.IsTrue(tolerance.IsInTolerance(0.210.Millimeters(), 200000.Nanometers()));
            Assert.IsFalse(tolerance.IsInTolerance(0.189.Millimeters(), 200000.Nanometers()));
            Assert.IsFalse(tolerance.IsInTolerance(0.211.Millimeters(), 200000.Nanometers()));
        }

        [TestMethod]
        public void IsInPercentageLengthTolerance()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Percentage);
            Assert.IsTrue(tolerance.IsInTolerance(180.Micrometers(), 200.Micrometers()));
            Assert.IsTrue(tolerance.IsInTolerance(220.Micrometers(), 200.Micrometers()));
            Assert.IsFalse(tolerance.IsInTolerance(179.Micrometers(), 200.Micrometers()));
            Assert.IsFalse(tolerance.IsInTolerance(221.Micrometers(), 200.Micrometers()));

            // With conversion
            Assert.IsTrue(tolerance.IsInTolerance(0.180.Millimeters(), 200.Micrometers()));
            Assert.IsTrue(tolerance.IsInTolerance(0.220.Millimeters(), 200.Micrometers()));
            Assert.IsFalse(tolerance.IsInTolerance(0.179.Millimeters(), 200.Micrometers()));
            Assert.IsFalse(tolerance.IsInTolerance(0.221.Millimeters(), 200.Micrometers()));
        }
    }
}
