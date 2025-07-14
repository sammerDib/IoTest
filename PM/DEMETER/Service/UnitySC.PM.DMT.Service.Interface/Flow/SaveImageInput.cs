using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Data.Ada;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Collection;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class SaveImageInput : IFlowInput
    {
        public SaveImageInput(RecipeInfo recipeInfo, RemoteProductionInfo remoteProductionInfo, AdaWriter adaWriter,
            object adaWriterLock, DMTResult dmtResType, string imageName, string saveFullPath, bool keep32BitsDepth = false,
            bool applyUniformityCorrection = false, int uniformityCorrectionTargetSaturationLevel = -1, 
            float uniformityCorrectionAcceptableRatioOfSaturatedPixels = float.NaN)
        {
            RemoteProductionInfo = remoteProductionInfo;
            RecipeInfo = recipeInfo;

            DMTResultType = dmtResType;
            ImageSide = dmtResType.GetSide();
            ImageName = imageName;
            SaveFullPath = saveFullPath;
            AdaWriter = adaWriter;
            AdaWriterLock = adaWriterLock;
            Keep32BitsDepth = keep32BitsDepth;
            ApplyUniformityCorrection = applyUniformityCorrection;
            UniformityCorrectionTargetSaturationLevel = uniformityCorrectionTargetSaturationLevel;
            UniformityCorrectionAcceptableRatioOfSaturatedPixels = uniformityCorrectionAcceptableRatioOfSaturatedPixels;
        }

        public SaveImageInput()
        {
        }

        [XmlIgnore]
        public USPImageMil ImageMilToSave { get; set; }

        [XmlIgnore]
        public ImageData ImageDataToSave { get; set; }

        [XmlIgnore]
        public ImageData ImageMaskToSave { get; set; }

        public RemoteProductionInfo RemoteProductionInfo { get; set; }

        [XmlIgnore]
        public RecipeInfo RecipeInfo { get; set; }

        public bool Keep32BitsDepth { get; set; }

        // Database ID from Acquisition ResultItem Table given by Preregistering
        public long InternalDbResItemId { get; set; } = -1;

        public bool ApplyUniformityCorrection { get; set; }

        public int UniformityCorrectionTargetSaturationLevel { get; set; } = -1;
        
        public float UniformityCorrectionAcceptableRatioOfSaturatedPixels { get; set; } = float.NaN;

        public Side ImageSide { get; set; }

        public DMTResult DMTResultType { get; set; }

        public string SaveFullPath { get; set; }

        public string ImageName { get; set; }

        [XmlIgnore]
        public AdaWriter AdaWriter { get; set; }

        [XmlIgnore]
        public object AdaWriterLock { get; set; }

        public double ExposureTimeMs { get; set; }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);
            var propertyList = new List<object>
                               {
                                   RemoteProductionInfo,
                                   AdaWriter,
                                   AdaWriterLock
                               };
            // RemoteProductionInfo can be null
            foreach (object property in propertyList.Where(prop => prop is null && prop != RemoteProductionInfo))
            {
                result.IsValid = false;
                result.Message.Add($"Cannot save an image without {nameof(property)}");
            }

            if (ImageMilToSave is null && ImageDataToSave is null)
            {
                result.IsValid = false;
                result.Message.Add("No image to save");
            }

            if (!(ImageDataToSave is null) && !(ImageMilToSave is null))
            {
                result.IsValid = false;
                result.Message.Add($"Cannot save both an {typeof(ImageData)} and an {typeof(USPImageMil)}");
            }

            if (ImageDataToSave is null && !(ImageMaskToSave is null))
            {
                result.IsValid = false;
                result.Message.Add("Cannot save a mask without an image");
            }

            if (SaveFullPath.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add("Save path cannot be null or empty");
            }

            if (ImageName.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add("Image name cannot be null or empty");
            }

            if (ApplyUniformityCorrection)
            {
                if (UniformityCorrectionTargetSaturationLevel == -1)
                {
                    result.IsValid = false;
                    result.Message.Add("Target saturation level must be set when ApplyUniformityCorrection is true");
                }

                if (UniformityCorrectionAcceptableRatioOfSaturatedPixels == float.NaN)
                {
                    result.IsValid = false;
                    result.Message.Add("Acceptable percentage of saturated pixels must set when ApplyUniformityCorrection is true");
                }
            }

            return result;
        }
    }
}
