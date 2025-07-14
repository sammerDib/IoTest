using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public static class ReferencePlanePositionsHelper
    {
        public class AirGapWithPosition
        {
            public AirGapWithPosition(Length airGap, double xPosition, double yPosition)
            {
                AirGap = airGap;
                XPosition = xPosition;
                YPosition = yPosition;
            }

            public Length AirGap { get; set; }
            public double XPosition { get; set; }
            public double YPosition { get; set; }
        }

        public static List<XYPosition> GenerateDefaultReferencePlanePositions(List<Angle> defaultReferencePlanePointsAngularPositions, Length distanceFromCenter)
        {
            var defaultReferencePlanePositions = new List<XYPosition>();
            var waferCenter = new XYPosition(new WaferReferential(), 0, 0);

            foreach (Angle angle in defaultReferencePlanePointsAngularPositions)
            {
                var referencePlanePosition = new XYPosition(new WaferReferential(), distanceFromCenter.Millimeters, 0);
                MathTools.ApplyAntiClockwiseRotation(angle, referencePlanePosition, waferCenter);
                defaultReferencePlanePositions.Add(referencePlanePosition);
            }

            return defaultReferencePlanePositions;
        }

        public static void RotateUnreachableDefaultReferencePlanePositions(List<XYPosition> unreachablePlanePositions, Angle rotationAngle)
        {
            var waferCenter = new XYPosition(new WaferReferential(), 0, 0);
            foreach (var planePosition in unreachablePlanePositions)
            {
                MathTools.ApplyAntiClockwiseRotation(rotationAngle, planePosition, waferCenter);
            }
        }

        public static bool AreDefaultReferencePlanePositionsUnreachable(AnaHardwareManager hardwareManager, string probeId)
        {
            if (probeId == null)
            {
                return false;
            }

            bool isProbeDown = hardwareManager.Probes[probeId].Configuration.ModulePosition == ModulePositions.Down;
            return isProbeDown && hardwareManager.Chuck.Configuration.IsOpenChuck;
        }

        public static void ThrowIfMaxReferencePlaneAngleExceeded(List<AirGapWithPosition> referencePlaneAirGaps, Angle maxRefPlaneAngle)
        {
            // Plane points are supposed to be on a centered regular polyhedron. As
            // a consequence, the max angle formed among all points must be between 2
            // consecutive points.
            double maxTanPlaneAngle = double.NegativeInfinity;
            for (int currentGapIndex = 0; currentGapIndex < referencePlaneAirGaps.Count; currentGapIndex++)
            {
                int nextGapIndex = (currentGapIndex + 1) % referencePlaneAirGaps.Count;
                var currentGap = referencePlaneAirGaps[currentGapIndex];
                var nextGap = referencePlaneAirGaps[nextGapIndex];
                double distance = Math.Sqrt(Math.Pow(currentGap.XPosition - nextGap.XPosition, 2) + Math.Pow(currentGap.YPosition - nextGap.YPosition, 2));
                double airGapDifference = Math.Abs((currentGap.AirGap - nextGap.AirGap).Millimeters);
                maxTanPlaneAngle = Math.Max(maxTanPlaneAngle, airGapDifference / distance);
            }

            Angle maxReferencePlaneAngle = Math.Atan(maxTanPlaneAngle).Radians();
            if (maxReferencePlaneAngle > maxRefPlaneAngle)
            {
                throw new Exception($"Plane angle exceeds the configuration limit: {maxReferencePlaneAngle}>{maxRefPlaneAngle}");
            }
        }

    }
}
