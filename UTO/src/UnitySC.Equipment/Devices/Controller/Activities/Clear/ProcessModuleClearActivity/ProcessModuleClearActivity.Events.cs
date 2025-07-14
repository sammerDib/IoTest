using Agileo.StateMachine;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    public partial class ProcessModuleClearActivity
    {
        private class RobotDone : Event
        {
        }

        private class PmDone : Event
        {
        }

        private class PmReadyToTransfer : Event
        {
        }

        private class PmNotReadyToTransfer : Event
        {
        }
    }
}
