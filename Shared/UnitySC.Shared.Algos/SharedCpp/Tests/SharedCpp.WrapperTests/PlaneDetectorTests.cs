using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosCppWrapper;

namespace SharedCpp.WrapperTests
{
    [TestClass]
    public class PlaneDetectorTests
    {
        private double _tolerance = 1e-6;

        private void AssertPointsEqual(Point3d expected, Point3d actual)
        {
            Assert.AreEqual(expected.X, actual.X, _tolerance, "X should be equal");
            Assert.AreEqual(expected.Y, actual.Y, _tolerance, "Y should be equal");
            Assert.AreEqual(expected.Z, actual.Z, _tolerance, "Z should be equal");
        }

        [TestMethod]
        public void TooFewPointsReturnsNull()
        {
            // Given only 2 points
            Point3d[] points = { new Point3d(0, 0, 0), new Point3d(0, 1, 0) };

            // When detecting plane
            Plane plane = PlaneDetector.FindLeastSquarePlane(points);

            // The it is null
            Assert.IsNull(plane);
        }

        [TestMethod]
        public void InfinitePointReturnsNull()
        {
            // Given an infinite coordinate
            Point3d[] points = { new Point3d(0, 0, 0), new Point3d(0, 1, 0), new Point3d(double.PositiveInfinity, 1, 0) };

            // When detecting plane
            Plane plane = PlaneDetector.FindLeastSquarePlane(points);

            // The it is null
            Assert.IsNull(plane);
        }

        [TestMethod]
        public void ColinearPointsReturnsNull()
        {
            // Given 3 colinear points
            Point3d[] points = { new Point3d(0, 0, 0), new Point3d(1, 1, 1), new Point3d(2, 2, 2) };

            // When detecting plane
            Plane plane = PlaneDetector.FindLeastSquarePlane(points);

            // The it is null
            Assert.IsNull(plane);
        }

        [TestMethod]
        public void BasicHorizontalPlaneFound()
        {
            // Given points on a horizontal plane
            Point3d[] points = { new Point3d(1, 0, 0), new Point3d(0, 1, 0), new Point3d(0, 0, 0) };

            // When detecting plane
            Plane plane = PlaneDetector.FindLeastSquarePlane(points);

            // Then plane is found
            AssertPointsEqual(new Point3d(1.0 / 3, 1.0 / 3, 0), plane.Center);
            AssertPointsEqual(new Point3d(0, 0, -1), plane.Normal);
        }

        [TestMethod]
        public void BasicTiltedPlaneFound()
        {
            // Given points on a tilted plane
            Point3d[] points = { new Point3d(1, 0, 0), new Point3d(0, 1, 0), new Point3d(0, 0, 1) };

            // When detecting plane
            Plane plane = PlaneDetector.FindLeastSquarePlane(points);

            // Then plane is found
            AssertPointsEqual(new Point3d(1.0 / 3, 1.0 / 3, 1.0 / 3), plane.Center);
            AssertPointsEqual(new Point3d(-1, -1, -1), plane.Normal);
        }

        [TestMethod]
        public void ComplexHorizontalPlaneFound()
        {
            // Given points on a horizontal plane
            Point3d[] points = {
                new Point3d(1, 1, 1), new Point3d(1, 1, -1),
                new Point3d(-1, 1, 2), new Point3d(-1, 1, -2),
                new Point3d(-1, -1, 3), new Point3d(-1, -1, -3),
                new Point3d(1, -1, 4), new Point3d(1, -1, -4)
            };

            // When detecting plane
            Plane plane = PlaneDetector.FindLeastSquarePlane(points);

            // Then plane is found
            AssertPointsEqual(new Point3d(0, 0, 0), plane.Center);
            AssertPointsEqual(new Point3d(0, 0, -1), plane.Normal);
        }
    }
}
