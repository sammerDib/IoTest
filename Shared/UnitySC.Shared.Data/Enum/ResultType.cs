using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Data.Enum.PostProcessing;

namespace UnitySC.Shared.Data.Enum
{
    // [Specific result (8 bits)][Result format (8 bits)][Result Category (4 bits)][Side(2 bits)][Actor Type (10 bits)]

    public enum ResultType
    {
        Empty = 0,
        // Not defined  need to have by default a result category, an ASE format, and 1 in specif to macth result extension format 
        NotDefined = ActorType.Unknown | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.ASE,

        #region DEMETER
        DMT_Brightfield_Front = DMTResult.BrightField_Front,
        DMT_Brightfield_Back = DMTResult.BrightField_Back,
        DMT_CurvatureX_Front = DMTResult.CurvatureX_Front,
        DMT_CurvatureX_Back = DMTResult.CurvatureX_Back,
        DMT_CurvatureY_Front = DMTResult.CurvatureY_Front,
        DMT_CurvatureY_Back = DMTResult.CurvatureY_Back,
        DMT_Dark_Front = DMTResult.LowAngleDarkField_Front,
        DMT_Dark_Back = DMTResult.LowAngleDarkField_Back,
        DMT_ObliqueLight_Front = DMTResult.HighAngleDarkField_Front,
        DMT_ObliqueLight_Back = DMTResult.HighAngleDarkField_Back,

        //DISABLED
        DMT_AmplitudeX_Back = DMTResult.AmplitudeX_Back,
        DMT_AmplitudeX_Front = DMTResult.AmplitudeX_Front,
        DMT_AmplitudeY_Back = DMTResult.AmplitudeY_Back,
        DMT_AmplitudeY_Front = DMTResult.AmplitudeY_Front,

        //// FOR RnD DEBUG Or Reserded
        //////
        ///// Do NOT Uncomment below until it has not been declared as offcial Production acquisition. offcial list should be stated above
        //////
        //DMT_UnwrappedPhaseX_Back = DMTResult.UnwrappedPhaseX_Back,
        //DMT_UnwrappedPhaseX_Front = DMTResult.UnwrappedPhaseX_Front,
        //DMT_UnwrappedPhaseY_Back = DMTResult.UnwrappedPhaseY_Back,
        //DMT_UnwrappedPhaseY_Front = DMTResult.UnwrappedPhaseY_Front,
        //DMT_TopoPhaseX_Front = DMTResult.TopoPhaseX_Front,
        //DMT_TopoPhaseX_Back = DMTResult.TopoPhaseX_Back,
        //DMT_TopoPhaseY_Front = DMTResult.TopoPhaseY_Front,
        //DMT_TopoPhaseY_Back = DMTResult.TopoPhaseY_Back,
        //DMT_TopoPhaseZ_Front = DMTResult.TopoPhaseZ_Front,
        //DMT_TopoPhaseZ_Back = DMTResult.TopoPhaseZ_Back,
        //DMT_TopoPhaseNX_Front = DMTResult.TopoPhaseNX_Front,
        //DMT_TopoPhaseNX_Back = DMTResult.TopoPhaseNX_Back,
        //DMT_TopoPhaseNY_Front = DMTResult.TopoPhaseNY_Front,
        //DMT_TopoPhaseNY_Back = DMTResult.TopoPhaseNY_Back,
        //DMT_TopoPhaseNZ_Front = DMTResult.TopoPhaseNZ_Front,
        //DMT_TopoPhaseNZ_Back = DMTResult.TopoPhaseNZ_Back,
        //DMT_PhaseMask_Front = DMTResult.PhaseMask_Front,
        //DMT_PhaseMask_Back = DMTResult.PhaseMask_Back,
        //DMT_TopoBackLight_Front = DMTResult.TopoBackLight_Front,
        //DMT_TopoBackLight_Back = DMTResult.TopoBackLight_Back,

        #endregion DEMETER

        #region HLS [Helios - lightspeed]

        HLS_Amplitude_WideFW = HLSResult.Amplitude_WideFW,
        HLS_Amplitude_NarrowBW = HLSResult.Amplitude_NarrowBW,
        HLS_Saturation_WideFW = HLSResult.Saturation_WideFW,
        HLS_Saturation_NarrowBW = HLSResult.Saturation_NarrowBW,
        HLS_Haze_WideFW = HLSResult.Haze_WideFW,
        HLS_Haze_NarrowBW = HLSResult.Haze_NarrowBW,
        HLS_Haze_Total = HLSResult.Haze_Total,
        HLS_Visibilility_WideFW = HLSResult.Visibilility_WideFW,
        HLS_Visibilility_NarrowBW = HLSResult.Visibilility_NarrowBW,
        HLS_Likelyhood_WideFW = HLSResult.Likelyhood_WideFW,
        HLS_Likelyhood_NarrowBW = HLSResult.Likelyhood_NarrowBW,

        #endregion HLS

        #region ADC

        ADC_Klarf = ADCResult.Klarf,
        ADC_ASO = ADCResult.ASO,
        ADC_ASE = ADCResult.ASE,            //Obsolete for unified plateform ?
        ADC_DFHaze = ADCResult.DFHaze,      //Obsolete for unified plateform ? Darkfield haze
        ADC_Crown = ADCResult.Crown,
        ADC_YieldMap = ADCResult.YieldMap,
        ADC_EyeEdge = ADCResult.EyeEdge,
        ADC_GlobalTopo = ADCResult.GlobalTopo,
        ADC_HeightMes = ADCResult.HeightMes,
        ADC_Haze = ADCResult.Haze,          // HeLioS & LS haze

        #endregion ADC

        #region ANALYSE

        ANALYSE_Thickness = ANALYSEResult.Thickness,
        ANALYSE_TSV = ANALYSEResult.TSV,
        ANALYSE_Trench = ANALYSEResult.Trench,
        ANALYSE_Pillar = ANALYSEResult.Pillar,
        ANALYSE_Step = ANALYSEResult.Step,
        ANALYSE_PeriodicStructure = ANALYSEResult.PeriodicStructure,
        ANALYSE_NanoTopo = ANALYSEResult.NanoTopo,
        ANALYSE_Overlay = ANALYSEResult.Overlay,
        ANALYSE_CD = ANALYSEResult.CD,
        ANALYSE_EBR = ANALYSEResult.EBR,
        ANALYSE_Topography = ANALYSEResult.Topography,
        ANALYSE_Bow = ANALYSEResult.Bow,
        ANALYSE_Warp = ANALYSEResult.Warp,
        ANALYSE_EdgeTrim = ANALYSEResult.EdgeTrim,
        ANALYSE_Roughness = ANALYSEResult.Roughness,
        ANALYSE_XYCalibration = ANALYSEResult.XYCalibration,

        #endregion ANALYSE

        #region EMERA
        EME_Visible0 = EMEResult.Visible0,
        EME_Visible0_LowRes = EMEResult.Visible0_LowRes,
        EME_Visible90 = EMEResult.Visible90,
        EME_Visible90_LowRes = EMEResult.Visible90_LowRes,
        EME_UV_NoFilter = EMEResult.UV_NoFilter,
        EME_UV_NoFilter_LowRes = EMEResult.UV_NoFilter_LowRes,
        EME_UV_LinearPolarizingFilter = EMEResult.UV_LinearPolarizingFilter,
        EME_UV_LinearPolarizingFilter_LowRes = EMEResult.UV_LinearPolarizingFilter_LowRes,
        EME_UV_BandPass450nm50 = EMEResult.UV_BandPass450nm50,
        EME_UV_BandPass450nm50_LowRes = EMEResult.UV_BandPass450nm50_LowRes,
        EME_UV_BandPass470nm50 = EMEResult.UV_BandPass470nm50,
        EME_UV_BandPass470nm50_LowRes = EMEResult.UV_BandPass470nm50_LowRes,
        EME_UV_BandPass550nm50 = EMEResult.UV_BandPass550nm50,
        EME_UV_BandPass550nm50_LowRes = EMEResult.UV_BandPass550nm50_LowRes,
        EME_UV_LowPass650nm = EMEResult.UV_LowPass650nm,
        EME_UV_LowPass650nm_LowRes = EMEResult.UV_LowPass650nm_LowRes,
        EME_UV_LowPass750nm = EMEResult.UV_LowPass750nm,
        EME_UV_LowPass750nm_LowRes = EMEResult.UV_LowPass750nm_LowRes,

        EME_UV2_NoFilter = EMEResult.UV2_NoFilter,
        EME_UV2_NoFilter_LowRes = EMEResult.UV2_NoFilter_LowRes,
        EME_UV2_LinearPolarizingFilter = EMEResult.UV2_LinearPolarizingFilter,
        EME_UV2_LinearPolarizingFilter_LowRes = EMEResult.UV2_LinearPolarizingFilter_LowRes,
        EME_UV2_BandPass450nm50 = EMEResult.UV2_BandPass450nm50,
        EME_UV2_BandPass450nm50_LowRes = EMEResult.UV2_BandPass450nm50_LowRes,
        EME_UV2_BandPass470nm50 = EMEResult.UV2_BandPass470nm50,
        EME_UV2_BandPass470nm50_LowRes = EMEResult.UV2_BandPass470nm50_LowRes,
        EME_UV2_BandPass550nm50 = EMEResult.UV2_BandPass550nm50,
        EME_UV2_BandPass550nm50_LowRes = EMEResult.UV2_BandPass550nm50_LowRes,
        EME_UV2_LowPass650nm = EMEResult.UV2_LowPass650nm,
        EME_UV2_LowPass650nm_LowRes = EMEResult.UV2_LowPass650nm_LowRes,
        EME_UV2_LowPass750nm = EMEResult.UV2_LowPass750nm,
        EME_UV2_LowPass750nm_LowRes = EMEResult.UV2_LowPass750nm_LowRes,

        EME_UV3_NoFilter = EMEResult.UV3_NoFilter,
        EME_UV3_NoFilter_LowRes = EMEResult.UV3_NoFilter_LowRes,
        EME_UV3_LinearPolarizingFilter = EMEResult.UV3_LinearPolarizingFilter,
        EME_UV3_LinearPolarizingFilter_LowRes = EMEResult.UV3_LinearPolarizingFilter_LowRes,
        EME_UV3_BandPass450nm50 = EMEResult.UV3_BandPass450nm50,
        EME_UV3_BandPass450nm50_LowRes = EMEResult.UV3_BandPass450nm50_LowRes,
        EME_UV3_BandPass470nm50 = EMEResult.UV3_BandPass470nm50,
        EME_UV3_BandPass470nm50_LowRes = EMEResult.UV3_BandPass470nm50_LowRes,
        EME_UV3_BandPass550nm50 = EMEResult.UV3_BandPass550nm50,
        EME_UV3_BandPass550nm50_LowRes = EMEResult.UV3_BandPass550nm50_LowRes,
        EME_UV3_LowPass650nm = EMEResult.UV3_LowPass650nm,
        EME_UV3_LowPass650nm_LowRes = EMEResult.UV3_LowPass650nm_LowRes,
        EME_UV3_LowPass750nm = EMEResult.UV3_LowPass750nm,
        EME_UV3_LowPass750nm_LowRes = EMEResult.UV3_LowPass750nm_LowRes,

        #endregion
    }
}
