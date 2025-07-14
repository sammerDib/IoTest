using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test
{
    [TestClass]
    public class PointInsideCircularWafer
    {
        [TestMethod]
        public void Nominal_case()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 1.Millimeters();

            var point = new PointUnits(-148.Millimeters(), 0.Millimeters());

            // When
            var wafer = new CircularWafer(waferCenter, waferRadius, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsTrue(isInside);
        }

        [TestMethod]
        public void Point_located_inside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var angleOfPointOnCircle = 45.Degrees();
            var locationOfPointOnCircle = new PointUnits(waferCenter.X + waferRadius * Math.Cos(angleOfPointOnCircle.Radians), waferCenter.Y + waferRadius * Math.Sin(angleOfPointOnCircle.Radians));
            var point = new PointUnits(locationOfPointOnCircle.X - 1.Millimeters(), locationOfPointOnCircle.Y - 1.Millimeters());

            // When
            var wafer = new CircularWafer(waferCenter, waferRadius, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsTrue(isInside);
        }

        [TestMethod]
        public void Point_located_outside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var angleOfPointOnCircle = 45.Degrees();
            var locationOfPointOnCircle = new PointUnits(waferCenter.X + waferRadius * Math.Cos(angleOfPointOnCircle.Radians), waferCenter.Y + waferRadius * Math.Sin(angleOfPointOnCircle.Radians));
            var point = new PointUnits(locationOfPointOnCircle.X + 1.Millimeters(), locationOfPointOnCircle.Y + 1.Millimeters());

            // When
            var wafer = new CircularWafer(waferCenter, waferRadius, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }

        [TestMethod]
        public void Point_located_on_edge_is_inside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var point = new PointUnits(0.Millimeters(), 150.Millimeters());

            // When
            var wafer = new CircularWafer(waferCenter, waferRadius, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsTrue(isInside);
        }

        [TestMethod]
        public void Point_located_inside_edge_exclusion_is_outside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 2.Millimeters();

            var angleOfPointOnCircle = 45.Degrees();
            var locationOfPointOnCircle = new PointUnits(waferCenter.X + waferRadius * Math.Cos(angleOfPointOnCircle.Radians), waferCenter.Y + waferRadius * Math.Sin(angleOfPointOnCircle.Radians));
            var point = new PointUnits(locationOfPointOnCircle.X - 1.Millimeters(), locationOfPointOnCircle.Y - 1.Millimeters());

            // When
            var wafer = new CircularWafer(waferCenter, waferRadius, edgeExclusion);
            var isInside = wafer.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }
    }
}
