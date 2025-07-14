using System;
using System.Windows;

using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Shared
{
    public class ROI
    {
        /// <summary> En µm </summary>
        public double WaferRadius { get; set; } = 100000;

        public RoiType RoiType;

        /// <summary> En microns, dans le repère wafer </summary>
        public Rect Rect = new Rect(-30000, -30000, 60000, 60000);

        /// <summary> En microns </summary>
        public double EdgeExclusion = 3000/*µm*/;

        /// <summary> Rectangle englobant en µm </summary>
        public Rect SurroundingRect
        {
            get
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular:
                        return Rect;

                    case RoiType.WholeWafer:
                        return RoiHelper.CreateSurroundingRectForWholeWaferRoi(WaferRadius, EdgeExclusion);

                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
        }
    }
}
