using System;
using UnitySC.PM.WOTAN.Common;

namespace UnitySC.PM.WOTAN.Processing
{

    public class AlignResult
    {
        private double _markTypeScore;
        private int _angularPosition;
        private int _shiftXPx, _shiftYPx;

        private readonly LaneImage _laneImage;
        private readonly int _angularSize;

        public AlignResult()
        {
            throw new System.Exception("A LaneImage object must be specified.");
        }

        public AlignResult(LaneImage laneImage, int angularSize)
        {
            _angularSize = angularSize;
            _laneImage = laneImage;
            _markTypeScore = double.MaxValue;
        }

        public double MarkTypeScore { get => _markTypeScore; set => _markTypeScore = value; }
        public int AngularPosition { get => _angularPosition; set => _angularPosition = value; }
        public int ShiftXPx { get => _shiftXPx; set => _shiftXPx = value; }
        public int ShiftYPx { get => _shiftYPx; set => _shiftYPx = value; }
        public double ShiftXUm { get => _laneImage.ToUm(_shiftXPx); }
        public double ShiftYUm { get => _laneImage.ToUm(_shiftYPx); }

        public double AngleCorrection(AngleUnit angleUnit) // [0; 360] or [0; 2PI] trigonometric
        {
            double angularPortion = (double)_angularPosition / (double)_angularSize;
            switch (angleUnit)
            {
                case AngleUnit.Deg:
                    return 360 * (1 - angularPortion);
                case AngleUnit.Rad:
                    return 2 * Math.PI * (1 - angularPortion);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
