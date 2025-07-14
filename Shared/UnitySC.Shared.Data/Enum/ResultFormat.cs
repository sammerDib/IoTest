namespace UnitySC.Shared.Data.Enum
{
    //[Result format(8 bits)]
    // 0 - 255 --- Max 255
    public enum ResultFormat
    {
        Unknow = 0 << PMEnumHelper.ResultFormatShift,
        Klarf = 1 << PMEnumHelper.ResultFormatShift,
        ASE = 2 << PMEnumHelper.ResultFormatShift,
        ASO = 3 << PMEnumHelper.ResultFormatShift,
        DFHaze = 4 << PMEnumHelper.ResultFormatShift,
        Crown = 5 << PMEnumHelper.ResultFormatShift,
        YieldMap = 6 << PMEnumHelper.ResultFormatShift,
        EyeEdge = 7 << PMEnumHelper.ResultFormatShift,
        GlobalTopo = 8 << PMEnumHelper.ResultFormatShift,
        HeightMes = 9 << PMEnumHelper.ResultFormatShift,
        Haze = 10 << PMEnumHelper.ResultFormatShift,
        // 11 <-> 15 (reserved for later inspection result)

        //Metrology measures
        Metrology = 15 << PMEnumHelper.ResultFormatShift,

        // acquisition results
        FullImage = 64 << PMEnumHelper.ResultFormatShift,
        FullImage_3D = 65 << PMEnumHelper.ResultFormatShift,
        MosaicImage = 128 << PMEnumHelper.ResultFormatShift,
        MosaicImage_3D = 129 << PMEnumHelper.ResultFormatShift,
    }
}
