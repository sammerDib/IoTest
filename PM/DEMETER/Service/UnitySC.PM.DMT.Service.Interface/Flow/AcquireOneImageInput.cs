using System;
using System.Windows.Media;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{

    [Serializable]
    public class AcquireOneImageInput : IFlowInput
    {
        public AcquireOneImageInput(Side cameraSide, Side screenSide, double exposureTimeMs, MeasureType measureType,
            AcquisitionScreenDisplayImage displayImageType, Color? color = null, USPImageMil screenImage = null)
        {
            CameraSide = cameraSide;
            ScreenSide = screenSide;
            ExposureTimeMs = exposureTimeMs;
            DisplayImageType = displayImageType;
            ScreenImage = screenImage;
            MeasureType = measureType;
            if (color.HasValue)
            {
                ScreenColor = color.Value;
            }
        }

        public AcquireOneImageInput(MeasureBase measure, USPImageMil screenImage = null)
        {
            CameraSide = measure.Side;
            ScreenSide = measure.Side;
            ExposureTimeMs = measure.ExposureTimeMs;
            switch (measure)
            {
                case BrightFieldMeasure bfMeasure:
                    DisplayImageType = AcquisitionScreenDisplayImage.Color;
                    ScreenColor = bfMeasure.Color;
                    MeasureType = MeasureType.BrightFieldMeasure;
                    break;
                case HighAngleDarkFieldMeasure hadMeasure:
                    DisplayImageType = AcquisitionScreenDisplayImage.HighAngleDarkFieldMask;
                    ScreenImage = screenImage;
                    MeasureType = MeasureType.HighAngleDarkFieldMeasure;
                    break;
                case BackLightMeasure blMeasure:
                    DisplayImageType = AcquisitionScreenDisplayImage.Color;
                    ScreenColor = Colors.White;
                    ScreenSide = blMeasure.Side == Side.Front ? Side.Back : Side.Front;
                    MeasureType = MeasureType.BacklightMeasure;
                    break;
            }
        }

        public AcquireOneImageInput()
        {
        }

        public MeasureType MeasureType;

        public AcquisitionScreenDisplayImage DisplayImageType { get; set; }

        [XmlIgnore]
        public USPImageMil ScreenImage { get; set; }

        public Color ScreenColor { get; set; }

        public double ExposureTimeMs { get; set; }

        public Side ScreenSide { get; set; }

        public Side CameraSide { get; set; }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);
            if (ScreenSide == Side.Unknown || CameraSide == Side.Unknown)
            {
                result.IsValid = false;
                result.Message.Add("ScreenSide or CameraSide cannot be unknown");
            }

            switch (DisplayImageType)
            {
                case AcquisitionScreenDisplayImage.HighAngleDarkFieldMask:
                    if (ScreenImage is null)
                    {
                        result.IsValid = false;
                        result.Message.Add("ScreenImage cannot be null when DisplayImageType is HighAngleDarkfieldMask");
                    }

                    break;
                case AcquisitionScreenDisplayImage.Color:
                    break;
                default:
                    result.IsValid = false;
                    result.Message.Add(
                        $"DisplayImageType ${Enum.GetName(typeof(AcquisitionScreenDisplayImage), DisplayImageType)} cannot be used for AcquireOneImageInput");
                    break;
            }

            return result;
        }
    }
}
