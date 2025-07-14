namespace UnitySC.Shared.Data.Enum
{
    // [Side(2 bits)]
    public enum Side
    {
        Unknown = 0 << PMEnumHelper.SideShift,
        Back = 1 << PMEnumHelper.SideShift,
        Front = 2 << PMEnumHelper.SideShift,
    }
}
