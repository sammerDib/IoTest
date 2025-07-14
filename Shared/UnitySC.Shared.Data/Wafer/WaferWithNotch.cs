using System;
using System.Windows;

using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    public class WaferWithNotch : CircularWafer, IWaferShape
    {
        public WaferWithNotch(PointUnits waferCenter, Length waferRadius, Length edgeExclusionSize, NotchDimentionalCharacteristic notch) : base(waferCenter, waferRadius, edgeExclusionSize)
        {
            NotchRadius = notch.Depth;
            NotchLocationOnWafer = notch.Angle;
        }

        public override bool IsInside(PointUnits point, bool applyEdgeExlusion = true)
        {
            return IsinsideValidAreaOfCircularWafer(point, applyEdgeExlusion) && !IsInsideNotch(point);
        }

        protected bool IsInsideNotch(PointUnits point)
        {
            // when the notch is at 0 in dimensional caracteristics, it is at -PI/2 in Analyse
            var realNotchAngle = NotchLocationOnWafer - (Math.PI / 2).Radians();
            var notchCenter = new PointUnits(WaferCenter.X + WaferRadius * Math.Cos(realNotchAngle.Radians), WaferCenter.Y + WaferRadius * Math.Sin(realNotchAngle.Radians));
            return MathUtils.IsInsideCircle(point, notchCenter, NotchRadius);
        }

        public Length NotchRadius;
        public Angle NotchLocationOnWafer; //the wafer is considered as a trigonometric circle
    }
}
