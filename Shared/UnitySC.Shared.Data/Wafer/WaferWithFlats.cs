using System;
using System.Collections.Generic;

using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    public class WaferWithFlats : CircularWafer, IWaferShape
    {
        public WaferWithFlats(PointUnits waferCenter, Length waferRadius, Length edgeExclusionSize, List<FlatDimentionalCharacteristic> flats) : base(waferCenter, waferRadius, edgeExclusionSize)
        {
            Flats = flats;
        }

        public override bool IsInside(PointUnits point, bool applyEdgeExlusion = true)
        {
            return IsinsideValidAreaOfCircularWafer(point, applyEdgeExlusion) && !IsInsideFlats(point, applyEdgeExlusion);
        }

        protected bool IsInsideFlats(PointUnits point, bool applyEdgeExlusion = true)
        {
            var validWaferRadius = WaferRadius;
            if (applyEdgeExlusion)
                validWaferRadius -= EdgeExclusionSize;

            var isInside = false;
            foreach (FlatDimentionalCharacteristic flat in Flats)
            {
                var centralAngleOfFlat = ComputeCentralAngleOfChord(flat.ChordLength, WaferRadius * 2);

                var firstPointOnFlat = ComputeFirstIntersectionBetweenChordAndCircle(WaferCenter, validWaferRadius, centralAngleOfFlat, flat.Angle);
                var secondPointOnFlat = ComputeSecondIntersectionBetweenChordAndCircle(WaferCenter, validWaferRadius, centralAngleOfFlat, flat.Angle);
                var isOnTheGoodSide = MathUtils.PositionFromLine(firstPointOnFlat, secondPointOnFlat, point) < 0;
                if (isOnTheGoodSide)
                {
                    return true;
                }
            }
            return isInside;
        }

        private static Angle ComputeCentralAngleOfChord(Length chordLength, Length circleDiameter)
        {
            var centralAngle = Math.Asin(chordLength.Millimeters / circleDiameter.Millimeters) * 2;
            return centralAngle.Radians();
        }

        private static PointUnits ComputeFirstIntersectionBetweenChordAndCircle(PointUnits waferCenter, Length waferRadius, Angle centralAngleOfFlat, Angle flatPositionAngle)
        {
            var endAngleOfFlatOnWafer = flatPositionAngle - (centralAngleOfFlat / 2);
            return new PointUnits(waferCenter.X + waferRadius * Math.Cos(endAngleOfFlatOnWafer.Radians), waferCenter.Y + waferRadius * Math.Sin(endAngleOfFlatOnWafer.Radians));
        }

        private static PointUnits ComputeSecondIntersectionBetweenChordAndCircle(PointUnits waferCenter, Length waferRadius, Angle centralAngleOfFlat, Angle flatPositionAngle)
        {
            var endAngleOfFlatOnWafer = flatPositionAngle + (centralAngleOfFlat / 2);
            return new PointUnits(waferCenter.X + waferRadius * Math.Cos(endAngleOfFlatOnWafer.Radians), waferCenter.Y + waferRadius * Math.Sin(endAngleOfFlatOnWafer.Radians));
        }

        public List<FlatDimentionalCharacteristic> Flats;
    }
}
