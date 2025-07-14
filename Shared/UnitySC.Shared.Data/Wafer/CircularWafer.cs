using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    public class CircularWafer : IWaferShape
    {
        public CircularWafer(PointUnits waferCenter, Length waferRadius, Length edgeExclusionSize)
        {
            WaferCenter = waferCenter;
            WaferRadius = waferRadius;
            EdgeExclusionSize = edgeExclusionSize;
        }

        public virtual bool IsInside(PointUnits point, bool applyEdgeExlusion = true)
        {
            return IsinsideValidAreaOfCircularWafer(point, applyEdgeExlusion);
        }

        protected bool IsinsideValidAreaOfCircularWafer(PointUnits point, bool applyEdgeExlusion = true)
        {
            var radius = WaferRadius;
            if (applyEdgeExlusion)
                radius -= EdgeExclusionSize;

            return MathUtils.IsInsideCircle(point, WaferCenter, radius);
        }

        public PointUnits WaferCenter;
        public Length WaferRadius;
        public Length EdgeExclusionSize;
    }
}
