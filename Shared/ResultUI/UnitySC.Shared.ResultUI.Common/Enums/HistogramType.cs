using System.ComponentModel;

namespace UnitySC.Shared.ResultUI.Common.Enums
{
    public enum HistogramType
    {
        [Description("Defect classes by Slot")]
        MultibarsDefectClasses,

        [Description("Lot Total by Defect class")]
        Cumul,

        [Description("Lot Average by Defect class")]
        Average,

        [Description("Slots by Defect class")]
        MultibarsSlots,

    }
}
