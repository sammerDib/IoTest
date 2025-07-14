using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class GlobalTopoSystemCalibrationInput : IFlowInput
    {
        public List<int> Periods;

        public Side Side { get; set; }

        public float PixelSize { get; set; }

        public GlobalTopoCalibrationWaferDefinition WaferDefinition { get; set; }

        public GlobalTopoCameraCalibrationResult GlobalTopoCameraCalibResult { get; set; }

        [XmlIgnore]
        public ImageData CheckerBoardImg { get; set; }

        [XmlIgnore]
        public ImageData UnwrappedPhaseMapX { get; set; }

        [XmlIgnore]
        public ImageData UnwrappedPhaseMapY { get; set; }

        public GlobalTopoSystemCalibrationInput()
        { }

        public GlobalTopoSystemCalibrationInput(List<int> periods, Side side, float pixelSize,
            GlobalTopoCalibrationWaferDefinition waferDefinition, GlobalTopoCameraCalibrationResult globalTopoCameraCalibResult,
            ImageData checkerBoardImg, ImageData unwrappedPhaseMapX, ImageData unwrappedPhaseMapY)
        {
            Periods = periods;
            Side = side;
            PixelSize = pixelSize;
            WaferDefinition = waferDefinition;
            CheckerBoardImg = checkerBoardImg;
            UnwrappedPhaseMapX = unwrappedPhaseMapX;
            UnwrappedPhaseMapY = unwrappedPhaseMapY;
            GlobalTopoCameraCalibResult = globalTopoCameraCalibResult;
        }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (Periods == null || Periods.Count <= 0)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid period list given.");
            }
            if (Side == Side.Unknown)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid side given ${Enum.GetName(typeof(Side), Side)}, should be Front or Back");
            }

            if (PixelSize <= 0.0)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid PixeSize given {PixelSize}, should be greater than 0");
            }

            if (WaferDefinition is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid wafer definition given, should not be null");
            }

            if (GlobalTopoCameraCalibResult is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid global topo camera calibration given, should not be null");
            }

            if (CheckerBoardImg is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid CheckerBoardImg image given, should not be null");
            }

            if (UnwrappedPhaseMapX is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid PhaseMapX image given, should not be null");
            }

            if (UnwrappedPhaseMapY is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid PhaseMapY image given, should not be null");
            }

            return result;
        }
    }
}
