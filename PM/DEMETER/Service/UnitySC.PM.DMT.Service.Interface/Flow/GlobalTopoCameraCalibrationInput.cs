using System;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class GlobalTopoCameraCalibrationInput : IFlowInput
    {
        public Side Side { get; set; }

        public float PixelSize { get; set; }

        [XmlIgnore]
        public USPImageMil[] CameraCalibrationImages { get; set; }

        public GlobalTopoCalibrationWaferDefinition WaferDefinition { get; set; }

        public GlobalTopoCameraCalibrationInput()
        { }

        public GlobalTopoCameraCalibrationInput(Side side, float pixelSize,
            GlobalTopoCalibrationWaferDefinition waferDefinition, USPImageMil[] cameraCalibrationImages)
        {
            Side = side;
            PixelSize = pixelSize;
            WaferDefinition = waferDefinition;
            CameraCalibrationImages = cameraCalibrationImages;
        }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

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

            if (CameraCalibrationImages == null || CameraCalibrationImages.Length < 6)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid camera calibration images given, at least 6 images needed");
            }

            if (WaferDefinition is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid wafer definition given, should not be null");
            }

            return result;
        }
    }
}
