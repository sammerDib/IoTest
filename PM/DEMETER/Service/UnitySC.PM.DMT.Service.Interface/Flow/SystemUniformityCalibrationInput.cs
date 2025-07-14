using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class SystemUniformityCalibrationInput : IFlowInput
    {
        [XmlIgnore]
        public ImageData BrightFieldImage { get; set; }

        public float WaferRadiusInMm { get; set; }

        public float PixelSizeInMm { get; set; }

        public GlobalTopoCameraCalibrationResult GlobalTopoCameraCalibResult { get; set; }
        public GlobalTopoSystemCalibrationResult GlobalTopoSystemCalibResult { get; set; }

        public FresnelCoefficients FresnelCoefficients { get; set; }

        public List<Length> ScreenWavelengthPeaks { get; set; }

        public UnitySC.Shared.Data.Enum.Polarisation Polarisation { get; set; }

        public SystemUniformityCalibrationInput()
        { }

        public SystemUniformityCalibrationInput(ImageData brightFieldImage, float waferRadiusInMm, float pixelSizeInMm, GlobalTopoCameraCalibrationResult globalTopoCameraCalibResult, GlobalTopoSystemCalibrationResult globalTopoSystemCalibResult, FresnelCoefficients fresnelCoefficients, List<Length> screenWavelengthPeaks, UnitySC.Shared.Data.Enum.Polarisation polarisation)
        {
            BrightFieldImage = brightFieldImage;
            WaferRadiusInMm = waferRadiusInMm;
            PixelSizeInMm = pixelSizeInMm;
            GlobalTopoCameraCalibResult = globalTopoCameraCalibResult;
            GlobalTopoSystemCalibResult = globalTopoSystemCalibResult;
            FresnelCoefficients = fresnelCoefficients;
            ScreenWavelengthPeaks = screenWavelengthPeaks;
            Polarisation = polarisation;
        }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (GlobalTopoSystemCalibResult is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid global topo system calibration given, should not be null");
            }

            if (BrightFieldImage is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid BrightFieldImage image given, should not be null");
            }

            if (FresnelCoefficients is null)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid FresnelCoefficients given, should not be null");
            }

            if (ScreenWavelengthPeaks.IsEmpty())
            {
                result.IsValid = false;
                result.Message.Add($"Invalid ScreenWavelengthPeaks given, the list should not be empty");
            }

            if (PixelSizeInMm <= 0.0f)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid PixelSizeInMm given, 0 or negative");
            }

            if (WaferRadiusInMm <= 0.0f)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid WaferRadiusInMm given, 0 or negative");
            }

            return result;
        }
    }
}
