using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true)]
    public enum EquipmentState
    {
        Undefined,
        Offline,
        Idle,
        Active,
        Suspended,
        Error,
        StartingUp,
        Initialized,
        Initializing,
        ShuttingDown,
        Pending
    }
}
