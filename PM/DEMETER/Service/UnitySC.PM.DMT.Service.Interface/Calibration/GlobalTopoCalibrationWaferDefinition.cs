using System;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    [Serializable]
    public class GlobalTopoCalibrationWaferDefinition
    {
        public float LeftCheckerBoardPositionX;
        public float LeftCheckerBoardPositionY;
        public float TopCheckerBoardPositionX;
        public float TopCheckerBoardPositionY;
        public float RightCheckerBoardPositionX;
        public float RightCheckerBoardPositionY;
        public float BottomCheckerBoardPositionX;
        public float BottomCheckerBoardPositionY;

        public int SquareXNumber;
        public int SquareYNumber;
        public float SquareSizeMm;

        public float WaferRadiusMm;
    }
}
