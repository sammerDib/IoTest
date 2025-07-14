using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    public class SampleWafer : IWaferShape
    {
        public SampleWafer(PointUnits waferCenter, Length waferHeight, Length waferWidth, Length edgeExclusionSize)
        {
            Center = waferCenter;
            Height = waferHeight;
            Width = waferWidth;
            EdgeExclusionSize = edgeExclusionSize;
        }

        public bool IsInside(PointUnits point, bool applyEdgeExlusion = true)
        {
            return IsinsideValidAreaOfSampleWafer(point);
        }

        public PointUnits Center;
        public Length Height;
        public Length Width;
        public Length EdgeExclusionSize;

        private bool IsinsideValidAreaOfSampleWafer(PointUnits point)
        {
            return MathUtils.IsInsideRectangle(point, Center, Height - EdgeExclusionSize * 2, Width - EdgeExclusionSize * 2);
        }
    }
}
