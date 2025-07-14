using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true),
     Guid(Constants.ModuleStateInterfaceString)]
    public enum ModuleState
    {
        Stopped,
        Initializing,
        Idle,
        Pending,
        Executing,
        Stopping,
        Error,
        Suspended
    }
}
