using UnitySC.Shared.Data.Geometry;

namespace UnitySC.Shared.Data
{
    public interface IWaferShape
    {
        bool IsInside(PointUnits point, bool applyEdgeExlusion=true);
    }
}
