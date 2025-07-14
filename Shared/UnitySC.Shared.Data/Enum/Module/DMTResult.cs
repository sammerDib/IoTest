using System;

namespace UnitySC.Shared.Data.Enum.Module
{
    internal static class DMTResultIndexFlag
    {
        public const int START_DISABLED = 5;                    // last "Production Results"  index
        public const int START_RNDOUTPUTS = START_DISABLED + 2; // last Production Results Disabled index
        public const int START_RESERVED = START_RNDOUTPUTS + 6; // last RnD Debug Outputs index
    }

    public enum DMTResult
    {
        #region Production Acquisition Results (bind to ADC Dataloader)

        // DEFLECTO
        // Curvature X - 8bits full image
        CurvatureX_Front = ActorType.DEMETER | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        CurvatureX_Back = ActorType.DEMETER | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        // Curvature Y - 8bits full image
        CurvatureY_Front = ActorType.DEMETER | 2 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        CurvatureY_Back = ActorType.DEMETER | 2 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        // Dark Picture - 8bits full image
        LowAngleDarkField_Front = ActorType.DEMETER | 3 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        LowAngleDarkField_Back = ActorType.DEMETER | 3 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,

        // Brightfield (old reflecto) - 8bits full image
        BrightField_Front = ActorType.DEMETER | 4 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        BrightField_Back = ActorType.DEMETER | 4 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,

        // ObliqueLight (old custom image) - 8bits full image
        HighAngleDarkField_Front = ActorType.DEMETER | 5 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        HighAngleDarkField_Back = ActorType.DEMETER | 5 << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,

        // Add Validated DEMETER Result Custommer Result -- HERE

        #endregion

        #region Production Results Disabled (bind to ADC Dataloader but disabled (user cannot use it))
        // DEFLECTO
        // Amplitude X - 8bits full image
        AmplitudeX_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_DISABLED + 1) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        AmplitudeX_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_DISABLED + 1) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        // Amplitude Y  - 8bitsfull image
        AmplitudeY_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_DISABLED + 2) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        AmplitudeY_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_DISABLED + 2) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        #endregion

        // the result below should not be used with dataloader or within database

        #region RnD Debug Outputs - only file ouputs (No database, No ADC Dataloader)

        UnwrappedPhaseX_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 1) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        UnwrappedPhaseX_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 1) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        UnwrappedPhaseY_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 2) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        UnwrappedPhaseY_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 2) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,

        TopoPhaseNX_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 3) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        TopoPhaseNX_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 3) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        TopoPhaseNY_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 4) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        TopoPhaseNY_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 4) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        TopoPhaseNZ_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 5) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        TopoPhaseNZ_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 5) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,

        PhaseMask_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 6) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        PhaseMask_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RNDOUTPUTS + 6) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,

        #endregion

        #region Waiting for specification 
        TopoPhaseX_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RESERVED + 1) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        TopoPhaseX_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RESERVED + 1) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        TopoPhaseY_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RESERVED + 2) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        TopoPhaseY_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RESERVED + 2) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        TopoPhaseZ_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RESERVED + 3) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        TopoPhaseZ_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RESERVED + 3) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,

        TopoBackLight_Front = ActorType.DEMETER | (DMTResultIndexFlag.START_RESERVED + 4) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Front | ResultFormat.FullImage,
        TopoBackLight_Back = ActorType.DEMETER | (DMTResultIndexFlag.START_RESERVED + 4) << PMEnumHelper.ResultSpecificShift | ResultCategory.Acquisition | Side.Back | ResultFormat.FullImage,
        #endregion

    }

    public static class DMTResultExtensions
    {
        public static Side GetSide(this DMTResult dmtRes)
        {
            return (Side)ApplyMask(dmtRes, PMEnumHelper.SideShift, PMEnumHelper.SideSize);
        }

        public static int GetResultExtensionId(this DMTResult dmtRes)
        {
            int res = ApplyMask(dmtRes, PMEnumHelper.ResultFormatShift, PMEnumHelper.ResultFormatSize + PMEnumHelper.ResultSpecificSize) >> PMEnumHelper.ResultFormatShift;
            return res;
        }

        internal static int ApplyMask(DMTResult dmtRes, int shift, int size)
        {
            int maskWithoutShift = (1 << size) - 1;
            int mask = maskWithoutShift << shift;
            int res = (int)dmtRes & mask;
            return res;
        }

        public static ResultType ToResultType(this DMTResult dmtRes)
        {
            if (System.Enum.IsDefined(typeof(ResultType), (int)dmtRes))
                    return (ResultType)dmtRes;
            throw new ArgumentException($"DMTResult <{dmtRes}> is not official acquisition and should not be use as an PM Output, Only for  debug purpose");
        }
    }
}
