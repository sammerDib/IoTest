using System;
using System.Windows;

namespace UnitySC.PM.DMT.Service.Interface.Recipe
{
    public static class RoiHelper
    {
        public static Rect CreateSurroundingRectForWholeWaferRoi(double waferRadiusMicrometers,
            double edgeExclusionMicrometers)
        {
            double xCoordinate = waferRadiusMicrometers - edgeExclusionMicrometers;
            return CreateSquareCenteredOnOrigin(xCoordinate);
        }

        public static Rect CreateRectInsideWholeWafer(double waferRadiusMicrometers, double edgeExclusionMicrometers)
        {
            double xCoordinate = (waferRadiusMicrometers - edgeExclusionMicrometers) / Math.Sqrt(2);
            return CreateSquareCenteredOnOrigin(xCoordinate);
        }

        private static Rect CreateSquareCenteredOnOrigin(double xEdgeCoordinate)
        {
            var topLeft = new Point(-xEdgeCoordinate, xEdgeCoordinate);
            var bottomRight = new Point(xEdgeCoordinate, -xEdgeCoordinate);
            return new Rect(topLeft, bottomRight);
        }
    }
}
