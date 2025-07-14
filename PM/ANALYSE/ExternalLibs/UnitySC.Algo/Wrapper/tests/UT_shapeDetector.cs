using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgosLibrary;
using System.Collections.Generic;
using System;

namespace UnitySC.Algorithms.Wrapper.Tests
{
    [TestClass]
    public class ShapeDetectorTest
    {
        [TestInitialize]
        public void Init() { }

        [TestMethod]
        public void Ellipse_detector_correctly_detects_ellipse()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("shape/1_centered_ellipse.png"));
            Point2d expectedCenter = new Point2d(img.Width / 2, img.Height / 2);
            int expectedEllipseNb = 1;
            Tuple<double, double> expectedAxes = new Tuple<double, double>(295, 385);
            int cannyThreshold = 100;
            int detectionTolerance = 10;
            var param = new EllipseFinderParams(expectedAxes, detectionTolerance, cannyThreshold);

            // When
            var ellipses = ShapeDetector.EllipseDetect(img, param);

            // Then : We obtain correct values
            double expectedMajorAxis = expectedAxes.Item1 > expectedAxes.Item2 ? expectedAxes.Item1 : expectedAxes.Item2;
            double expectedMinorAxis = expectedAxes.Item1 <= expectedAxes.Item2 ? expectedAxes.Item1 : expectedAxes.Item2;

            Assert.AreEqual(expectedEllipseNb, ellipses.Length);
            for (int i = 0; i < ellipses.Length; i++)
            {
                var majorAxis = ellipses[i].WidthAxis > ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
                var minorAxis = ellipses[i].WidthAxis <= ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
                Assert.AreEqual(expectedCenter.X, ellipses[i].Center.X, 11);
                Assert.AreEqual(expectedCenter.Y, ellipses[i].Center.Y, 11);
                Assert.AreEqual(expectedMinorAxis, minorAxis, 5);
                Assert.AreEqual(expectedMajorAxis, majorAxis, 5);
            }
        }

        [TestMethod]
        public void Cicle_detector_correctly_detects_circle()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("shape/1_centered_circle.jpg"));
            Point2d expectedCenter = new Point2d(img.Width / 2, img.Height / 2);
            int expectedCircleNb = 1;
            int expectedCircleDiameter = 170;
            double minDistBetweenTwoCircles = 10;
            int cannyThreshold = 100;
            int detectionTolerance = 10;
            var param = new CircleFinderParams(minDistBetweenTwoCircles, expectedCircleDiameter, detectionTolerance, cannyThreshold);

            // When
            var circles = ShapeDetector.CircleDetect(img, param);

            // Then : We obtain correct values
            Assert.AreEqual(expectedCircleNb, circles.Length);
            for (int i = 0; i < circles.Length; i++)
            {
                Assert.AreEqual(expectedCenter.X, circles[i].Center.X, 10);
                Assert.AreEqual(expectedCenter.Y, circles[i].Center.Y, 10);
                Assert.AreEqual(expectedCircleDiameter, circles[i].Diameter, 6);
            }
        }
    }
}
