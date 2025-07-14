using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("1ACFFA6D-55F2-44ED-8998-48F983E5BDED")]
    public enum SecsFormat
    {
        Undefined = 0x00000063,
        List = 0x00000000,
        Binary = 0x00000008,
        Boolean = 0x00000009,
        Ascii = 0x00000010,
        Jis8 = 0x00000011,
        Character = 0x00000012,
        Int8 = 0x00000018,
        Int1 = 0x00000019,
        Int2 = 0x0000001A,
        Int4 = 0x0000001C,
        Float8 = 0x00000020,
        Float4 = 0x00000024,
        UInt8 = 0x00000028,
        UInt1 = 0x00000029,
        UInt2 = 0x0000002A,
        UInt4 = 0x0000002C
    }
}
