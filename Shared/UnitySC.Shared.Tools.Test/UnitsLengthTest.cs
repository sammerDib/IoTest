using System;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Tools.Test
{
    [TestClass]
    public class UnitsLengthTest
    {
        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
        public void LengthCreationTest()
        {
            var lengthMicro = new Length(10, LengthUnit.Micrometer);
            Assert.AreEqual(lengthMicro.Value, 10);
            Assert.AreEqual(0.01, lengthMicro.Millimeters);
            Assert.AreEqual(10000, lengthMicro.Nanometers);
            Assert.AreEqual(0.001, lengthMicro.Centimeters);
            Assert.AreEqual(1e-5, lengthMicro.Meters);
            Assert.AreEqual(LengthUnit.Micrometer, lengthMicro.Unit);
            Assert.AreEqual("µm", lengthMicro.UnitSymbol);
        }

        [TestMethod]
        public void LengthConversionTest()
        {
            var lengthMicro = new Length(10, LengthUnit.Micrometer);
            // Conversion
            var lengthMili = lengthMicro.ToUnit(LengthUnit.Millimeter);
            Assert.AreEqual(0.01, lengthMili.Value);
            Assert.AreEqual(LengthUnit.Micrometer, lengthMicro.Unit);

            var lengthMicro2 = lengthMili.ToUnit(LengthUnit.Micrometer);
            Assert.AreEqual(10, lengthMicro2.Value);
        }

        [TestMethod]
        public void LengthArithmeticTest()
        {
            var lengthMicro = new Length(10, LengthUnit.Micrometer);
            // Sum
            var lengthMicro2 = new Length(5, LengthUnit.Micrometer);
            var lengthMicroSum = lengthMicro + lengthMicro2;
            Assert.AreEqual(15, lengthMicroSum.Value);
            Assert.AreEqual(LengthUnit.Micrometer, lengthMicroSum.Unit);

            var lengthNano = new Length(2, LengthUnit.Nanometer);
            lengthMicroSum = lengthMicro + lengthNano;
            Assert.AreEqual(10.002, lengthMicroSum.Value);
            Assert.AreEqual(LengthUnit.Micrometer, lengthMicroSum.Unit);

            var lengthNearMax = new Length((double)(decimal.MaxValue - 1), LengthUnit.Micrometer);
            var nearMaxMilimeters = lengthNearMax.Millimeters;
            Assert.AreEqual(7.9228162514264339E+25, nearMaxMilimeters);
        }

        [TestMethod]
        public void LengthEqualityTest()
        {
            // Equality
            var length1 = 4.5.Micrometers();
            var length2 = 0.0045.Millimeters();

            Assert.IsTrue(length1 == length2);
            Assert.IsTrue(length1.Equals(length2));

            // Almost equal
            var length3 = length1 + 0.00001.Micrometers();
            Assert.IsTrue(length1 != length3);
            Assert.IsFalse(length1.Near(length3, 0.000001.Micrometers()));
            Assert.IsTrue(length1.Near(length3, 0.00001.Micrometers()));
            Assert.IsTrue(length1.Near(length3, 0.0001.Micrometers()));
        }

        [TestMethod]
        public void LengthComparisonTest()
        {
            var length1 = 4.5.Micrometers();
            var length2 = 0.0045.Millimeters();
            // Greater than
            var length3 = length2 + length1;
            Assert.IsTrue(length3 > length2);
            Assert.IsFalse(length3 < length2);
        }

        [TestMethod]
        public void LengthToStringTest()
        {
            var length1 = 4.5.Micrometers();
            // ToString
            var lengthString = length1.ToString();
            Assert.AreEqual("4.5 µm", lengthString);
        }

        [TestMethod]
        public void LengthToMostRepresentativeTest()
        {
            /// 0.05mm will be 50µm
            var length = new Length(0.05, LengthUnit.Millimeter);
            var lengthRepresentative = length.ToMostRepresentativeUnit();
            Assert.AreEqual(LengthUnit.Micrometer, lengthRepresentative.Unit);
            Assert.AreEqual(50.0, lengthRepresentative.Value);

            /// 0.35mm will be 350µm
            length = new Length(0.35, LengthUnit.Millimeter);
            lengthRepresentative = length.ToMostRepresentativeUnit();
            Assert.AreEqual(LengthUnit.Micrometer, lengthRepresentative.Unit);
            Assert.AreEqual(350.0, lengthRepresentative.Value);

            /// 50000mm will be 50m
            length = new Length(50000, LengthUnit.Millimeter);
            lengthRepresentative = length.ToMostRepresentativeUnit();
            Assert.AreEqual(LengthUnit.Meter, lengthRepresentative.Unit);
            Assert.AreEqual(50.0, lengthRepresentative.Value);

            /// 55mm will be 5.5cm
            length = new Length(55, LengthUnit.Millimeter);
            lengthRepresentative = length.ToMostRepresentativeUnit();
            Assert.AreEqual(LengthUnit.Centimeter, lengthRepresentative.Unit);
            Assert.AreEqual(5.5, lengthRepresentative.Value);

            /// 0.0005µm will be 0.5nm
            length = new Length(0.0005, LengthUnit.Micrometer);
            lengthRepresentative = length.ToMostRepresentativeUnit();
            Assert.AreEqual(LengthUnit.Nanometer, lengthRepresentative.Unit);
            Assert.AreEqual(0.5, lengthRepresentative.Value);
        }
    }
}
