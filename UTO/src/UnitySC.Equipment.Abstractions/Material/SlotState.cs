namespace UnitySC.Equipment.Abstractions.Material
{
    /// <summary>
    /// Define the Slot States as on the "RTI EFEM Protocol v2.26_210503.pdf".
    /// </summary>
    /// <remarks>
    /// /!\ Slot States are not the same on the "EFEM Controller Comm Specs 211006.pdf" /!\
    /// but checked with Unity, it makes more sens to return the same values as HW
    /// </remarks>
    public enum SlotState
    {
        NoWafer     = 0,
        HasWafer    = 1,
        DoubleWafer = 2,
        CrossWafer  = 3,
        FrontBow    = 4,
        Thick       = 7,
        Thin        = 8,
        Error       = 9
    }
}
