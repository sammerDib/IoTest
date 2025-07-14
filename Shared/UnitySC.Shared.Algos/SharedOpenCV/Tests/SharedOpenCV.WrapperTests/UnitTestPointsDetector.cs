using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosOpenCVWrapper;

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class UnitTestPointsDetector
    {
        private static double s_precision = 1e-4;

        [TestMethod]
        public void Points_detector_correctly_detects_transformation_params()
        {
            // Given point on x-axis rotated to be on the y-axis a translation and scale
            var from = new List<Point2d>
            {
                new Point2d(0.0, 0.0),
                new Point2d(1.0, 0.0),
                new Point2d(2.0, 0.0),
            };

            var to = new List<Point2d>
            {
                new Point2d(0.5, 0.2),
                new Point2d(0.5, 2.2),
                new Point2d(0.5, 4.2),
            };

            // When calling function
            var res = PointsDetector.OptimalTransformationParameters(from.ToArray(), to.ToArray());

            // Then transformation parameters are the proper ones
            Assert.IsNotNull(res);
            Assert.AreEqual(0.5, res.Translation.X, s_precision, "Unexpected x-translation");
            Assert.AreEqual(0.2, res.Translation.Y, s_precision, "Unexpected y-translation");
            Assert.AreEqual(2.0, res.Scale, s_precision, "Unexpected scale");
            Assert.AreEqual(Math.PI / 2, res.RotationRad, s_precision, "Unexpected rotation");
        }

        [TestMethod]
        public void Points_detector_Less_than_3_points_returns_null()
        {
            // Given bad input: only 2 points
            var from = new List<Point2d>
            {
                new Point2d(0.0, 0.0),
                new Point2d(1.0, 0.0),
            };

            var to = new List<Point2d>
            {
                new Point2d(0.5, 0.2),
                new Point2d(0.5, 2.2),
            };

            // When calling function
            var res = PointsDetector.OptimalTransformationParameters(from.ToArray(), to.ToArray());

            // Then result is null
            Assert.IsNull(res);
        }

        [TestMethod]
        public void Points_detector_Different_number_points_returns_null()
        {
            // Given bad input: only 2 points
            var from = new List<Point2d>
            {
                new Point2d(0.0, 0.0),
                new Point2d(1.0, 0.0),
                new Point2d(2.0, 0.0),
                new Point2d(3.0, 0.0),
            };

            var to = new List<Point2d>
            {
                new Point2d(0.5, 0.2),
                new Point2d(0.5, 2.2),
                new Point2d(0.5, 4.2),
            };

            // When calling function
            var res = PointsDetector.OptimalTransformationParameters(from.ToArray(), to.ToArray());

            // Then result is null
            Assert.IsNull(res);
        }

        [TestMethod]
        public void Points_detector_Infeasible_input_returns_null()
        {
            // Given bad input: only 2 points
            var from = new List<Point2d>
            {
                new Point2d(1.0, 0.0),
                new Point2d(1.0, 0.0),
                new Point2d(1.0, 0.0),
            };

            var to = new List<Point2d>
            {
                new Point2d(1.0, 0.0),
                new Point2d(-1.0, 0.0),
                new Point2d(-1.0, 0.0),
            };

            // When calling function
            var res = PointsDetector.OptimalTransformationParameters(from.ToArray(), to.ToArray());

            // Then result is null
            Assert.IsNull(res);
        }
    }
}
