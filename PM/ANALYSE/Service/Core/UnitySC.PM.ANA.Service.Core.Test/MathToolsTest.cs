using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test
{
    [TestClass]
    public class MathToolsTest
    {
        [DataTestMethod]
        [DataRow(-90, -150, 140, -150, 150, -160, 150)]
        [DataRow(-180, -150, 140, -150, 150, -150, 160)]
        [DataRow(-270, -150, 140, -150, 150, -140, 150)]
        [DataRow(-360, -150, 140, -150, 150, -150, 140)]
        [DataRow(90, -150, 140, -150, 150, -140, 150)]
        [DataRow(180, -150, 140, -150, 150, -150, 160)]
        [DataRow(270, -150, 140, -150, 150, -160, 150)]
        [DataRow(360, -150, 140, -150, 150, -150, 140)]
        public void Expect_correct_rotation(int angleInDegrees, double initialXPosition, double initialYPosition, double centerX, double centerY, double expectedXPosition, double expectedYPosition)
        {
            var position = new XYZTopZBottomPosition(new WaferReferential(), initialXPosition, initialYPosition, 0, 0);
            var centerRotation = new XYPosition(new WaferReferential(), centerX, centerY);

            MathTools.ApplyAntiClockwiseRotation(angleInDegrees.Degrees(), position, centerRotation);
            Assert.AreEqual(expectedXPosition, position.X, 10e-12);
            Assert.AreEqual(expectedYPosition, position.Y, 10e-12);
        }

        [TestMethod]
        public void Expect_correct_angle()
        {
            var positionA = new XYPosition(new WaferReferential(), -150, 140);
            var positionB = new XYPosition(new WaferReferential(), -140, 150);

            var angle = MathTools.ComputeAngleFromTwoPositions(positionA, positionB);
            Assert.AreEqual(45, angle.Degrees, 10e-5);
        }

        [TestMethod]
        public void Orthogonal_projection_of_point_onto_abscisse_axes()
        {
            var positionA = new XYPosition(new WaferReferential(), 0, 0);
            var positionB = new XYPosition(new WaferReferential(), 140, 0);
            var position = new XYPosition(new WaferReferential(), 12, 50);

            var positionOnLineAB = MathTools.OrthogonalProjectionOfPointOntoLine(positionA, positionB, position);

            Assert.AreEqual(12, positionOnLineAB.X);
            Assert.AreEqual(0, positionOnLineAB.Y);
        }

        [TestMethod]
        public void Orthogonal_projection_of_point_onto_ordinate_axes()
        {
            var positionA = new XYPosition(new WaferReferential(), 0, 0);
            var positionB = new XYPosition(new WaferReferential(), 0, 140);
            var position = new XYPosition(new WaferReferential(), 12, 50);

            var positionOnLineAB = MathTools.OrthogonalProjectionOfPointOntoLine(positionA, positionB, position);

            Assert.AreEqual(0, positionOnLineAB.X);
            Assert.AreEqual(50, positionOnLineAB.Y);
        }

        [TestMethod]
        public void Orthogonal_projection_of_point_onto_line()
        {
            var positionA = new XYPosition(new WaferReferential(), 0, 0);
            var positionB = new XYPosition(new WaferReferential(), 10, -10);
            var position = new XYPosition(new WaferReferential(), 1, -3);

            var positionOnLineAB = MathTools.OrthogonalProjectionOfPointOntoLine(positionA, positionB, position);

            Assert.AreEqual(2.0, positionOnLineAB.X, 10e-10);
            Assert.AreEqual(-2.0, positionOnLineAB.Y, 10e-10);
        }

        [TestMethod]
        public void Expect_0_degrees_angle_from_vectors()
        {
            var origVect1 = new XYPosition(new WaferReferential(), -145, 0);
            var destVect1 = new XYPosition(new WaferReferential(), 145, 0);
            var orgiVect2 = new XYPosition(new WaferReferential(), -145, 0);
            var destVect2 = new XYPosition(new WaferReferential(), 145, 0);

            var angle = MathTools.ComputeAngleFromTwoVectors(origVect1, destVect1, orgiVect2, destVect2);

            Assert.AreEqual(0, angle.Degrees, 10e-5);
        }

        [TestMethod]
        public void Expect_positive_45_degrees_angle_from_vectors()
        {
            var origVect1 = new XYPosition(new WaferReferential(), 100, 0);
            var destVect1 = new XYPosition(new WaferReferential(), -100, 0);
            var orgiVect2 = new XYPosition(new WaferReferential(), 50, 50);
            var destVect2 = new XYPosition(new WaferReferential(), -50, -50);

            var angle = MathTools.ComputeAngleFromTwoVectors(origVect1, destVect1, orgiVect2, destVect2);

            Assert.AreEqual(45, angle.Degrees, 10e-5);
        }

        [TestMethod]
        public void Expect_negative_45_degrees_angle_from_vectors()
        {
            var origVect1 = new XYPosition(new WaferReferential(), 100, 0);
            var destVect1 = new XYPosition(new WaferReferential(), -100, 0);
            var orgiVect2 = new XYPosition(new WaferReferential(), 50, -50);
            var destVect2 = new XYPosition(new WaferReferential(), -50, 50);

            var angle = MathTools.ComputeAngleFromTwoVectors(origVect1, destVect1, orgiVect2, destVect2);

            Assert.AreEqual(-45, angle.Degrees, 10e-5);
        }

        [TestMethod]
        public void Expect_positive_90_degrees_angle_from_vectors()
        {
            var origVect1 = new XYPosition(new WaferReferential(), -10, 0);
            var destVect1 = new XYPosition(new WaferReferential(), 10, 0);
            var orgiVect2 = new XYPosition(new WaferReferential(), 2, -3);
            var destVect2 = new XYPosition(new WaferReferential(), 2, 3);

            var angle = MathTools.ComputeAngleFromTwoVectors(origVect1, destVect1, orgiVect2, destVect2);

            Assert.AreEqual(90, angle.Degrees, 10e-5);
        }

        [TestMethod]
        public void Expect_negative_90_degrees_angle_from_vectors()
        {
            var origVect1 = new XYPosition(new WaferReferential(), -10, 0);
            var destVect1 = new XYPosition(new WaferReferential(), 10, 0);
            var orgiVect2 = new XYPosition(new WaferReferential(), 2, 3);
            var destVect2 = new XYPosition(new WaferReferential(), 2, -3);

            var angle = MathTools.ComputeAngleFromTwoVectors(origVect1, destVect1, orgiVect2, destVect2);

            Assert.AreEqual(-90, angle.Degrees, 10e-5);

        }

        [TestMethod]
        public void Expect_positive_180_degrees_angle_from_vectors()
        {
            var origVect1 = new XYPosition(new WaferReferential(), -145, 0);
            var destVect1 = new XYPosition(new WaferReferential(), 145, 0);
            var orgiVect2 = new XYPosition(new WaferReferential(), 145, 0);
            var destVect2 = new XYPosition(new WaferReferential(), -145, 0);

            var angle = MathTools.ComputeAngleFromTwoVectors(origVect1, destVect1, orgiVect2, destVect2);

            Assert.AreEqual(180, angle.Degrees, 10e-5);
        }

        [TestMethod]
        public void Expect_negative_180_degrees_angle_from_vectors()
        {
            var origVect1 = new XYPosition(new WaferReferential(), 145, 0);
            var destVect1 = new XYPosition(new WaferReferential(), -145, 0);
            var orgiVect2 = new XYPosition(new WaferReferential(), -145, 0);
            var destVect2 = new XYPosition(new WaferReferential(), 145, 0);

            var angle = MathTools.ComputeAngleFromTwoVectors(origVect1, destVect1, orgiVect2, destVect2);

            Assert.AreEqual(-180, angle.Degrees, 10e-5);
        }
    }
}
