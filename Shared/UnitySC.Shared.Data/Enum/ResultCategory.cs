namespace UnitySC.Shared.Data.Enum
{
    // [Result Category (4 bits)]
    public enum ResultCategory
    {
        Unknow = 0 << PMEnumHelper.ResultCategoryShift,
        Acquisition = 1 << PMEnumHelper.ResultCategoryShift,
        Result = 2 << PMEnumHelper.ResultCategoryShift,
        Config = 3 << PMEnumHelper.ResultCategoryShift
    }
}