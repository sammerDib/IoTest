using Agileo.StateMachine;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    internal partial class UnloadFromPm
    {
        private class RobotDone : Event
        {
        }

        private class PMDone : Event
        {
        }

        private class ReadyToTransfer : Event
        {
        }

        private class NotReadyToTransfer : Event
        {
        }
    }
}
