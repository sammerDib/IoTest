namespace UnitySC.Shared.Data.Enum.PostProcessing
{
    public enum ADCResult
    {
        Klarf = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.Klarf,
        ASE = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.ASE,
        ASO = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.ASO,
        DFHaze = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.DFHaze,
        Crown = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.Crown,
        YieldMap = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.YieldMap,
        EyeEdge = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.EyeEdge,
        GlobalTopo = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.GlobalTopo,
        HeightMes = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.HeightMes,
        Haze = ActorType.ADC | 1 << PMEnumHelper.ResultSpecificShift | ResultCategory.Result | Side.Unknown | ResultFormat.Haze,
    }
}