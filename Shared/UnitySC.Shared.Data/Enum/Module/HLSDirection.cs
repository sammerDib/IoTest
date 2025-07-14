namespace UnitySC.Shared.Data.Enum.Module
{
    public enum LSDirection
    {
        Unknow = 0,
        Wide = 1,   // FW
        Narrow = 2  // BW
    }

    public enum HLSDirection
    {
        Unknow = 0 << HLSResultHelper.HLSDirectionShift,
        FW_Wide = (int)LSDirection.Wide << HLSResultHelper.HLSDirectionShift,
        BW_Narrow = (int)LSDirection.Narrow << HLSResultHelper.HLSDirectionShift,
        Total = 3 << HLSResultHelper.HLSDirectionShift,
    }


}
