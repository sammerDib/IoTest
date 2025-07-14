using System.ComponentModel;

namespace UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum
{
    public enum StopConfig
    {
        [Description("Cancel the PM process and then evacuate all wafers.")]
        CancelProcess,
        [Description("Continue the current process and then evacuate all wafers.")]
        FinishProcessForWaferInPm,
        [Description("Complete the process for all wafers present on the tool and then evacuate.")]
        FinishProcessForAllWafersOnTools
    }
}
