using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test
{
    [TestClass]
    public class PointInsideWaferWithFlatsTest
    {
        [TestMethod]
        public void Nominal_case()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 1.Millimeters();

            var flat1 = new FlatDimentionalCharacteristic();
            flat1.Angle = 0.Degrees();
            flat1.ChordLength = 100.Millimeters();
            var flat2 = new FlatDimentionalCharacteristic();
            flat2.Angle = 270.Degrees();
            flat2.ChordLength = 100.Millimeters();
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { flat1, flat2 };

            var point = new PointUnits(-148.Millimeters(), 0.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

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
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { };

            var angleOfPointOnCircle = 45.Degrees();
            var locationOfPointOnCircle = new PointUnits(waferCenter.X + waferRadius * Math.Cos(angleOfPointOnCircle.Radians), waferCenter.Y + waferRadius * Math.Sin(angleOfPointOnCircle.Radians));
            var point = new PointUnits(locationOfPointOnCircle.X - 1.Millimeters(), locationOfPointOnCircle.Y - 1.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

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
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { };

            var angleOfPointOnCircle = 45.Degrees();
            var locationOfPointOnCircle = new PointUnits(waferCenter.X + waferRadius * Math.Cos(angleOfPointOnCircle.Radians), waferCenter.Y + waferRadius * Math.Sin(angleOfPointOnCircle.Radians));
            var point = new PointUnits(locationOfPointOnCircle.X + 1.Millimeters(), locationOfPointOnCircle.Y + 1.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

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
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { };

            var angleOfPointOnCircle = 45.Degrees();
            var locationOfPointOnCircle = new PointUnits(waferCenter.X + waferRadius * Math.Cos(angleOfPointOnCircle.Radians), waferCenter.Y + waferRadius * Math.Sin(angleOfPointOnCircle.Radians));
            var point = new PointUnits(0.Millimeters(), -150.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

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
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { };

            var angleOfPointOnCircle = 45.Degrees();
            var locationOfPointOnCircle = new PointUnits(waferCenter.X + waferRadius * Math.Cos(angleOfPointOnCircle.Radians), waferCenter.Y + waferRadius * Math.Sin(angleOfPointOnCircle.Radians));
            var point = new PointUnits(locationOfPointOnCircle.X - 1.Millimeters(), locationOfPointOnCircle.Y - 1.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }

        [TestMethod]
        public void Point_located_inside_flat_at_right_is_outside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var flat = new FlatDimentionalCharacteristic();
            flat.Angle = 0.Degrees();
            flat.ChordLength = 100.Millimeters();
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { flat };

            var point = new PointUnits(149.Millimeters(), 0.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }

        [TestMethod]
        public void Point_located_inside_flat_at_left_is_outside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var flat = new FlatDimentionalCharacteristic();
            flat.Angle = 180.Degrees();
            flat.ChordLength = 100.Millimeters();
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { flat };

            var point = new PointUnits(-149.Millimeters(), 0.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }

        [TestMethod]
        public void Point_located_inside_flat_at_top_is_outside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var flat = new FlatDimentionalCharacteristic();
            flat.Angle = 90.Degrees();
            flat.ChordLength = 100.Millimeters();
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { flat };

            var point = new PointUnits(0.Millimeters(), 149.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }

        [TestMethod]
        public void Point_located_inside_flat_at_bottom_is_outside_wafer()
        {
            // Given
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());
            var waferRadius = 150.Millimeters();
            var edgeExclusion = 0.Millimeters();

            var flat = new FlatDimentionalCharacteristic();
            flat.Angle = 270.Degrees();
            flat.ChordLength = 100.Millimeters();
            List<FlatDimentionalCharacteristic> flats = new List<FlatDimentionalCharacteristic>() { flat };

            var point = new PointUnits(0.Millimeters(), -149.Millimeters());

            // When
            var waferwithFlats = new WaferWithFlats(waferCenter, waferRadius, edgeExclusion, flats);
            var isInside = waferwithFlats.IsInside(point);

            // Then
            Assert.IsFalse(isInside);
        }
    }
}
