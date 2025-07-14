using System;
using System.Windows.Media;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class AutoExposureInput : IFlowInput
    {

        public AutoExposureInput() { }

        public AutoExposureInput(Side side, MeasureType measureType, ROI roiForMask,
            AcquisitionScreenDisplayImage displayImageType,
            Color color, Measure.Fringe fringe, USPImageMil screenImage, int targetSaturation,
            double? initialAutoExposureTimeMs = null)
        {
            Side = side;
            InitialAutoExposureTimeMs = initialAutoExposureTimeMs;
            MeasureType = measureType;
            DisplayImageType = displayImageType;
            RoiForMask = roiForMask;
            Color = color;
            Fringe = fringe;
            ScreenImage = screenImage;
            TargetSaturation = targetSaturation;
        }

        public AutoExposureInput(MeasureBase measure, USPImageMil screenImage)
        {
            Side = measure.Side;
            InitialAutoExposureTimeMs = measure.ExposureTimeMs;
            MeasureType = measure.MeasureType;
            RoiForMask = measure.ROI;
            TargetSaturation = measure.AutoExposureTargetSaturation;
            switch (measure)
            {
                case BrightFieldMeasure bfMeasure:
                    Color = bfMeasure.Color;
                    DisplayImageType = AcquisitionScreenDisplayImage.Color;
                    break;
                case DeflectometryMeasure dfMeasure:
                    Fringe = dfMeasure.Fringe;
                    ScreenImage = screenImage;
                    DisplayImageType = AcquisitionScreenDisplayImage.FringeImage;
                    break;
                case HighAngleDarkFieldMeasure highAngleDarkfieldMeasure:
                    DisplayImageType = AcquisitionScreenDisplayImage.HighAngleDarkFieldMask;
                    ScreenImage = screenImage;
                    break;
                case BackLightMeasure blMeasure:
                    DisplayImageType = AcquisitionScreenDisplayImage.Color;
                    Color = Colors.White;
                    break;
            }
        }

        public Side Side { get; set; }
        
        public bool IgnorePerspectiveCalibration { get; set; }

        public double? InitialAutoExposureTimeMs { get; set; }

        public MeasureType MeasureType { get; set; }

        public ROI RoiForMask { get; set; }

        public AcquisitionScreenDisplayImage DisplayImageType { get; set; }

        public Color Color { get; set; }
        
        public Measure.Fringe Fringe { get; set; }

        [XmlIgnore] public USPImageMil ScreenImage { get; set; }

        public int TargetSaturation { get; set; }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);
            if (Side == Side.Unknown)
            {
                result.IsValid = false;
                result.Message.Add($"Invalid side given ${Enum.GetName(typeof(Side), Side)}, should be Front or Back");
            }

            if (DisplayImageType == AcquisitionScreenDisplayImage.FringeImage && (Fringe is null || ScreenImage is null))
            {
                result.IsValid = false;
                result.Message.Add("Fringe image hasn't been provided, cannot perform AutoExposure");
            }

            if (DisplayImageType == AcquisitionScreenDisplayImage.HighAngleDarkFieldMask && ScreenImage is null)
            {
                result.IsValid = false;
                result.Message.Add("High angle dark-field mask hasn't been provided, cannot perform AutoExposure");
            }

            if (IgnorePerspectiveCalibration && RoiForMask.RoiType == RoiType.WholeWafer)
            {
                result.IsValid = false;
                result.Message.Add("Calibrations can only perform auto exposures on rectangular ROIs as perspective calibration is not available");
            }

            return result;
        }
    }
}
