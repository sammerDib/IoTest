using System;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Tools.Test
{
    [TestClass]
    public class UnitsAngleTest
    {

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
        public void AngleCreationTest()
        {
            var angleDegree = new Angle(10, AngleUnit.Degree);
            Assert.AreEqual(10, angleDegree.Value );
            Assert.AreEqual(0.174532925199433, angleDegree.Radians );
            Assert.AreEqual(AngleUnit.Degree,angleDegree.Unit );
            Assert.AreEqual("°", angleDegree.UnitSymbol);
        }

        [TestMethod]
        public void AngleConversionTest()
        {
            var angleDegree = new Angle(10, AngleUnit.Degree);
            // Conversion
            var angleRadian = angleDegree.ToUnit(AngleUnit.Radian);
            Assert.AreEqual(0.174532925199433, angleRadian.Value );

            // ConvertBack
            var angleDegree2 = angleRadian.ToUnit(AngleUnit.Degree);
            Assert.AreEqual(angleDegree.Value, angleDegree2.Value );
        }

        [TestMethod]
        public void AngleArithmeticTest()
        {
            var angleDegree = new Angle(10, AngleUnit.Degree);
            // Sum
            var angleDegree3 = 80.Degrees();
            var angleSum = angleDegree + angleDegree3;
            Assert.AreEqual(90, angleSum.Value);

            // Dif
            var angleDif = angleDegree3 - angleDegree;
            Assert.AreEqual(70, angleDif.Value );
        }

        [TestMethod]
        public void AngleEqualityTest()
        {
            // Equality
            var angle1 = 45.Degrees();
            var angle2 = 45.Degrees();
            var angle3 = (Math.PI / 4).Radians();

            Assert.IsTrue(angle1 == angle2);

            // Almost Equality
            Assert.IsTrue(angle1.Near(angle3, 1e-10));
        }

        [TestMethod]
        public void AngleComparisonTest()
        {
            var angle1 = 45.Degrees();
            var angle2 = 50.Degrees();
            // Greater than
            Assert.IsTrue(angle2 > angle1);
            Assert.IsFalse(angle2 < angle1);
        }

        [TestMethod]
        public void AngleAbsTest()
        {
            var angle1 = 45.Degrees();
            Assert.AreEqual(45, angle1.Abs().Value);

            var angle2 = -30.Degrees();
            Assert.AreEqual(30, angle2.Abs().Value);
        }

        [TestMethod]
        public void AngleModuloTest()
        {
            var angle1 = 45.Degrees();
            var angle2=angle1.Modulo(ModuloType.Positive);
            Assert.AreEqual(angle1.Value, angle2.Value);
            Assert.AreEqual(angle1.Unit, angle2.Unit);

            angle1 = 370.Degrees();
            angle2 = angle1.Modulo(ModuloType.Positive);
            Assert.AreEqual(10, angle2.Value);

            angle1 = 360.Degrees();
            angle2 = angle1.Modulo(ModuloType.Positive);
            Assert.AreEqual(0, angle2.Value);

            angle1 = 0.Degrees();
            angle2 = angle1.Modulo(ModuloType.Positive);
            Assert.AreEqual(0, angle2.Value);

            angle1 = -20.Degrees();
            angle2 = angle1.Modulo(ModuloType.Positive);
            Assert.AreEqual(340, angle2.Value);

            angle1 = (2*Math.PI+1.5).Radians();
            angle2 = angle1.Modulo(ModuloType.Positive);
            Assert.IsTrue(angle2.Value.Near(1.5,1e-5));



            angle1 = 45.Degrees();
            angle2 = angle1.Modulo(ModuloType.PositiveAndNegative);
            Assert.AreEqual(angle1.Value, angle2.Value);
            Assert.AreEqual(angle1.Unit, angle2.Unit);

            angle1 = 370.Degrees();
            angle2 = angle1.Modulo(ModuloType.PositiveAndNegative);
            Assert.AreEqual(10, angle2.Value);

            angle1 = 360.Degrees();
            angle2 = angle1.Modulo(ModuloType.PositiveAndNegative);
            Assert.AreEqual(0, angle2.Value);

            angle1 = 0.Degrees();
            angle2 = angle1.Modulo(ModuloType.PositiveAndNegative);
            Assert.AreEqual(0, angle2.Value);

            angle1 = -20.Degrees();
            angle2 = angle1.Modulo(ModuloType.PositiveAndNegative);
            Assert.AreEqual(-20, angle2.Value);
            
            angle1 = 180.Degrees();
            angle2 = angle1.Modulo(ModuloType.PositiveAndNegative);
            Assert.AreEqual(180, angle2.Value);

            angle1 = (2 * Math.PI + 1.5).Radians();
            angle2 = angle1.Modulo(ModuloType.PositiveAndNegative);
            Assert.IsTrue(angle2.Value.Near(1.5, 1e-5));

            // Example : NearestAngleToZero
            Angle nearestAngleToZero;
            angle1 = 365.Degrees();
            angle2 = -3.Degrees();
            var angle1Modulo = angle1.Modulo(ModuloType.PositiveAndNegative);
            var distanceToZeroAngle1 = angle1Modulo - 0.Degrees();
            var distanceToZeroAngle2 = angle2 - 0.Degrees();
            nearestAngleToZero = distanceToZeroAngle2.Abs() > distanceToZeroAngle1.Abs() ? angle1 : angle2;
            Assert.AreEqual(angle2, nearestAngleToZero);
        }




        [TestMethod]
        public void AngleToStringTest()
        {
            var angleDegree = new Angle(10, AngleUnit.Degree);
            // ToString
            var angleString = angleDegree.ToString();
            Assert.AreEqual("10 °", angleString );
        }
    }
}
