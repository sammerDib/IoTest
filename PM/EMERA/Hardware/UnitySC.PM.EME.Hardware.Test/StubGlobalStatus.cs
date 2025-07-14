using System;

using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Hardware.Test
{
    internal class StubGlobalStatus : IGlobalStatusServer
    {
        private PMGlobalStates _globalState;
        private PMControlMode _controlMode;

        public event GlobalStatusChangedEventHandler GlobalStatusChanged;

        public event ToolModeChangedEventHandler ToolModeChanged;

        public void AddMessage(Message message)
        {
        }


        public PMGlobalStates GetGlobalState()
        {
            return _globalState;
        }

        public bool ReleaseLocalHardware()
        {
            return true;
        }

        public bool ReserveLocalHardware()
        {
            return true;
        }

        public PMControlMode GetControlMode()
        {
            return _controlMode;
        }

        public void SetControlMode(PMControlMode controlMode)
        {
            _controlMode = controlMode;
        }

        public void SetGlobalState(PMGlobalStates globalState)
        {
            _globalState = globalState;
        }

        public void SetGlobalStatus(GlobalStatus globalStatus)
        {
            _globalState = (PMGlobalStates)globalStatus.CurrentState;
        }

        public void SetToolModeStatus(ToolMode toolMode)
        {
            throw new NotImplementedException();
        }
    }
}
