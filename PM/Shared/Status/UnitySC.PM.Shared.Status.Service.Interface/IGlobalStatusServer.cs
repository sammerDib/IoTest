using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Status.Service.Interface
{
    public delegate void GlobalStatusChangedEventHandler(GlobalStatus state);

    public delegate void ToolModeChangedEventHandler(ToolMode mode);

    public interface IGlobalStatusServer
    {
        void SetGlobalState(PMGlobalStates globalState);

        void SetGlobalStatus(GlobalStatus globalStatus);

        void SetToolModeStatus(ToolMode toolMode);

        PMGlobalStates GetGlobalState();

        void SetControlMode(PMControlMode controlMode);

        PMControlMode GetControlMode();

        void AddMessage(Message message);

        bool ReserveLocalHardware();

        bool ReleaseLocalHardware();

        event GlobalStatusChangedEventHandler GlobalStatusChanged;

        event ToolModeChangedEventHandler ToolModeChanged;
    }
}
