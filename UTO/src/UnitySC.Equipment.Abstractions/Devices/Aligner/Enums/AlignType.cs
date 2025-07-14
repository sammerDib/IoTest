using System.ComponentModel;

namespace UnitySC.Equipment.Abstractions.Devices.Aligner.Enums
{
    public enum AlignType
    {
        [Description("Aligner align the wafer without checking sub o-flat location.(Standard way)")]
        AlignWaferWithoutCheckingSubO_FlatLocation = 0,

        [Description("Aligner align the wafer for main o-flat with checking sub o-flat location (only for 3/4/6 type)")]
        AlignWaferForMainO_FlatCheckingSubO_FlatLocation = 1,

        [Description("Aligner align the wafer for sub o-flat with checking sub o-flat location (only for 3/4/6 type)")]
        AlignWaferForSubO_FlatCheckingSubO_FlatLocation = 2
    }
}
