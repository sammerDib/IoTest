using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test
{
    [TestClass]
    public class PointInsideSampleWafer
    {
        [TestMethod]
        public void Nominal_case()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferHeight = 300.Millimeters();
            var waferWidth = 200.Millimeters();
            var edgeExclusion = 10.Millimeters();

            var point = new PointUnits(-89.Millimeters(), 0.Millimeters());

            // When
            var wafer = new SampleWafer(waferCenter, waferHeight, waferWidth, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsTrue(isInside);
        }

        [TestMethod]
        public void Point_located_inside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferHeight = 300.Millimeters();
            var waferWidth = 200.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var point = new PointUnits(99.Millimeters(), 149.Millimeters());

            // When
            var wafer = new SampleWafer(waferCenter, waferHeight, waferWidth, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsTrue(isInside);
        }

        [TestMethod]
        public void Point_located_outside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferHeight = 300.Millimeters();
            var waferWidth = 200.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var point = new PointUnits(101.Millimeters(), 151.Millimeters());

            // When
            var wafer = new SampleWafer(waferCenter, waferHeight, waferWidth, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }

        [TestMethod]
        public void Point_located_on_edge_is_inside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferHeight = 300.Millimeters();
            var waferWidth = 200.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var point = new PointUnits(100.Millimeters(), 150.Millimeters());

            // When
            var wafer = new SampleWafer(waferCenter, waferHeight, waferWidth, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsTrue(isInside);
        }

        [TestMethod]
        public void Point_located_inside_edge_exclusion_is_outside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferHeight = 300.Millimeters();
            var waferWidth = 200.Millimeters();
            var edgeExclusion = 2.Millimeters();

            var point = new PointUnits(99.Millimeters(), 149.Millimeters());

            // When
            var wafer = new SampleWafer(waferCenter, waferHeight, waferWidth, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }
    }
}
