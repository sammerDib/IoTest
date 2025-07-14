namespace UnitySC.Shared.Data.Enum.Module
{
    public enum ANALYSEResult
    {
        Thickness = 1 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        TSV = 2 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        Trench = 3 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        Pillar = 4 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        Step = 5 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        PeriodicStructure = 6 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        NanoTopo = 7 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        Overlay = 8 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        CD = 9 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        EBR = 10 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        Topography = 11 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        Bow = 12 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        Warp = 13 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        EdgeTrim = 14 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        Roughness = 15 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology,
        XYCalibration = 16 << PMEnumHelper.ResultSpecificShift | ActorType.ANALYSE | ResultCategory.Result | Side.Unknown | ResultFormat.Metrology
    }
}
