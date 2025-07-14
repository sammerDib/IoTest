namespace UnitySC.Shared.Data.Enum.Module
{
    // LS<->HLS GLOSSARY REMINDER
    // -----------
    // wide (FW)
    // Narrow (BW)
    // both (total) = fw+bw
    // Acq Map 
    // ------
    // amplitude  (16bits - image tif)
    // visibility likelyhood saturation (8bits- image tif)
    // haze (float32 -- image 3da)



    public enum HLSResult
    {

        Amplitude_WideFW = ActorType.HeLioS | 1 << HLSResultHelper.HLSResultShift | HLSDirection.FW_Wide | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage,
        Amplitude_NarrowBW = ActorType.HeLioS | 1 << HLSResultHelper.HLSResultShift | HLSDirection.BW_Narrow | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage,

        Saturation_WideFW = ActorType.HeLioS | 2 << HLSResultHelper.HLSResultShift | HLSDirection.FW_Wide | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage,
        Saturation_NarrowBW = ActorType.HeLioS | 2 << HLSResultHelper.HLSResultShift | HLSDirection.BW_Narrow | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage,

        Haze_WideFW = ActorType.HeLioS | 3 << HLSResultHelper.HLSResultShift | HLSDirection.FW_Wide | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage_3D,
        Haze_NarrowBW = ActorType.HeLioS | 3 << HLSResultHelper.HLSResultShift | HLSDirection.BW_Narrow | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage_3D,
        Haze_Total = ActorType.HeLioS | 3 << HLSResultHelper.HLSResultShift | HLSDirection.Total | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage_3D,

        Visibilility_WideFW = ActorType.HeLioS | 4 << HLSResultHelper.HLSResultShift | HLSDirection.FW_Wide | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage,
        Visibilility_NarrowBW = ActorType.HeLioS | 4 << HLSResultHelper.HLSResultShift | HLSDirection.BW_Narrow | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage,

        Likelyhood_WideFW = ActorType.HeLioS | 5 << HLSResultHelper.HLSResultShift | HLSDirection.FW_Wide | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage,
        Likelyhood_NarrowBW = ActorType.HeLioS | 5 << HLSResultHelper.HLSResultShift | HLSDirection.BW_Narrow | ResultCategory.Acquisition | Side.Unknown | ResultFormat.FullImage,

    }
}
