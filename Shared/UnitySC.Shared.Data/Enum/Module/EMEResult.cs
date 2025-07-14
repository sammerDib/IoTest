namespace UnitySC.Shared.Data.Enum.Module
{
    // [Specific Module result (8 bits)] ---- cf ResultType
    // ==> [EME Filter (3 bits)][EME LightSource (2 bits)[EME result (3 bits)]

    public enum EMEResult
    {
        #region Production Acquisition EMERA Results (bind to ADC Dataloader or Database viwer)
        // Visible Light Direction 0°
        Visible0 = ActorType.EMERA | 1 << EMEResultHelper.EMEResultShift | EMELightSource.Visible | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        Visible0_LowRes = ActorType.EMERA | 1 << EMEResultHelper.EMEResultShift | EMELightSource.Visible | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,

        // Visible Light Direction 90°
        Visible90 = ActorType.EMERA | 2 << EMEResultHelper.EMEResultShift | EMELightSource.Visible | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        Visible90_LowRes = ActorType.EMERA | 2 << EMEResultHelper.EMEResultShift | EMELightSource.Visible | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,

        // UV Light Type 1
        UV_NoFilter = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV_NoFilter_LowRes = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV_LinearPolarizingFilter = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LinearPolarizingFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV_LinearPolarizingFilter_LowRes = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LinearPolarizingFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV_BandPass450nm50 = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass450nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV_BandPass450nm50_LowRes = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass450nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV_BandPass470nm50 = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass470nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV_BandPass470nm50_LowRes = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass470nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV_BandPass550nm50 = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass550nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV_BandPass550nm50_LowRes = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass550nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV_LowPass650nm = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass650nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV_LowPass650nm_LowRes = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass650nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV_LowPass750nm = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass750nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV_LowPass750nm_LowRes = ActorType.EMERA | 3 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass750nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,

        // UV Light Type 2
        UV2_NoFilter = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV2_NoFilter_LowRes = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV2_LinearPolarizingFilter = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LinearPolarizingFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV2_LinearPolarizingFilter_LowRes = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LinearPolarizingFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV2_BandPass450nm50 = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass450nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV2_BandPass450nm50_LowRes = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass450nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV2_BandPass470nm50 = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass470nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV2_BandPass470nm50_LowRes = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass470nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV2_BandPass550nm50 = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass550nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV2_BandPass550nm50_LowRes = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass550nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV2_LowPass650nm = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass650nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV2_LowPass650nm_LowRes = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass650nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV2_LowPass750nm = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass750nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV2_LowPass750nm_LowRes = ActorType.EMERA | 4 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass750nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,

        // UV Light Type 3
        UV3_NoFilter = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV3_NoFilter_LowRes = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.NoFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV3_LinearPolarizingFilter = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LinearPolarizingFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV3_LinearPolarizingFilter_LowRes = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LinearPolarizingFilter | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV3_BandPass450nm50 = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass450nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV3_BandPass450nm50_LowRes = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass450nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV3_BandPass470nm50 = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass470nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV3_BandPass470nm50_LowRes = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass470nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV3_BandPass550nm50 = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass550nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV3_BandPass550nm50_LowRes = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.BandPass550nm50 | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV3_LowPass650nm = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass650nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV3_LowPass650nm_LowRes = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass650nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,
        UV3_LowPass750nm = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass750nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.MosaicImage,
        UV3_LowPass750nm_LowRes = ActorType.EMERA | 5 << EMEResultHelper.EMEResultShift | EMELightSource.UV | EMEFilter.LowPass750nm | Side.Unknown | ResultCategory.Acquisition | ResultFormat.FullImage,

        #endregion
    }

    public enum EMELightSource // [EME LightSource (2 bits)] MAX allowed == 4 ( 4 sources possible)
    {
        Unknown = 0 << EMEResultHelper.EMESourceShift,
        Visible = 1 << EMEResultHelper.EMESourceShift,
        UV = 2 << EMEResultHelper.EMESourceShift,
    }

    public enum EMEFilter // [EME Filter (3 bits)] MAX allowed == 7 ( 8 differents filtre possible)
    {
        /// <summary>
        /// undefined
        /// </summary>
        Unknown = 0 << EMEResultHelper.EMEFilterShift,
        /// <summary>
        /// Empty filter
        /// </summary>
        NoFilter = 1 << EMEResultHelper.EMEFilterShift,
        /// <summary>
        /// Linear Polarizing Filter
        /// </summary>
        LinearPolarizingFilter = 2 << EMEResultHelper.EMEFilterShift,
        /// <summary>
        /// Band-pass filter centered at 450nm with a bandwidth of 50nm
        /// </summary>
        BandPass450nm50 = 3 << EMEResultHelper.EMEFilterShift,
        /// <summary>
        /// Band-pass filter centered at 450nm with a bandwidth of 50nm
        /// </summary>
        BandPass470nm50 = 4 << EMEResultHelper.EMEFilterShift,
        /// <summary>
        /// Band-pass filter centered at 550nm with a bandwidth of 50nm
        /// </summary>        
        BandPass550nm50 = 5 << EMEResultHelper.EMEFilterShift,
        /// <summary>
        /// Low-pass filter with a cutoff at 650nm
        /// </summary>        
        LowPass650nm = 6 << EMEResultHelper.EMEFilterShift,
        /// <summary>
        /// Low-pass filter with a cutoff at 750nm
        /// </summary>
        LowPass750nm = 7 << EMEResultHelper.EMEFilterShift
    }

    public static class EMEResultHelper
    {
        // [Specific Module result (8 bits)] ---- cf ResultType
        // ==> [EME Filter (3 bits)][EME LightSource (2 bits)[EME result (3 bits)]
        // 8 différents results possible associated with 3 differences light sort and up to 7 different filter
        // /!\  a prévoir plus tard il sera possible d'introduire un type polarisation 
        public const int EMEResultSize = 3;
        public const int EMEResultShift = PMEnumHelper.ResultSpecificShift;
        public const int EMESourceSize = 2;
        public const int EMESourceShift = EMEResultShift + EMEResultSize;
        public const int EMEFilterSize = 3;
        public const int EMEFilterShift = EMESourceShift + EMESourceSize;

        public static EMELightSource GetEMELightSource(this ResultType resultType)
        {
            return (EMELightSource)PMEnumHelper.ApplyMask(resultType, EMESourceShift, EMESourceSize);
        }
        public static EMEFilter GetEMEFilter(this ResultType resultType)
        {
            return (EMEFilter)PMEnumHelper.ApplyMask(resultType, EMEFilterShift, EMEFilterSize);
        }

        public static int GetEMEAcquisitionTypeId(this ResultType resultType)
        {
            return PMEnumHelper.ApplyMask(resultType, EMEResultShift, EMEResultSize) >> EMEResultShift;
        }
    }
}
